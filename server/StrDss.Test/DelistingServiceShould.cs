using AutoFixture.Xunit2;
using Castle.Core.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StrDss.Model.DelistingDtos;
using StrDss.Model.LocalGovernmentDtos;
using StrDss.Model.PlatformDtos;
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
            PlatformDto platform,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            configMock.Setup(x => x.GetValue(typeof(string), "" )).Returns("https://ches.example.com");

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, "reason");

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
            PlatformDto platform = null;

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyListingUrl_ReturnsListingUrlError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidListingUrl_ReturnsInvalidUrlError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_HostEmailSentFalseAndEmptyHostEmail_ReturnsHostEmailRequiredError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
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
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is required", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidHostEmail_ReturnsInvalidHostEmailError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.HostEmail = "invalidemail";

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("hostEmail", result.Keys);
            Assert.Single(result["hostEmail"]);
            Assert.Equal("Host email is invalid", result["hostEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_NullReason_ReturnsReasonIdError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            reason = null;

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("reasonId", result.Keys);
            Assert.Single(result["reasonId"]);
            Assert.Equal($"Reason ID ({dto.ReasonId}) does not exist.", result["reasonId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_EmptyLgContactEmail_ReturnsLgContactEmailRequiredError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = string.Empty;

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is required", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactEmail_ReturnsInvalidLgContactEmailError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactEmail = "invalidemail";

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("lgContactEmail", result.Keys);
            Assert.Single(result["lgContactEmail"]);
            Assert.Equal("Local government contact email is invalid", result["lgContactEmail"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidLgContactPhone_ReturnsInvalidLgContactPhoneError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.LgContactPhone = "invalidphone";

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("lgContactPhone", result.Keys);
            Assert.Single(result["lgContactPhone"]);
            Assert.Equal("Local government contact phone number is invalid", result["lgContactPhone"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingWarning_InvalidStrBylawUrl_ReturnsStrBylawUrlRequiredError(
            DelistingWarningCreateDto dto,
            PlatformDto platform,
            string reason,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.StrBylawUrl = "invalidurl";

            // Act
            var result = await sut.ValidateDelistingWarning(dto, platform, reason);

            // Assert
            Assert.Contains("strByLawUrl", result.Keys);
            Assert.Single(result["strByLawUrl"]);
            Assert.Equal("URL is required", result["strByLawUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullPlatform_ReturnsPlatformIdError(
            DelistingRequestCreateDto dto,
            LocalGovernmentDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            PlatformDto platform = null;

            // Act
            var result = await sut.ValidateDelistingRequest(dto, platform, lg);

            // Assert
            Assert.Contains("platformId", result.Keys);
            Assert.Single(result["platformId"]);
            Assert.Equal($"Platform ID ({dto.PlatformId}) does not exist.", result["platformId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_NullLocalGovernment_ReturnsLocalGovernmentIdError(
            DelistingRequestCreateDto dto,
            PlatformDto platform,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            LocalGovernmentDto lg = null;

            // Act
            var result = await sut.ValidateDelistingRequest(dto, platform, lg);

            // Assert
            Assert.Contains("lgId", result.Keys);
            Assert.Single(result["lgId"]);
            Assert.Equal($"Local Government ID ({dto.LgId}) does not exist.", result["lgId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_EmptyListingUrl_ReturnsListingUrlError(
            DelistingRequestCreateDto dto,
            PlatformDto platform,
            LocalGovernmentDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = string.Empty;

            // Act
            var result = await sut.ValidateDelistingRequest(dto, platform, lg);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Listing URL is required", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidListingUrl_ReturnsInvalidUrlError(
            DelistingRequestCreateDto dto,
            PlatformDto platform,
            LocalGovernmentDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.ListingUrl = "invalidurl";

            // Act
            var result = await sut.ValidateDelistingRequest(dto, platform, lg);

            // Assert
            Assert.Contains("listingUrl", result.Keys);
            Assert.Single(result["listingUrl"]);
            Assert.Equal("Invalid URL", result["listingUrl"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateDelistingRequest_InvalidCcListEmail_ReturnsInvalidCcListEmailError(
            DelistingRequestCreateDto dto,
            PlatformDto platform,
            LocalGovernmentDto lg,
            [Frozen] Mock<IConfiguration> configMock,
            [Frozen] Mock<IChesTokenApi> chesTokenApiMock,
            [Frozen] Mock<ILogger<DelistingService>> loggerMock,
            DelistingService sut)
        {
            // Arrange
            dto.CcList = new List<string> { "invalidemail" };

            // Act
            var result = await sut.ValidateDelistingRequest(dto, platform, lg);

            // Assert
            Assert.Contains("ccList", result.Keys);
            Assert.Single(result["ccList"]);
            Assert.Equal("Email (invalidemail) is invalid", result["ccList"].First());
        }
    }
}
