using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.OrganizationDtos;
using StrDss.Service.EmailTemplates;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IDelistingService
    {
        Task<Dictionary<string, List<string>>> CreateTakedownNoticeAsync(TakedownNoticeCreateDto dto);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownNoticePreviewAsync(TakedownNoticeCreateDto dto);
        Task<Dictionary<string, List<string>>> CreateTakedownRequestAsync(TakedownRequestCreateDto dto);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownRequestPreviewAsync(TakedownRequestCreateDto dto);
    }
    public class DelistingService : ServiceBase, IDelistingService
    {
        private IConfiguration _config;
        private IEmailMessageService _emailService;
        private IOrganizationService _orgService;
        private IEmailMessageRepository _emailRepo;

        public DelistingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IConfiguration config, IEmailMessageService emailService, IOrganizationService orgService, IEmailMessageRepository emailRepo, ILogger<StrDssLogger> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _config = config;
            _emailService = emailService;
            _orgService = orgService;
            _emailRepo = emailRepo;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownNoticeAsync(TakedownNoticeCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var reasonDto = await _emailService.GetMessageReasonByMessageTypeAndId(EmailMessageTypes.NoticeOfTakedown, dto.ReasonId);

            var errors = await ValidateTakedownNoticeAsync(dto, platform, reasonDto);
            if (errors.Count > 0)
            {
                return errors;
            }

            await SendTakedownNoticeAsync(dto, platform, reasonDto);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateTakedownNoticeAsync(TakedownNoticeCreateDto dto, OrganizationDto? platform, DropdownNumDto? reasonDto)
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

        private async Task SendTakedownNoticeAsync(TakedownNoticeCreateDto dto, OrganizationDto? platform, DropdownNumDto? reasonDto)
        {
            var template = GetTakedownNoticeTemplate(dto, platform, reasonDto);

            var sent = await template.SendEmail();

            if (sent)
            {
                var emailEntity = new DssEmailMessage
                {
                    EmailMessageType = EmailMessageTypes.NoticeOfTakedown,
                    MessageDeliveryDtm = DateTime.UtcNow,
                    MessageTemplateDsc = template.GetContent(),
                    IsHostContactedExternally = dto.HostEmailSent,
                    IsSubmitterCcRequired = dto.SendCopy,
                    MessageReasonId = reasonDto?.Id,
                    LgPhoneNo = dto.LgContactPhone,
                    UnreportedListingNo = dto.ListingId,
                    HostEmailAddressDsc = dto.HostEmail,
                    LgEmailAddressDsc = dto.LgContactEmail,
                    CcEmailAddressDsc = string.Join("; ", dto.CcList),
                    UnreportedListingUrl = dto.ListingUrl,
                    LgStrBylawUrl = dto.StrBylawUrl,
                    InitiatingUserIdentityId = _currentUser.Id,
                    AffectedByUserIdentityId = null,
                    InvolvedInOrganizationId = dto.PlatformId
                };

                await _emailRepo.AddEmailMessage(emailEntity);

                _unitOfWork.Commit();
            }
        }

        private TakedownNotice GetTakedownNoticeTemplate(TakedownNoticeCreateDto dto, OrganizationDto? platform, DropdownNumDto? reasonDto, bool preview = false)
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

            var template = new TakedownNotice(_emailService)
            {
                Reason = reasonDto.Description,
                Url = dto.ListingUrl,
                ListingId = dto.ListingId,
                LgContactInfo = dto.LgContactEmail,
                LgStrBylawLink = dto.StrBylawUrl,
                To = dto.ToList,
                Cc = dto.CcList,
                Info = dto.ListingUrl,
                Comment = dto.Comment,
                Preview = preview
            };
            return template;
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownNoticePreviewAsync(TakedownNoticeCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var reasonDto = await _emailService.GetMessageReasonByMessageTypeAndId(EmailMessageTypes.NoticeOfTakedown, dto.ReasonId);

            var errors = await ValidateTakedownNoticeAsync(dto, platform, reasonDto);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template = GetTakedownNoticeTemplate(dto, platform, reasonDto, true);

            return (errors, new EmailPreview { Content = template.GetContent().HtmlToPlainText() });
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownRequestAsync(TakedownRequestCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(dto.LgId);

            var errors = await ValidateTakedownRequestAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return errors;
            }

            await SendTakedownRequestAsync(dto, platform, lg);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateTakedownRequestAsync(TakedownRequestCreateDto dto, OrganizationDto? platform, OrganizationDto? lg)
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

        private async Task SendTakedownRequestAsync(TakedownRequestCreateDto dto, OrganizationDto? platform, OrganizationDto? lg)
        {
            var template = GetTakedownRequestTemplate(dto, platform, lg);
    
            var sent = await template.SendEmail();

            if (sent)
            {
                var emailEntity = new DssEmailMessage
                {
                    EmailMessageType = EmailMessageTypes.TakedownRequest,
                    MessageDeliveryDtm = DateTime.UtcNow,
                    MessageTemplateDsc = template.GetContent(),
                    IsHostContactedExternally = false,
                    IsSubmitterCcRequired = dto.SendCopy,
                    MessageReasonId = null,
                    LgPhoneNo = null,
                    UnreportedListingNo = dto.ListingId,
                    HostEmailAddressDsc = null,
                    LgEmailAddressDsc = null,
                    CcEmailAddressDsc = string.Join("; ", dto.CcList),
                    UnreportedListingUrl = dto.ListingUrl,
                    LgStrBylawUrl = null,
                    InitiatingUserIdentityId = _currentUser.Id,
                    AffectedByUserIdentityId = null,
                    InvolvedInOrganizationId = dto.PlatformId
                };

                await _emailRepo.AddEmailMessage(emailEntity);

                _unitOfWork.Commit();
            }
        }
        private TakedownRequest GetTakedownRequestTemplate(TakedownRequestCreateDto dto, OrganizationDto? platform, OrganizationDto? lg, bool preview = false)
        {
            var platformContact = platform.ContactPeople.First(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());

            dto.ToList.Add(platformContact.EmailAddressDsc);

            var lgContact = lg.ContactPeople.FirstOrDefault(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty());

            if (dto.SendCopy)
            {
                dto.CcList.Add(_currentUser.EmailAddress);
            }

            var template = new TakedownRequest(_emailService)
            {
                Url = dto.ListingUrl,
                ListingId = dto.ListingId,
                LgContactInfo = lgContact.EmailAddressDsc,
                LgName = lg.OrganizationNm,
                To = dto.ToList,
                Cc = dto.CcList,
                Info = dto.ListingUrl,
                Preview = preview
            };
            return template;
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownRequestPreviewAsync(TakedownRequestCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(dto.LgId);

            var errors = await ValidateTakedownRequestAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template = GetTakedownRequestTemplate(dto, platform, lg, true);

            return (errors, new EmailPreview { Content = template.GetContent().HtmlToPlainText() });

        }
    }
}
