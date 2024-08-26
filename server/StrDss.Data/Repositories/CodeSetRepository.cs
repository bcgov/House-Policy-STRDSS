using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using StrDss.Model.CommonCode;

namespace StrDss.Data.Repositories
{
    public interface ICodeSetRepository
    {
        Task<List<CommonCodeDto>> LoadCodeSetAsync();
    }
    public class CodeSetRepository : RepositoryBase<DssUserIdentity>, ICodeSetRepository
    {
        public CodeSetRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<List<CommonCodeDto>> LoadCodeSetAsync()
        {
            var commonCodes = new List<CommonCodeDto>();

            commonCodes.AddRange(await
                _dbContext.DssBusinessLicenceStatusTypes
                .AsNoTracking()
                .Select(x => new CommonCodeDto
                {
                    CodeSet = CodeSet.LicenceStatus,
                    CodeName = x.LicenceStatusType,
                    CodeValue = x.LicenceStatusTypeNm
                }).ToListAsync()
            );

            return commonCodes;
        }
    }
}
