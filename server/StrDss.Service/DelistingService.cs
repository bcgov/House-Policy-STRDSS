﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.OrganizationDtos;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IDelistingService
    {
        Task<Dictionary<string, List<string>>> CreateDelistingWarningAsync(DelistingWarningCreateDto dto);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetDelistingWarningPreviewAsync(DelistingWarningCreateDto dto);
        Task<Dictionary<string, List<string>>> CreateDelistingRequestAsync(DelistingRequestCreateDto dto);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetDelistingRequestPreviewAsync(DelistingRequestCreateDto dto);
    }
    public class DelistingService : ServiceBase, IDelistingService
    {
        private IConfiguration _config;
        private IEmailMessageService _emailService;
        private IOrganizationService _orgService;
        private ILogger<DelistingService> _logger;

        public DelistingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IConfiguration config, IEmailMessageService emailService, IOrganizationService orgService, ILogger<DelistingService> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor)
        {
            _config = config;
            _emailService = emailService;
            _orgService = orgService;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<string>>> CreateDelistingWarningAsync(DelistingWarningCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var reasonDto = await _emailService.GetMessageReasonByMessageTypeAndId(EmailMessageTypes.NoticeOfTakedown, dto.ReasonId);

            var errors = await ValidateDelistingWarningAsync(dto, platform, reasonDto);
            if (errors.Count > 0)
            {
                return errors;
            }

            await SendDelistingWarningAsync(dto, platform, reasonDto);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateDelistingWarningAsync(DelistingWarningCreateDto dto, OrganizationDto? platform, DropdownNumDto? reasonDto)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();
            RegexInfo regex;

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }
            else
            {
                if (platform.OrganizationType != OrganizationTypes.Platform)
                {
                    errors.AddItem("platformId", $"Organization ({dto.PlatformId}) is not a platform");
                }

                if (platform.ContactPeople == null || !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty()))
                {
                    errors.AddItem("platformId", $"Platform ({dto.PlatformId}) does not have the primary contact info");
                }
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

            if (reasonDto == null)
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

        private async Task SendDelistingWarningAsync(DelistingWarningCreateDto dto, OrganizationDto? platform, DropdownNumDto? reasonDto)
        {
            var contact = platform.ContactPeople.First(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());

            dto.ToList.Add(contact.EmailAddressDsc);
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
                Body = FormatDelistingWarningEmailContent(dto, reasonDto, true),
                Cc = dto.CcList.ToArray(),
                DelayTS = 0,
                Encoding = "utf-8",
                From = NoReply.Default,
                Priority = "normal",
                Subject = "Notice of Takedown",
                To = dto.ToList.ToArray(),
                Info = dto.ListingUrl
            };

            await _emailService.SendEmailAsync(emailContent);
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetDelistingWarningPreviewAsync(DelistingWarningCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var reasonDto = await _emailService.GetMessageReasonByMessageTypeAndId(EmailMessageTypes.NoticeOfTakedown, dto.ReasonId);

            var errors = await ValidateDelistingWarningAsync(dto, platform, reasonDto);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var contact = platform.ContactPeople.First(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());
            dto.ToList.Add(contact.EmailAddressDsc);

            if (dto.HostEmail.IsNotEmpty())
            {
                dto.ToList.Add(dto.HostEmail);
            }

            return (errors, new EmailPreview { Content = (FormatDelistingWarningEmailContent(dto, reasonDto, false)).HtmlToPlainText() });
        }

        private string FormatDelistingWarningEmailContent(DelistingWarningCreateDto dto, DropdownNumDto? reasonDto, bool contentOnly)
        {
            var reason = reasonDto?.Description;
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

        public async Task<Dictionary<string, List<string>>> CreateDelistingRequestAsync(DelistingRequestCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(dto.LgId);

            var errors = await ValidateDelistingRequestAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return errors;
            }

            await SendDelistingRequestAsync(dto, platform);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateDelistingRequestAsync(DelistingRequestCreateDto dto, OrganizationDto? platform, OrganizationDto? lg)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();
            RegexInfo regex;

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({dto.PlatformId}) does not exist.");
            }
            else
            {
                if (platform.OrganizationType != OrganizationTypes.Platform)
                {
                    errors.AddItem("platformId", $"Organization ({dto.PlatformId}) is not a platform");
                }

                if (platform.ContactPeople == null || !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty()))
                {
                    errors.AddItem("platformId", $"Platform ({dto.PlatformId}) does not have the primary contact info");
                }
            }

            if (lg == null)
            {
                errors.AddItem("lgId", $"Local Government ID ({dto.LgId}) does not exist.");
            }
            else
            {
                if (lg.OrganizationType != OrganizationTypes.LG)
                {
                    errors.AddItem("platformId", $"Organization ({dto.PlatformId}) is not a local government");
                }
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

        private async Task SendDelistingRequestAsync(DelistingRequestCreateDto dto, OrganizationDto? platform)
        {
            var contact = platform.ContactPeople.First(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());
            dto.ToList.Add(contact.EmailAddressDsc);

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
                From = NoReply.Default,
                Priority = "normal",
                Subject = "Takedown Request",
                To = dto.ToList.ToArray(),
                Info = dto.ListingUrl
            };

            await _emailService.SendEmailAsync(emailContent);
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetDelistingRequestPreviewAsync(DelistingRequestCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(dto.LgId);

            var errors = await ValidateDelistingRequestAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var contact = platform.ContactPeople.First(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());
            dto.ToList.Add(contact.EmailAddressDsc);

            return (errors, new EmailPreview { Content = FormatDelistingRequestEmailContent(dto, false).HtmlToPlainText() });

        }

        private string FormatDelistingRequestEmailContent(DelistingRequestCreateDto dto, bool contentOnly)
        {
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
