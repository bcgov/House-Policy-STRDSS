using AutoMapper;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IRentalListingReportRepository
    {
        Task AddRentalLisitngReportAsync(DssRentalListingReport report);
    }
    public class RentalListingReportRepository : RepositoryBase<DssRentalListingReport>, IRentalListingReportRepository
    {
        public RentalListingReportRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddRentalLisitngReportAsync(DssRentalListingReport report)
        {
            await _dbSet.AddAsync(report);
        }
    }
}
