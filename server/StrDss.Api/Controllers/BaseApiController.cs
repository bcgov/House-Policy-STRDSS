using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Model;

namespace StrDss.Api.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected ICurrentUser _currentUser;
        protected IMapper _mapper;
        protected IConfiguration _config;

        public BaseApiController(ICurrentUser currentUser, IMapper mapper, IConfiguration config)
        {
            _currentUser = currentUser;
            _mapper = mapper;
            _config = config;
        }
    }
}
