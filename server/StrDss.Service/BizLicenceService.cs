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
    public interface IBizLicenceService
    {
        Task<BizLicenceDto?> GetBizLicence(long businessLicenceId);
        Task ProcessBizLicenceUploadAsync();
        Task<(long?, string?)> GetMatchingBusinessLicenceIdAndNo(long orgId, string effectiveBizLicNo);
    }
    public class BizLicenceService : ServiceBase, IBizLicenceService
    {
        private IBizLicenceRepository _bizLicenceRepo;
        private IUploadDeliveryRepository _uploadRepo;
        private IOrganizationRepository _orgRepo;
        private ICodeSetRepository _codeSetRepo;

        public BizLicenceService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IBizLicenceRepository bizLicenceRepo, IUploadDeliveryRepository uploadRepo, IOrganizationRepository orgRepo, ICodeSetRepository codeSetRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _bizLicenceRepo = bizLicenceRepo;
            _uploadRepo = uploadRepo;
            _orgRepo = orgRepo;
            _codeSetRepo = codeSetRepo;
        }

        public async Task<BizLicenceDto?> GetBizLicence(long businessLicenceId)
        {
            return await _bizLicenceRepo.GetBizLicence(businessLicenceId);
        }

        public async Task ProcessBizLicenceUploadAsync()
        {
            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            var upload = await _uploadRepo.GetUploadToProcessAsync(UploadDeliveryTypes.LicenceData);

            if (upload != null)
            {
                using var transaction = _unitOfWork.BeginTransaction();

                await _bizLicenceRepo.CreateBizLicTempTable();

                await ProcessBizLicenceUploadAsync(upload);

                _unitOfWork.Commit();

                var errorCount = upload.DssUploadLines.Count(x => x.IsValidationFailure);

                if (errorCount == 0)
                {
                    await _bizLicenceRepo.ProcessBizLicTempTable(upload.ProvidingOrganizationId);
                    _logger.LogInformation($"Success: Finished Business Licence Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");
                }
                else
                {
                    _logger.LogInformation($"Fail: Finished Business Licence Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");
                }

                transaction.Commit();
            }            
        }

        private async Task ProcessBizLicenceUploadAsync(DssUploadDelivery upload)
        {
            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Business Licence Upload {upload.UploadDeliveryId} - {upload.ProvidingOrganizationId} - {upload.ProvidingOrganization.OrganizationNm}");

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

            var row = csvReader.GetRecord<BizLicenceRowUntyped>(); 

            _logger.LogInformation($"Processing listing ({row.OrgCd} - {row.BusinessLicenceNo})");

            var org = await _orgRepo.GetOrganizationByOrgCdAsync(row.OrgCd);

            var errors = new Dictionary<string, List<string>>();

            _validator.Validate(Entities.BizLicenceRowUntyped, row, errors);

            if (errors.Count > 0)
            {
                SaveUploadLine(line, errors, true, "");
                return (row.OrgCd, row.BusinessLicenceNo);
            }

            row.LicenceStatusType = row.LicenceStatusType.IsEmpty() ? "ISSUED" : row.LicenceStatusType;

            await _bizLicenceRepo.InsertRowToBizLicTempTable(row, org.OrganizationId);

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

        public async Task<(long?, string?)> GetMatchingBusinessLicenceIdAndNo(long orgId, string effectiveBizLicNo)
        {
            return await _bizLicenceRepo.GetMatchingBusinessLicenceIdAndNo(orgId, effectiveBizLicNo);
        }
    }
}
