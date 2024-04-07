using AutoMapper;
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
        public RentalListingReportRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser) 
            : base(dbContext, mapper, currentUser)
        {
        }

        public async Task AddRentalLisitngReportAsync(DssRentalListingReport report)
        {
            await _dbSet.AddAsync(report);
        }
    }
}
