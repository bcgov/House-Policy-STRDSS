namespace StrDss.Model
{
    /// <summary>
    /// Parent row for paged aggregated listings; child rows load via grouped/listings expand (uncached).
    /// </summary>
    public class RentalListingGroupSummaryDto
    {
        public int ListingCount { get; set; }
        public string? BcRegistryNo { get; set; }
        public string? EffectiveBusinessLicenceNo { get; set; }
        public string? EffectiveHostNm { get; set; }
        public string? MatchAddressTxt { get; set; }
        /// <summary>Best-match unit/suite; expand must send with matchAddressTxt for no-reg groups.</summary>
        public string? MatchUnitNo { get; set; }
        /// <summary>
        /// Trimmed property-owner name from the anchor listing. Falls back to <see cref="EffectiveHostNm"/>
        /// when that listing has no property-owner name.
        /// </summary>
        public string? PrimaryHostNm { get; set; }
        public int NightsBookedYtdQty { get; set; }
        public string? BusinessLicenceNo { get; set; }
        public string? LastActionNm { get; set; }
        public DateTime? LastActionDtm { get; set; }
        public DateOnly? LatestReportPeriodYm { get; set; }
        public long? BusinessLicenceId { get; set; }
        public DateOnly? BusinessLicenceExpiryDt { get; set; }
        public string? LicenceStatusType { get; set; }
        public bool HasMultipleProperties { get; set; }
        /// <summary>
        /// RentalListingId of the anchor (latest LastActionDtm / LatestReportPeriodYm) row in the group.
        /// <see cref="PrimaryHostNm"/> is that listing's property-owner name from dss_rental_listing_summary.
        /// </summary>
        public long? AnchorRentalListingId { get; set; }
    }
}
