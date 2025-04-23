using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

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
    /// The current processing status of the uploaded file: Pending, Processed, or Failed
    /// </summary>
    public string UploadStatus { get; set; } = null;

    /// <summary>
    /// The total number of lines in the uploaded file
    /// </summary>
    public int UploadLinesTotal { get; set; }

    /// <summary>
    /// The number of lines int the uploaded file that successfully processed
    /// </summary>
    public int UploadLinesSuccess { get; set; }

    /// <summary>
    /// The number of lines in the uploaded file that failed to process
    /// </summary>
    public int UploadLinesError { get; set; }

    /// <summary>
    /// The number of lines in the uploaded file that were processed
    /// </summary>
    public int UploadLinesProcessed { get; set; }

    /// <summary>
    /// The current processing status of the uploaded file: Pending, Processed, or Failed
    /// </summary>
    public string RegistrationStatus { get; set; } = null;


    /// <summary>
    /// The current processing status of the registration validation: Pending, Processed, or Failed
    /// </summary>
    public int RegistrationLinesSuccess { get; set; }

    /// <summary>
    /// The number of lines in the uploaded file that failed to validate the registration number
    /// </summary>
    public int RegistrationLinesError { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the user who uploaded the file
    /// </summary>
    public Guid? UploadUserGuid { get; set; }

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

    /// <summary>
    /// Date and time the file was uploaded
    /// </summary>
    public DateTime UploadDate { get; set; }

    public virtual ICollection<DssUploadLine> DssUploadLines { get; set; } = new List<DssUploadLine>();

    public virtual DssOrganization ProvidingOrganization { get; set; } = null!;
}
