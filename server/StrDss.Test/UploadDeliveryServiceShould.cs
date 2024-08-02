using AutoFixture.Xunit2;
using CsvHelper.Configuration;
using Moq;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model.OrganizationDtos;
using System.Globalization;
using Xunit;
using StrDss.Service;
using System.IO;

namespace StrDss.Test
{
    public class UploadDeliveryServiceShould
    {
        private static StringReader GetCsvData(string reportPeriod)
        {
            var headerRecord = "rpt_period, org_cd, listing_id";
            var record1 = $"{reportPeriod}, org1, 1";
            var record2 = $"{reportPeriod}, org2, 2";
            var csvData = headerRecord + Environment.NewLine + record1 + Environment.NewLine + record2;
            var textReader = new StringReader(csvData);
            return textReader;
        }

        private string[] mandatoryFields =  new string[] { "rpt_period", "org_cd", "listing_id" };
        private string reportType = UploadDeliveryTypes.ListingData;

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_InvalidReportPeriod_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "202403";
            var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("ReportPeriod", result.Keys);
            Assert.Single(result["ReportPeriod"]);
            Assert.Equal(regex.ErrorMessage, result["ReportPeriod"].First());
        }


        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_EmptyReportPeriod_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
             var reportPeriod = "";
            var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("ReportPeriod", result.Keys);
            Assert.Single(result["ReportPeriod"]);
            Assert.Equal(regex.ErrorMessage, result["ReportPeriod"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_CurrentOrFutureReportPeriod_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = $"{DateTime.UtcNow.ToString("yyyy-MM")}";
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("ReportPeriod", result.Keys);
            Assert.Single(result["ReportPeriod"]);
            Assert.Equal("Report period cannot be current or future month.", result["ReportPeriod"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_DuplicateUpload_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(true);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("File", result.Keys);
            Assert.Single(result["File"]);
            Assert.Equal("The file has already been uploaded", result["File"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_ValidInput_ReturnsNoErrors(
           long orgId,
           string hashValue,
           List<DssUploadLine> uploadLines,
           [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
           [Frozen] Mock<IOrganizationRepository> orgRepoMock,
           UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var platform = new OrganizationDto { OrganizationType = OrganizationTypes.Platform };
            var validOrgCds = new List<string> { "ORG1", "ORG2" };
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(platform);
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(validOrgCds);

            using StringReader textReader = GetCsvData(reportPeriod);

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Empty(result);
        }


        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_InvalidOrgId_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), It.IsAny<long>(), hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(It.IsAny<long>())).ReturnsAsync(null as OrganizationDto);

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("OrganizationId", result.Keys);
            Assert.Single(result["OrganizationId"]);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_ReportPeriodMismatch_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var record = "2024-04, org1, 1";
            var csvData = "rpt_period, org_cd, listing_id\n" + record;
            textReader = new StringReader(csvData);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("rpt_period", result.Keys);
            Assert.Single(result["rpt_period"]);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_ReportPeriodMissing_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var record = ", org1, 1";
            var csvData = "rpt_period, org_cd, listing_id\n" + record;
            textReader = new StringReader(csvData);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("rpt_period", result.Keys);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_OrgCodeMissing_ReturnsError(
            long orgId,
            string hashValue,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var record = "2024-03, , 1";
            var csvData = "rpt_period, org_cd, listing_id\n" + record;
            var textReader = new StringReader(csvData);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("org_cd", result.Keys);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_InvalidOrgCd_ReturnsError(
            long orgId,
            string hashValue,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var platform = new OrganizationDto { OrganizationType = OrganizationTypes.Platform };
            var validOrgCds = new List<string> { "ORG1", "ORG3" };
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(platform);
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(validOrgCds);

            using StringReader textReader = GetCsvData(reportPeriod);

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("org_cd", result.Keys);
            Assert.Single(result["org_cd"]);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_MissingListingId_ReturnsError(
            long orgId,
            string hashValue,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var record = "2024-03, org_1, ";
            var csvData = "rpt_period, org_cd, listing_id\n" + record;
            var textReader = new StringReader(csvData);

            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("listing_id", result.Keys);
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAndParseUploadAsync_DuplicateListings_ReturnsError(
            long orgId,
            string hashValue,
            [Frozen] TextReader textReader,
            List<DssUploadLine> uploadLines,
            [Frozen] Mock<IUploadDeliveryRepository> uploadRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UploadDeliveryService listingService)
        {
            // Arrange
            var reportPeriod = "2024-03";
            var duplicateRecord = "202403, org1, 1";
            var csvData = "rpt_period, org_cd, listing_id\n" + duplicateRecord + "\n" + duplicateRecord;
            textReader = new StringReader(csvData);
            var errors = new Dictionary<string, List<string>>();

            uploadRepoMock.Setup(x => x.IsDuplicateRentalReportUploadAsnyc(It.IsAny<DateOnly>(), orgId, hashValue)).ReturnsAsync(false);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(orgId)).ReturnsAsync(new OrganizationDto { OrganizationType = OrganizationTypes.Platform });
            orgRepoMock.Setup(x => x.GetManagingOrgCdsAsync(orgId)).ReturnsAsync(new List<string>());

            // Act
            var (result, header) = await listingService.ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            // Assert
            Assert.Contains("listing_id", result.Keys);
            Assert.Single(result["listing_id"]);
            Assert.Equal("Duplicate listing ID(s) found: org1-1. Each listing ID must be unique within an organization code.", result["listing_id"].First());
        }
    }
}
