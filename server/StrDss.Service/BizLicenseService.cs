using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Service.CsvHelpers;
using System.Diagnostics;

namespace StrDss.Service
{
    public interface IBizLicenseService
    {
        Task<BizLicenseDto?> GetBizLicense(long businessLicenceId);
        Task ProcessBizLicenseUploadAsync();
        Task<(long?, string?)> GetMatchingBusinessLicenseIdAndNo(long orgId, string effectiveBizLicNo);
    }
    public class BizLicenseService : ServiceBase, IBizLicenseService
    {
        private IBizLicenseRepository _bizLicenseRepo;
        private IUploadDeliveryRepository _uploadRepo;
        private IOrganizationRepository _orgRepo;
        private ICodeSetRepository _codeSetRepo;

        public BizLicenseService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IBizLicenseRepository bizLicenseRepo, IUploadDeliveryRepository uploadRepo, IOrganizationRepository orgRepo, ICodeSetRepository codeSetRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _bizLicenseRepo = bizLicenseRepo;
            _uploadRepo = uploadRepo;
            _orgRepo = orgRepo;
            _codeSetRepo = codeSetRepo;
        }

        public async Task<BizLicenseDto?> GetBizLicense(long businessLicenceId)
        {
            return await _bizLicenseRepo.GetBizLicense(businessLicenceId);
        }

        public async Task ProcessBizLicenseUploadAsync()
        {
            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            var upload = await _uploadRepo.GetUploadToProcessAsync(UploadDeliveryTypes.LicenseData);

            if (upload != null)
            {
                using var transaction = _unitOfWork.BeginTransaction();

                await _bizLicenseRepo.CreateBizLicTempTable();

                await ProcessBizLicenseUploadAsync(upload);

                _unitOfWork.Commit();

                var errorCount = upload.DssUploadLines.Count(x => x.IsValidationFailure);

                if (errorCount == 0)
                {
                    await _bizLicenseRepo.ProcessBizLicTempTable(upload.ProvidingOrganizationId);
                    _logger.LogInformation($"Success: Finished Business License Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");
                }
                else
                {
                    _logger.LogInformation($"Fail: Finished Business License Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");
                }

                transaction.Commit();
            }            
        }

        private async Task ProcessBizLicenseUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Business License Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");

            var count = 0;

            var header = upload.SourceHeaderTxt ?? "";

            var linesToProcess = await _uploadRepo.GetUploadLinesToProcessAsync(upload.UploadDeliveryId);
            var lineCount = linesToProcess.Count;

            foreach (var lineToProcess in linesToProcess)
            {
                var stopwatch = Stopwatch.StartNew();

                var uploadLine = await _uploadRepo.GetUploadLineAsync(upload.UploadDeliveryId, lineToProcess.OrgCd, lineToProcess.SourceRecordNo);

                count++;
                var errors = new Dictionary<string, List<string>>();
                var csvConfig = CsvHelperUtils.GetConfig(errors, false);

                var csv = header + Environment.NewLine + uploadLine!.SourceLineTxt;
                var textReader = new StringReader(csv);
                var csvReader = new CsvReader(textReader, csvConfig);

                var (orgCd, sourceRecordNo) = await ProcessLine(upload, header, uploadLine, csvReader);

                stopwatch.Stop();

                _logger.LogInformation($"Finishing line ({orgCd} - {sourceRecordNo}): {stopwatch.Elapsed.TotalMilliseconds} milliseconds. {count}/{lineCount}");
            }

            processStopwatch.Stop();

            _logger.LogInformation($"Finished: {upload.ReportPeriodYm?.ToString("yyyy-MM")}, {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task<(string orgCd, string sourceRecordNo)> ProcessLine(DssUploadDelivery upload, string header, DssUploadLine line, CsvReader csvReader)
        {
            csvReader.Read();

            csvReader.ReadHeader();

            csvReader.Read();

            var row = csvReader.GetRecord<BizLicenseRowUntyped>(); 

            _logger.LogInformation($"Processing listing ({row.OrgCd} - {row.BusinessLicenceNo})");

            var org = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd);

            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(Entities.BizLicenseRowUntyped, row, errors);

            if (errors.Count > 0)
            {
                SaveUploadLine(line, errors, true, "");
                return (row.OrgCd, row.BusinessLicenceNo);
            }

            row.LicenceStatusType = row.LicenceStatusType.IsEmpty() ? "ISSUED" : row.LicenceStatusType;

            await _bizLicenseRepo.InsertRowToBizLicTempTable(row, org.OrganizationId);

            SaveUploadLine(line, errors, false, "");

            return (row.OrgCd, row.BusinessLicenceNo);
        }

        private void SaveUploadLine(DssUploadLine uploadLine, Dictionary<string, List<string>> errors, bool isValidationFailure, string systemError)
        {
            uploadLine.IsValidationFailure = isValidationFailure;
            uploadLine.ErrorTxt = errors.ParseErrorWithUnderScoredKeyName();

            uploadLine.IsSystemFailure = systemError.IsNotEmpty();
            if (uploadLine.IsSystemFailure)
            {
                uploadLine.ErrorTxt = systemError;
            }

            uploadLine.IsProcessed = true;
        }

        public async Task<(long?, string?)> GetMatchingBusinessLicenseIdAndNo(long orgId, string effectiveBizLicNo)
        {
            return await _bizLicenseRepo.GetMatchingBusinessLicenseIdAndNo(orgId, effectiveBizLicNo);
        }
    }
}
