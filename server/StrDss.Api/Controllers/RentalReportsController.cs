using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Model;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalReportsController : BaseApiController
    {
        public RentalReportsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config) 
            : base(currentUser, mapper, config)
        {
        }


    }
}
