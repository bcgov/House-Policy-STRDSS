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
                        .Select(e => $"{e.Code}: {e.Message}")
                        .ToList();

                    registrationText = string.Join("\n", resp.Errors.Select(e => $"{e.Code}: {e.Message}"));
                }
            }
            else
            {
                _logger.LogInformation("Permit status is: ." + resp.Status);

                registrationText = resp.Status;
            }
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            _logger.LogInformation("Validate permit call return 401: " + ex.Message);
            isValid = false;
            registrationText = RegistrationValidationText.ValidationException401;
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            _logger.LogInformation("Validate permit call returned 404: " + ex.Message);
            isValid = false;
            registrationText = RegistrationValidationText.ValidationException404;

        }
        catch (ApiException ex)
        {
            registrationText = HandleApiException(ex);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Validate permit call threw an exception: " + ex.Message);
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

    private string HandleApiException(ApiException ex)
    {
        if (ex.StatusCode == 401)
        {
            _logger.LogInformation("Validate permit call return 401: " + ex.Message);
            return RegistrationValidationText.ValidationException401;
        }
        if (ex.StatusCode == 404)
        {
            _logger.LogInformation("Validate permit call returned 404: " + ex.Message);
            return RegistrationValidationText.ValidationException404;

        }
        if (ex.StatusCode == 400)
        {
            _logger.LogInformation("Validate permit call returned 400.");
            return RegistrationValidationText.ValidationException;
            //var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(ex.Response);
            //if (errorResponse?.RootCause != null)
            //{
            //    var match = Regex.Match(errorResponse.RootCause, @"code:(?<code>[^,]+),message:(?<message>[^,\]]+)");
            //    if (match.Success)
            //    {
            //        string code = match.Groups["code"].Value;
            //        string message = match.Groups["message"].Value;
            //        return $"{code}: {message}";
            //    }
            //    else
            //    {
            //        return errorResponse.RootCause; // Fallback to the raw rootCause
            //    }
            //}
        }
        return RegistrationValidationText.ValidationException;
    }
}
