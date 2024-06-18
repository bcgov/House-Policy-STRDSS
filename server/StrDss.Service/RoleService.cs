using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;

namespace StrDss.Service
{
    public interface IRoleService
    {

    }
    public class RoleService : ServiceBase, IRoleService
    {
        private IRoleRepository _roleRepo;

        public RoleService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IRoleRepository roleRepo) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _roleRepo = roleRepo;
        }
    }
}
