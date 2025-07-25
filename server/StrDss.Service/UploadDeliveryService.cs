﻿using AutoMapper;
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
        Task<Dictionary<string, List<string>>> UploadData(string reportType, string reportPeriod, long orgId, Stream stream);
        Task<(Dictionary<string, List<string>>, string header)> ValidateAndParseUploadAsync(string reportPeriod, long orgId, string reportType, string hashValue, string[] mandatoryFields, TextReader textReader, List<DssUploadLine> uploadLines);
        Task<PagedDto<UploadHistoryViewDto>> GetUploadHistory(long? orgId, int pageSize, int pageNumber, string orderBy, string direction, string[] reportTypes);
        Task<(byte[]?, bool hasAccess)> GetErrorFile(long uploadId);
        Task<DssUploadDelivery?> GetNonTakedownUploadToProcessAsync();
        Task<DssUploadDelivery?> GetUploadToProcessAsync(string reportType);
        Task<byte[]?> DownloadValidationReportAsync(long uploadId);
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

        public async Task<Dictionary<string, List<string>>> UploadData(string reportType, string reportPeriod, long orgId, Stream stream)
        {
            if (_currentUser.OrganizationType != OrganizationTypes.BCGov && _currentUser.OrganizationId != orgId)
            {
                var authError = new Dictionary<string, List<string>>();
                authError.AddItem("OrganizationId", $"The user is not authorized to upload the file. The user's organization ({_currentUser.OrganizationId}) is not {orgId}.");
            }

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

            DateOnly? reportPeriodYm = 
                mandatoryFields.Contains("rpt_period") ? new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1) : null;

            DateTime utcNow = DateTime.UtcNow;
            var entity = new DssUploadDelivery
            {
                UploadDeliveryType = reportType,
                ReportPeriodYm = reportPeriodYm,
                SourceHashDsc = hashValue,
                SourceBin = sourceBin,
                ProvidingOrganizationId = orgId,
                UploadUserGuid = _currentUser.UserGuid,
                SourceHeaderTxt = header,
                UploadStatus = UploadStatus.Pending,
                RegistrationStatus = UploadStatus.Pending,
                UploadLinesTotal = uploadLines.Count,
                UploadDate = utcNow,
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
                return ["rpt_period", "org_cd", "listing_id"];
            }
            else if (reportType == UploadDeliveryTypes.TakedownData)
            {
                return ["rpt_period", "rpt_type", "org_cd", "listing_id", "reason"];
            }
            else if (reportType == UploadDeliveryTypes.LicenceData)
            {
                return ["org_cd", "bus_lic_no", "bus_lic_exp_dt", "rental_address"];
            }
            else if (reportType == UploadDeliveryTypes.RegistrationData)
            {
                return ["reg_no", "rental_street", "rental_postal", "rental_address"];
            }
            else
            {
                return [];
            }
        }

        public async Task<(Dictionary<string, List<string>>, string header)> ValidateAndParseUploadAsync(string reportPeriod, long orgId, 
            string reportType, string hashValue, string[] mandatoryFields, TextReader textReader, List<DssUploadLine> uploadLines)
        {
            var errors = new Dictionary<string, List<string>>();
            DateOnly? firstDayOfReportMonth = null;

            if (mandatoryFields.Contains("rpt_period"))
            {
                var regex = RegexDefs.GetRegexInfo(RegexDefs.YearMonth);
                if (!Regex.IsMatch(reportPeriod, regex.Regex))
                {
                    errors.AddItem("ReportPeriod", regex.ErrorMessage);
                    return (errors, "");
                }

                firstDayOfReportMonth = new DateOnly(Convert.ToInt32(reportPeriod.Substring(0, 4)), Convert.ToInt32(reportPeriod.Substring(5, 2)), 1);
                var firstDayOfCurrentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                if (firstDayOfReportMonth >= firstDayOfCurrentMonth)
                {
                    errors.AddItem("ReportPeriod", "Report period cannot be current or future month.");
                    return (errors, "");
                }
            }

            if (!mandatoryFields.Contains("reg_no"))
            {
                var isDuplicate = await _uploadRepo.IsDuplicateRentalReportUploadAsnyc(firstDayOfReportMonth, orgId, hashValue);
                if (isDuplicate)
                {
                    errors.AddItem("File", "The file has already been uploaded");
                    return (errors, "");
                }
            }            

            var org = await _orgRepo.GetOrganizationByIdAsync(orgId);
            if (org == null)
            {
                errors.AddItem("OrganizationId", $"Organization ID [{orgId}] doesn't exist.");
                return (errors, "");
            }

            if (reportType == UploadDeliveryTypes.LicenceData)
            {
                if (org.OrganizationType != OrganizationTypes.LG)
                {
                    errors.AddItem("OrganizationId", $"Organization type of the organization [{orgId}] is not {OrganizationTypes.LG}.");
                }
            }
            else
            {
                if (org.OrganizationType != OrganizationTypes.Platform)
                {
                    errors.AddItem("OrganizationId", $"Organization type of the organization [{orgId}] is not {OrganizationTypes.Platform}.");
                }
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
            var bizLicenceMissing = 0;
            var takedownReasonMismatch = 0;
            var registrationDataMissing = 0;
            var regNoMissing = 0;
            var rentalStreetMissing = 0;
            var rentalPostalMissing = 0;

            var listingIds = new List<string>();
            var duplicateListingIds = new List<string>();            

            var bizLicences = new List<string>();
            var duplicateBizLicences = new List<string>();

            var orgCds = new List<string>();

            while (csv.Read())
            {
                UploadLine row = null!;

                try
                {
                    row = csv.GetRecord<UploadLine>();

                    if (mandatoryFields.Contains("rpt_period"))
                    {
                        var normalizedRowPeriod = DateUtils.NormalizeReportPeriod(row.RptPeriod);
                        if (normalizedRowPeriod == null || reportPeriod != normalizedRowPeriod)
                        {
                            reportPeriodMismatch++;
                        }
                        if (row.RptPeriod.IsEmpty()) reportPeriodMissing++;
                    }

                    if (row.OrgCd.IsEmpty()) orgCdMissing++;

                    if (mandatoryFields.Contains("listing_id") && row.ListingId.IsEmpty()) listingIdMissing++;

                    if (mandatoryFields.Contains("bus_lic_no") && row.BusinessLicenceNo.IsEmpty()) bizLicenceMissing++;

                    if (mandatoryFields.Contains("rpt_type") && row.RptType != reportType)
                    {
                        reportTypeMismatch++;
                    }

                    if(mandatoryFields.Contains("reason"))
                    {
                        if (row.TakeDownReason.IsEmpty())
                        {
                            takedownReasonMismatch++;
                        }
                        else if (row.TakeDownReason != TakeDownReasonStatus.LGRequest && row.TakeDownReason != TakeDownReasonStatus.InvalidRegistration)
                        {
                            takedownReasonMismatch++;
                        }
                    }

                    if (row.OrgCd.IsNotEmpty() && !orgCds.Contains(row.OrgCd.ToUpper())) orgCds.Add(row.OrgCd.ToUpper());

                    if (mandatoryFields.Contains("listing_id"))
                    {
                        if (!listingIds.Contains($"{row.OrgCd}-{row.ListingId.ToUpper()}"))
                        {
                            listingIds.Add($"{row.OrgCd}-{row.ListingId.ToUpper()}");
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
                            duplicateListingIds.Add($"{row.OrgCd}-{row.ListingId.ToUpper()}");
                        }
                    }

                    if (mandatoryFields.Contains("bus_lic_no"))
                    {
                        if (!bizLicences.Contains($"{row.OrgCd}-{row.BusinessLicenceNo.ToUpper()}"))
                        {
                            bizLicences.Add($"{row.OrgCd}-{row.BusinessLicenceNo.ToUpper()}");
                            uploadLines.Add(new DssUploadLine
                            {
                                IsValidationFailure = false,
                                IsSystemFailure = false,
                                IsProcessed = false,
                                SourceOrganizationCd = row.OrgCd,
                                SourceRecordNo = row.BusinessLicenceNo,
                                SourceLineTxt = csv.Parser.RawRecord
                            });
                        }
                        else
                        {
                            duplicateBizLicences.Add($"{row.OrgCd}-{row.BusinessLicenceNo.ToUpper()}");
                        }
                    }

                    if (mandatoryFields.Contains("reg_no")) 
                    {
                        // Determine if 'rental_address' is provided
                        bool hasRentalAddress = !row.RentalAddress.IsEmpty();

                        // Determine if 'reg_no', 'rental_street', and 'rental_postal' are provided
                        bool hasRegNo = !row.RegNo.IsEmpty();
                        bool hasRentalStreet = !row.RentalStreet.IsEmpty();
                        bool hasRentalPostal = !row.RentalPostal.IsEmpty();
                        bool hasCompleteAddressFields = hasRegNo && hasRentalStreet && hasRentalPostal;

                        // Validate that either 'rental_address' is provided, or all 'reg_no', 'rental_street', and 'rental_postal' are provided
                        if (!hasRentalAddress && !hasCompleteAddressFields)
                        {
                            registrationDataMissing++;
                            if (!hasRegNo) regNoMissing++;
                            if (!hasRentalStreet) rentalStreetMissing++;
                            if (!hasRentalPostal) rentalPostalMissing++;
                            continue; // Skip further processing for this row
                        }

                        // Add to uploadLines
                        uploadLines.Add(new DssUploadLine
                        {
                            IsValidationFailure = false,
                            IsSystemFailure = false,
                            IsProcessed = false,
                            SourceOrganizationCd = org.OrganizationCd,
                            SourceRecordNo = $"{Guid.NewGuid()}",
                            SourceLineTxt = csv.Parser.RawRecord
                        });
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

            foreach (var orgCd in orgCds)
            {
                if (!validOrgCds.Contains(orgCd))
                {
                    invalidOrgCds.Add(orgCd);
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

            if (bizLicenceMissing > 0)
            {
                errors.AddItem("bus_lic_no", $"Business Licence No missing in {bizLicenceMissing} record(s). Please provide a Business Licence No.");
            }

            if (duplicateBizLicences.Count > 0)
            {
                errors.AddItem("bus_lic_no", $"Duplicate Business Licence No(s) found: {string.Join(", ", duplicateBizLicences.ToArray())}. Each Business Licence No must be unique within an organization code.");
            }

            if (takedownReasonMismatch > 0)
            {
                errors.AddItem("reason", $"Takedown reason missing/mismatch found in {takedownReasonMismatch} record(s). The file contains missing/mismatch takedown reason(s).");
            }

            if (registrationDataMissing > 0)
            {
                errors.AddItem("RegistrationData", $"Either 'rental_address' or all of 'reg_no', 'rental_street', and 'rental_postal' must be provided. Missing in {registrationDataMissing} record(s).");
            }

            if (regNoMissing > 0)
            {
                errors.AddItem("reg_no", $"Registration No missing in {regNoMissing} record(s). Please provide a Registration No.");
            }

            if (rentalStreetMissing > 0)
            {
                errors.AddItem("rental_street", $"Rental Street missing in {rentalStreetMissing} record(s). Please provide a Rental Street.");
            }

            if (rentalPostalMissing > 0)
            {
                errors.AddItem("rental_postal", $"Rental Postal missing in {rentalPostalMissing} record(s). Please provide a Rental Postal.");
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

        public async Task<PagedDto<UploadHistoryViewDto>> GetUploadHistory(long? orgId, int pageSize, int pageNumber, string orderBy, string direction, string[] reportTypes)
        {
            return await _uploadRepo.GetUploadHistory(orgId, pageSize, pageNumber, orderBy, direction, reportTypes);
        }

        public async Task<(byte[]?, bool hasAccess)> GetErrorFile(long uploadId)
        {
            var upload = await _uploadRepo.GetRentalListingUploadWithErrors(uploadId);

            if (upload == null) return (null, true);

            // so far, there are two types of error files - ListingData and LicenceData
            var licenceData = upload.UploadDeliveryType == UploadDeliveryTypes.LicenceData;

            var hasPermission = licenceData
                ? _currentUser.Permissions.Contains(Permissions.LicenceFileUpload)
                : _currentUser.Permissions.Contains(Permissions.ListingFileUpload);

            if (!hasPermission)
            {
                return (null, false);
            }

            var linesWithError = await _uploadRepo.GetUploadLineIdsWithErrors(uploadId, licenceData);

            var memoryStream = new MemoryStream(upload.SourceBin!);
            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);

            var errors = new Dictionary<string, List<string>>();
            var csvConfig = CsvHelperUtils.GetConfig(errors, false);

            using var csv = new CsvReader(textReader, csvConfig);

            var contents = new StringBuilder();

            csv.Read();
            var header = "errors," + csv.Parser.RawRecord.TrimEndNewLine();

            contents.AppendLine(header);

            foreach (var lineId in linesWithError)
            {
                var line = await _uploadRepo.GetUploadLineWithError(lineId);
                contents.AppendLine($"\"{line.ErrorText ?? ""}\"," + line.LineText.TrimEndNewLine());
            }

            return (Encoding.UTF8.GetBytes(contents.ToString()), hasPermission);
        }

        public async Task<DssUploadDelivery?> GetNonTakedownUploadToProcessAsync()
        {
            return await _uploadRepo.GetNonTakedownUploadToProcessAsync();
        }

        public async Task<DssUploadDelivery?> GetUploadToProcessAsync(string reportType)
        {
            return await _uploadRepo.GetUploadToProcessAsync(reportType);
        }

        public async Task<byte[]?> DownloadValidationReportAsync(long uploadId)
        {
            var upload = await _uploadRepo.GetUploadDeliveryAsync(uploadId);
            if (upload == null) return null;
            var lines = await _uploadRepo.GetUploadLineIdsWithErrors(uploadId, true);

            var memoryStream = new MemoryStream(upload.SourceBin!);
            using TextReader textReader = new StreamReader(memoryStream, Encoding.UTF8);

            var report = new Dictionary<string, List<string>>();
            var csvConfig = CsvHelperUtils.GetConfig(report, false);

            using var csv = new CsvReader(textReader, csvConfig);

            var contents = new StringBuilder();

            csv.Read();
            var header = "validation_result,code,details," + csv.Parser.RawRecord.TrimEndNewLine();

            contents.AppendLine(header);

            foreach (var lineId in lines)
            {
                var line = await _uploadRepo.GetUploadLineWithError(lineId);
                var text = string.IsNullOrEmpty(line.RegistrationTxt) ? "" : line.RegistrationTxt;

                string? code = null;
                string? message = null;
                string result = "Fail";

                if (text.Contains(":"))
                {
                    var parts = text.Split(':', 2);
                    code = parts[0].Trim();
                    message = parts.Length > 1 ? parts[1].Trim() : null;

                    // Check for API Failure?
                    if (string.Compare(code, "500") == 0)
                    {
                        result = "API Failure";
                        message = "";
                    }
                    
                    // We are only a pass if the message is "Active"
                    if (string.Compare(message, "Active", StringComparison.OrdinalIgnoreCase) == 0) result = "Pass";
                }
                else
                {
                    // This is the case where there was no API call, but we checked to see if they were STRAA exempt
                    message = text;
                    result = string.Compare(message, RegistrationValidationText.STRAAExempt, StringComparison.OrdinalIgnoreCase) == 0 ? "Pass" : "Fail";
                }

                contents.AppendLine($"\"{result}\",\"{code}\",\"{message}\"," + line.LineText.TrimEndNewLine());
            }

            return Encoding.UTF8.GetBytes(contents.ToString());
        }
    }
}
