using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IBizLicenseRepository
    {
        Task<BizLicenseDto?> GetBizLicense(long businessLicenceId);
    }

    public class BizLicenseRepository : RepositoryBase<DssBusinessLicence>, IBizLicenseRepository
    {
        public BizLicenseRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<BizLicenseDto?> GetBizLicense(long businessLicenceId)
        {
            return _mapper.Map<BizLicenseDto>(await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.BusinessLicenceId == businessLicenceId));            
        }
    }
}
