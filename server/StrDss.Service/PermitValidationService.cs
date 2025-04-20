using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Service.HttpClients;
using Microsoft.Extensions.Configuration;
using StrDss.Service;
using NetTopologySuite.Operation.Valid;
using StrDss.Data.Repositories;

namespace StrDss.Service
{
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

        try
        {
            Response resp = await _regApiClient.ValidatePermitAsync(body, _apiAccount);

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
                   Dictionary<string, List<string>> errorDetails = resp.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToList());
                    registrationText = errorDetails.ParseError();
                }
            }
            else if (!string.Equals(resp.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                isValid = false;
                registrationText = resp.Status;

            }
        }
        catch (ApiException ex)
        {
            isValid = false;
            registrationText = ex.StatusCode == 404 ? RegistrationValidationText.ValidationException404 :
                ex.StatusCode == 401 ? RegistrationValidationText.ValidationException401 :
                RegistrationValidationText.ValidationException;
        }
        catch (Exception ex)
        {
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
            _logger.LogError($"API Exception: {ex.StatusCode} - {ex.Message}", ex);
        }

        return (isExempt, registrationText);
    }
}
