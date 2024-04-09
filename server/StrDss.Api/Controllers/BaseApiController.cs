using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Common;
using StrDss.Model;

namespace StrDss.Api.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected ICurrentUser _currentUser;
        protected IMapper _mapper;
        protected IConfiguration _config;
        protected ILogger<StrDssLogger> _logger;

        public BaseApiController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger)
        {
            _currentUser = currentUser;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }
    }
}
