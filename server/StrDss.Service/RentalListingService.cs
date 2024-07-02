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
        Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync();
        Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId);
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

                host.HasValidEmail = hasValidEmail;
            }

            listing.HasAtLeastOneValidHostEmail = listing.HostsInfo.Any(x => x.HasValidEmail);

            if (clearHosts) listing.HostsInfo.Clear();
        }

        public async Task CreateRentalListingExportFiles()
        {
            var listingIds = await _listingRepo.GetRentalListingIdsToExport();
            var headers = RentalListingExport.GetHeadersAsCsv();

            var lgExport = InitializeExport(headers);
            var allExport = InitializeExport(headers);
            var prExport = InitializeExport(headers);
            var count = 0;
            var totalCount = listingIds.Count;
            var lgId = 0L;
            var lg = string.Empty;

            var stopWatchForAll = new Stopwatch();
            var stopWatch = Stopwatch.StartNew();

            foreach (var listingId in listingIds)
            {
                count++;

                var listing = await _listingRepo.GetRentalListingToExport(listingId);
                if (listing == null) continue;

                if (lg != listing.ManagingOrganizationNm)
                {
                    await ProcessExportForLocalGovernment(lgExport, lgId, lg);
                    lg = listing.ManagingOrganizationNm;
                    lgId = listing.ManagingOrganizationId ?? 0;
                    lgExport = InitializeExport(headers);
                }

                var line = ToCsvString(listing);

                lgExport.Add(line);
                allExport.Add(line);
                if (listing.IsPrincipalResidenceRequired == true)
                {
                    prExport.Add(line);
                }

                LogProgress(count, totalCount, stopWatch);
            }

            await CreateFinalExports(allExport, prExport, lgExport, lg, lgId);
            stopWatchForAll.Stop();
            _logger.LogInformation($"Rental Listing Export - Finished - {stopWatchForAll.Elapsed.TotalSeconds} seconds");
        }

        private List<string> InitializeExport(string headers)
        {
            return new List<string> { headers };
        }

        private async Task ProcessExportForLocalGovernment(List<string> export, long orgId, string orgName)
        {
            if (export.Count > 1 && orgId != 0)
            {
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for {orgName}");
                var extract = await _listingRepo.GetRentalListingExtractByOrgId(orgId);
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", export));
                extract.IsPrRequirementFiltered = false;
                extract.RentalListingExtractNm = orgName ?? string.Empty;
                extract.FilteringOrganizationId = orgId;
                _unitOfWork.Commit();
            }
        }

        private void LogProgress(int count, int totalCount, Stopwatch stopWatch)
        {
            if (count % 10 == 0)
            {
                _logger.LogInformation($"Rental Listing Export - {count}/{totalCount} - {stopWatch.Elapsed.TotalSeconds} seconds ");
                stopWatch.Restart();
            }
        }

        private async Task CreateFinalExports(List<string> allExport, List<string> prExport, List<string> lgExport, string lg, long lgId)
        {
            if (allExport.Count > 1)
            {
                _logger.LogInformation("Rental Listing Export - Creating a zip file for all rental listings");
                var extract = await _listingRepo.GetRentalListingExtractByExtractNm("BC");
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", allExport));
                extract.IsPrRequirementFiltered = false;
                _unitOfWork.Commit();
            }

            if (prExport.Count > 1)
            {
                _logger.LogInformation("Rental Listing Export - Creating a zip file for all PR required listings");
                var extract = await _listingRepo.GetRentalListingExtractByExtractNm("BC_PR");
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", prExport));
                extract.IsPrRequirementFiltered = true;
                _unitOfWork.Commit();
            }

            await ProcessExportForLocalGovernment(lgExport, lgId, lg);
        }

        private static string ToCsvString(RentalListingExportDto listing)
        {
            var builder = new StringBuilder();

            builder.Append(FormatCsvField(listing.LatestReportPeriodYm)).Append(','); // Most Recent Platform Report Month
            builder.Append(FormatCsvField(listing.ListingStatusType)).Append(','); // Status
            builder.Append(FormatCsvField(listing.ManagingOrganizationNm)).Append(','); // Jurisdiction assigned to
            builder.Append(FormatCsvField("economic region name")).Append(','); // economic_region_name
            builder.Append(FormatCsvField(listing.IsPrincipalResidenceRequired)).Append(','); // pr_requirement
            builder.Append(FormatCsvField(listing.IsBusinessLicenceRequired)).Append(','); // BL_requirement
            builder.Append(FormatCsvField(listing.OfferingOrganizationCd)).Append(','); // Platform Code
            builder.Append(FormatCsvField(listing.PlatformListingNo)).Append(','); // Listing ID
            builder.Append(FormatCsvField(listing.PlatformListingUrl)).Append(','); // URL Address
            builder.Append(FormatCsvField(listing.OriginalAddressTxt)).Append(','); // Platform listing address
            builder.Append(FormatCsvField(listing.MatchAddressTxt)).Append(','); // Geocoder Best match address (current month) - complete address
            builder.Append(FormatCsvField(listing.AddressSort2LocalityNm)).Append(','); // Geocoder Best match address (current month) -city only
            builder.Append(FormatCsvField(listing.BusinessLicenceNo)).Append(','); // Local Government Business Licence Number
            builder.Append(FormatCsvField(listing.IsEntireUnit)).Append(','); // Accommodation Type
            builder.Append(FormatCsvField(listing.AvailableBedroomsQty)).Append(','); // Number of Bedrooms available for STR
            builder.Append(FormatCsvField(listing.NightsBookedQty00)).Append(','); // Number of nights booked (Current month)
            builder.Append(FormatCsvField(listing.NightsBookedQty01)).Append(','); // Number of nights booked (Current month - 1)
            builder.Append(FormatCsvField(listing.NightsBookedQty02)).Append(','); // Number of nights booked (Current month - 2)
            builder.Append(FormatCsvField(listing.NightsBookedQty03)).Append(','); // Number of nights booked (Current month - 3)
            builder.Append(FormatCsvField(listing.NightsBookedQty04)).Append(','); // Number of nights booked (Current month - 4)
            builder.Append(FormatCsvField(listing.NightsBookedQty05)).Append(','); // Number of nights booked (Current month - 5)
            builder.Append(FormatCsvField(listing.NightsBookedQty06)).Append(','); // Number of nights booked (Current month - 6)
            builder.Append(FormatCsvField(listing.NightsBookedQty07)).Append(','); // Number of nights booked (Current month - 7)
            builder.Append(FormatCsvField(listing.NightsBookedQty08)).Append(','); // Number of nights booked (Current month - 8)
            builder.Append(FormatCsvField(listing.NightsBookedQty09)).Append(','); // Number of nights booked (Current month - 9)
            builder.Append(FormatCsvField(listing.NightsBookedQty10)).Append(','); // Number of nights booked (Current month - 10)
            builder.Append(FormatCsvField(listing.NightsBookedQty11)).Append(','); // Number of nights booked (Current month - 11)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty00)).Append(','); // Number of separate reservations (Current month)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty01)).Append(','); // Number of separate reservations (Current month - 1)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty02)).Append(','); // Number of separate reservations (Current month - 2)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty03)).Append(','); // Number of separate reservations (Current month - 3)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty04)).Append(','); // Number of separate reservations (Current month - 4)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty05)).Append(','); // Number of separate reservations (Current month - 5)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty06)).Append(','); // Number of separate reservations (Current month - 6)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty07)).Append(','); // Number of separate reservations (Current month - 7)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty08)).Append(','); // Number of separate reservations (Current month - 8)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty09)).Append(','); // Number of separate reservations (Current month - 9)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty10)).Append(','); // Number of separate reservations (Current month - 10)
            builder.Append(FormatCsvField(listing.SeparateReservationsQty11)).Append(','); // Number of separate reservations (Current month - 11)
            builder.Append(FormatCsvField(listing.PropertyHostName)).Append(','); // Property Host name
            builder.Append(FormatCsvField(listing.PropertyHostEmail)).Append(','); // Property Host email address
            builder.Append(FormatCsvField(listing.PropertyHostPhoneNumber)).Append(','); // Property Host phone number
            builder.Append(FormatCsvField(listing.PropertyHostFaxNumber)).Append(','); // Property Host fax number
            builder.Append(FormatCsvField(listing.PropertyHostMailingAddress)).Append(','); // Property Host Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost1Name)).Append(','); // Supplier Host 1 name
            builder.Append(FormatCsvField(listing.SupplierHost1Email)).Append(','); // Supplier Host 1 email address
            builder.Append(FormatCsvField(listing.SupplierHost1PhoneNumber)).Append(','); // Supplier Host 1 phone number
            builder.Append(FormatCsvField(listing.SupplierHost1FaxNumber)).Append(','); // Supplier Host 1 fax number
            builder.Append(FormatCsvField(listing.SupplierHost1MailingAddress)).Append(','); // Supplier Host 1 Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost1Id)).Append(','); // Host ID of Supplier Host 1
            builder.Append(FormatCsvField(listing.SupplierHost2Name)).Append(','); // Supplier Host 2 name
            builder.Append(FormatCsvField(listing.SupplierHost2Email)).Append(','); // Supplier Host 2 email address
            builder.Append(FormatCsvField(listing.SupplierHost2PhoneNumber)).Append(','); // Supplier Host 2 phone number
            builder.Append(FormatCsvField(listing.SupplierHost2FaxNumber)).Append(','); // Supplier Host 2 fax number
            builder.Append(FormatCsvField(listing.SupplierHost2MailingAddress)).Append(','); // Supplier Host 2 Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost2Id)).Append(','); // Host ID of Supplier Host 2
            builder.Append(FormatCsvField(listing.SupplierHost3Name)).Append(','); // Supplier Host 3 name
            builder.Append(FormatCsvField(listing.SupplierHost3Email)).Append(','); // Supplier Host 3 email address
            builder.Append(FormatCsvField(listing.SupplierHost3PhoneNumber)).Append(','); // Supplier Host 3 phone number
            builder.Append(FormatCsvField(listing.SupplierHost3FaxNumber)).Append(','); // Supplier Host 3 fax number
            builder.Append(FormatCsvField(listing.SupplierHost3MailingAddress)).Append(','); // Supplier Host 3 Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost3Id)).Append(','); // Host ID of Supplier Host 3
            builder.Append(FormatCsvField(listing.SupplierHost4Name)).Append(','); // Supplier Host 4 name
            builder.Append(FormatCsvField(listing.SupplierHost4Email)).Append(','); // Supplier Host 4 email address
            builder.Append(FormatCsvField(listing.SupplierHost4PhoneNumber)).Append(','); // Supplier Host 4 phone number
            builder.Append(FormatCsvField(listing.SupplierHost4FaxNumber)).Append(','); // Supplier Host 4 fax number
            builder.Append(FormatCsvField(listing.SupplierHost4MailingAddress)).Append(','); // Supplier Host 4 Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost4Id)).Append(','); // Host ID of Supplier Host 4
            builder.Append(FormatCsvField(listing.SupplierHost5Name)).Append(','); // Supplier Host 5 name
            builder.Append(FormatCsvField(listing.SupplierHost5Email)).Append(','); // Supplier Host 5 email address
            builder.Append(FormatCsvField(listing.SupplierHost5PhoneNumber)).Append(','); // Supplier Host 5 phone number
            builder.Append(FormatCsvField(listing.SupplierHost5FaxNumber)).Append(','); // Supplier Host 5 fax number
            builder.Append(FormatCsvField(listing.SupplierHost5MailingAddress)).Append(','); // Supplier Host 5 Mailing Address
            builder.Append(FormatCsvField(listing.SupplierHost5Id)).Append(','); // Host ID of Supplier Host 5
            builder.Append(FormatCsvField(listing.LastActionNm1)).Append(','); // Last Action Taken
            builder.Append(FormatCsvField(listing.LastActionDtm1)).Append(','); // Date of Last Action Taken
            builder.Append(FormatCsvField(listing.LastActionNm2)).Append(','); // Previous Action Taken 1
            builder.Append(FormatCsvField(listing.LastActionDtm2)); // Date of Previous Action Taken 1

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

        public async Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId)
        {
            return await _listingRepo.GetRetalListingExportAsync(extractId);
        }

        public async Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync()
        {
            return await _listingRepo.GetRetalListingExportsAsync();
        }
}
}
