using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A rental listing report line that has been extracted from the source
/// </summary>
public partial class DssRentalListingLine
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingLineId { get; set; }

    /// <summary>
    /// Indicates that there has been a validation problem that prevents successful ingestion of the rental listing
    /// </summary>
    public bool IsValidationFailure { get; set; }

    /// <summary>
    /// Indicates that a system fault has prevented complete ingestion of the rental listing
    /// </summary>
    public bool IsSystemFailure { get; set; }

    /// <summary>
    /// An immutable system code that identifies the listing organization (e.g. AIRBNB)
    /// </summary>
    public string OrganizationCd { get; set; } = null!;

    /// <summary>
    /// The platform issued identification number for the listing
    /// </summary>
    public string PlatformListingNo { get; set; } = null!;

    /// <summary>
    /// Full text of the report line that could not be interpreted
    /// </summary>
    public string SourceLineTxt { get; set; } = null!;

    /// <summary>
    /// Freeform description of the problem found while attempting to interpret the report line
    /// </summary>
    public string? ErrorTxt { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long IncludingRentalListingReportId { get; set; }

    public virtual DssRentalListingReport IncludingRentalListingReport { get; set; } = null!;
}
