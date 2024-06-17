using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
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
    }
}
