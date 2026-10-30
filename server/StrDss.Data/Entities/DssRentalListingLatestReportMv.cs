namespace StrDss.Data.Entities;

/// <summary>
/// Latest reported period for an organization and platform listing number.
/// Backed by materialized view dss_rental_listing_latest_report_mv.
/// </summary>
public class DssRentalListingLatestReportMv
{
    /// <summary>
    /// Platform organization that offered the listing.
    /// </summary>
    public long OfferingOrganizationId { get; set; }

    /// <summary>
    /// Platform issued identification number for the listing.
    /// </summary>
    public string PlatformListingNo { get; set; } = null!;

    /// <summary>
    /// Latest report period among reported snapshots of this listing.
    /// </summary>
    public DateOnly LatestReportPeriodYm { get; set; }
}
