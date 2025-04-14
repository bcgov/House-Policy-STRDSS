using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Service.HttpClients;
using Microsoft.Extensions.Configuration;
using StrDss.Service;

namespace StrDss.Service
{
    public interface IPermitValidationService
    {
        Task<(bool isValid, Dictionary<string, List<string>> errors)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode);
    }
}
public class PermitValidationService : IPermitValidationService
{
    private IRegistrationApiClient _regApiClient;
    private IConfiguration _config;
    private ILogger<StrDssLogger> _logger;
    private readonly string? _apiAccount;

    public PermitValidationService(IRegistrationApiClient regApiClient, IConfiguration config, ILogger<StrDssLogger> logger)
    {
        _regApiClient = regApiClient;
        _config = config;
        _logger = logger;
        _apiAccount = _config.GetValue<string>("REGISTRATION_API_ACCOUNT");
    }

    public async Task<(bool isValid, Dictionary<string, List<string>> errors)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode)
    {
        bool isValid = true;
        Dictionary<string, List<string>> errorDetails = new();

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
                    errorDetails.Add("UNKNOWN ERROR", new List<string> { "Response did not contain a status or error message." });
                    _logger.LogError("Response did not contain a status or error message.");
                }
                else
                {
                    errorDetails = resp.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToList());
                }                    
            }
            else if (!string.Equals(resp.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            {
                isValid = false;
                errorDetails.Add("INACTIVE PERMIT", new List<string> { "Error: registration status returned as " + resp.Status });
                _logger.LogError("Registration status returned as " + resp.Status);
            }
        }
        catch (ApiException ex)
        {
            isValid = false;
            if (ex.StatusCode == 404)
            {
                errorDetails.Add("NOT FOUND", new List<string> { "Error: Permit not found (404)." });
            }
            else if (ex.StatusCode == 401)
            {
                errorDetails.Add("UNAUTHORIZED", new List<string> { "Error: Unauthorized access (401)." });
            }
            else
            {
                errorDetails.Add("EXCEPTION", new List<string> { "Error: Service threw an undhandled exception." });
            }
            _logger.LogError($"API Exception: {ex.StatusCode} - {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            isValid = false;
            errorDetails.Add("EXCEPTION", new List<string> { "Error: Service threw an undhandled exception." });
            _logger.LogError($"General Exception: {ex.Message}", ex);
        }

        return (isValid, errorDetails);
    }
}
