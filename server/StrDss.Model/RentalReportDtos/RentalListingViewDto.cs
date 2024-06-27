namespace StrDss.Model.RentalReportDtos
{
    public class RentalListingViewDto
    {
        public long? RentalListingId { get; set; }

        public string? ListingStatusType { get; set; }

        public int? ListingStatusSortNo { get; set; }

        public DateOnly? LatestReportPeriodYm { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsNew { get; set; }

        public bool? IsTakenDown { get; set; }

        public long? OfferingOrganizationId { get; set; }
        public string? OfferingOrganizationCd { get; set; }

        public string? OfferingOrganizationNm { get; set; }

        public string? PlatformListingNo { get; set; }

        public string? PlatformListingUrl { get; set; }

        public string? OriginalAddressTxt { get; set; }

        public short? MatchScoreAmt { get; set; }

        public string? MatchAddressTxt { get; set; }

        public string? AddressSort1ProvinceCd { get; set; }

        public string? AddressSort2LocalityNm { get; set; }

        public string? AddressSort3LocalityTypeDsc { get; set; }

        public string? AddressSort4StreetNm { get; set; }

        public string? AddressSort5StreetTypeDsc { get; set; }

        public string? AddressSort6StreetDirectionDsc { get; set; }

        public string? AddressSort7CivicNo { get; set; }

        public string? AddressSort8UnitNo { get; set; }

        public string? ListingContactNamesTxt { get; set; }

        public long? ManagingOrganizationId { get; set; }

        public string? ManagingOrganizationNm { get; set; }

        public bool? IsPrincipalResidenceRequired { get; set; }

        public bool? IsBusinessLicenceRequired { get; set; }

        public bool? IsEntireUnit { get; set; }

        public short? AvailableBedroomsQty { get; set; }

        public long? NightsBookedYtdQty { get; set; }

        public long? SeparateReservationsYtdQty { get; set; }

        public string? BusinessLicenceNo { get; set; }

        public string? BcRegistryNo { get; set; }

        public string? LastActionNm { get; set; }

        public DateTime? LastActionDtm { get; set; }
        public bool HasAtLeastOneValidHostEmail { get; set; }
        public List<HostInfo> HostsInfo { get; set; } = new List<HostInfo>();

        public virtual ICollection<RentalListingContactDto> Hosts { get; set; } = new List<RentalListingContactDto>();
        public virtual ICollection<ListingHistoryDto> ListingHistory { get; set; } = new List<ListingHistoryDto>();
        public virtual ICollection<ActionHistoryDto> ActionHistory { get; set; } = new List<ActionHistoryDto>();
    }

    public class HostInfo
    {
        public string Host { get; set; } = "";
        public bool HasValidEmail { get; set; }
    }
}
