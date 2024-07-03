using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
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

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("{listingId}")]
        public async Task<ActionResult> GetRentalListing(long listingId)
        {
            var listing = await _listingService.GetRentalListing(listingId);

            if(listing == null)
            {
                return NotFound();
            }

            return Ok(listing);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("exports")]
        public async Task<ActionResult<List<RentalListingExtractDto>>> GetRetalListingExports()
        {
            return Ok(await _listingService.GetRetalListingExportsAsync());
        }


        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("exports/{extractId}")]
        public async Task<ActionResult> GetRetalListingExportAsync(long extractId)
        {
            var extract = await _listingService.GetRetalListingExportAsync(extractId);

            if (extract == null)
            {
                return NotFound();
            }

            if (_currentUser.OrganizationType == OrganizationTypes.LG && extract.FilteringOrganizationId != _currentUser.OrganizationId)
            {
                return Unauthorized();
            }

            return File(extract.SourceBin!, "application/zip", $"STRlisting_{extract.RentalListingExtractNm}_{extract.UpdDtm.ToString("yyyyMMdd")}.zip");
        }

        [HttpGet("exports/run")]
        public async Task<ActionResult<List<RentalListingExtractDto>>> CreateRetalListingExports()
        {
            await _listingService.CreateRentalListingExportFiles();
            return Ok();
        }
    }
}
