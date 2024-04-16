using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A potential status for a user access request (e.g. Requested, Approved, or Denied)
/// </summary>
public partial class DssAccessRequestStatus
{
    /// <summary>
    /// System-consistent code for the request status
    /// </summary>
    public string AccessRequestStatusCd { get; set; } = null!;

    /// <summary>
    /// Business term for the request status
    /// </summary>
    public string AccessRequestStatusNm { get; set; } = null!;

    public virtual ICollection<DssUserIdentity> DssUserIdentities { get; set; } = new List<DssUserIdentity>();
}
