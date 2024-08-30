using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using StrDss.Service.CsvHelpers;
using System.Diagnostics;

namespace StrDss.Service
{
    public interface IBizLicenceService
    {
        Task<BizLicenceDto?> GetBizLicence(long businessLicenceId);
        Task ProcessBizLicenceUploadAsync();
        Task<(long?, string?)> GetMatchingBusinessLicenceIdAndNo(long orgId, string effectiveBizLicNo);
        Task<List<BizLicenceSearchDto>> SearchBizLicence(long orgId, string bizLicNo);
        Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> LinkBizLicence(long rentalListingId, long licenceId);
    }
    public class BizLicenceService : ServiceBase, IBizLicenceService
    {
        private IBizLicenceRepository _bizLicenceRepo;
        private IUploadDeliveryRepository _uploadRepo;
        private IOrganizationRepository _orgRepo;
        private ICodeSetRepository _codeSetRepo;
        private IRentalListingRepository _listingRepo;
        private IRentalListingService _listingService;

        public BizLicenceService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IBizLicenceRepository bizLicenceRepo, IUploadDeliveryRepository uploadRepo, IOrganizationRepository orgRepo, ICodeSetRepository codeSetRepo, IRentalListingRepository listingRepo,
            IRentalListingService listingService) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _bizLicenceRepo = bizLicenceRepo;
            _uploadRepo = uploadRepo;
            _orgRepo = orgRepo;
            _codeSetRepo = codeSetRepo;
            _listingRepo = listingRepo;
            _listingService = listingService;
        }

        public async Task<BizLicenceDto?> GetBizLicence(long businessLicenceId)
        {
            return await _bizLicenceRepo.GetBizLicence(businessLicenceId);
        }

        public async Task ProcessBizLicenceUploadAsync()
        {
            var isListingUploadProcessRunning = await _listingRepo.IsListingUploadProcessRunning();

            if (isListingUploadProcessRunning)
            {
                _logger.LogInformation("Skipping ProcessBizLicenceUploadAsync: Rental Listing Upload Process is running");
                return;
            }


            if (!_validator.CommonCodes.Any())
            {
                _validator.CommonCodes = await _codeSetRepo.LoadCodeSetAsync();
            }

            var upload = await _uploadRepo.GetUploadToProcessAsync(UploadDeliveryTypes.LicenceData);

            if (upload == null) return;

            var processStopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Processing Business Licence Upload Id ({upload.UploadDeliveryId}): {upload.ProvidingOrganization.OrganizationNm}");

            using var transaction = _unitOfWork.BeginTransaction();

            await _bizLicenceRepo.CreateBizLicTempTable();

            await ProcessBizLicenceUploadAsync(upload);

            _unitOfWork.Commit();

            var errorCount = upload.DssUploadLines.Count(x => x.IsValidationFailure);

            if (errorCount == 0) await _bizLicenceRepo.ProcessBizLicTempTable(upload.ProvidingOrganizationId);

            transaction.Commit();

            processStopwatch.Stop();

            var msg = errorCount == 0 ?
                $"Success: Finished Business Licence Upload Id ({upload.UploadDeliveryId}): {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds" :
                $"Fail: Finished Business Licence Upload Id ({upload.UploadDeliveryId}): {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds";

            _logger.LogInformation($"Finished: Business Licence Upload Id ({upload.UploadDeliveryId}): {upload.ProvidingOrganization.OrganizationNm} - {processStopwatch.Elapsed.TotalSeconds} seconds");
        }

        private async Task ProcessBizLicenceUploadAsync(DssUploadDelivery upload)
        {
            var count = 0;

            var header = upload.SourceHeaderTxt ?? "";

            var linesToProcess = await _uploadRepo.GetUploadLinesToProcessAsync(upload.UploadDeliveryId);
            var lineCount = linesToProcess.Count;

            foreach (var lineToProcess in linesToProcess)
            {
                var uploadLine = await _uploadRepo.GetUploadLineAsync(upload.UploadDeliveryId, lineToProcess.OrgCd, lineToProcess.SourceRecordNo);

                count++;
                var errors = new Dictionary<string, List<string>>();
                var csvConfig = CsvHelperUtils.GetConfig(errors, false);

                var csv = header + Environment.NewLine + uploadLine!.SourceLineTxt;
                var textReader = new StringReader(csv);
                var csvReader = new CsvReader(textReader, csvConfig);

                var (orgCd, sourceRecordNo) = await ProcessLine(upload, header, uploadLine, csvReader);
            }
        }

        private async Task<(string orgCd, string sourceRecordNo)> ProcessLine(DssUploadDelivery upload, string header, DssUploadLine line, CsvReader csvReader)
        {
            csvReader.Read();

            csvReader.ReadHeader();

            csvReader.Read();

            var row = csvReader.GetRecord<BizLicenceRowUntyped>(); 

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

        public async Task<List<BizLicenceSearchDto>> SearchBizLicence(long orgId, string bizLicNo)
        {
            return await _bizLicenceRepo.SearchBizLicence(orgId, bizLicNo);
        }

        public async Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> LinkBizLicence(long rentalListingId, long licenceId)
        {
            var errors = new Dictionary<string, List<string>>();

            var listing = await _listingService.GetRentalListing(rentalListingId);

            if (listing == null)
            {
                errors.AddItem("rentalListingId", $"Rental Listing with the rental listing ID {rentalListingId} does not exist or the user doesn't have access to the listing.");
            }

            var licence = await _bizLicenceRepo.GetBizLicence(licenceId);

            if (licence == null)
            {
                errors.AddItem("licencdId", $"Business Licence with the business licence ID {licenceId} does not exist");
            }

            if (errors.Any())
            {
                return (errors, null);
            }

            if (_currentUser.OrganizationType != OrganizationTypes.BCGov && licence!.ProvidingOrganizationId != _currentUser.OrganizationId)
            {
                errors.AddItem("licenceId", $"The user doesn't have access to the licence data");
            }

            if (errors.Any())
            {
                return (errors, null);
            }

            await _bizLicenceRepo.LinkBizLicence(rentalListingId, licenceId);
            _unitOfWork.Commit();

            return (errors, await _listingService.GetRentalListing(rentalListingId));
        }
    }
}
