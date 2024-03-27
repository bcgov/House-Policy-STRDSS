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
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            DelistingService sut)
        {
            // Arrange
            configMock.Setup(x => x.GetValue(typeof(string), "")).Returns("https://ches.example.com");
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_NullPlatform_ReturnsPlatformIdError(
            DelistingWarningCreateDto dto,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.PlatformId = 0;

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyListingUrl_ReturnsListingUrlError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidListingUrl_ReturnsInvalidUrlError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_HostEmailSentFalseAndEmptyHostEmail_ReturnsHostEmailRequiredError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmailSent = false;
            dto.HostEmail = string.Empty;

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is required", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidHostEmail_ReturnsInvalidHostEmailError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = "invalidemail";

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is invalid", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_NullReason_ReturnsReasonIdError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ReasonId = 0;

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("reasonId", result.Keys);
            Assert.Single(result["reasonId"]);
            Assert.Equal($"Reason ID ({dto.ReasonId}) does not exist.", result["reasonId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyLgContactEmail_ReturnsLgContactEmailRequiredError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = string.Empty;

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is required", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactEmail_ReturnsInvalidLgContactEmailError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = "invalidemail";

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is invalid", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactPhone_ReturnsInvalidLgContactPhoneError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactPhone = "invalidphone";

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("lgContactPhone", result.Keys);
            Assert.Single(result["lgContactPhone"]);
            Assert.Equal("Local government contact phone number is invalid", result["lgContactPhone"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidStrBylawUrl_ReturnsStrBylawUrlRequiredError(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.StrBylawUrl = "invalidurl";

            // Act
            var result = await sut.CreateDelistingWarningAsync(dto);;

            // Assert
            Assert.Contains("strByLawUrl", result.Keys);
            Assert.Single(result["strByLawUrl"]);
            Assert.Equal("URL is invalid", result["strByLawUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullPlatform_ReturnsPlatformIdError(
            DelistingRequestCreateDto dto,
            OrganizationDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.PlatformId = 0;

            // Act
            var result = await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullLocalGovernment_ReturnsLocalGovernmentIdError(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgId = 0;

            // Act
            var result = await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains("lgId", result.Keys);
            Assert.Single(result["lgId"]);
            Assert.Equal($"Local Government ID ({dto.LgId}) does not exist.", result["lgId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_EmptyListingUrl_ReturnsListingUrlError(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            OrganizationDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidListingUrl_ReturnsInvalidUrlError(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            OrganizationDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            OrganizationDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenCalled_ShouldSendEmail(
            DelistingWarningCreateDto dto,
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
            // Act
            await sut.CreateDelistingWarningAsync(dto);

            // Assert
            emailServiceMock.Verify(m => m.SendEmailAsync(It.IsAny<EmailContent>()), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenHostEmailIsNotEmpty_AddsHostEmailToToList(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = "host@example.com";
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Act
            await sut.CreateDelistingWarningAsync(dto);

            // Assert
            Assert.Contains(dto.HostEmail, dto.ToList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenHostEmailIsEmpty_DoesNotAddHostEmailToToList(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = string.Empty;

            // Act
            await sut.CreateDelistingWarningAsync(dto);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Assert
            Assert.DoesNotContain(dto.HostEmail, dto.ToList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenSendCopyIsTrue_AddsCurrentUserEmailToCcList(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = true;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns(currentUserEmail);
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Act
            await sut.CreateDelistingWarningAsync(dto);

            // Assert
            Assert.Contains(currentUserEmail, dto.CcList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingWarningAsync_WhenSendCopyIsFalse_DoesNotAddCurrentUserEmailToCcList(
            DelistingWarningCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = false;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns("user@example.com");
            emailServiceMock.Setup(x => x.GetMessageReasonByMessageTypeAndId(It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new DropdownNumDto { Id = 1, Description = "reason1" });

            // Act
            await sut.CreateDelistingWarningAsync(dto);

            // Assert
            Assert.DoesNotContain(currentUserEmail, dto.CcList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenCalled_ShouldSendEmail(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            currentUserMock.Setup(m => m.EmailAddress).Returns("currentUser@example.com");
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);

            var lg = CommonUtils.CloneObject(platform);
            lg.OrganizationType = OrganizationTypes.LG;
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.LgId)).ReturnsAsync(lg);

            // Act
            await sut.CreateDelistingRequestAsync(dto);

            // Assert
            emailServiceMock.Verify(m => m.SendEmailAsync(It.IsAny<EmailContent>()), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenSendCopyIsTrue_AddsCurrentUserEmailToCcList(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            [Frozen] Mock<IOrganizationService> orgServiceMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = true;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns(currentUserEmail);
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.PlatformId)).ReturnsAsync(platform);

            var lg = CommonUtils.CloneObject(platform);
            lg.OrganizationType = OrganizationTypes.LG;
            orgServiceMock.Setup(x => x.GetOrganizationByIdAsync(dto.LgId)).ReturnsAsync(lg);

            // Act
            await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.Contains(currentUserEmail, dto.CcList);
        }

        [Theory]
        [AutoDomainData]
        public async Task SendDelistingRequestAsync_WhenSendCopyIsFalse_DoesNotAddCurrentUserEmailToCcList(
            DelistingRequestCreateDto dto,
            OrganizationDto platform,
            [Frozen] Mock<IEmailMessageService> emailServiceMock,
            [Frozen] Mock<ICurrentUser> currentUserMock,
            DelistingService sut)
        {
            // Arrange
            dto.SendCopy = false;
            var currentUserEmail = "user@example.com";
            currentUserMock.Setup(m => m.EmailAddress).Returns("user@example.com");

            // Act
            await sut.CreateDelistingRequestAsync(dto);

            // Assert
            Assert.DoesNotContain(currentUserEmail, dto.CcList);
        }
    }
}
