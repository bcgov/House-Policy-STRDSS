using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrDss.Data.Repositories
{
    public interface IUploadDeliveryRepository
    {
        Task<bool> IsDuplicateRentalReport(DateOnly periodYm, long orgId, string hashValue);
        Task AddUploadDeliveryAsync(DssUploadDelivery report);
        //Task<DssUploadDelivery?> GetReportToProcessAsync();
        //Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo);
    }

    public class UploadDeliveryRepository : RepositoryBase<DssUploadDelivery>, IUploadDeliveryRepository
    {
        public UploadDeliveryRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddUploadDeliveryAsync(DssUploadDelivery report)
        {
            await _dbSet.AddAsync(report);
        }

        public async Task<bool> IsDuplicateRentalReport(DateOnly periodYm, long orgId, string hashValue)
        {
            return await _dbSet.AsNoTracking()
                .AnyAsync(x => x.ReportPeriodYm == periodYm && x.ProvidingOrganizationId == orgId && x.SourceHashDsc == hashValue);
        }

        //public async Task<DssRentalListingLine?> GetListingLineAsync(long reportId, string orgCd, string listingNo)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<DssUploadDelivery?> GetReportToProcessAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
