﻿using AutoMapper;
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
using StrDss.Service.CsvHelpers;
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
        Task ProcessTakedownRequestBatchEmailsAsync();
        Task<Dictionary<string, List<string>>> SendBatchTakedownRequestAsync(long platformId, Stream stream);
        Task<Dictionary<string, List<string>>> SendBatchTakedownNoticeAsync(long platformId, Stream stream);
        Task<Dictionary<string, List<string>>> CreateTakedownNoticesFromListingAsync(TakedownNoticesFromListingDto[] listings);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownNoticesFromListingPreviewAsync(TakedownNoticesFromListingDto[] listings);
        Task<Dictionary<string, List<string>>> CreateTakedownRequestsFromListingAsync(TakedownRequestsFromListingDto[] listings);
        Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownRequestsFromListingPreviewAsync(TakedownRequestsFromListingDto[] listings);
    }
    public class DelistingService : ServiceBase, IDelistingService
    {
        private IRentalListingRepository _listingRepo;
        private IConfiguration _config;
        private IEmailMessageService _emailService;
        private IOrganizationService _orgService;
        private IEmailMessageRepository _emailRepo;

        public DelistingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IRentalListingRepository listingRepo,
            IConfiguration config, IEmailMessageService emailService, IOrganizationService orgService, IEmailMessageRepository emailRepo, ILogger<StrDssLogger> logger)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _listingRepo = listingRepo;
            _config = config;
            _emailService = emailService;
            _orgService = orgService;
            _emailRepo = emailRepo;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownNoticeAsync(TakedownNoticeCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(_currentUser.OrganizationId);

            var errors = await ValidateTakedownNoticeAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return errors;
            }

            await SendTakedownNoticeAsync(dto, platform, lg);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateTakedownNoticeAsync(TakedownNoticeCreateDto dto, OrganizationDto? platform, OrganizationDto? lg)
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

                if (platform.ContactPeople == null || 
                    !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.NoticeOfTakedown))
                {
                    errors.AddItem("platformId", $"Platform ({dto.PlatformId}) does not have the primary '{EmailMessageTypes.NoticeOfTakedown}' contact info");
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

            if (lg == null)
            {
                errors.AddItem("currentUser", $"User's organization ({_currentUser.OrganizationId}) does not exist.");
            }
            else
            {
                if (lg.OrganizationType != OrganizationTypes.LG)
                {
                    errors.AddItem("currentUser", $"User's organization ({_currentUser.OrganizationId}) is not a local government");
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

        private async Task SendTakedownNoticeAsync(TakedownNoticeCreateDto dto, OrganizationDto? platform, OrganizationDto? lg)
        {
            var template = GetTakedownNoticeTemplate(dto, platform, lg);

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = dto.HostEmailSent,
                IsSubmitterCcRequired = true,
                LgPhoneNo = dto.LgContactPhone,
                UnreportedListingNo = dto.ListingId,
                HostEmailAddressDsc = dto.HostEmail,
                LgEmailAddressDsc = dto.LgContactEmail,
                CcEmailAddressDsc = string.Join("; ", dto.CcList),
                UnreportedListingUrl = dto.ListingUrl,
                LgStrBylawUrl = dto.StrBylawUrl,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = dto.PlatformId,
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();
        }
        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownNoticePreviewAsync(TakedownNoticeCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(_currentUser.OrganizationId);

            var errors = await ValidateTakedownNoticeAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template = GetTakedownNoticeTemplate(dto, platform, lg, true);

            return (errors, new EmailPreview { Content = template.GetContent().HtmlToPlainText() });
        }

        private TakedownNotice GetTakedownNoticeTemplate(TakedownNoticeCreateDto dto, OrganizationDto? platform, OrganizationDto? lg, bool preview = false)
        {
            // To: [host] (optional), [Local Gov contact info email]
            if (dto.HostEmail.IsNotEmpty()) dto.ToList.Add(dto.HostEmail);
            dto.ToList.Add(dto.LgContactEmail);

            // BCC: [sender], [platform], [Additional CCs] (optional)
            dto.CcList.Add(_currentUser.EmailAddress);

            var platformEmails = platform!.ContactPeople
                .Where(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.NoticeOfTakedown)
                .Select(x => x.EmailAddressDsc)
                .ToArray();

            dto.CcList.AddRange(platformEmails);

            var template = new TakedownNotice(_emailService)
            {
                Url = dto.ListingUrl,
                ListingId = dto.ListingId,
                LgName = lg!.OrganizationNm,
                To = dto.ToList,
                Bcc = dto.CcList,
                EmailsToHide = platformEmails,
                Info = dto.ListingUrl,
                Comment = dto.Comment,
                Preview = preview
            };
            return template;
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownNoticesFromListingAsync(TakedownNoticesFromListingDto[] listings)
        {
            var errors = new Dictionary<string, List<string>>();
            var emailRegex = RegexDefs.GetRegexInfo(RegexDefs.Email);
            var templates = new List<TakedownNoticeFromListing>();
            var organization = await GetOrganizationAsync(errors);

            if (organization == null)
            {
                return errors;
            }

            foreach (var listing in listings)
            {
                var rentalListing = await _listingRepo.GetRentalListingForTakedownAction(listing.RentalListingId, true);

                if (rentalListing == null) continue;

                ValidateRentalListingAccess(rentalListing, organization, errors);

                var template = CreateTakedownNoticeTemplate(listing, rentalListing);
                templates.Add(template);

                ValidateCcListEmails(listing.CcList, emailRegex, errors);
                ValidateLocalGovernmentContactEmail(listing, emailRegex, errors);
                ValidateAndSetHostEmails(listing, rentalListing, emailRegex, template, errors);

                SetTemplateEmailFields(listing, rentalListing, template);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await SendTakedownNoticeEmailsFromListingAsync(listings, templates, errors);

            return errors;
        }

        private void ValidateLocalGovernmentContactEmail(TakedownNoticesFromListingDto listing, RegexInfo emailRegex, Dictionary<string, List<string>> errors)
        {
            if (listing.LgContactEmail.IsEmpty())
            {
                errors.AddItem("lgContactEmail", $"Local government contact email is required");
            }
            else if (!Regex.IsMatch(listing.LgContactEmail, emailRegex.Regex))
            {
                errors.AddItem("lgContactEmail", $"Local government contact email is invalid");
            }
        }

        private void ValidateAndSetHostEmails(TakedownNoticesFromListingDto listing, RentalListingForTakedownDto rentalListing, RegexInfo emailRegex, TakedownNoticeFromListing template, Dictionary<string, List<string>> errors)
        {
            if (!listing.HostEmailSent)
            {
                var hostEmails = rentalListing.HostEmails.Where(email => email != null && Regex.IsMatch(email, emailRegex.Regex)).ToList();

                if (!hostEmails.Any())
                {
                    errors.AddItem("hostEmail", $"No valid host email for the listing {rentalListing.OrganizationCd} - {rentalListing.PlatformListingNo}");
                }
                else
                {
                    listing.HostEmails = hostEmails;
                    template.To = hostEmails;
                }
            }
        }

        private void SetTemplateEmailFields(TakedownNoticesFromListingDto listing, RentalListingForTakedownDto rentalListing, TakedownNoticeFromListing template)
        {
            template.To = template.To.Add(listing.LgContactEmail);
            listing.CcList ??= new List<string>();
            listing.CcList.Add(_currentUser.EmailAddress);
            listing.CcList.AddRange(rentalListing.PlatformEmails);
            template.Bcc = listing.CcList;
            template.EmailsToHide = rentalListing.PlatformEmails;
        }

        private TakedownNoticeFromListing CreateTakedownNoticeTemplate(TakedownNoticesFromListingDto listing, RentalListingForTakedownDto rentalListing)
        {
            listing.ProvidingPlatformId = rentalListing.ProvidingPlatformId;

            return new TakedownNoticeFromListing(_emailService)
            {
                RentalListingId = listing.RentalListingId,
                Url = rentalListing.PlatformListingUrl ?? "",
                OrgCd = rentalListing.OrganizationCd,
                ListingId = rentalListing.PlatformListingNo,
                Comment = listing.Comment,
                LgName = _currentUser.OrganizationName,
                Info = $"{rentalListing.OrganizationCd}-{rentalListing.PlatformListingNo}"
            };
        }
        private async Task SendTakedownNoticeEmailsFromListingAsync(TakedownNoticesFromListingDto[] listings, List<TakedownNoticeFromListing> templates, Dictionary<string, List<string>> errors)
        {
            foreach (var template in templates)
            {
                try
                {
                    await SendTakedownNoticeEmailFromListingAsync(listings, template);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    errors.AddItem($"{template.OrgCd}-{template.ListingId}", "Failed to send email for the listing.");
                }
            }
        }

        private async Task SendTakedownNoticeEmailFromListingAsync(TakedownNoticesFromListingDto[] listings, TakedownNoticeFromListing template)
        {
            var listing = listings.First(x => x.RentalListingId == template.RentalListingId);

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = listing.HostEmailSent,
                IsSubmitterCcRequired = true,
                LgPhoneNo = "",
                UnreportedListingNo = template.ListingId,
                HostEmailAddressDsc = listing.HostEmails.FirstOrDefault(),
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = string.Join("; ", template.Bcc),
                UnreportedListingUrl = template.Url,
                LgStrBylawUrl = "",
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = listing.ProvidingPlatformId,
                ConcernedWithRentalListingId = listing.RentalListingId,
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownNoticesFromListingPreviewAsync(TakedownNoticesFromListingDto[] listings)
        {
            var errors = new Dictionary<string, List<string>>();
            var emailRegex = RegexDefs.GetRegexInfo(RegexDefs.Email);
            var templates = new List<TakedownNoticeFromListing>();
            var organization = await GetOrganizationAsync(errors);

            if (organization == null)
            {
                return (errors, new EmailPreview());
            }

            foreach (var listing in listings)
            {
                var rentalListing = await _listingRepo.GetRentalListingForTakedownAction(listing.RentalListingId, true);

                if (rentalListing == null) continue;

                ValidateRentalListingAccess(rentalListing, organization, errors);

                var template = CreateTakedownNoticeTemplate(listing, rentalListing);
                templates.Add(template);

                ValidateCcListEmails(listing.CcList, emailRegex, errors);
                ValidateLocalGovernmentContactEmail(listing, emailRegex, errors);
                ValidateAndSetHostEmails(listing, rentalListing, emailRegex, template, errors);

                SetTemplateEmailFields(listing, rentalListing, template);
            }

            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template1 = templates.FirstOrDefault();

            if (template1 == null)
            {
                errors.AddItem("template", "Wasn't able to create email templates from the selected listings");
                return (errors, new EmailPreview());
            }

            template1.Preview = true;

            return (errors, new EmailPreview()
            {
                Content = template1.GetHtmlPreview()
            });
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownRequestsFromListingAsync(TakedownRequestsFromListingDto[] listings)
        {
            var errors = new Dictionary<string, List<string>>();
            var templates = new List<TakedownRequestFromListing>();

            var organization = await GetOrganizationAsync(errors);
            if (organization == null)
            {
                return errors;
            }

            await ProcessListings(listings, errors, templates, organization);

            if (errors.Count == 0)
            {
                await SendTakedownRequestEmailsFromListingAsync(listings, templates, errors);
            }

            return errors;
        }

        private async Task<OrganizationDto?> GetOrganizationAsync(Dictionary<string, List<string>> errors)
        {
            var organization = await _orgService.GetOrganizationByIdAsync(_currentUser.OrganizationId);

            if (organization == null)
            {
                errors.AddItem("organization", "No organization found for the user");
            }

            return organization;
        }

        private async Task ProcessListings(TakedownRequestsFromListingDto[] listings, Dictionary<string, List<string>> errors,
            List<TakedownRequestFromListing> templates, OrganizationDto organization)
        {
            var emailRegex = RegexDefs.GetRegexInfo(RegexDefs.Email);

            foreach (var listing in listings)
            {
                ValidateCcListEmails(listing.CcList, emailRegex, errors);

                var rentalListing = await _listingRepo.GetRentalListingForTakedownAction(listing.RentalListingId, false);

                if (rentalListing == null) continue;

                if (!ValidateRentalListingAccess(rentalListing, organization, errors)) continue;

                listing.ProvidingPlatformId = rentalListing.ProvidingPlatformId;
                listing.Url = rentalListing.PlatformListingUrl ?? "";
                listing.ListingId = rentalListing.PlatformListingNo;

                var template = CreateTakedownRequestTemplateFromListing(listing, rentalListing);
                templates.Add(template);
            }
        }

        private void ValidateCcListEmails(List<string> ccList, RegexInfo emailRegex, Dictionary<string, List<string>> errors)
        {
            foreach (var email in ccList)
            {
                if (!Regex.IsMatch(email, emailRegex.Regex))
                {
                    errors.AddItem("ccList", $"Email ({email}) is invalid");
                }
            }
        }

        private bool ValidateRentalListingAccess(RentalListingForTakedownDto rentalListing, OrganizationDto organization, Dictionary<string, List<string>> errors)
        {
            if (rentalListing.LocalGovernmentId != _currentUser.OrganizationId)
            {
                errors.AddItem("No access", $"The listing {rentalListing.OrganizationCd} - {rentalListing.PlatformListingNo} does not belong to {organization.OrganizationNm}");
                return false;
            }

            return true;
        }

        private TakedownRequestFromListing CreateTakedownRequestTemplateFromListing(TakedownRequestsFromListingDto listing, RentalListingForTakedownDto rentalListing)
        {
            var template = new TakedownRequestFromListing(_emailService)
            {
                RentalListingId = listing.RentalListingId,
                Url = rentalListing.PlatformListingUrl ?? "",
                OrgCd = rentalListing.OrganizationCd,
                ListingId = rentalListing.PlatformListingNo,
                Info = $"{rentalListing.OrganizationCd}-{rentalListing.PlatformListingNo}",
                To = new string[] { _currentUser.EmailAddress },
                Cc = listing.CcList ?? new List<string>()
            };

            return template;
        }

        private async Task SendTakedownRequestEmailsFromListingAsync(TakedownRequestsFromListingDto[] listings, List<TakedownRequestFromListing> templates, Dictionary<string, List<string>> errors)
        {
            foreach (var template in templates)
            {
                try
                {
                    await SendTakedownRequestEmailFromListingAsync(listings, template);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    errors.AddItem($"{template.OrgCd}-{template.ListingId}", "Failed to send email for the listing.");
                }
            }
        }

        private async Task SendTakedownRequestEmailFromListingAsync(TakedownRequestsFromListingDto[] listings, TakedownRequestFromListing template)
        {
            var listing = listings.First(x => x.RentalListingId == template.RentalListingId);

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = false,
                LgPhoneNo = null,
                UnreportedListingNo = listing.ListingId,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = string.Join("; ", template.Cc),
                UnreportedListingUrl = listing.Url,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = listing.ProvidingPlatformId,
                RequestingOrganizationId = _currentUser.OrganizationId,
                IsWithStandardDetail = listing.IsWithStandardDetail,
                CustomDetailTxt = listing.CustomDetailTxt,
                ConcernedWithRentalListingId = listing.RentalListingId,
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();
        }

        public async Task<(Dictionary<string, List<string>> errors, EmailPreview preview)> GetTakedownRequestsFromListingPreviewAsync(TakedownRequestsFromListingDto[] listings)
        {
            var errors = new Dictionary<string, List<string>>();
            var templates = new List<TakedownRequestFromListing>();

            var organization = await GetOrganizationAsync(errors);
            if (organization == null)
            {
                if (errors.Count > 0)
                {
                    return (errors, new EmailPreview());
                }
            }

            await ProcessListings(listings, errors, templates, organization!);

            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template = templates.FirstOrDefault();

            if (template == null)
            {
                errors.AddItem("template", "Wasn't able to create email templates from the selected listings");
                return (errors, new EmailPreview());
            }

            template.Preview = true;

            return (errors, new EmailPreview()
            {
                Content = template.GetHtmlPreview()
            });
        }

        public async Task<Dictionary<string, List<string>>> CreateTakedownRequestAsync(TakedownRequestCreateDto dto)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(dto.PlatformId);
            var lg = await _orgService.GetOrganizationByIdAsync(_currentUser.OrganizationId);

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

                if (platform.ContactPeople == null 
                    || !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.TakedownRequest))
                {
                    errors.AddItem("platformId", $"There's no primary '{EmailMessageTypes.TakedownRequest}' contact email for {platform.OrganizationNm}");
                }

                if (platform.ContactPeople == null
                    || !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.BatchTakedownRequest))
                {
                    errors.AddItem("platformId", $"There's no primary '{EmailMessageTypes.BatchTakedownRequest}' contact email for {platform.OrganizationNm}");
                }
            }

            if (lg == null)
            {
                errors.AddItem("currentUser", $"User's organization ({_currentUser.OrganizationId}) does not exist.");
            }
            else
            {
                if (lg.OrganizationType != OrganizationTypes.LG)
                {
                    errors.AddItem("currentUser", $"User's organization ({_currentUser.OrganizationId}) is not a local government");
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

        private async Task SendTakedownRequestAsync(TakedownRequestCreateDto dto, OrganizationDto platform, OrganizationDto lg)
        {
            var template = GetTakedownRequestTemplate(dto);

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent(),
                IsHostContactedExternally = false,
                LgPhoneNo = null,
                UnreportedListingNo = dto.ListingId,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = string.Join("; ", dto.CcList),
                UnreportedListingUrl = dto.ListingUrl,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = dto.PlatformId,
                RequestingOrganizationId = lg!.OrganizationId,
                IsWithStandardDetail = dto.IsWithStandardDetail,
                CustomDetailTxt = dto.CustomDetailTxt,
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            _unitOfWork.Commit();
        }
        private TakedownRequest GetTakedownRequestTemplate(TakedownRequestCreateDto dto, bool preview = false)
        {
            dto.ToList.Add(_currentUser.EmailAddress);

            var template = new TakedownRequest(_emailService)
            {
                Url = dto.ListingUrl,
                ListingId = dto.ListingId,
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
            var lg = await _orgService.GetOrganizationByIdAsync(_currentUser.OrganizationId);

            var errors = await ValidateTakedownRequestAsync(dto, platform, lg);
            if (errors.Count > 0)
            {
                return (errors, new EmailPreview());
            }

            var template = GetTakedownRequestTemplate(dto, true);

            return (errors, new EmailPreview { Content = template.GetContent().HtmlToPlainText() });

        }

        public async Task ProcessTakedownRequestBatchEmailsAsync()
        {
            var allEmails = await _emailRepo.GetTakedownRequestEmailsToBatch();

            var platformIds = allEmails.Select(x => x.InvolvedInOrganizationId ?? 0).Distinct().ToList();

            foreach(var platformId in platformIds)
            {
                var platform = await _orgService.GetOrganizationByIdAsync(platformId);

                try
                {
                    await ProcessTakedownRequestBatchEmailAsync(platform, allEmails);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while processing '{EmailMessageTypes.BatchTakedownRequest}' email for {platform.OrganizationNm}");
                    _logger.LogError(ex.ToString());
                    //send email to admin?
                } 
            }
        }

        private async Task ProcessTakedownRequestBatchEmailAsync(OrganizationDto platform, List<DssEmailMessage> allEmails)
        {            
            var contacts = platform.ContactPeople
                .Where(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.BatchTakedownRequest)
                .Select(x => x.EmailAddressDsc)
                .ToArray();

            if (contacts.Length == 0)
            {
                throw new Exception($"There's no primary '{EmailMessageTypes.BatchTakedownRequest}' contact email for {platform.OrganizationNm}");
            }

            var emails = allEmails.Where(x => x.InvolvedInOrganizationId == platform.OrganizationId).ToList();

            var csvRecords = emails.Select(x =>
                new TakedownRequestCsvRecord 
                { 
                    ListingId = x.UnreportedListingNo ?? "", 
                    Url = x.UnreportedListingUrl ?? "", 
                    RequestedBy = x.RequestingOrganization?.OrganizationNm ?? "",
                    TakedownRequest = (x.IsWithStandardDetail ?? false) ? "Remove the listing from the platform, do not allow transactions for payments associated with the listing, and cancel all booking associated with the listing." : "",
                    TakedownRequestDetail = x.CustomDetailTxt ?? ""
                })
                .ToList();

            var content = CsvHelperUtils.GetBase64CsvString(csvRecords);
            var date = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow).ToString("yyyy-MM-dd-HH-mm");
            var fileName = $"{platform.OrganizationNm} - {date}.csv";
            var adminEmail = _config.GetValue<string>("ADMIN_EMAIL") ?? throw new Exception($"There's no admin eamil.");

            var template = new BatchTakedownRequest(_emailService)
            {
                To = contacts,
                Bcc = new string[] { adminEmail! },
                Info = $"{EmailMessageTypes.BatchTakedownRequest} for {platform.OrganizationNm}",
                Attachments = new EmailAttachment[] { new EmailAttachment {
                    Content = content,
                    ContentType = "text/csv",
                    Encoding = "base64",
                    Filename = fileName
                }},
            };

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent() + $" Attachement: {fileName} ({csvRecords.Count})",
                IsHostContactedExternally = false,
                IsSubmitterCcRequired = false,
                LgPhoneNo = null,
                UnreportedListingNo = null,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = null,
                UnreportedListingUrl = null,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = null,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = platform.OrganizationId,
                RequestingOrganizationId = null
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            foreach (var email in emails)
            {
                email.BatchingEmailMessageId = emailEntity.EmailMessageId;
            }

            _unitOfWork.Commit();

            _unitOfWork.CommitTransaction(transaction);
        }

        public async Task<Dictionary<string, List<string>>> SendBatchTakedownRequestAsync(long platformId, Stream stream)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(platformId);

            var errors = await ValidateBatchTakedownRequestAsync(platform);
            if (errors.Count > 0)
            {
                return errors;
            }

            //the existence of the contact email has been validated above
            var contacts = platform.ContactPeople
                .Where(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.BatchTakedownRequest)
                .Select(x => x.EmailAddressDsc)
                .ToArray();

            var content = CommonUtils.StreamToBase64(stream);
            var date = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow).ToString("yyyy-MM-dd-HH-mm");
            var fileName = $"{platform.OrganizationNm} - {date}.csv";
            var adminEmail = _config.GetValue<string>("ADMIN_EMAIL") ?? throw new Exception($"There's no admin eamil.");

            var template = new BatchTakedownRequest(_emailService)
            {
                To = contacts,
                Bcc = new string[] { adminEmail! },
                Info = $"{EmailMessageTypes.BatchTakedownRequest} for {platform.OrganizationNm}",
                Attachments = new EmailAttachment[] { new EmailAttachment {
                    Content = content,
                    ContentType = "text/csv",
                    Encoding = "base64",
                    Filename = fileName
                }},
            };

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent() + $" Attachement: {fileName}",
                IsHostContactedExternally = false,
                IsSubmitterCcRequired = false,
                LgPhoneNo = null,
                UnreportedListingNo = null,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = null,
                UnreportedListingUrl = null,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = platform.OrganizationId,
                RequestingOrganizationId = null
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            emailEntity.BatchingEmailMessageId = emailEntity.EmailMessageId;

            _unitOfWork.Commit();

            _unitOfWork.CommitTransaction(transaction);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateBatchTakedownRequestAsync(OrganizationDto? platform)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({platform!.OrganizationId}) does not exist.");
            }
            else
            {
                if (platform.OrganizationType != OrganizationTypes.Platform)
                {
                    errors.AddItem("platformId", $"Organization ({platform!.OrganizationId}) is not a platform");
                }

                if (platform.ContactPeople == null ||
                    !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.BatchTakedownRequest))
                {
                    errors.AddItem("platformId", $"Platform ({platform!.OrganizationId}) does not have the primary '{EmailMessageTypes.BatchTakedownRequest}' contact info");
                }
            }

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> SendBatchTakedownNoticeAsync(long platformId, Stream stream)
        {
            var platform = await _orgService.GetOrganizationByIdAsync(platformId);

            var errors = await ValidateBatchTakedownNoticeAsync(platform);
            if (errors.Count > 0)
            {
                return errors;
            }

            //the existence of the contact email has been validated above
            var contacts = platform!.ContactPeople
                .Where(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.NoticeOfTakedown)
                .Select(x => x.EmailAddressDsc)
                .ToArray();

            var content = CommonUtils.StreamToBase64(stream);
            var date = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow).ToString("yyyy-MM-dd-HH-mm");
            var fileName = $"Notice of non-compliance - {platform.OrganizationNm} - {date}.csv";
            var adminEmail = _config.GetValue<string>("ADMIN_EMAIL") ?? throw new Exception($"There's no admin eamil.");

            var template = new BatchTakedownNotice(_emailService)
            {
                To = contacts,
                Bcc = new string[] { adminEmail! },
                Info = $"{EmailMessageTypes.NoticeOfTakedown} for {platform.OrganizationNm}",
                Attachments = new EmailAttachment[] { new EmailAttachment {
                    Content = content,
                    ContentType = "text/csv",
                    Encoding = "base64",
                    Filename = fileName
                }},
            };

            var emailEntity = new DssEmailMessage
            {
                EmailMessageType = template.EmailMessageType,
                MessageDeliveryDtm = DateTime.UtcNow,
                MessageTemplateDsc = template.GetContent() + $" Attachement: {fileName}",
                IsHostContactedExternally = false,
                IsSubmitterCcRequired = false,
                LgPhoneNo = null,
                UnreportedListingNo = null,
                HostEmailAddressDsc = null,
                LgEmailAddressDsc = null,
                CcEmailAddressDsc = null,
                UnreportedListingUrl = null,
                LgStrBylawUrl = null,
                InitiatingUserIdentityId = _currentUser.Id,
                AffectedByUserIdentityId = null,
                InvolvedInOrganizationId = platform.OrganizationId,
                RequestingOrganizationId = null
            };

            await _emailRepo.AddEmailMessage(emailEntity);

            emailEntity.ExternalMessageNo = await template.SendEmail();

            using var transaction = _unitOfWork.BeginTransaction();

            _unitOfWork.Commit();

            emailEntity.BatchingEmailMessageId = emailEntity.EmailMessageId;

            _unitOfWork.Commit();

            _unitOfWork.CommitTransaction(transaction);

            return errors;
        }

        private async Task<Dictionary<string, List<string>>> ValidateBatchTakedownNoticeAsync(OrganizationDto? platform)
        {
            await Task.CompletedTask;

            var errors = new Dictionary<string, List<string>>();

            if (platform == null)
            {
                errors.AddItem("platformId", $"Platform ID ({platform!.OrganizationId}) does not exist.");
            }
            else
            {
                if (platform.OrganizationType != OrganizationTypes.Platform)
                {
                    errors.AddItem("platformId", $"Organization ({platform!.OrganizationId}) is not a platform");
                }

                if (platform.ContactPeople == null ||
                    !platform.ContactPeople.Any(x => x.IsPrimary && x.EmailAddressDsc.IsNotEmpty() && x.EmailMessageType == EmailMessageTypes.BatchTakedownRequest))
                {
                    errors.AddItem("platformId", $"Platform ({platform!.OrganizationId}) does not have the primary '{EmailMessageTypes.BatchTakedownRequest}' contact info");
                }
            }

            return errors;
        }
    }
}
