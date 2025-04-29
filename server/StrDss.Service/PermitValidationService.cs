using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Service.HttpClients;
using Microsoft.Extensions.Configuration;
using StrDss.Service;
using NetTopologySuite.Operation.Valid;
using StrDss.Data.Repositories;
using StrDss.Data.Entities;
using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace StrDss.Service
{
    public class ApiErrorResponse
    {
        public string ErrorMessage { get; set; }
        public string RootCause { get; set; }
    }

    public interface IPermitValidationService
    {
        Task<(bool isValid, string registrationText)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode);
        Task<(bool isStraaExempt, string registrationText)> CheckStraaExemptionStatus(string rentalAddress);
    }
}
public class PermitValidationService : IPermitValidationService
{
    private IRegistrationApiClient _regApiClient;
    private IGeocoderApi _geocoder;
    private IOrganizationRepository _orgRepo;
    private IConfiguration _config;
    private ILogger<StrDssLogger> _logger;
    private readonly string? _apiAccount;

    public PermitValidationService(IRegistrationApiClient regApiClient, IGeocoderApi geocoder, IOrganizationRepository orgRepo,  IConfiguration config, ILogger<StrDssLogger> logger)
    {
        _regApiClient = regApiClient;
        _geocoder = geocoder;
        _orgRepo = orgRepo;
        _config = config;
        _logger = logger;
        _apiAccount = _config.GetValue<string>("REGISTRATION_API_ACCOUNT");
    }

    public async Task<(bool isValid, string registrationText)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode)
    {
        bool isValid = true;
        string registrationText = RegistrationValidationText.Success;

        Body body = new()
        {
            Identifier = regNo,
            Address = new()
            {
                UnitNumber = unitNumber,
                StreetNumber = streetNumber,
                PostalCode = postalCode
            }
        };

        Response resp;
        try
        {
            _logger.LogInformation("Calling validate permit.");
           resp = await _regApiClient.ValidatePermitAsync(body, _apiAccount);

            // If we didn't get a Status field back, then there was an error
            if (string.IsNullOrEmpty(resp.Status))
            {
                isValid = false;
                if (resp.Errors.Count == 0)
                {
                    registrationText = RegistrationValidationText.StatusNotFound;
                }
                else
                {
                    _logger.LogInformation("Validate permit returned an error.");
                    List<string> errorDetails = resp.Errors
                        .Select(e => $"{e.Code}:{e.Message}")
                        .ToList();

                    registrationText = string.Join("\n", resp.Errors.Select(e => $"{e.Code}:{e.Message}"));
                }
            }
            else
            {
                _logger.LogInformation("Permit status is: ." + resp.Status);

                registrationText = "200:"+resp.Status;
            }
        }
        catch (ApiException<Response2> ex)
        {
            isValid = false;
            _logger.LogError($"Error Code: {ex.StatusCode}, Additional Properties: {string.Join(", ", ex.Result.AdditionalProperties)}");
            // Extract the rootCause from AdditionalProperties
            if (ex.Result.AdditionalProperties.TryGetValue("rootCause", out var rootCauseObj) && rootCauseObj is string rootCause)
            {
                // Use regex to extract all "message" fields
                var matches = Regex.Matches(rootCause, @"message:(?<message>[^,]+)");
                if (matches.Count > 0)
                {
                    // Combine all messages into a single string separated by commas
                    var errorMessages = matches.Select(m => m.Groups["message"].Value.Trim());
                    registrationText = $"{ex.StatusCode}:{string.Join(", ", errorMessages)}";
                }
                else
                {
                    registrationText = $"{ex.StatusCode}:Unknown error in rootCause.";
                }
            }
            else
            {
                registrationText = $"{ex.StatusCode}:Unknown error.";
            }

        }
        catch (ApiException ex)
        {
            isValid = false;
            if(ex.StatusCode == 401)
            {                
                registrationText = ex.StatusCode + ":" + RegistrationValidationText.ValidationException401;
            } 
            else if (ex.StatusCode == 404)
            {
                registrationText = ex.StatusCode + ":" + RegistrationValidationText.ValidationException404;
            }
            else
            {
                registrationText = ex.StatusCode + ":" + RegistrationValidationText.ValidationException;
            }
            _logger.LogError($"Validate permit call return {ex.StatusCode}: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Validate permit call threw an exception: " + ex.Message);
            isValid = false;
            registrationText = RegistrationValidationText.ValidationException;
        }

        return (isValid, registrationText);
    }

    public async Task<(bool isStraaExempt, string registrationText)> CheckStraaExemptionStatus(string rentalAddress)
    {
        bool isExempt = true;
        string registrationText = RegistrationValidationText.STRAAExempt;

        DssPhysicalAddress newAddress = new()
        {
            OriginalAddressTxt = rentalAddress
        };

        try
        {
            var geoError = await _geocoder.GetAddressAsync(newAddress);
            if (!string.IsNullOrEmpty(geoError))
            {
                _logger.LogError($"Geocoder error: {geoError}");
                 registrationText = RegistrationValidationText.AddressNotFound;
                isExempt = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Geocoder Api threw an undhandled exception: {ex.Message}");
            registrationText = RegistrationValidationText.AddressNotFound;
            isExempt = false;
        }

        if (isExempt && newAddress.LocationGeometry is not null && newAddress.LocationGeometry is Point point)
        {
            var containingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
            if (containingOrganizationId == null)
            {
                _logger.LogError("Could not determine containing jurisdiction from rental address.");
                registrationText = RegistrationValidationText.JurisdictionNotFound;
                isExempt = false;
            }
            else
            {
                var org = await _orgRepo.GetOrganizationByIdAsync(containingOrganizationId.Value);
                if (org == null || org.IsStraaExempt == null)
                {
                    _logger.LogError("Could not determine straaExempt status from the containing jurisdiction.");
                    registrationText = RegistrationValidationText.ExemptionStatusNotFound;
                    isExempt = false;
                }

                if (org.IsStraaExempt == false)
                {
                    _logger.LogError("Organization is not exempt from STRAA.");
                    registrationText = RegistrationValidationText.NotSTRAAExempt;
                    isExempt = false;
                }
            }
        }

        return (isExempt, registrationText);
    }
}
