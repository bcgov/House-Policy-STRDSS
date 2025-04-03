using StrDss.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AngleSharp.Io;

namespace StrDss.Service.HttpClients.RegistrationAPI
{
    public interface IValidatePermitClient
    {
        Task<(bool isValid, Dictionary<string, List<string>> errors)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode);
    }

    public class ValidatePermitClient : IValidatePermitClient
    {
        private IRegistrationApiClient _regClient;
        private IConfiguration _config;
        private string? _apiAccount;

        public ValidatePermitClient(IRegistrationApiClient regClient, IConfiguration config)
        {
            _regClient = regClient;
            _config = config;
            _apiAccount = _config.GetValue<string>("REGISTRATION_API_ACCOUNT");
        }

        public async Task<(bool isValid, Dictionary<string, List<string>> errors)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode)
        {
            bool isValid = true;
            Dictionary<string, List<string>> errorDetails = new();

            StrDss.Service.HttpClients.Body body = new()
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
                Response resp = await _regClient.ValidatePermitAsync(body, _apiAccount);

                // If we didn't get a Status field back, then there was an error
                if (string.IsNullOrEmpty(resp.Status))
                {
                    isValid = false;
                    if (resp.Errors.Count == 0)
                        errorDetails.Add("UNKNOWN ERROR", new List<string> { "Response did not contain a status or error message." });
                    else
                        errorDetails = resp.Errors
                            .GroupBy(e => e.Code)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToList());
                }

                // If the status is not "ACTIVE" then there is an issue with the registration
                if (!string.Equals(resp.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
                {
                    isValid = false;
                    errorDetails.Add("INACTIVE PERMIT", new List<string> { "Error: registration status returned as " + resp.Status });
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                errorDetails.Add("EXCEPTION", new List<string> { ex.Message });
            }

            return (isValid, errorDetails);
        }
    }

}
