using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Model.RentalReportDtos;
using StrDss.Service.HttpClients;
using System.Diagnostics;
using System.Text;

namespace StrDss.Service
{
    public interface IRentalListingService
    {
        Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, 
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, int pageSize, int pageNumber, string orderBy, string direction);
        Task<PagedDto<RentalListingGroupDto>> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, int pageSize, int pageNumber, string orderBy, string direction);
        Task<int> GetGroupedRentalListingsCount(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete);
        Task<RentalListingViewDto?> GetRentalListing(long rentalListingId);
        Task CreateRentalListingExportFiles();
        Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync();
        Task<RentalListingExtractDto?> GetRetalListingExportAsync(long extractId);
        Task<RentalListingExtractDto?> GetRetalListingExportByNameAsync(string extractName);
        Task<List<AddressDto>> GetAddressCandidatesAsync(string addressText, int maxResults);
        Task<Dictionary<string, List<string>>> ConfirmAddressAsync(long rentalListingId);
        Task<Dictionary<string, List<string>>> UpdateAddressAsync(UpdateListingAddressDto dto);
        Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> LinkBizLicence(long rentalListingId, long licenceId);
        Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> UnLinkBizLicence(long rentalListingId);
        Task ResetLgTransferFlag();
        Task<bool> ListingDataToProcessExists();
    }
    public class RentalListingService : ServiceBase, IRentalListingService
    {
        private IRentalListingRepository _listingRepo;
        private IGeocoderApi _geocoder;
        private IOrganizationRepository _orgRepo;
        private IBizLicenceRepository _bizLicenceRepo;

        public RentalListingService(ICurrentUser currentUser, IFieldValidatorService validator, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StrDssLogger> logger,
            IRentalListingRepository listingRep, IGeocoderApi geocoder, IOrganizationRepository orgRepo, IBizLicenceRepository bizLicenceRepo)
            : base(currentUser, validator, unitOfWork, mapper, httpContextAccessor, logger)
        {
            _listingRepo = listingRep;
            _geocoder = geocoder;
            _orgRepo = orgRepo;
            _bizLicenceRepo = bizLicenceRepo;
        }
        public async Task<PagedDto<RentalListingViewDto>> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var listings = await _listingRepo.GetRentalListings(all, address, url, listingId, hostName, businessLicence,
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete,
                pageSize, pageNumber, orderBy, direction);

            foreach (var listing in listings.SourceList)
            {
                ProcessHosts(listing, true);
            }

            return listings;
        }

        public async Task<PagedDto<RentalListingGroupDto>> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var listings = await _listingRepo.GetGroupedRentalListings(all, address, url, listingId, hostName, businessLicence,
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete,
                pageSize, pageNumber, orderBy, direction);

            foreach (var group in listings.SourceList)
            {
                foreach(var listing in group.Listings)
                {
                    ProcessHosts(listing, true);
                }
            }

            return listings;
        }

        public async Task<int> GetGroupedRentalListingsCount(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete)
        {
            var count = await _listingRepo.GetGroupedRentalListingsCount(all, address, url, listingId, hostName, businessLicence,
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete);

            return count;
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
            foreach (var host in listing.Hosts)
            {
                if (host.EmailAddressDsc == null) continue;

                var hasValidEmail = CommonUtils.IsValidEmailAddress(host.EmailAddressDsc);

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
            // Throttle export frequency: Prevent running if less than 2 hours since last export
            var latestExtractTime = _listingRepo.GetLatestRentalListingExportTime();
            var currentTime = DateTime.UtcNow;
            var timeDifference = currentTime - latestExtractTime;

            if (timeDifference < TimeSpan.FromHours(2))
            {
                _logger.LogInformation("Skipping CreateRentalListingExportFiles: Last export was {TimeDifference:hh\\:mm\\:ss} ago, less than 2 hours", timeDifference);
                return;
            }

            var ListingDataToProcessExists = await this.ListingDataToProcessExists();

            if (ListingDataToProcessExists)
            {
                _logger.LogInformation("Skipping CreateRentalListingExportFiles: Rental Listing Upload Process is running");
                return;
            }

            var listingIds = await _listingRepo.GetRentalListingIdsToExport();
            var headers = RentalListingExport.GetHeadersAsCsv();
            var finHeaders = RentalListingExport.GetFinHeadersAsCsv();

            var lgExport = InitializeExport(headers);
            var allExport = InitializeExport(headers);
            var finExport = InitializeExport(finHeaders);
            var prExport = InitializeExport(headers);
            var count = 0;
            var totalCount = listingIds.Count;
            var lgId = 0L;
            var lg = string.Empty;

            var stopWatchForAll = Stopwatch.StartNew();

            var currentMonth = DateUtils.ConvertUtcToPacificTime(new DateTime(currentTime.Year, currentTime.Month, 1)).AddDays(-1).ToString("yyyy-MM");

            foreach (var listingId in listingIds)
            {
                count++;

                var listing = await _listingRepo.GetRentalListingToExport(listingId);
                if (listing == null) continue;

                listing.CurrentMonth = currentMonth;

                if (lg != listing.ManagingOrganizationNm)
                {
                    await ProcessExportForLocalGovernment(lgExport, lgId, lg!);
                    lg = listing.ManagingOrganizationNm;
                    lgId = listing.ManagingOrganizationId ?? 0;
                    lgExport = InitializeExport(headers);
                }

                var line = ToCsvString(listing, RentalListingExport.Headers);
                var finLine = ToCsvString(listing, RentalListingExport.FinHeaders);

                lgExport.Add(line);
                allExport.Add(line);
                finExport.Add(finLine);

                if (listing.IsPrincipalResidenceRequired == true)
                {
                    prExport.Add(line);
                }
            }

            await CreateFinalExports(allExport, finExport, prExport, lgExport, lg, lgId);
            stopWatchForAll.Stop();
            _logger.LogInformation($"Rental Listing Export - Finished - {stopWatchForAll.Elapsed.TotalSeconds} seconds");
        }

        private List<string> InitializeExport(string headers)
        {
            return new List<string> { headers };
        }

        private async Task ProcessExportForLocalGovernment(List<string> export, long orgId, string orgName)
        {
            var date = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow).ToString("yyyyMMdd");

            if (export.Count > 1 && orgId != 0)
            {
                _logger.LogInformation($"Rental Listing Export - Creating a zip file for {orgName}");
                var extract = await _listingRepo.GetOrCreateRentalListingExtractByOrgId(orgId);
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", export), $"STRlisting_{orgName}_{date}");
                extract.IsPrRequirementFiltered = false;
                extract.RentalListingExtractNm = orgName ?? string.Empty;
                extract.FilteringOrganizationId = orgId;
                _unitOfWork.Commit();
            }
        }

        private async Task CreateFinalExports(List<string> allExport, List<string> finExport, List<string> prExport, List<string> lgExport, string lg, long lgId)
        {
            var date = DateUtils.ConvertUtcToPacificTime(DateTime.UtcNow).ToString("yyyyMMdd");

            if (allExport.Count > 1)
            {
                _logger.LogInformation("Rental Listing Export - Creating a zip file for all rental listings");
                var extract = await _listingRepo.GetOrCreateRentalListingExtractByExtractNm(ListingExportFileNames.All);
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", allExport), $"STRlisting_{ListingExportFileNames.All}_{date}");
                extract.IsPrRequirementFiltered = false;
                _unitOfWork.Commit();                
            }

            if (finExport.Count > 1)
            {
                _logger.LogInformation("Rental Listing Export - Creating a zip file for all rental listings for FIN");
                var extract = await _listingRepo.GetOrCreateRentalListingExtractByExtractNm(ListingExportFileNames.Fin);
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", finExport), $"STRlisting_{ListingExportFileNames.Fin}_{date}");
                extract.IsPrRequirementFiltered = false;
                _unitOfWork.Commit();
            }

            if (prExport.Count > 1)
            {
                _logger.LogInformation("Rental Listing Export - Creating a zip file for all PR required listings");
                var extract = await _listingRepo.GetOrCreateRentalListingExtractByExtractNm(ListingExportFileNames.AllPr);
                extract.SourceBin = CommonUtils.CreateZip(string.Join("\r\n", prExport), $"STRlisting_{ListingExportFileNames.AllPr}_{date}");
                extract.IsPrRequirementFiltered = true;
                _unitOfWork.Commit();
            }

            await ProcessExportForLocalGovernment(lgExport, lgId, lg);
        }

        private static string ToCsvString(RentalListingExportDto listing, string[] headers)
        {
            var builder = new StringBuilder();

            if (headers.Contains(CsvCols.MostRecentPlatformReportMonth))
                builder.Append(FormatCsvField(listing.LatestReportPeriodYm)).Append(',');
            if (headers.Contains(CsvCols.Status))
                builder.Append(FormatCsvField(listing.ListingStatusTypeNm)).Append(',');
            if (headers.Contains(CsvCols.JurisdictionAssignedTo))
                builder.Append(FormatCsvField(listing.ManagingOrganizationNm)).Append(',');
            if (headers.Contains(CsvCols.EconomicRegionName))
                builder.Append(FormatCsvField(listing.EconomicRegionDsc)).Append(',');
            if (headers.Contains(CsvCols.PrRequirement))
                builder.Append(FormatCsvField(listing.IsPrincipalResidenceRequired)).Append(',');
            if (headers.Contains(CsvCols.BlRequirement))
                builder.Append(FormatCsvField(listing.IsBusinessLicenceRequired)).Append(',');
            if (headers.Contains(CsvCols.PlatformCode))
                builder.Append(FormatCsvField(listing.OfferingOrganizationCd)).Append(',');
            if (headers.Contains(CsvCols.ListingId))
                builder.Append(FormatCsvField(listing.PlatformListingNo)).Append(',');
            if (headers.Contains(CsvCols.UrlAddress))
                builder.Append(FormatCsvField(listing.PlatformListingUrl)).Append(',');
            if (headers.Contains(CsvCols.PlatformListingAddress))
                builder.Append(FormatCsvField(listing.OriginalAddressTxt)).Append(',');
            if (headers.Contains(CsvCols.GeocoderBestMatchAddressComplete))
                builder.Append(FormatCsvField(listing.MatchAddressTxt)).Append(',');
            if (headers.Contains(CsvCols.GeocoderBestMatchAddressCity))
                builder.Append(FormatCsvField(listing.AddressSort2LocalityNm)).Append(',');
            if (headers.Contains(CsvCols.LocalGovernmentBusinessLicenceNumber))
                builder.Append(FormatCsvField(listing.BusinessLicenceNo)).Append(',');
            if (headers.Contains(CsvCols.EntireUnit))
                builder.Append(FormatCsvField(listing.IsEntireUnit)).Append(',');
            if (headers.Contains(CsvCols.NumberOfBedroomsAvailableForStr))
                builder.Append(FormatCsvField(listing.AvailableBedroomsQty)).Append(',');
            if (headers.Contains(CsvCols.CurrentMonth))
                builder.Append(FormatCsvField(listing.CurrentMonth)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedCurrentMonth))
                builder.Append(FormatCsvField(listing.NightsBookedQty00)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth1))
                builder.Append(FormatCsvField(listing.NightsBookedQty01)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth2))
                builder.Append(FormatCsvField(listing.NightsBookedQty02)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth3))
                builder.Append(FormatCsvField(listing.NightsBookedQty03)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth4))
                builder.Append(FormatCsvField(listing.NightsBookedQty04)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth5))
                builder.Append(FormatCsvField(listing.NightsBookedQty05)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth6))
                builder.Append(FormatCsvField(listing.NightsBookedQty06)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth7))
                builder.Append(FormatCsvField(listing.NightsBookedQty07)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth8))
                builder.Append(FormatCsvField(listing.NightsBookedQty08)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth9))
                builder.Append(FormatCsvField(listing.NightsBookedQty09)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth10))
                builder.Append(FormatCsvField(listing.NightsBookedQty10)).Append(',');
            if (headers.Contains(CsvCols.NumberOfNightsBookedPreviousMonth11))
                builder.Append(FormatCsvField(listing.NightsBookedQty11)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsCurrentMonth))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty00)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth1))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty01)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth2))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty02)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth3))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty03)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth4))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty04)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth5))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty05)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth6))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty06)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth7))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty07)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth8))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty08)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth9))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty09)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth10))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty10)).Append(',');
            if (headers.Contains(CsvCols.NumberOfSeparateReservationsPreviousMonth11))
                builder.Append(FormatCsvField(listing.SeparateReservationsQty11)).Append(',');
            if (headers.Contains(CsvCols.PropertyHostName))
                builder.Append(FormatCsvField(listing.PropertyHostName)).Append(',');
            if (headers.Contains(CsvCols.PropertyHostEmailAddress))
                builder.Append(FormatCsvField(listing.PropertyHostEmail)).Append(',');
            if (headers.Contains(CsvCols.PropertyHostPhoneNumber))
                builder.Append(FormatCsvField(listing.PropertyHostPhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.PropertyHostFaxNumber))
                builder.Append(FormatCsvField(listing.PropertyHostFaxNumber)).Append(',');
            if (headers.Contains(CsvCols.PropertyHostMailingAddress))
                builder.Append(FormatCsvField(listing.PropertyHostMailingAddress)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost1Name))
                builder.Append(FormatCsvField(listing.SupplierHost1Name)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost1EmailAddress))
                builder.Append(FormatCsvField(listing.SupplierHost1Email)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost1PhoneNumber))
                builder.Append(FormatCsvField(listing.SupplierHost1PhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost1FaxNumber))
                builder.Append(FormatCsvField(listing.SupplierHost1FaxNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost1MailingAddress))
                builder.Append(FormatCsvField(listing.SupplierHost1MailingAddress)).Append(',');
            if (headers.Contains(CsvCols.HostIdOfSupplierHost1))
                builder.Append(FormatCsvField(listing.SupplierHost1Id)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost2Name))
                builder.Append(FormatCsvField(listing.SupplierHost2Name)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost2EmailAddress))
                builder.Append(FormatCsvField(listing.SupplierHost2Email)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost2PhoneNumber))
                builder.Append(FormatCsvField(listing.SupplierHost2PhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost2FaxNumber))
                builder.Append(FormatCsvField(listing.SupplierHost2FaxNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost2MailingAddress))
                builder.Append(FormatCsvField(listing.SupplierHost2MailingAddress)).Append(',');
            if (headers.Contains(CsvCols.HostIdOfSupplierHost2))
                builder.Append(FormatCsvField(listing.SupplierHost2Id)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost3Name))
                builder.Append(FormatCsvField(listing.SupplierHost3Name)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost3EmailAddress))
                builder.Append(FormatCsvField(listing.SupplierHost3Email)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost3PhoneNumber))
                builder.Append(FormatCsvField(listing.SupplierHost3PhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost3FaxNumber))
                builder.Append(FormatCsvField(listing.SupplierHost3FaxNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost3MailingAddress))
                builder.Append(FormatCsvField(listing.SupplierHost3MailingAddress)).Append(',');
            if (headers.Contains(CsvCols.HostIdOfSupplierHost3))
                builder.Append(FormatCsvField(listing.SupplierHost3Id)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost4Name))
                builder.Append(FormatCsvField(listing.SupplierHost4Name)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost4EmailAddress))
                builder.Append(FormatCsvField(listing.SupplierHost4Email)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost4PhoneNumber))
                builder.Append(FormatCsvField(listing.SupplierHost4PhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost4FaxNumber))
                builder.Append(FormatCsvField(listing.SupplierHost4FaxNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost4MailingAddress))
                builder.Append(FormatCsvField(listing.SupplierHost4MailingAddress)).Append(',');
            if (headers.Contains(CsvCols.HostIdOfSupplierHost4))
                builder.Append(FormatCsvField(listing.SupplierHost4Id)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost5Name))
                builder.Append(FormatCsvField(listing.SupplierHost5Name)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost5EmailAddress))
                builder.Append(FormatCsvField(listing.SupplierHost5Email)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost5PhoneNumber))
                builder.Append(FormatCsvField(listing.SupplierHost5PhoneNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost5FaxNumber))
                builder.Append(FormatCsvField(listing.SupplierHost5FaxNumber)).Append(',');
            if (headers.Contains(CsvCols.SupplierHost5MailingAddress))
                builder.Append(FormatCsvField(listing.SupplierHost5MailingAddress)).Append(',');
            if (headers.Contains(CsvCols.HostIdOfSupplierHost5))
                builder.Append(FormatCsvField(listing.SupplierHost5Id)).Append(',');
            if (headers.Contains(CsvCols.LastActionTaken))
                builder.Append(FormatCsvField(listing.LastActionNm)).Append(',');
            if (headers.Contains(CsvCols.DateOfLastActionTaken))
                builder.Append(FormatCsvField(listing.LastActionDtm)).Append(',');
            if (headers.Contains(CsvCols.PreviousActionTaken1))
                builder.Append(FormatCsvField(listing.LastActionNm1)).Append(',');
            if (headers.Contains(CsvCols.DateOfPreviousActionTaken1))
                builder.Append(FormatCsvField(listing.LastActionDtm1)).Append(',');
            if (headers.Contains(CsvCols.PreviousActionTaken2))
                builder.Append(FormatCsvField(listing.LastActionNm2)).Append(',');
            if (headers.Contains(CsvCols.DateOfPreviousActionTaken2))
                builder.Append(FormatCsvField(listing.LastActionDtm2)).Append(',');

            if (builder.Length > 0)
                builder.Length--;

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

        public async Task<RentalListingExtractDto?> GetRetalListingExportByNameAsync(string extractName)
        {
            return await _listingRepo.GetRetalListingExportByNameAsync(extractName);
        }

        public async Task<List<RentalListingExtractDto>> GetRetalListingExportsAsync()
        {
            return await _listingRepo.GetRetalListingExportsAsync();
        }

        public async Task<List<AddressDto>> GetAddressCandidatesAsync(string addressText, int maxResults)
        {
            var addresses = await _geocoder.GetAddressCandidatesAsync(addressText, maxResults);

            foreach (var address in addresses)
            {
                var orgId = await _orgRepo.GetContainingOrganizationId(address.LocationGeometry);
                if (orgId != null)
                {
                    address.OrganizationId = await _orgRepo.GetManagingOrgId(orgId.Value);
                }
            }

            return addresses;
        }

        public async Task<Dictionary<string, List<string>>> ConfirmAddressAsync(long rentalListingId) 
        {
            var errors = new Dictionary<string, List<string>>();

            var listing = await _listingRepo.GetRentalListing(rentalListingId, false);

            if (listing == null)
            {
                errors.AddItem("entity", $"Rental listing with the ID {rentalListingId} does not exist.");
                return errors;
            }

            if (_currentUser.OrganizationType == OrganizationTypes.LG && listing.ManagingOrganizationId != _currentUser.OrganizationId)
            {
                errors.AddItem("managingOrganization", $"The user is not authorized to confirm address.");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            await _listingRepo.ConfirmAddressAsync(rentalListingId);

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<Dictionary<string, List<string>>> UpdateAddressAsync(UpdateListingAddressDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var listing = await _listingRepo.GetRentalListing(dto.RentalListingId, false);

            if (listing == null)
            {
                errors.AddItem("entity", $"Rental listing with ID {dto.RentalListingId} does not exist.");
                return errors;
            }

            if (_currentUser.OrganizationType == OrganizationTypes.LG && listing.ManagingOrganizationId != _currentUser.OrganizationId)
            {
                errors.AddItem("managingOrganization", $"The user is not authorized to update address.");
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            var listingEntity = await _listingRepo.UpdateAddressAsync(dto);
            var addressEntity = listingEntity!.LocatingPhysicalAddress!;

            var originalAddress = addressEntity.OriginalAddressTxt;
            var originalOrgId = addressEntity.ContainingOrganizationId;

            addressEntity.OriginalAddressTxt = dto.AddressString;
            var error = await _geocoder.GetAddressAsync(addressEntity);

            addressEntity.OriginalAddressTxt = originalAddress;
            addressEntity.IsMatchCorrected = true;

            if (error.IsEmpty() && addressEntity.LocationGeometry is not null && addressEntity.LocationGeometry is Point point)
            {
                addressEntity.ContainingOrganizationId = await _orgRepo.GetContainingOrganizationId(point);
            }
            else
            {
                throw new Exception(error);
            }

            if (addressEntity.ContainingOrganizationId != originalOrgId)
            {
                listingEntity.IsLgTransferred = true;
                listingEntity.LgTransferDtm = DateTime.UtcNow;

                listingEntity.IsChangedBusinessLicence = false;
                listingEntity.EffectiveBusinessLicenceNo = CommonUtils.SanitizeAndUppercaseString(listingEntity.BusinessLicenceNo);
                listingEntity.GoverningBusinessLicenceId = null;

                if (addressEntity.ContainingOrganizationId != null && listingEntity.EffectiveBusinessLicenceNo.IsNotEmpty())
                {
                    var (bizLicId, _) = await _bizLicenceRepo.GetMatchingBusinessLicenceIdAndNo(addressEntity.ContainingOrganizationId.Value, listingEntity.BusinessLicenceNo);
                    listingEntity.GoverningBusinessLicenceId = bizLicId;
                }
            }

            listingEntity.IsChangedAddress = true;

            _unitOfWork.Commit();

            return errors;
        }

        public async Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> LinkBizLicence(long rentalListingId, long licenceId)
        {
            var errors = new Dictionary<string, List<string>>();

            var listing = await GetRentalListing(rentalListingId);

            if (listing == null)
            {
                errors.AddItem("rentalListingId", $"Rental Listing with the rental listing ID {rentalListingId} does not exist or the user doesn't have access to the listing.");
            }

            var licence = await _bizLicenceRepo.GetBizLicence(licenceId);

            if (licence == null)
            {
                errors.AddItem("licencdId", $"Business Licence with the business licence ID {licenceId} does not exist");
            }

            if (errors.Any())
            {
                return (errors, null);
            }

            if (_currentUser.OrganizationType != OrganizationTypes.BCGov && licence!.ProvidingOrganizationId != _currentUser.OrganizationId)
            {
                errors.AddItem("licenceId", $"The user doesn't have access to the licence data");
            }

            if (errors.Any())
            {
                return (errors, null);
            }

            await _listingRepo.LinkBizLicence(rentalListingId, licenceId);
            _unitOfWork.Commit();

            return (errors, await GetRentalListing(rentalListingId));
        }

        public async Task<(Dictionary<string, List<string>>, RentalListingViewDto?)> UnLinkBizLicence(long rentalListingId)
        {
            var errors = new Dictionary<string, List<string>>();

            var listing = await GetRentalListing(rentalListingId);

            if (listing == null)
            {
                errors.AddItem("rentalListingId", $"Rental Listing with the rental listing ID {rentalListingId} does not exist or the user doesn't have access to the listing.");
            }

            if (errors.Any())
            {
                return (errors, null);
            }

            await _listingRepo.UnLinkBizLicence(rentalListingId);
            _unitOfWork.Commit();

            return (errors, await GetRentalListing(rentalListingId));
        }

        public async Task ResetLgTransferFlag()
        {
            await _listingRepo.ResetLgTransferFlag();
        }

        public async Task<bool> ListingDataToProcessExists()
        {
            return await _listingRepo.ListingDataToProcessExists();
        }
    }
}
