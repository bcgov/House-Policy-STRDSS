using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Service;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalListingsController : BaseApiController
    {
        private IRentalListingService _listingService;

        public RentalListingsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IRentalListingService listingService) 
            : base(currentUser, mapper, config, logger)
        {
            _listingService = listingService;
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet]
        public async Task<ActionResult> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense,
            int pageSize, int pageNumber, string orderBy = "ListingStatusSortNo", string direction = "asc")
        {
            var listings = await _listingService.GetRentalListings(all, address, url, listingId, hostName, businessLicense, pageSize, pageNumber, orderBy, direction);

            return Ok(listings);
        }
    }
}
