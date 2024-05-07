using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IPhysicalAddressRepository
    {
        Task AddPhysicalAddressAsync(DssPhysicalAddress address);
        Task<DssPhysicalAddress?> GetPhysicalAdderssAsync(string address);
    }
    public class PhysicalAddressRepository : RepositoryBase<DssPhysicalAddress>, IPhysicalAddressRepository
    {
        public PhysicalAddressRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddPhysicalAddressAsync(DssPhysicalAddress address)
        {
            await _dbSet.AddAsync(address);
        }
        
        public async Task<DssPhysicalAddress?> GetPhysicalAdderssAsync(string address)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.OriginalAddressTxt.ToLowerInvariant() == address.ToLowerInvariant());
        }
    }
}
