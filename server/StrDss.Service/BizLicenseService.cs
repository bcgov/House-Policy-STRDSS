using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrDss.Service
{
    public interface IBizLicenseService
    {

    }
    public class BizLicenseService : ServiceBase, IBizLicenseService
    {
        public BizLicenseService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger) 
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
        }
    }
}
