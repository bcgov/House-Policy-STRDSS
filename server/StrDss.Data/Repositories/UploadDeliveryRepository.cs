using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using System.Diagnostics;

namespace StrDss.Data.Repositories
{
    public interface IUploadDeliveryRepository
    {
        Task<bool> IsDuplicateRentalReportUploadAsnyc(DateOnly periodYm, long orgId, string hashValue);
        Task AddUploadDeliveryAsync(DssUploadDelivery upload);
        Task<DssUploadDelivery?> GetUploadToProcessAsync(string reportType);
        Task<DssUploadDelivery[]> GetUploadsToProcessAsync(string reportType);
        Task<DssUploadDelivery?> GetRentalListingUploadWithErrors(long uploadId);
        Task<DssUploadLine?> GetUploadLineAsync(long uploadId, string orgCd, string listingId);
        Task<List<UploadLineToProcess>> GetUploadLinesToProcessAsync(long uploadId);
        Task<List<DssUploadLine>> GetUploadLineEntitiesToProcessAsync(long uploadId);
        Task<long[]> GetUploadLineIdsWithErrors(long uploadId);
        Task<UploadLineError> GetUploadLineWithError(long lineId);
        Task<bool> UploadHasErrors(long uploadId);
        Task<int> GetTotalNumberOfUploadLines(long uploadId);
    }

    public class UploadDeliveryRepository : RepositoryBase<DssUploadDelivery>, IUploadDeliveryRepository
    {
        public UploadDeliveryRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddUploadDeliveryAsync(DssUploadDelivery upload)
        {
            await _dbSet.AddAsync(upload);
        }

        public async Task<bool> IsDuplicateRentalReportUploadAsnyc(DateOnly periodYm, long orgId, string hashValue)
        {
            return await _dbSet.AsNoTracking()
                .AnyAsync(x => x.ReportPeriodYm == periodYm && x.ProvidingOrganizationId == orgId && x.SourceHashDsc == hashValue);
        }

        public async Task<DssUploadDelivery?> GetUploadToProcessAsync(string reportType)
        {
            return await _dbSet
                .Include(x => x.ProvidingOrganization)
                .Where(x => x.DssUploadLines.Any(line => !line.IsProcessed) && x.UploadDeliveryType == reportType)
                .OrderBy(x => x.ProvidingOrganizationId) 
                    .ThenBy(x => x.ReportPeriodYm)
                        .ThenBy(x => x.UpdDtm) //Users can upload the same listing multiple times. The processing of these listings follows a first-come, first-served approach.
                .FirstOrDefaultAsync();
        }

        public async Task<DssUploadDelivery[]> GetUploadsToProcessAsync(string reportType)
        {
            return await _dbSet
                .Include(x => x.ProvidingOrganization)
                .Where(x => x.DssUploadLines.Any(line => !line.IsProcessed) && x.UploadDeliveryType == reportType)
                .OrderBy(x => x.ProvidingOrganizationId)
                    .ThenBy(x => x.ReportPeriodYm)
                        .ThenBy(x => x.UpdDtm) //Users can upload the same listing multiple times. The processing of these listings follows a first-come, first-served approach.
                .ToArrayAsync();
        }

        public async Task<DssUploadLine?> GetUploadLineAsync(long uploadId, string orgCd, string listingId)
        {
            var stopwatch = Stopwatch.StartNew();

            var line = await _dbContext.DssUploadLines.FirstOrDefaultAsync(x => 
                x.IncludingUploadDeliveryId ==  uploadId && 
                x.SourceOrganizationCd == orgCd && 
                x.SourceRecordNo == listingId &&
                x.IsProcessed == false);

            stopwatch.Stop();

            _logger.LogDebug($"Fetched listing ({orgCd} - {listingId}) - {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

            return line;
        }

        public async Task<List<UploadLineToProcess>> GetUploadLinesToProcessAsync(long uploadId)
        {
            return await _dbContext.DssUploadLines.AsNoTracking()
                .Where(x => x.IncludingUploadDeliveryId == uploadId && x.IsProcessed == false)
                .Select(x => new UploadLineToProcess { ListingId = x.SourceRecordNo, OrgCd = x.SourceOrganizationCd })
                .ToListAsync();
        }

        public async Task<List<DssUploadLine>> GetUploadLineEntitiesToProcessAsync(long uploadId)
        {
            return await _dbContext.DssUploadLines
                .Where(x => x.IncludingUploadDeliveryId == uploadId && x.IsProcessed == false)
                .ToListAsync();
        }

        public async Task<DssUploadDelivery?> GetRentalListingUploadWithErrors(long uploadId)
        {
            var query = _dbSet.AsNoTracking()
                .Where(x => x.UploadDeliveryId == uploadId && x.DssUploadLines.Any(x => x.IsSystemFailure || x.IsValidationFailure));
                
            if (_currentUser.OrganizationType == OrganizationTypes.Platform)
            {
                query = query.Where(x => x.ProvidingOrganizationId == _currentUser.OrganizationId);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<long[]> GetUploadLineIdsWithErrors(long uploadId)
        {
            return await _dbContext.DssUploadLines.AsNoTracking()
                .Where(x => x.IncludingUploadDeliveryId == uploadId && (x.IsValidationFailure || x.IsSystemFailure))
                .Select(x => x.UploadLineId)
                .ToArrayAsync();
        }

        public async Task<UploadLineError> GetUploadLineWithError(long lineId)
        {
            return await _dbContext.DssUploadLines.AsNoTracking()
                .Where(x => x.UploadLineId == lineId)
                .Select(x => new UploadLineError { LineText = x.SourceLineTxt, ErrorText = x.ErrorTxt })
                .FirstAsync();
        }

        public async Task<bool> UploadHasErrors(long uploadId)
        {
            return await _dbContext.DssUploadLines.AsNoTracking()
                .AnyAsync(x => x.IncludingUploadDeliveryId == uploadId && (x.IsValidationFailure || x.IsSystemFailure));
        }
        public async Task<int> GetTotalNumberOfUploadLines(long uploadId)
        {
            return await _dbContext.DssUploadLines.Where(x => x.IncludingUploadDeliveryId == uploadId).CountAsync();
        }
    }
}
