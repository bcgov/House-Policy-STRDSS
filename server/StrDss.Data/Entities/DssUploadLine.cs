using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// An upload delivery line that has been extracted from the source
/// </summary>
public partial class DssUploadLine
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UploadLineId { get; set; }

    /// <summary>
    /// Indicates that there has been a validation problem that prevents successful ingestion of the upload line
    /// </summary>
    public bool IsValidationFailure { get; set; }

    /// <summary>
    /// Indicates that a system fault has prevented complete ingestion of the upload line
    /// </summary>
    public bool IsSystemFailure { get; set; }

    /// <summary>
    /// An immutable system code identifying the organization who created the information in the upload line (e.g. AIRBNB)
    /// </summary>
    public string SourceOrganizationCd { get; set; } = null!;

    /// <summary>
    /// The immutable identification number for the source record, such as a rental listing number
    /// </summary>
    public string SourceRecordNo { get; set; } = null!;

    /// <summary>
    /// Full text of the uploaod line
    /// </summary>
    public string SourceLineTxt { get; set; } = null!;

    /// <summary>
    /// Freeform description of the problem found while attempting to interpret the report line
    /// </summary>
    public string? ErrorTxt { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long IncludingUploadDeliveryId { get; set; }

    /// <summary>
    /// Indicates that no further ingestion attempt is required for the upload line
    /// </summary>
    public bool IsProcessed { get; set; }

    public virtual DssUploadDelivery IncludingUploadDelivery { get; set; } = null!;
}
