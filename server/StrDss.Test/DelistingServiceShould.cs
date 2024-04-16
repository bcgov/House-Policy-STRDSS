using AutoFixture.Xunit2;
using Castle.Core.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.DelistingDtos;
using StrDss.Model.OrganizationDtos;
using StrDss.Service;
using StrDss.Service.HttpClients;
using Xunit;

namespace StrDss.Test
{
    public class DelistingServiceShould
    {
        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_ValidDto_ReturnsNoErrors(
            TakedownNoticeCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            DelistingService sut)
        {
            // Arrange
            configMock.Setup(x => x.GetValue(typeof(string), "")).Returns("https://ches.example.com");
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });
            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.NoticeOfTakedown;
            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_NullPlatform_ReturnsPlatformIdError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.PlatformId = 0;

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyListingUrl_ReturnsListingUrlError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidListingUrl_ReturnsInvalidUrlError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_HostEmailSentFalseAndEmptyHostEmail_ReturnsHostEmailRequiredError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmailSent = false;
            dto.HostEmail = string.Empty;

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is required", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidHostEmail_ReturnsInvalidHostEmailError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = "invalidemail";

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is invalid", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_NullReason_ReturnsReasonIdError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.ReasonId = 0;

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("reasonId", result.Keys);
            Assert.Single(result["reasonId"]);
            Assert.Equal($"Reason ID ({dto.ReasonId}) does not exist.", result["reasonId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyLgContactEmail_ReturnsLgContactEmailRequiredError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = string.Empty;

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is required", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactEmail_ReturnsInvalidLgContactEmailError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = "invalidemail";

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is invalid", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactPhone_ReturnsInvalidLgContactPhoneError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactPhone = "invalidphone";

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("lgContactPhone", result.Keys);
            Assert.Single(result["lgContactPhone"]);
            Assert.Equal("Local government contact phone number is invalid", result["lgContactPhone"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidStrBylawUrl_ReturnsStrBylawUrlRequiredError(
            TakedownNoticeCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.StrBylawUrl = "invalidurl";

            // Act
            var result = await sut.CreateTakedownNoticeAsync(dto);;

            // Assert
            Assert.Contains("strByLawUrl", result.Keys);
            Assert.Single(result["strByLawUrl"]);
            Assert.Equal("URL is invalid", result["strByLawUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullPlatform_ReturnsPlatformIdError(
            TakedownRequestCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.PlatformId = 0;

            // Act
            var result = await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullLocalGovernment_ReturnsLocalGovernmentIdError(
            TakedownRequestCreateDto dto,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            DelistingService sut)
        {
            // Arrange
            currentUserMock.Setup(m => m.OrganizationId).Returns(0);

            // Act
            var result = await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains("currentUser", result.Keys);
            Assert.Single(result["currentUser"]);
            Assert.Equal($"User's organization (0) does not exist.", result["currentUser"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_EmptyListingUrl_ReturnsListingUrlError(
            TakedownRequestCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidListingUrl_ReturnsInvalidUrlError(
            TakedownRequestCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            TakedownRequestCreateDto dto,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenCalled_ShouldSendEmail(
            TakedownNoticeCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            currentUserMock.Setup(m => m.EmailAddress).Returns("currentUser@example.com");
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });
            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.NoticeOfTakedown;

            // Act
            await sut.CreateTakedownNoticeAsync(dto);

            // Assert
            emailServiceMock.Verify(m => m.SendEmailAsync(It.IsAny<EmailContent>()), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenHostEmailIsNotEmpty_AddsHostEmailToToList(
            TakedownNoticeCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = "host@example.com";
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });
            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.NoticeOfTakedown;
            // Act
            await sut.CreateTakedownNoticeAsync(dto);

            // Assert
            Assert.Contains(dto.HostEmail, dto.ToList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenHostEmailIsEmpty_DoesNotAddHostEmailToToList(
            TakedownNoticeCreateDto dto,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = string.Empty;

            // Act
            await sut.CreateTakedownNoticeAsync(dto);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Assert
            Assert.DoesNotContain(dto.HostEmail, dto.ToList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_Always_AddsCurrentUserEmailToCcList(
            TakedownNoticeCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns(currentUserEmail);
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });
            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.NoticeOfTakedown;

            // Act
            await sut.CreateTakedownNoticeAsync(dto);

            // Assert
            Assert.Contains(currentUserEmail, dto.CcList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenCalled_ShouldSendEmail(
            TakedownRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.PlatformId = 2;
            currentUserMock.Setup(m => m.EmailAddress).Returns("currentUser@example.com");
            currentUserMock.Setup(m => m.OrganizationId).Returns(1);
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            
            var lg = CommonUtils.CloneObject(platform);
            lg.OrganizationType = OrganizationTypes.LG;
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(1)).ReturnsAsync(lg);

            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.TakedownRequest;

            // Act
            await sut.CreateTakedownRequestAsync(dto);

            // Assert
            emailServiceMock.Verify(m => m.SendEmailAsync(It.IsAny<EmailContent>()), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenSendCopyIsTrue_AddsCurrentUserEmailToList(
            TakedownRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = true;
            dto.PlatformId = 2;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns(currentUserEmail);
            currentUserMock.Setup(m => m.OrganizationId).Returns(1);
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);

            var lg = CommonUtils.CloneObject(platform);
            lg.OrganizationType = OrganizationTypes.LG;
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(1)).ReturnsAsync(lg);

            platform.ContactPeople.First().EmailMessageType = EmailMessageTypes.TakedownRequest;

            // Act
            await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.Contains(currentUserEmail, dto.ToList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenSendCopyIsFalse_DoesNotAddCurrentUserEmailToCcList(
            TakedownRequestCreateDto dto,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = false;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns("user@example.com");

            // Act
            await sut.CreateTakedownRequestAsync(dto);

            // Assert
            Assert.DoesNotContain(currentUserEmail, dto.CcList);
        }
    }
}
