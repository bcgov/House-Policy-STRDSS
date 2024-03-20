using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A set of access rights and privileges within the application that may be granted to users
/// </summary>
public partial class DssUserRole
{
    /// <summary>
    /// The immutable system code that identifies the role
    /// </summary>
    public string UserRoleCd { get; set; } = null!;

    /// <summary>
    /// The human-readable name that is given for the role
    /// </summary>
    public string UserRoleNm { get; set; } = null!;

    public virtual ICollection<DssUserIdentity> UserIdentities { get; set; } = new List<DssUserIdentity>();

    public virtual ICollection<DssUserPrivilege> UserPrivilegeCds { get; set; } = new List<DssUserPrivilege>();
}
