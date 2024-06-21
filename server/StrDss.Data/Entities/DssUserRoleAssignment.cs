using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// The association of a grantee credential to a role for the purpose of conveying application privileges
/// </summary>
public partial class DssUserRoleAssignment
{
    /// <summary>
    /// Foreign key
    /// </summary>
    public long UserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public string UserRoleCd { get; set; } = null!;

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime? UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssUserIdentity UserIdentity { get; set; } = null!;

    public virtual DssUserRole UserRoleCdNavigation { get; set; } = null!;
}
