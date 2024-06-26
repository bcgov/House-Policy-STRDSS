using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IRentalListingService
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RentalListingViewDto?> GetRentalListing(long rentalListingId);
    }
    public class RentalListingService : ServiceBase, IRentalListingService
    {
        private IRentalListingRepository _listingRepo;

        public RentalListingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IRentalListingRepository listingRep)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _listingRepo = listingRep;
        }

        public async Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var listings = await _listingRepo.GetRentalListings(all, address, url, listingId, hostName, businessLicense, pageSize, pageNumber, orderBy, direction);

            var emailRegex = RegexDefs.GetRegexInfo(RegexDefs.Email);

            foreach (var listing in listings.SourceList) 
            {
                foreach (var host in listing.Hosts)
                {
                    if (host.EmailAddressDsc == null) continue;

                    var hasValidEmail = Regex.IsMatch(host.EmailAddressDsc, emailRegex.Regex);

                    var hostType = host.IsPropertyOwner ? "Property Owner" : "Supplier Host";

                    listing.HostsInfo.Add(new HostInfo
                    {
                        Host = $"{hostType}: {host.FullNm}; {host.EmailAddressDsc}",
                        HasValidEmail = hasValidEmail,
                    });
                }

                listing.HasAtLeastOneValidHostEmail = listing.HostsInfo.Any(x => x.HasValidEmail);
                listing.Hosts = new List<RentalListingContactDto>();
            }

            return listings;
        }

        public async Task<RentalListingViewDto?> GetRentalListing(long rentalListingId)
        {
            return await _listingRepo.GetRentalListing(rentalListingId);
        }

        //listing IDs order by ManagingOrganizationId, IsPrincipalResidenceRequired
        //List<string>, lg, all, pr
        //loop through listings
        //fetch listing, monthly data, action history, etc for the line
        //if lg ends, creates lg zip and insert to DB and clear lg
        //after loop, creates all zip and pr zip and insert to dB.
        private string GenerateCsv(IEnumerable<DssRentalListingVw> rentalListings)
        {
            var csv = new StringBuilder();
            csv.AppendLine("RentalListingId,ListingStatusType,ListingStatusSortNo,LatestReportPeriodYm,IsActive,IsNew,IsTakenDown,OfferingOrganizationId,OfferingOrganizationCd,OfferingOrganizationNm,PlatformListingNo,PlatformListingUrl,OriginalAddressTxt,MatchScoreAmt,MatchAddressTxt,AddressSort1ProvinceCd,AddressSort2LocalityNm,AddressSort3LocalityTypeDsc,AddressSort4StreetNm,AddressSort5StreetTypeDsc,AddressSort6StreetDirectionDsc,AddressSort7CivicNo,AddressSort8UnitNo,ListingContactNamesTxt,ManagingOrganizationId,ManagingOrganizationNm,IsPrincipalResidenceRequired,IsBusinessLicenceRequired,IsEntireUnit,AvailableBedroomsQty,NightsBookedYtdQty,SeparateReservationsYtdQty,BusinessLicenceNo,BcRegistryNo,LastActionNm,LastActionDtm");

            foreach (var listing in rentalListings)
            {
                csv.AppendLine($"{EscapeCsvField(listing.RentalListingId)},{EscapeCsvField(listing.ListingStatusType)},{EscapeCsvField(listing.ListingStatusSortNo)},{EscapeCsvField(listing.LatestReportPeriodYm)},{EscapeCsvField(listing.IsActive)},{EscapeCsvField(listing.IsNew)},{EscapeCsvField(listing.IsTakenDown)},{EscapeCsvField(listing.OfferingOrganizationId)},{EscapeCsvField(listing.OfferingOrganizationCd)},{EscapeCsvField(listing.OfferingOrganizationNm)},{EscapeCsvField(listing.PlatformListingNo)},{EscapeCsvField(listing.PlatformListingUrl)},{EscapeCsvField(listing.OriginalAddressTxt)},{EscapeCsvField(listing.MatchScoreAmt)},{EscapeCsvField(listing.MatchAddressTxt)},{EscapeCsvField(listing.AddressSort1ProvinceCd)},{EscapeCsvField(listing.AddressSort2LocalityNm)},{EscapeCsvField(listing.AddressSort3LocalityTypeDsc)},{EscapeCsvField(listing.AddressSort4StreetNm)},{EscapeCsvField(listing.AddressSort5StreetTypeDsc)},{EscapeCsvField(listing.AddressSort6StreetDirectionDsc)},{EscapeCsvField(listing.AddressSort7CivicNo)},{EscapeCsvField(listing.AddressSort8UnitNo)},{EscapeCsvField(listing.ListingContactNamesTxt)},{EscapeCsvField(listing.ManagingOrganizationId)},{EscapeCsvField(listing.ManagingOrganizationNm)},{EscapeCsvField(listing.IsPrincipalResidenceRequired)},{EscapeCsvField(listing.IsBusinessLicenceRequired)},{EscapeCsvField(listing.IsEntireUnit)},{EscapeCsvField(listing.AvailableBedroomsQty)},{EscapeCsvField(listing.NightsBookedYtdQty)},{EscapeCsvField(listing.SeparateReservationsYtdQty)},{EscapeCsvField(listing.BusinessLicenceNo)},{EscapeCsvField(listing.BcRegistryNo)},{EscapeCsvField(listing.LastActionNm)},{EscapeCsvField(listing.LastActionDtm)}");
            }

            return csv.ToString();
        }

        private static string EscapeCsvField(object? field)
        {
            if (field == null)
            {
                return string.Empty;
            }

            var fieldString = field.ToString();

            if (fieldString!.Contains('"'))
            {
                fieldString = fieldString.Replace("\"", "\"\"");
            }

            if (fieldString.Contains(',') || fieldString.Contains('\n') || fieldString.Contains('\r'))
            {
                fieldString = $"\"{fieldString}\"";
            }

            return fieldString;
        }
    }
}
