using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IListingActionRepository
    {
        Task AddActionAsync(DssRentalListingAction action);
        Task<bool> ActionExistsForEmailAsync(long emailMessageId);
    }

    public class ListingActionRepository : RepositoryBase<DssRentalListingAction>, IListingActionRepository
    {
        public ListingActionRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger)
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddActionAsync(DssRentalListingAction action)
        {
            await _dbSet.AddAsync(action);
        }

        public async Task<bool> ActionExistsForEmailAsync(long emailMessageId)
        {
            return await _dbContext.DssRentalListingActions
                .AsNoTracking()
                .AnyAsync(x => x.SourceEmailMessageId == emailMessageId);
        }
    }
}
