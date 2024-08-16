using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A delivery of uploaded information that is relevant to a specific month
/// </summary>
public partial class DssUploadDelivery
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UploadDeliveryId { get; set; }

    /// <summary>
    /// Identifies the treatment applied to ingesting the uploaded information
    /// </summary>
    public string UploadDeliveryType { get; set; } = null!;

    /// <summary>
    /// The month to which the delivery batch is relevant (always set to the first day of the month)
    /// </summary>
    public DateOnly? ReportPeriodYm { get; set; }

    /// <summary>
    /// The hash value of the information that was uploaded
    /// </summary>
    public string SourceHashDsc { get; set; } = null!;

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

    /// <summary>
    /// Full text of the header line
    /// </summary>
    public string? SourceHeaderTxt { get; set; }

    public virtual ICollection<DssUploadLine> DssUploadLines { get; } = new List<DssUploadLine>();

    public virtual DssOrganization ProvidingOrganization { get; set; } = null!;
}
