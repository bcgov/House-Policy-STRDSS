namespace StrDss.Data.Entities;

/// <summary>
/// Precomputed search values for one current master rental listing.
/// Backed by dss_rental_listing_summary.
/// </summary>
public class DssRentalListingSummary
{
    /// <summary>
    /// Master rental listing. Report snapshots are not stored.
    /// </summary>
    public long RentalListingId { get; set; }

    /// <summary>
    /// Long name of the latest listing action. Null when the listing has no action.
    /// </summary>
    public string? LastActionNm { get; set; }

    /// <summary>
    /// Timestamp of that same action. Null when the listing has no action.
    /// </summary>
    public DateTime? LastActionDtm { get; set; }

    /// <summary>
    /// Action id of that same action. A new action replaces the name and timestamp.
    /// </summary>
    public long? LastRentalListingActionId { get; set; }

    /// <summary>
    /// Latest report month for this platform listing. Null when it has never been reported.
    /// </summary>
    public DateOnly? LatestReportPeriodYm { get; set; }

    /// <summary>
    /// Trimmed property-owner contact name. Null when the listing has no property-owner name.
    /// </summary>
    public string? PrimaryHostFullNm { get; set; }

    /// <summary>
    /// Digits only from the listing business licence. Null when the licence has no digits.
    /// </summary>
    public string? BusinessLicenceDigits { get; set; }

    /// <summary>
    /// Trimmed effective host name. Blank becomes null.
    /// </summary>
    public string? EffectiveHostNm { get; set; }

    /// <summary>
    /// Trimmed registration number. Null when the listing has none.
    /// </summary>
    public string? BcRegistryNo { get; set; }

    /// <summary>
    /// Trimmed matched address.
    /// </summary>
    public string? MatchAddressTxt { get; set; }

    /// <summary>
    /// Trimmed unit number.
    /// </summary>
    public string? MatchUnitNo { get; set; }

    /// <summary>
    /// Trimmed listing business licence, before digit extraction.
    /// </summary>
    public string? ListingBusinessLicenceNo { get; set; }

    /// <summary>
    /// Timestamp when this summary row was last rebuilt.
    /// </summary>
    public DateTime UpdDtm { get; set; }
}
