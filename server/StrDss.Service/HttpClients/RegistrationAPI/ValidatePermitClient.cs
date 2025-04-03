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
        Task<(bool isValid, string? error)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode);
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

        public async Task<(bool isValid, string? error)> ValidateRegistrationPermitAsync(string regNo, string unitNumber, string streetNumber, string postalCode)
        {
            StrDss.Service.HttpClients.Body validateRegBody = new()
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
                var regResponse = await _regClient.ValidatePermitAsync(validateRegBody, _apiAccount);
                if (regResponse.Errors.Count != 0)
                {
                    return (false, string.Join(", ", regResponse.Errors.Select(e => e.Code + ": " + e.Message)));
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}
