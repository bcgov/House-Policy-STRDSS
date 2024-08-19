using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A potential status for a BUSINESS LICENCE (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)
/// </summary>
public partial class DssBusinessLicenceStatusType
{
    /// <summary>
    /// System-consistent code for the business licence status (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)
    /// </summary>
    public string LicenceStatusType { get; set; } = null!;

    /// <summary>
    /// Business term for the licence status (e.g. Pending, Issued, Suspended, Revoked, Cancelled, Expired)
    /// </summary>
    public string LicenceStatusTypeNm { get; set; } = null!;

    /// <summary>
    /// Relative order in which the business prefers to see the status listed
    /// </summary>
    public short LicenceStatusSortNo { get; set; }

    public virtual ICollection<DssBusinessLicence> DssBusinessLicences { get; set; } = new List<DssBusinessLicence>();
}
