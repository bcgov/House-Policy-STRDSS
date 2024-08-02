using AutoMapper;
using CsvHelper.TypeConversion;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Service.CsvHelpers;
using System.Text;
using StrDss.Data.Repositories;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IUploadDeliveryService
    {
        Task<Dictionary<string, List<string>>> UploadPlatformData(string reportType, string reportPeriod, long orgId, Stream stream);
        Task<(Dictionary<string, List<string>>, string header)> ValidateAndParseUploadAsync(string reportPeriod, long orgId, string reportType, string hashValue, string[] mandatoryFields, TextReader textReader, List<DssUploadLine> uploadLines);
    }
    public class UploadDeliveryService : ServiceBase, IUploadDeliveryService
    {
        private IUploadDeliveryRepository _uploadRepo;
        private IOrganizationRepository _orgRepo;

        public UploadDeliveryService(
            IUploadDeliveryRepository uploadRepo, IOrganizationRepository orgRepo,
            ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _uploadRepo = uploadRepo;
            _orgRepo = orgRepo;
        }

        public async Task<Dictionary<string, List<string>>> UploadPlatformData(string reportType, string reportPeriod, long orgId, Stream stream)
        {
            byte[] sourceBin;
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                sourceBin = memoryStream.ToArray();
            }

            var hashValue = sourceBin.GetSha256Hash();

            stream.Position = 0;
            using TextReader textReader = new StreamReader(stream, Encoding.UTF8);

            var uploadLines = new List<DssUploadLine>();

            var mandatoryFields = GetMandatoryFields(reportType);

            var (errors, header) = await ValidateAndParseUploadAsync(reportPeriod, orgId, reportType, hashValue, mandatoryFields, textReader, uploadLines);

            if (errors.Count > 0) return errors;

            var entity = new DssUploadDelivery
            {
                UploadDeliveryType = reportType,
                ReportPeriodYm = new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1),
                SourceHashDsc = hashValue,
                SourceBin = sourceBin,
                ProvidingOrganizationId = orgId,
                UpdUserGuid = _currentUser.UserGuid,
                SourceHeaderTxt = header,
            };

            foreach (var line in uploadLines)
            {
                entity.DssUploadLines.Add(line);
            }

            await _uploadRepo.AddUploadDeliveryAsync(entity);

            _unitOfWork.Commit();

            return errors;
        }

        private string[] GetMandatoryFields(string reportType)
        {
            if (reportType == UploadDeliveryTypes.ListingData)
            {
                return new string[] { "rpt_period", "org_cd", "listing_id" };
            }
            else if (reportType == UploadDeliveryTypes.TakedownData)
            {
                return new string[] { "rpt_period", "rpt_type", "org_cd", "listing_id" };
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public async Task<(Dictionary<string, List<string>>, string header)> ValidateAndParseUploadAsync(string reportPeriod, long orgId, string reportType, string hashValue, string[] mandatoryFields, TextReader textReader, List<DssUploadLine> uploadLines)
        {
            var errors = new Dictionary<string, List<string>>();

            var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
            if (!Regex.IsMatch(reportPeriod, regex.Regex))
            {
                errors.AddItem("ReportPeriod", regex.ErrorMessage);
                return (errors, "");
            }

            var firstDayOfReportMonth = new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1);
            var firstDayOfCurrentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            if (firstDayOfReportMonth >= firstDayOfCurrentMonth)
            {
                errors.AddItem("ReportPeriod", "Report period cannot be current or future month.");
            }

            var isDuplicate = await _uploadRepo.IsDuplicateRentalReportUploadAsnyc(firstDayOfReportMonth, orgId, hashValue);
            if (isDuplicate)
            {
                errors.AddItem("File", "The file has already been uploaded");
                return (errors, "");
            }

            var platform = await _orgRepo.GetOrganizationByIdAsync(orgId);
            if (platform == null)
            {
                errors.AddItem("OrganizationId", $"Organization ID [{orgId}] doesn't exist.");
            }
            else if (platform.OrganizationType != OrganizationTypes.Platform)
            {
                errors.AddItem("OrganizationId", $"Organization type of the organization [{orgId}] is not {OrganizationTypes.Platform}.");
            }

            if (errors.Count > 0)
            {
                return (errors, "");
            }

            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            using var csv = new CsvReader(textReader, csvConfig);            

            csv.Read();
            var headerExists = csv.ReadHeader();

            if (!headerExists)
            {
                errors.AddItem("File", "Header deosn't exist.");
                return (errors, "");
            }

            var header = csv.Parser.RawRecord;

            if (reportType == UploadDeliveryTypes.ListingData && csv.HeaderRecord.Contains("rpt_type"))
            {
                errors.AddItem("File", "The listing data file contains an invalid column: 'rpt_type'. Please remove the 'rpt_type' column and try again.");
                return (errors, "");
            }

            if (!CheckCommonMandatoryFields(csv.HeaderRecord, mandatoryFields, errors))
            {
                return (errors, header);
            }

            var reportPeriodMismatch = 0;
            var reportPeriodMissing = 0;
            var reportTypeMismatch = 0;
            var orgCdMissing = 0;
            var invalidOrgCds = new List<string>();
            var listingIdMissing = 0;
            var listingIds = new List<string>();
            var duplicateListingIds = new List<string>();

            var orgCds = new List<string>();

            while (csv.Read())
            {
                UploadLine row = null!;

                try
                {
                    row = csv.GetRecord<UploadLine>();

                    if (row.RptPeriod != reportPeriod) reportPeriodMismatch++;
                    if (row.RptPeriod.IsEmpty()) reportPeriodMissing++;
                    if (row.OrgCd.IsEmpty()) orgCdMissing++;
                    if (row.ListingId.IsEmpty()) listingIdMissing++;

                    if (mandatoryFields.Contains("rpt_type") && row.RptType != reportType)
                    {
                        reportTypeMismatch++;
                    }

                    if (row.OrgCd.IsNotEmpty() && !orgCds.Contains(row.OrgCd.ToUpper())) orgCds.Add(row.OrgCd.ToUpper());
                    if (!listingIds.Contains($"{row.OrgCd}-{row.ListingId.ToLower()}"))
                    {
                        listingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                        uploadLines.Add(new DssUploadLine
                        {
                            IsValidationFailure = false,
                            IsSystemFailure = false,
                            IsProcessed = false,
                            SourceOrganizationCd = row.OrgCd,
                            SourceRecordNo = row.ListingId,
                            SourceLineTxt = csv.Parser.RawRecord
                        });
                    }
                    else
                    {
                        duplicateListingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                    }
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelper.MissingFieldException)
                {
                    break;
                }
                catch (CsvHelper.ReaderException ex)
                {
                    _logger.LogWarning(ex.Message);
                    throw;
                }
                catch (CsvHelperException ex)
                {
                    _logger.LogInformation(ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }

            var validOrgCds = await _orgRepo.GetManagingOrgCdsAsync(orgId);

            foreach (var org in orgCds)
            {
                if (!validOrgCds.Contains(org))
                {
                    invalidOrgCds.Add(org);
                }
            }

            if (reportPeriodMismatch > 0)
            {
                errors.AddItem("rpt_period", $"Report period mismatch found in {reportPeriodMismatch} record(s). The file contains report period(s) other than the selected period of {reportPeriod}");
            }

            if (reportPeriodMissing > 0)
            {
                errors.AddItem("rpt_period", $"Report period missing in {reportPeriodMissing} record(s). Please provide a report period.");
            }

            if (reportTypeMismatch > 0)
            {
                errors.AddItem("rpt_type", $"Report type  mismatch found in {reportTypeMismatch} record(s). The file contains report type other than the '{reportType}'");
            }

            if (orgCdMissing > 0)
            {
                errors.AddItem("org_cd", $"Organization code missing in {orgCdMissing} record(s). Please provide an organization code.");
            }

            if (invalidOrgCds.Count > 0)
            {
                errors.AddItem("org_cd", $"Invalid organization code(s) found: {string.Join(", ", invalidOrgCds.ToArray())}. Please use one of the following valid organization code(s): {string.Join(", ", validOrgCds)}");
            }

            if (listingIdMissing > 0)
            {
                errors.AddItem("listing_id", $"Listing ID missing in {listingIdMissing} record(s). Please provide a listing ID.");
            }

            if (duplicateListingIds.Count > 0)
            {
                errors.AddItem("listing_id", $"Duplicate listing ID(s) found: {string.Join(", ", duplicateListingIds.ToArray())}. Each listing ID must be unique within an organization code.");
            }

            return (errors, header);
        }

        private bool CheckCommonMandatoryFields(string[] headers, string[] mandatoryFields, Dictionary<string, List<string>> errors)
        {
            headers = CsvHelperUtils.GetLowercaseFieldsFromCsvHeaders(headers);

            foreach (var field in mandatoryFields)
            {
                if (!headers.Any(x => x.ToLower() == field))
                    errors.AddItem("File", $"Header [{field}] is missing");
            }

            if (errors.Count > 0)
                errors.AddItem("File", "Please ensure the file headers are correct");

            return errors.Count == 0;
        }

        private async Task<Dictionary<string, List<string>>> ParseAndValidate(string reportPeriod, long orgId, CsvReader csv, List<DssUploadLine> uploadLines)
        {
            var errors = new Dictionary<string, List<string>>();

            var reportPeriodMismatch = 0;
            var reportPeriodMissing = 0;
            var orgCdMissing = 0;
            var invalidOrgCds = new List<string>();
            var listingIdMissing = 0;
            var listingIds = new List<string>();
            var duplicateListingIds = new List<string>();

            var orgCds = new List<string>();

            while (csv.Read())
            {
                UploadLine row = null!;

                try
                {
                    row = csv.GetRecord<UploadLine>();

                    if (row.RptPeriod != reportPeriod) reportPeriodMismatch++;
                    if (row.RptPeriod.IsEmpty()) reportPeriodMissing++;
                    if (row.OrgCd.IsEmpty()) orgCdMissing++;
                    if (row.ListingId.IsEmpty()) listingIdMissing++;

                    if (row.OrgCd.IsNotEmpty() && !orgCds.Contains(row.OrgCd.ToUpper())) orgCds.Add(row.OrgCd.ToUpper());
                    if (!listingIds.Contains($"{row.OrgCd}-{row.ListingId.ToLower()}"))
                    {
                        listingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                        uploadLines.Add(new DssUploadLine
                        {
                            IsValidationFailure = false,
                            IsSystemFailure = false,
                            IsProcessed = false,
                            SourceOrganizationCd = row.OrgCd,
                            SourceRecordNo = row.ListingId,
                            SourceLineTxt = csv.Parser.RawRecord
                        });
                    }
                    else
                    {
                        duplicateListingIds.Add($"{row.OrgCd}-{row.ListingId.ToLower()}");
                    }
                }
                catch (TypeConverterException ex)
                {
                    errors.AddItem(ex.MemberMapData.Member.Name, ex.Message);
                    break;
                }
                catch (CsvHelper.MissingFieldException)
                {
                    break;
                }
                catch (CsvHelper.ReaderException ex)
                {
                    _logger.LogWarning(ex.Message);
                    throw;
                }
                catch (CsvHelperException ex)
                {
                    _logger.LogInformation(ex.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }

            var validOrgCds = await _orgRepo.GetManagingOrgCdsAsync(orgId);

            foreach (var org in orgCds)
            {
                if (!validOrgCds.Contains(org))
                {
                    invalidOrgCds.Add(org);
                }
            }

            if (reportPeriodMismatch > 0)
            {
                errors.AddItem("rpt_period", $"Report period mismatch found in {reportPeriodMismatch} record(s). The file contains report period(s) other than the selected period of {reportPeriod}");
            }

            if (reportPeriodMissing > 0)
            {
                errors.AddItem("rpt_period", $"Report period missing in {reportPeriodMissing} record(s). Please provide a report period.");
            }

            if (orgCdMissing > 0)
            {
                errors.AddItem("org_cd", $"Organization code missing in {orgCdMissing} record(s). Please provide an organization code.");
            }

            if (invalidOrgCds.Count > 0)
            {
                errors.AddItem("org_cd", $"Invalid organization code(s) found: {string.Join(", ", invalidOrgCds.ToArray())}. Please use one of the following valid organization code(s): {string.Join(", ", validOrgCds)}");
            }

            if (listingIdMissing > 0)
            {
                errors.AddItem("listing_id", $"Listing ID missing in {listingIdMissing} record(s). Please provide a listing ID.");
            }

            if (duplicateListingIds.Count > 0)
            {
                errors.AddItem("listing_id", $"Duplicate listing ID(s) found: {string.Join(", ", duplicateListingIds.ToArray())}. Each listing ID must be unique within an organization code.");
            }

            return errors;
        }
    }
}
