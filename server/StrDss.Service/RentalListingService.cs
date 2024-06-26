using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace StrDss.Service
{
    public interface IRentalListingService
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicense, int pageSize, int pageNumber, string orderBy, string direction);
        Task<RentalListingViewDto?> GetRentalListing(long rentalListingId);
        Task CreateRentalListingExportFiles();
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

            foreach (var listing in listings.SourceList)
            {
                ProcessHosts(listing, true);
            }

            return listings;
        }

        public async Task<RentalListingViewDto?> GetRentalListing(long rentalListingId)
        {
            var listing = await _listingRepo.GetRentalListing(rentalListingId);

            if (listing == null) return null;

            ProcessHosts(listing);

            return listing;
        }

        private void ProcessHosts(RentalListingViewDto listing, bool clearHosts = false)
        {
            var emailRegex = RegexDefs.GetRegexInfo(RegexDefs.Email);

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

            if (clearHosts) listing.HostsInfo.Clear();
        }

        public async Task CreateRentalListingExportFiles()
        {
            var listingIds = await _listingRepo.GetRentalListingIdsToExport();
            var headers = RentalListingExport.GetHeadersAsCsv();

            var lgExport = new List<string> { headers };
            var allExport = new List<string> { headers };
            var prExport = new List<string> { headers };
            var count = 0;
            var totalCount = listingIds.Count;
            var lgId = 0L;
            var lg = "";

            var stopWatchForAll = new Stopwatch();
            var stopWatch = Stopwatch.StartNew();

            foreach (var listingId in listingIds)
            {
                count++;

                var listing = await _listingRepo.GetRentalListingToExport(listingId);

                if (listing == null) continue;

                if (count != 1 && lg != listing.ManagingOrganizationNm)
                {
                    if (lgExport.Count > 1)
                    {
                        var lgzip = CommonUtils.CreateZip(string.Join("\r\n", lgExport));
                        _logger.LogInformation($"Rental Listing Export - Creating a zip file for {lg}");

                        //insser the zip file to DB
                    }

                    lg = listing.ManagingOrganizationNm;
                    lgId = listing.ManagingOrganizationId ?? 0;

                    lgExport = new List<string> { headers };
                }

                var line = ToCsvString(listing);

                lgExport.Add(line);
                allExport.Add(line);

                if (listing.IsPrincipalResidenceRequired == true)
                {
                    prExport.Add(line);
                }


                if (count % 10 == 0)
                {
                    _logger.LogInformation($"Rental Listing Export - {count}/{totalCount} - {stopWatch.Elapsed.TotalSeconds} seconds ");
                    stopWatch.Restart();
                }
            }

            if (allExport.Count > 1) 
            {
                var allZip = CommonUtils.CreateZip(string.Join("\r\n", allExport));
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for all rental listings");

                //insser the zip file to DB
            }

            if (prExport.Count > 1)
            {
                var prZip = CommonUtils.CreateZip(string.Join("\r\n", prExport));
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for all PR required listings");

                //insser the zip file to DB
            }

            if (lgExport.Count > 1)
            {
                var lgZip = CommonUtils.CreateZip(string.Join("\r\n", lgExport));
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for {lg}");

                //insser the zip file to DB
            }

            stopWatchForAll.Stop();
            _logger.LogInformation($"Rental Listing Export - Finished - {stopWatchForAll.Elapsed.TotalSeconds} seconds");
        }

        private static string ToCsvString(RentalListingExportDto listing)
        {
            var builder = new StringBuilder();

            builder.Append(EscapeCsvField(listing.RentalListingId)).Append(',');
            builder.Append(EscapeCsvField(listing.ListingStatusType)).Append(',');
            builder.Append(EscapeCsvField(listing.ListingStatusSortNo)).Append(',');
            builder.Append(EscapeCsvField(listing.LatestReportPeriodYm)).Append(',');
            builder.Append(EscapeCsvField(listing.IsActive)).Append(',');
            builder.Append(EscapeCsvField(listing.IsNew)).Append(',');
            builder.Append(EscapeCsvField(listing.IsTakenDown)).Append(',');
            builder.Append(EscapeCsvField(listing.OfferingOrganizationId)).Append(',');
            builder.Append(EscapeCsvField(listing.OfferingOrganizationCd)).Append(',');
            builder.Append(EscapeCsvField(listing.OfferingOrganizationNm)).Append(',');
            builder.Append(EscapeCsvField(listing.PlatformListingNo)).Append(',');
            builder.Append(EscapeCsvField(listing.PlatformListingUrl)).Append(',');
            builder.Append(EscapeCsvField(listing.OriginalAddressTxt)).Append(',');
            builder.Append(EscapeCsvField(listing.MatchScoreAmt)).Append(',');
            builder.Append(EscapeCsvField(listing.MatchAddressTxt)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort1ProvinceCd)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort2LocalityNm)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort3LocalityTypeDsc)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort4StreetNm)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort5StreetTypeDsc)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort6StreetDirectionDsc)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort7CivicNo)).Append(',');
            builder.Append(EscapeCsvField(listing.AddressSort8UnitNo)).Append(',');
            builder.Append(EscapeCsvField(listing.ListingContactNamesTxt)).Append(',');
            builder.Append(EscapeCsvField(listing.ManagingOrganizationId)).Append(',');
            builder.Append(EscapeCsvField(listing.ManagingOrganizationNm)).Append(',');
            builder.Append(EscapeCsvField(listing.IsPrincipalResidenceRequired)).Append(',');
            builder.Append(EscapeCsvField(listing.IsBusinessLicenceRequired)).Append(',');
            builder.Append(EscapeCsvField(listing.IsEntireUnit)).Append(',');
            builder.Append(EscapeCsvField(listing.AvailableBedroomsQty)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedYtdQty)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsYtdQty)).Append(',');
            builder.Append(EscapeCsvField(listing.BusinessLicenceNo)).Append(',');
            builder.Append(EscapeCsvField(listing.BcRegistryNo)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionNm)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionDtm)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty00)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty01)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty02)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty03)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty04)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty05)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty06)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty07)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty08)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty09)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty10)).Append(',');
            builder.Append(EscapeCsvField(listing.NightsBookedQty11)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty00)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty01)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty02)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty03)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty04)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty05)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty06)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty07)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty08)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty09)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty10)).Append(',');
            builder.Append(EscapeCsvField(listing.SeparateReservationsQty11)).Append(',');
            builder.Append(EscapeCsvField(listing.PropertyHostName)).Append(',');
            builder.Append(EscapeCsvField(listing.PropertyHostEmail)).Append(',');
            builder.Append(EscapeCsvField(listing.PropertyHostPhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.PropertyHostFaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.PropertyHostMailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1Name)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1Email)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1PhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1FaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1MailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost1Id)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2Name)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2Email)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2PhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2FaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2MailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost2Id)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3Name)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3Email)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3PhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3FaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3MailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost3Id)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4Name)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4Email)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4PhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4FaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4MailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost4Id)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5Name)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5Email)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5PhoneNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5FaxNumber)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5MailingAddress)).Append(',');
            builder.Append(EscapeCsvField(listing.SupplierHost5Id)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionNm1)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionDtm1)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionNm2)).Append(',');
            builder.Append(EscapeCsvField(listing.LastActionDtm2));

            return builder.ToString();
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
