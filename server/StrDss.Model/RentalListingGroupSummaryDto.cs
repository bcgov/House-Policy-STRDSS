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
        /// Used after paging to hydrate <see cref="PrimaryHostNm"/> from the original-case property-owner
        /// contact (dss_rental_listing_contact.full_nm) without re-fetching contacts for the whole unpaged set.
        /// </summary>
        public long? AnchorRentalListingId { get; set; }
    }
}
