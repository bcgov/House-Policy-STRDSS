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
        Task<RentalListingResponseWithCountsDto> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction);
        Task<AggregatedListingResponseWithCountsDto> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent);
        Task<int> CountHostListingsAsync(string hostName);
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
        public async Task<RentalListingResponseWithCountsDto> GetRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent, int pageSize, int pageNumber, string orderBy, string direction)
        {
            var listings = await _listingRepo.GetRentalListings(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, recent,
                pageSize, pageNumber, orderBy, direction);

            foreach (var listing in listings.SourceList)
            {
                ProcessHosts(listing, true);
            }

            return listings;
        }

        public async Task<AggregatedListingResponseWithCountsDto> GetGroupedRentalListings(string? all, string? address, string? url, string? listingId, string? hostName, string? businessLicence, string? registrationNumber,
            bool? prRequirement, bool? blRequirement, long? lgId, string[] statusArray, bool? reassigned, bool? takedownComplete, bool recent)
        {
            return await _listingRepo.GetGroupedRentalListings(all, address, url, listingId, hostName, businessLicence, registrationNumber,
                prRequirement, blRequirement, lgId, statusArray, reassigned, takedownComplete, recent);
        }

        public async Task<int> CountHostListingsAsync(string hostName)
        {
            return await _listingRepo.CountHostListingsAsync(CommonUtils.SanitizeAndUppercaseString(hostName));
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

            // Define a mapping between CsvCols and the properties of listing
            var csvMappings = new Dictionary<string, Func<RentalListingExportDto, object>>
            {
                { CsvCols.MostRecentPlatformReportMonth, l => l.LatestReportPeriodYm },
                { CsvCols.Status, l => l.ListingStatusTypeNm },
                { CsvCols.JurisdictionAssignedTo, l => l.ManagingOrganizationNm },
                { CsvCols.EconomicRegionName, l => l.EconomicRegionDsc },
                { CsvCols.PrRequirement, l => l.IsPrincipalResidenceRequired },
                { CsvCols.BlRequirement, l => l.IsBusinessLicenceRequired },
                { CsvCols.PlatformCode, l => l.OfferingOrganizationCd },
                { CsvCols.ListingId, l => l.PlatformListingNo },
                { CsvCols.RegistrationNumber, l => l.BcRegistryNo },
                { CsvCols.UrlAddress, l => l.PlatformListingUrl },
                { CsvCols.PlatformListingAddress, l => l.OriginalAddressTxt },
                { CsvCols.GeocoderBestMatchAddressComplete, l => l.MatchAddressTxt },
                { CsvCols.GeocoderBestMatchAddressCity, l => l.AddressSort2LocalityNm },
                { CsvCols.LocalGovernmentBusinessLicenceNumber, l => l.BusinessLicenceNo },
                { CsvCols.EntireUnit, l => l.IsEntireUnit },
                { CsvCols.NumberOfBedroomsAvailableForStr, l => l.AvailableBedroomsQty },
                { CsvCols.CurrentMonth, l => l.CurrentMonth },
                { CsvCols.NumberOfNightsBookedCurrentMonth, l => l.NightsBookedQty00 },
                { CsvCols.NumberOfNightsBookedPreviousMonth1, l => l.NightsBookedQty01 },
                { CsvCols.NumberOfNightsBookedPreviousMonth2, l => l.NightsBookedQty02 },
                { CsvCols.NumberOfNightsBookedPreviousMonth3, l => l.NightsBookedQty03 },
                { CsvCols.NumberOfNightsBookedPreviousMonth4, l => l.NightsBookedQty04 },
                { CsvCols.NumberOfNightsBookedPreviousMonth5, l => l.NightsBookedQty05 },
                { CsvCols.NumberOfNightsBookedPreviousMonth6, l => l.NightsBookedQty06 },
                { CsvCols.NumberOfNightsBookedPreviousMonth7, l => l.NightsBookedQty07 },
                { CsvCols.NumberOfNightsBookedPreviousMonth8, l => l.NightsBookedQty08 },
                { CsvCols.NumberOfNightsBookedPreviousMonth9, l => l.NightsBookedQty09 },
                { CsvCols.NumberOfNightsBookedPreviousMonth10, l => l.NightsBookedQty10 },
                { CsvCols.NumberOfNightsBookedPreviousMonth11, l => l.NightsBookedQty11 },
                { CsvCols.NumberOfSeparateReservationsCurrentMonth, l => l.SeparateReservationsQty00 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth1, l => l.SeparateReservationsQty01 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth2, l => l.SeparateReservationsQty02 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth3, l => l.SeparateReservationsQty03 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth4, l => l.SeparateReservationsQty04 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth5, l => l.SeparateReservationsQty05 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth6, l => l.SeparateReservationsQty06 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth7, l => l.SeparateReservationsQty07 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth8, l => l.SeparateReservationsQty08 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth9, l => l.SeparateReservationsQty09 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth10, l => l.SeparateReservationsQty10 },
                { CsvCols.NumberOfSeparateReservationsPreviousMonth11, l => l.SeparateReservationsQty11 },
                { CsvCols.PropertyHostName, l => l.PropertyHostName },
                { CsvCols.PropertyHostEmailAddress, l => l.PropertyHostEmail },
                { CsvCols.PropertyHostPhoneNumber, l => l.PropertyHostPhoneNumber },
                { CsvCols.PropertyHostFaxNumber, l => l.PropertyHostFaxNumber },
                { CsvCols.PropertyHostMailingAddress, l => l.PropertyHostMailingAddress },
                { CsvCols.SupplierHost1Name, l => l.SupplierHost1Name },
                { CsvCols.SupplierHost1EmailAddress, l => l.SupplierHost1Email },
                { CsvCols.SupplierHost1PhoneNumber, l => l.SupplierHost1PhoneNumber },
                { CsvCols.SupplierHost1FaxNumber, l => l.SupplierHost1FaxNumber },
                { CsvCols.SupplierHost1MailingAddress, l => l.SupplierHost1MailingAddress },
                { CsvCols.HostIdOfSupplierHost1, l => l.SupplierHost1Id },
                { CsvCols.SupplierHost2Name, l => l.SupplierHost2Name },
                { CsvCols.SupplierHost2EmailAddress, l => l.SupplierHost2Email },
                { CsvCols.SupplierHost2PhoneNumber, l => l.SupplierHost2PhoneNumber },
                { CsvCols.SupplierHost2FaxNumber, l => l.SupplierHost2FaxNumber },
                { CsvCols.SupplierHost2MailingAddress, l => l.SupplierHost2MailingAddress },
                { CsvCols.HostIdOfSupplierHost2, l => l.SupplierHost2Id },
                { CsvCols.SupplierHost3Name, l => l.SupplierHost3Name },
                { CsvCols.SupplierHost3EmailAddress, l => l.SupplierHost3Email },
                { CsvCols.SupplierHost3PhoneNumber, l => l.SupplierHost3PhoneNumber },
                { CsvCols.SupplierHost3FaxNumber, l => l.SupplierHost3FaxNumber },
                { CsvCols.SupplierHost3MailingAddress, l => l.SupplierHost3MailingAddress },
                { CsvCols.HostIdOfSupplierHost3, l => l.SupplierHost3Id },
                { CsvCols.SupplierHost4Name, l => l.SupplierHost4Name },
                { CsvCols.SupplierHost4EmailAddress, l => l.SupplierHost4Email },
                { CsvCols.SupplierHost4PhoneNumber, l => l.SupplierHost4PhoneNumber },
                { CsvCols.SupplierHost4FaxNumber, l => l.SupplierHost4FaxNumber },
                { CsvCols.SupplierHost4MailingAddress, l => l.SupplierHost4MailingAddress },
                { CsvCols.HostIdOfSupplierHost4, l => l.SupplierHost4Id },
                { CsvCols.SupplierHost5Name, l => l.SupplierHost5Name },
                { CsvCols.SupplierHost5EmailAddress, l => l.SupplierHost5Email },
                { CsvCols.SupplierHost5PhoneNumber, l => l.SupplierHost5PhoneNumber },
                { CsvCols.SupplierHost5FaxNumber, l => l.SupplierHost5FaxNumber },
                { CsvCols.SupplierHost5MailingAddress, l => l.SupplierHost5MailingAddress },
                { CsvCols.HostIdOfSupplierHost5, l => l.SupplierHost5Id },
                { CsvCols.LastActionTaken, l => l.LastActionNm },
                { CsvCols.DateOfLastActionTaken, l => l.LastActionDtm },
                { CsvCols.PreviousActionTaken1, l => l.LastActionNm1 },
                { CsvCols.DateOfPreviousActionTaken1, l => l.LastActionDtm1 },
                { CsvCols.PreviousActionTaken2, l => l.LastActionNm2 },
                { CsvCols.DateOfPreviousActionTaken2, l => l.LastActionDtm2 }
            };

            foreach (var column in csvMappings)
            {
                if (headers.Contains(column.Key))
                {
                    builder.Append(FormatCsvField(column.Value(listing))).Append(',');
                }
            }

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
            CommonUtils.SanitizeObject(dto);

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
