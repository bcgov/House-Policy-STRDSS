using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.LocalGovernmentDtos;
using StrDss.Model.PlatformDtos;
using StrDss.Model.WarningReasonDtos;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IDelistingService
    {
        Task<Dictionary<string, List<string>>> ValidateDelistingWarning(DelistingWarningCreateDto dto, PlatformDto? platform, string? reason);
        Task<string> SendDelistingWarningAsync(DelistingWarningCreateDto dto, PlatformDto? platform);
        string FormatDelistingWarningEmailContent(DelistingWarningCreateDto dto, bool contentOnly);
        Task<Dictionary<string, List<string>>> ValidateDelistingRequest(DelistingRequestCreateDto dto, PlatformDto? platform, LocalGovernmentDto? lg);
        Task<string> SendDelistingRequestAsync(DelistingRequestCreateDto dto, PlatformDto? platform);
        string FormatDelistingRequestEmailContent(DelistingRequestCreateDto dto, bool contentOnly);
    }
    public class DelistingService : ServiceBase, IDelistingService
    {
        private IConfiguration _config;
        private IEmailService _emailService;
        private ILogger<DelistingService> _logger;

        public DelistingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration config, IEmailService emailService, ILogger<DelistingService> logger)
            : base(currentUser, validator, unitOfWork, mapper)
        {
            _config = config;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<Dictionary<string, List<string>>> ValidateDelistingWarning(DelistingWarningCreateDto dto, PlatformDto? platform, string? reason)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();
            RegexInfo regex;

            //Todo: Validate Platform ID
            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }


            if (dto.ListingUrl.IsEmpty())
            {
                errors.AddItem("listingUrl", "Listing URL is required");
            }
            else
            {
                regex = RegexDefs.GetRegexInfo(RegexDefs.Url);
                if (!Regex.IsMatch(dto.ListingUrl, regex.Regex))
                {
                    errors.AddItem("listingUrl", "Invalid URL");
                }
            }

            if (!dto.HostEmailSent)
            {
                if (dto.HostEmail.IsEmpty())
                {
                    errors.AddItem("hostEmail", $"Host email is required");
                }
                else
                {
                    regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
                    if (!Regex.IsMatch(dto.HostEmail, regex.Regex))
                    {
                        errors.AddItem("hostEmail", $"Host email is invalid");
                    }
                }
            }

            if (reason == null)
            {
                errors.AddItem("reasonId", $"Reason ID ({dto.ReasonId}) does not exist.");
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
            foreach (var email in dto.CcList)
            {
                if (!Regex.IsMatch(email, regex.Regex))
                {
                    errors.AddItem("ccList", $"Email ({email}) is invalid");
                }
            }

            if (dto.LgContactEmail.IsEmpty())
            {
                errors.AddItem("lgContactEmail", $"Local government contact email is required");
            }
            else
            {
                regex = RegexDefs.GetRegexInfo(RegexDefs.Email);
                if (!Regex.IsMatch(dto.LgContactEmail, regex.Regex))
                {
                    errors.AddItem("lgContactEmail", $"Local government contact email is invalid");
                }
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.PhoneNumber);
            if (dto.LgContactPhone.IsNotEmpty() && !Regex.IsMatch(dto.LgContactPhone, regex.Regex))
            {
                errors.AddItem("lgContactPhone", $"Local government contact phone number is invalid");
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.Url);
            if (dto.StrBylawUrl.IsNotEmpty() && !Regex.IsMatch(dto.StrBylawUrl, regex.Regex))
            {
                errors.AddItem("strByLawUrl", "URL is invalid");
            }

            return errors;
        }

        public async Task<string> SendDelistingWarningAsync(DelistingWarningCreateDto dto, PlatformDto? platform)
        {
            dto.ToList.Add(platform?.Email ?? "");
            if (dto.HostEmail.IsNotEmpty())
            {
                dto.ToList.Add(dto.HostEmail);
            }

            if (dto.SendCopy)
            {
                dto.CcList.Add(_currentUser.EmailAddress);
            }

            var emailContent = new EmailContent
            {
                Bcc = Array.Empty<string>(),
                BodyType = "html",
                Body = FormatDelistingWarningEmailContent(dto, true),
                Cc = dto.CcList.ToArray(),
                DelayTS = 0,
                Encoding = "utf-8",
                From = "no_reply@gov.bc.ca",
                Priority = "normal",
                Subject = "Notice of Takedown",
                To = dto.ToList.ToArray(),
                Info = dto.ListingUrl
            };

            return await _emailService.SendEmailAsync(emailContent);
        }

        public string FormatDelistingWarningEmailContent(DelistingWarningCreateDto dto, bool contentOnly)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var reason = WarningReasonDto.WarningReasons.FirstOrDefault(x => x.WarningReasonId == dto.ReasonId)?.Reason;
            var nl = Environment.NewLine;

            return (contentOnly ? "" : $@"To: {string.Join(";", dto.ToList)}<br/>cc: {string.Join(";", dto.CcList)}<br/><br/>")
                 + $@"Dear Short-term Rental Host,<br/>"
                 + $@"<br/>Short-term rental accommodations in your community must obtain a short-term rental (STR) business licence from the local government in order to operate.<br/><br/>Short-term rental accommodations are also regulated by the Province of B.C. Under the Short-term Rental Accommodations Act, short-term rental hosts in communities with a short-term rental business licence requirement must include a valid business licence number on any short-term rental listings advertised on an online platform. Short-term rental platforms are required to remove listings that do not meet this requirement if requested by the local government.<br/><br/>The short-term rental listing below is not in compliance with an applicable local government business licence requirement for the following reason:"
                 + $@"<b> {reason ?? ""}</b>"
                 + $@"<br/><br/>{dto.ListingUrl}"
                 + $@"<br/><br/>Listing ID Number: {dto.ListingId}"
                 + $@"<br/><br/>Unless you are able to demonstrate compliance with the business licence requirement, this listing may be removed from the short-term rental platform after 5 days. The local government has 90 days to submit a request to takedown the listing to the platform. For more information, contact:"
                 + $@"<br/><br/>Email: {dto.LgContactEmail}"
                 + (dto.LgContactPhone.IsEmpty() ? "" : $@"<br/>Phone: {dto.LgContactPhone}")
                 + (dto.StrBylawUrl.IsEmpty() ? "" : $@"<br/><br/>More information about our city's STR policies can be found at:<br/>{dto.StrBylawUrl}")
                 + (dto.Comment.IsEmpty() ? "" : $@"<br/><br/>{dto.Comment}");
        }

        public async Task<string> SendDelistingRequestAsync(DelistingRequestCreateDto dto, PlatformDto? platform)
        {
            dto.ToList.Add(platform?.Email ?? "");

            if (dto.SendCopy)
            {
                dto.CcList.Add(_currentUser.EmailAddress);
            }

            var emailContent = new EmailContent
            {
                Bcc = Array.Empty<string>(),
                BodyType = "html",
                Body = FormatDelistingRequestEmailContent(dto, true),
                Cc = dto.CcList.ToArray(),
                DelayTS = 0,
                Encoding = "utf-8",
                From = "no_reply@gov.bc.ca",
                Priority = "normal",
                Subject = "Takedown Request",
                To = dto.ToList.ToArray(),
                Info = dto.ListingUrl
            };

            return await _emailService.SendEmailAsync(emailContent);
        }

        public async Task<Dictionary<string, List<string>>> ValidateDelistingRequest(DelistingRequestCreateDto dto, PlatformDto? platform, LocalGovernmentDto? lg)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();
            RegexInfo regex;

            //Todo: Validate Platform ID
            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }

            if (lg == null)
            {
                errors.AddItem("lgId", $"Local Government ID ({dto.LgId}) does not exist.");
            }

            //Todo: Validate LG ID

            if (dto.ListingUrl.IsEmpty())
            {
                errors.AddItem("listingUrl", "Listing URL is required");
            }
            else
            {
                regex = RegexDefs.GetRegexInfo(RegexDefs.Url);
                if (!Regex.IsMatch(dto.ListingUrl, regex.Regex))
                {
                    errors.AddItem("listingUrl", "Invalid URL");
                }
            }

            regex = RegexDefs.GetRegexInfo(RegexDefs.Email);

            foreach (var email in dto.CcList)
            {
                if (!Regex.IsMatch(email, regex.Regex))
                {
                    errors.AddItem("ccList", $"Email ({email}) is invalid");
                }
            }

            return errors;
        }

        public string FormatDelistingRequestEmailContent(DelistingRequestCreateDto dto, bool contentOnly)
        {
            var platform = PlatformDto.Platforms.FirstOrDefault(x => x.PlatformId == dto.PlatformId);
            var nl = Environment.NewLine;

            return (contentOnly ? "" : $@"To: {string.Join(";", dto.ToList)}<br/>cc: {string.Join(";", dto.CcList)}")
                 + $@"<br/><br/>Request to platform service provider for takedown of non-compliant platform offering."
                 + $@"<br/><br/>The following short-term rental listing is not in compliance with an applicable local government business licence requirement:"
                 + $@"<br/><br/>{dto.ListingUrl}"
                 + $@"<br/><br/>Listing ID Number: {dto.ListingId}"
                 + $@"<br/><br/>In accordance, with 17(2) of the Short-term Rental Accommodations Act, please cease providing platform services in respect of the above platform offer within 3 days."
                 + $@"<br/><br/>[Name]<br/>[Title]<br/>[Local government]<br/>[Contact Information]";
        }
    }
}
