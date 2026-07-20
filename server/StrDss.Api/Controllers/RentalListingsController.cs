using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StrDss.Api.Authorization;
using StrDss.Api.Models;
using StrDss.Common;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using StrDss.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalListingsController : BaseApiController
    {
        private IRentalListingService _listingService;
        private IUploadDeliveryService _uploadService;

        public RentalListingsController(ICurrentUser currentUser, IMapper mapper, IConfiguration config, ILogger<StrDssLogger> logger,
            IRentalListingService listingService, IUploadDeliveryService uploadService) 
            : base(currentUser, mapper, config, logger)
        {
            _listingService = listingService;
            _uploadService = uploadService;
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet]
        public async Task<ActionResult> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, bool recent = false,
            int pageSize = 10, int pageNumber = 1, string orderBy = "latestReportPeriodYm", string direction = "desc", bool includeTotalCount = true)
        {
            var response = await _listingService.GetRentalListings(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, recent,
                pageSize, pageNumber, orderBy, direction, includeTotalCount);

            return Ok(response);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetRentalListingsCount(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, bool recent = false)
        {
            var count = await _listingService.GetRentalListingsCountAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, recent);

            return Ok(count);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("grouped/count")]
        [SwaggerOperation(Summary = "Count of aggregated listing groups (same filters as grouped list).")]
        public async Task<ActionResult<int>> GetGroupedRentalListingsCount(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, bool recent = false)
        {
            var count = await _listingService.GetGroupedRentalListingsCountAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, recent);

            return Ok(count);
        }

        /// <summary>
        /// Child rows for one group; uncached. Mode A: bcRegistryNo. Mode B (no reg): matchAddressTxt + matchUnitNo (optional) + effectiveHostNm + effectiveBusinessLicenceNo (listing BL, normalized for matching).
        /// </summary>
        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("grouped/listings")]
        [SwaggerOperation(Summary = "Expand one aggregated group into listing rows (same global filters + recent as the grid).")]
        public async Task<ActionResult<List<RentalListingTableRowDto>>> GetGroupedListingChildren(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, bool recent = false,
            string? bcRegistryNo = null, string? matchAddressTxt = null, string? matchUnitNo = null, string? effectiveHostNm = null, string? effectiveBusinessLicenceNo = null)
        {
            if (string.IsNullOrWhiteSpace(bcRegistryNo)
                && string.IsNullOrWhiteSpace(matchAddressTxt)
                && string.IsNullOrWhiteSpace(matchUnitNo)
                && string.IsNullOrWhiteSpace(effectiveHostNm)
                && string.IsNullOrWhiteSpace(effectiveBusinessLicenceNo))
            {
                return BadRequest(
                    "Provide bcRegistryNo for a registration-based group. For a no-registration group, supply the same composite key as the parent row: matchAddressTxt, matchUnitNo (optional), effectiveHostNm, and effectiveBusinessLicenceNo (business licence on the listing). Omitted or empty query parameters are treated as null key parts.");
            }

            var rows = await _listingService.GetGroupedListingChildrenAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, recent, bcRegistryNo, matchAddressTxt, matchUnitNo, effectiveHostNm, effectiveBusinessLicenceNo);

            return Ok(rows);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("grouped")]
        [SwaggerOperation(Summary = "Paged aggregated listing groups (parent grid only; expand loads child rows separately).")]
        public async Task<ActionResult<PagedDto<RentalListingGroupSummaryDto>>> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, bool recent = false,
            int pageSize = 10, int pageNumber = 1, string orderBy = "latestReportPeriodYm", string direction = "desc", bool includeTotalCount = true)
        {
            var page = await _listingService.GetGroupedRentalListingsPagedAsync(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, recent, pageSize, pageNumber, orderBy, direction, includeTotalCount);

            return Ok(page);
        }

        [ApiAuthorize(Permissions.ListingRead)]
        [HttpGet("host/count")]
        public async Task<ActionResult> CountHostListingsAsync(string hostName)
        {
            var count = await _listingService.CountHostListingsAsync(hostName);

            return Ok(count);
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

        /// <summary>
        /// Downloads a zipped CSV file containing all listings reported by platforms
        /// </summary>
        /// <returns>A zipped CSV file</returns>
        [ApiAuthorize(Permissions.RegistryView)]
        [SwaggerOperation(Tags = new string[] { Common.ApiTags.Aps })]
        [HttpGet("exports/fin")]
        public async Task<ActionResult> GetRetalListingExportFin()
        {
            var extract = await _listingService.GetRetalListingExportByNameAsync(ListingExportFileNames.Fin);

            if (extract == null)
            {
                return NotFound();
            }

            return File(extract.SourceBin!, "application/zip", $"STRlisting_{extract.RentalListingExtractNm}_{extract.UpdDtm.ToString("yyyyMMdd")}.zip");
        }

        [ApiAuthorize(Permissions.AddressWrite)]
        [HttpPut("{rentalListingId}/address")]
        public async Task<ActionResult> UpdateAddress(long rentalListingId, UpdateListingAddressDto dto)
        {
            dto.RentalListingId = rentalListingId;

            var errors = await _listingService.UpdateAddressAsync(dto);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.AddressWrite)]
        [HttpPut("{rentalListingId}/address/confirm")]
        public async Task<ActionResult> ConfirmAddress(long rentalListingId)
        {
            var errors = await _listingService.ConfirmAddressAsync(rentalListingId);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            return Ok();
        }

        [ApiAuthorize]
        [HttpGet("addresses/candidates")]
        public async Task<ActionResult<List<AddressDto>>> GetAddressCandidates(string addressString)
        {   
            CommonUtils.SanitizeObject(addressString);
            var addresses = await _listingService.GetAddressCandidatesAsync(addressString, 3);
            return Ok(addresses);
        }

        [ApiAuthorize(Permissions.TakdownFileUpload)]
        [HttpPost("takedownconfirmation")]
        public async Task<ActionResult> UploadTakedownConfrimation([FromForm] PlatformDataUploadDto dto)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (dto.File == null || dto.File.Length == 0)
            {
                errors.AddItem("File", $"File is null or empty.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            var maxSizeInMb = _config.GetValue<int>("RENTAL_LISTING_REPORT_MAX_SIZE");
            var maxSizeInB = (maxSizeInMb == 0 ? 2 : maxSizeInMb) * 1024 * 1024;

            if (dto.File.Length > maxSizeInB)
            {
                errors.AddItem("File", $"The file size exceeds the maximum size {maxSizeInMb}MB.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            if (!CommonUtils.IsTextFile(dto.File.ContentType))
            {
                errors.AddItem("File", $"Uploaded file is not a CSV file.");
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext);
            }

            using var stream = dto.File.OpenReadStream();

            errors = await _uploadService.UploadData(UploadDeliveryTypes.TakedownData, dto.ReportPeriod, dto.OrganizationId, stream);

            if (errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok();
        }

        [ApiAuthorize(Permissions.BlLinkWrite)]
        [HttpPut("{rentalListingId}/linkbl/{licenceId}")]
        public async Task<ActionResult> LinkBizLicence(long rentalListingId, long licenceId)
        {
            var (errors, listing) = await _listingService.LinkBizLicence(rentalListingId, licenceId);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok(listing);
        }

        [ApiAuthorize(Permissions.BlLinkWrite)]
        [HttpPut("{rentalListingId}/unlinkbl")]
        public async Task<ActionResult> UnLinkBizLicence(long rentalListingId)
        {
            var (errors, listing) = await _listingService.UnLinkBizLicence(rentalListingId);

            if (errors.Any())
            {
                return ValidationUtils.GetValidationErrorResult(errors, ControllerContext, "One or more validation errors occurred in uploaded file.");
            }

            return Ok(listing);
        }

        /// <summary>
        /// Dismisses the "ID updated" banner by setting the listing status from U to A (Active).
        /// </summary>
        [ApiAuthorize(Permissions.ListingRead)]
        [HttpPut("{rentalListingId}/dismissidupdated")]
        public async Task<ActionResult> DismissIdUpdated(long rentalListingId)
        {
            var updated = await _listingService.DismissIdUpdatedStatusAsync(rentalListingId);
            if (!updated)
                return NotFound();
            return NoContent();
        }
    }
}
