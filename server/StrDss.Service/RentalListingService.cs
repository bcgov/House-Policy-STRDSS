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

                if (lg != listing.ManagingOrganizationNm)
                {
                    if (lgExport.Count > 1 && lgId != 0)
                    {
                        _logger.LogInformation($"Rental Listing Export - Creating a zip file for {lg}");

                        var extract = await _listingRepo.GetRentalListingExtractByOrgId(lgId);

                        extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", lgExport));
                        extract.IsPrRequirementFiltered = false;
                        extract.RentalListingExtractNm = lg ?? "";
                        extract.FilteringOrganizationId = lgId;

                        _unitOfWork.Commit();
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
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for all rental listings");

                var extract = await _listingRepo.GetRentalListingExtractByExtractNm("BC");

                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", allExport));
                extract.IsPrRequirementFiltered = false;

                _unitOfWork.Commit();
            }

            if (prExport.Count > 1)
            {
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for all PR required listings");

                var extract = await _listingRepo.GetRentalListingExtractByExtractNm("BC_PR");

                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", prExport));
                extract.IsPrRequirementFiltered = true;

                _unitOfWork.Commit();
            }

            if (lgExport.Count > 1)
            {
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for {lg}");

                var extract = await _listingRepo.GetRentalListingExtractByOrgId(lgId);

                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", lgExport));
                extract.IsPrRequirementFiltered = false;
                extract.RentalListingExtractNm = lg ?? "";
                extract.FilteringOrganizationId = lgId;

                _unitOfWork.Commit();
            }

            stopWatchForAll.Stop();
            _logger.LogInformation($"Rental Listing Export - Finished - {stopWatchForAll.Elapsed.TotalSeconds} seconds");
        }

        private static string ToCsvString(RentalListingExportDto listing)
        {
            var builder = new StringBuilder();

            builder.Append(FormatCsvField(listing.LatestReportPeriodYm)).Append(',');
            builder.Append(FormatCsvField(listing.ListingStatusType)).Append(',');
            builder.Append(FormatCsvField(listing.ManagingOrganizationNm)).Append(',');
            builder.Append(FormatCsvField("economic region name")).Append(',');
            builder.Append(FormatCsvField(listing.IsPrincipalResidenceRequired)).Append(',');
            builder.Append(FormatCsvField(listing.IsBusinessLicenceRequired)).Append(',');
            builder.Append(FormatCsvField(listing.OfferingOrganizationCd)).Append(',');
            builder.Append(FormatCsvField(listing.PlatformListingNo)).Append(',');
            builder.Append(FormatCsvField(listing.PlatformListingUrl)).Append(',');
            builder.Append(FormatCsvField(listing.MatchAddressTxt)).Append(',');
            builder.Append(FormatCsvField(listing.AddressSort2LocalityNm)).Append(',');
            builder.Append(FormatCsvField(listing.BusinessLicenceNo)).Append(',');
            builder.Append(FormatCsvField(listing.IsEntireUnit)).Append(',');
            builder.Append(FormatCsvField(listing.AvailableBedroomsQty)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty00)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty01)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty02)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty03)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty04)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty05)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty06)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty07)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty08)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty09)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty10)).Append(',');
            builder.Append(FormatCsvField(listing.NightsBookedQty11)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty00)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty01)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty02)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty03)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty04)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty05)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty06)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty07)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty08)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty09)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty10)).Append(',');
            builder.Append(FormatCsvField(listing.SeparateReservationsQty11)).Append(',');
            builder.Append(FormatCsvField(listing.PropertyHostName)).Append(',');
            builder.Append(FormatCsvField(listing.PropertyHostEmail)).Append(',');
            builder.Append(FormatCsvField(listing.PropertyHostPhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.PropertyHostFaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.PropertyHostMailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1Name)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1Email)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1PhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1FaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1MailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost1Id)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2Name)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2Email)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2PhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2FaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2MailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost2Id)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3Name)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3Email)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3PhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3FaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3MailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost3Id)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4Name)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4Email)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4PhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4FaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4MailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost4Id)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5Name)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5Email)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5PhoneNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5FaxNumber)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5MailingAddress)).Append(',');
            builder.Append(FormatCsvField(listing.SupplierHost5Id)).Append(',');
            builder.Append(FormatCsvField(listing.LastActionNm1)).Append(',');
            builder.Append(FormatCsvField(listing.LastActionDtm1)).Append(',');
            builder.Append(FormatCsvField(listing.LastActionNm2)).Append(',');
            builder.Append(FormatCsvField(listing.LastActionDtm2));

            return builder.ToString();
        }

        private static string FormatCsvField(object? field)
        {
            if (field == null)
            {
                return string.Empty;
            }

            if (field is bool boolField)
            {
                return boolField ? "Y" : "N";
            }

            if (field is DateOnly dateOnlyField)
            {
                return dateOnlyField.ToString("yyyy-MM");
            }

            if (field is DateTime dateTimeField)
            {
                var pt = DateUtils.ConvertUtcToPacificTime(dateTimeField);
                return pt.ToString("yyyy-MM-dd");
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
