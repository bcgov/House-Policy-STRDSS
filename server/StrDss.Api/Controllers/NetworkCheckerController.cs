using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Service.HttpClients;

namespace StrDss.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkCheckerController : ControllerBase
    {
        private readonly IGeocoderApi _geocoder;
        private readonly IOrganizationRepository _orgRepo;

        public NetworkCheckerController(IGeocoderApi geocoder, IOrganizationRepository orgRepo)
        {
            _geocoder = geocoder;
            _orgRepo = orgRepo;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetNetworkStatus()
        {
            return Ok(new { status = true });
        }

        [AllowAnonymous]
        [HttpGet("geocoder")]
        public async Task<IActionResult> TestGeocoder([FromQuery] string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest(new { error = "Address query parameter is required." });

            try
            {
                var physicalAddress = new DssPhysicalAddress
                {
                    OriginalAddressTxt = address
                };

                var error = await _geocoder.GetAddressAsync(physicalAddress);

                return Ok(new
                {
                    success = string.IsNullOrEmpty(error),
                    error = error,
                    originalAddress = physicalAddress.OriginalAddressTxt,
                    matchedAddress = physicalAddress.MatchAddressTxt,
                    matchScore = physicalAddress.MatchScoreAmt,
                    siteNo = physicalAddress.SiteNo,
                    civicNo = physicalAddress.CivicNo,
                    streetName = physicalAddress.StreetNm,
                    locality = physicalAddress.LocalityNm,
                    province = physicalAddress.ProvinceCd,
                    containingOrganizationId = physicalAddress.ContainingOrganizationId,
                    hasLocationGeometry = physicalAddress.LocationGeometry != null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Exception occurred: {ex.Message}"
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("jurisdiction")]
        public async Task<IActionResult> GetJurisdictionInfo([FromQuery] string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest(new { error = "Address query parameter is required." });

            try
            {
                // First, geocode the address
                var physicalAddress = new DssPhysicalAddress
                {
                    OriginalAddressTxt = address
                };

                var geocoderError = await _geocoder.GetAddressAsync(physicalAddress);

                if (!string.IsNullOrEmpty(geocoderError))
                {
                    return Ok(new
                    {
                        success = false,
                        error = $"Geocoder error: {geocoderError}",
                        originalAddress = address
                    });
                }

                if (physicalAddress.LocationGeometry == null || !(physicalAddress.LocationGeometry is Point point))
                {
                    return Ok(new
                    {
                        success = false,
                        error = "No valid location geometry found for the address",
                        originalAddress = address,
                        matchedAddress = physicalAddress.MatchAddressTxt
                    });
                }

                // Look up the containing organization
                var containingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);

                if (containingOrganizationId == null)
                {
                    return Ok(new
                    {
                        success = true,
                        originalAddress = address,
                        matchedAddress = physicalAddress.MatchAddressTxt,
                        locationFound = true,
                        jurisdictionFound = false,
                        message = "No containing jurisdiction found for this location"
                    });
                }

                // Get the organization details
                var organization = await _orgRepo.GetOrganizationByIdAsync(containingOrganizationId.Value);

                if (organization == null)
                {
                    return Ok(new
                    {
                        success = true,
                        originalAddress = address,
                        matchedAddress = physicalAddress.MatchAddressTxt,
                        locationFound = true,
                        jurisdictionFound = false,
                        containingOrganizationId = containingOrganizationId.Value,
                        message = "Organization not found for the containing organization ID"
                    });
                }

                return Ok(new
                {
                    success = true,
                    originalAddress = address,
                    matchedAddress = physicalAddress.MatchAddressTxt,
                    locationFound = true,
                    jurisdictionFound = true,
                    jurisdiction = new
                    {
                        organizationId = organization.OrganizationId,
                        organizationName = organization.OrganizationNm,
                        organizationType = organization.OrganizationType,
                        organizationCode = organization.OrganizationCd,
                        isStraaExempt = organization.IsStraaExempt
                    },
                    geocoderInfo = new
                    {
                        matchScore = physicalAddress.MatchScoreAmt,
                        siteNo = physicalAddress.SiteNo,
                        civicNo = physicalAddress.CivicNo,
                        streetName = physicalAddress.StreetNm,
                        locality = physicalAddress.LocalityNm,
                        province = physicalAddress.ProvinceCd
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = $"Exception occurred: {ex.Message}",
                    originalAddress = address
                });
            }
        }
    }
}
