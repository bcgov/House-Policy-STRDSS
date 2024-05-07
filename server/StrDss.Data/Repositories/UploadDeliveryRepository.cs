using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IUploadDeliveryRepository
    {
        Task<bool> IsDuplicateRentalReportUploadAsnyc(DateOnly periodYm, long orgId, string hashValue);
        Task AddUploadDeliveryAsync(DssUploadDelivery upload);
        Task<List<DssUploadDelivery>> GetRentalReportUploadsToProcessAsync();
        //Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo);
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

        public async Task<List<DssUploadDelivery>> GetRentalReportUploadsToProcessAsync()
        {
            return await _dbSet
                .Include(x => x.DssUploadLines.Where(line => !line.IsProcessed))
                .Include(x => x.ProvidingOrganization)
                .Where(x => x.DssUploadLines.Any(line => !line.IsProcessed))
                .OrderBy(x => x.ProvidingOrganizationId) 
                    .ThenBy(x => x.ReportPeriodYm)
                        .ThenBy(x => x.UpdDtm) //Users can upload the same listing multiple times. The processing of these listings follows a first-come, first-served approach.
                .ToListAsync();
        }

        //public async Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
