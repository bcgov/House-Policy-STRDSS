using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A delivery of rental listing information that is relevant to a specific month
/// </summary>
public partial class DssRentalListingReport
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingReportId { get; set; }

    public bool IsProcessed { get; set; }

    /// <summary>
    /// The month to which the listing information is relevant (always set to the first day of the month)
    /// </summary>
    public DateOnly ReportPeriodYm { get; set; }

    /// <summary>
    /// The binary image of the information that was uploaded
    /// </summary>
    public byte[]? SourceBin { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long ProvidingOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual ICollection<DssRentalListingLine> DssRentalListingLines { get; set; } = new List<DssRentalListingLine>();

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();

    public virtual DssOrganization ProvidingOrganization { get; set; } = null!;
}
