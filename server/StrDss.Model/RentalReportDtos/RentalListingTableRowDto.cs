using System.Text.Json.Serialization;

namespace StrDss.Model.RentalReportDtos
{
    /// <summary>
    /// Slim DTO for Individual Listings table only. Used with EF projection to select only required columns.
    /// </summary>
    public class RentalListingTableRowDto
    {
        public long? RentalListingId { get; set; }
        public DateOnly? LatestReportPeriodYm { get; set; }

        /// <summary>Used only for server-side recent filter; not serialized to client.</summary>
        [JsonIgnore]
        public long? OfferingOrganizationId { get; set; }

        /// <summary>Used only for server-side search filters; not serialized to client.</summary>
        [JsonIgnore]
        public string? PlatformListingNo { get; set; }

        /// <summary>Used only for server-side search filters; not serialized to client.</summary>
        [JsonIgnore]
        public string? EffectiveBusinessLicenceNo { get; set; }
        public bool? IsLgTransferred { get; set; }
        public bool? IsTakenDown { get; set; }
        public string? BcRegistryNo { get; set; }
        public string? MatchAddressTxt { get; set; }
        public short? MatchScoreAmt { get; set; }
        public bool? IsMatchVerified { get; set; }
        public bool? IsMatchCorrected { get; set; }
        public bool? IsChangedAddress { get; set; }
        public short? NightsBookedYtdQty { get; set; }
        public string? BusinessLicenceNo { get; set; }
        public string? BusinessLicenceNoMatched { get; set; }
        public bool? IsChangedBusinessLicence { get; set; }
        public string? LastActionNm { get; set; }
        public DateTime? LastActionDtm { get; set; }
        public string? PlatformListingUrl { get; set; }
        public string? OfferingOrganizationNm { get; set; }
        public string? TakeDownReason { get; set; }
        public long? ManagingOrganizationId { get; set; }
        public string? ListingStatusType { get; set; }
        public bool? IsPrincipalResidenceRequired { get; set; }
        public bool? IsBusinessLicenceRequired { get; set; }
    }
}
