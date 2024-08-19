using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;

namespace StrDss.Service
{
    public interface IBizLicenseService
    {
        Task<BizLicenseDto?> GetBizLicense(long businessLicenceId);
    }
    public class BizLicenseService : ServiceBase, IBizLicenseService
    {
        private IBizLicenseRepository _bizLicenseRepo;

        public BizLicenseService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IBizLicenseRepository bizLicenseRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _bizLicenseRepo = bizLicenseRepo;
        }

        public async Task<BizLicenseDto?> GetBizLicense(long businessLicenceId)
        {
            return await _bizLicenseRepo.GetBizLicense(businessLicenceId);
        }
    }
}
