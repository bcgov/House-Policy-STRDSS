//using AutoFixture.Xunit2;
//using CsvHelper.Configuration;
//using CsvHelper;
//using Microsoft.Extensions.Logging;
//using Moq;
//using StrDss.Common;
//using StrDss.Data.Entities;
//using StrDss.Data.Repositories;
//using StrDss.Model.OrganizationDtos;
//using StrDss.Model.RentalReportDtos;
//using System.Globalization;
//using Xunit;
//using StrDss.Service;

//namespace StrDss.Test
//{
//    public class RentalListingReportServiceShould
//    {
//        [Theory]
//        [AutoDomainData]
//        public async Task ValidateAndParseUploadAsync_InvalidReportPeriod_ReturnsError(
//            string reportPeriod,
//            long orgId,
//            string hashValue,
//            [Frozen] TextReader textReader,
//            List<DssUploadLine> uploadLines,
//            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
//            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
//            [Frozen] Mock<ILogger<StrDssLogger>> loggerMock,
//            RentalListingReportService listingService)
//        {
//            // Arrange
//            var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
//            var errors = new Dictionary<string, List<string>>();

//            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
//            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
//            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

//            // Act
//            var result = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

//            // Assert
//            Assert.Contains("ReportPeriod", result.Keys);
//            Assert.Single(result["ReportPeriod"]);
//            Assert.Equal(regex.ErrorMessage, result["ReportPeriod"].First());
//        }

//        [Theory]
//        [AutoDomainData]
//        public async Task ValidateAndParseUploadAsync_DuplicateUpload_ReturnsError(
//            string reportPeriod,
//            long orgId,
//            string hashValue,
//            [Frozen] TextReader textReader,
//            List<DssUploadLine> uploadLines,
//            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
//            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
//            [Frozen] Mock<ILogger<StrDssLogger>> loggerMock,
//            RentalListingReportService listingService)
//        {
//            // Arrange
//            var errors = new Dictionary<string, List<string>>();

//            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(true);
//            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
//            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

//            // Act
//            var result = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

//            // Assert
//            Assert.Contains("File", result.Keys);
//            Assert.Single(result["File"]);
//            Assert.Equal("The file has already been uploaded", result["File"].First());
//        }

//        [Theory]
//        [AutoDomainData]
//        public async Task ValidateAndParseUploadAsync_ValidInput_ReturnsNoErrors(
//            string reportPeriod,
//            long orgId,
//            string hashValue,
//            [Frozen] TextReader textReader,
//            List<DssUploadLine> uploadLines,
//            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
//            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
//            [Frozen] Mock<ILogger<StrDssLogger>> loggerMock,
//            RentalListingReportService listingService)
//        {
//            // Arrange
//            var platform = new OrganizationDto { OrganizationType = OrganizationTypes.Platform };
//            var validOrgCds = new List<string> { "org1", "org2" };
//            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

//            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
//            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(platform);
//            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(validOrgCds);

//            var csvData = new List<string[]>
//        {
//            new string[] { "rpt_period", "org_cd", "listing_id" },
//            new string[] { reportPeriod, "org1", "1" },
//            new string[] { reportPeriod, "org2", "2" }
//        };

//            var csvReaderMock = new Mock<CsvReader>(textReader, csvConfig);
//            csvReaderMock.SetupSequence(x => x.Read())
//                .Returns(true)  // Read header
//                .Returns(true)  // Read first record
//                .Returns(true); // Read second record

//            csvReaderMock.Setup(x => x.ReadHeader()).Returns(true);
//            csvReaderMock.Setup(x => x.GetRecord<RentalListingRowUntyped>())
//                .Returns(new RentalListingRowUntyped { RptPeriod = reportPeriod, OrgCd = "org1", ListingId = "1" });

//            var csvReader = csvReaderMock.Object;

//            // Act
//            var result = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

//            // Assert
//            Assert.Empty(result);
//        }

//        [Theory]
//        [AutoDomainData]
//        public async Task ValidateAndParseUploadAsync_CurrentOrFutureReportPeriod_ReturnsError(
//            string reportPeriod,
//            long orgId,
//            string hashValue,
//            [Frozen] TextReader textReader,
//            List<DssUploadLine> uploadLines,
//            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
//            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
//            [Frozen] Mock<ILogger<StrDssLogger>> loggerMock,
//            RentalListingReportService listingService)
//        {
//            // Arrange
//            var firstDayOfReportMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
//            var errors = new Dictionary<string, List<string>>();

//            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
//            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
//            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

//            // Act
//            var result = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

//            // Assert
//            Assert.Contains("ReportPeriod", result.Keys);
//            Assert.Single(result["ReportPeriod"]);
//            Assert.Equal("Report period cannot be current or future month.", result["ReportPeriod"].First());
//        }

//        [Theory]
//        [AutoDomainData]
//        public async Task ValidateAndParseUploadAsync_InvalidOrganizationId_ReturnsError(
//            string reportPeriod,
//            long orgId,
//            string hashValue,
//            [Frozen] TextReader textReader,
//            List<DssUploadLine> uploadLines,
//            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
//            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
//            [Frozen] Mock<ILogger<StrDssLogger>> loggerMock,
//            RentalListingReportService listingService)
//        {
//            // Arrange
//            var errors = new Dictionary<string, List<string>>();

//            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
//            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync((OrganizationDto)null);

//            // Act
//            var result = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, hashValue, textReader, uploadLines);

//            // Assert
//            Assert.Contains("OrganizationId", result.Keys);
//            Assert.Single(result["OrganizationId"]);
//            Assert.Equal($"Organization ID [{orgId}] doesn't exist.", result["OrganizationId"].First());
//        }

//    }
//}
