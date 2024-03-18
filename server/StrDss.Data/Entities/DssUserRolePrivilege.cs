using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// The association of a granular application privilege to a role
/// </summary>
public partial class DssUserRolePrivilege
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UserRolePrivilegeId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long UserPrivilegeId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long UserRoleId { get; set; }

    public virtual DssUserPrivilege UserPrivilege { get; set; } = null!;

    public virtual DssUserRole UserRole { get; set; } = null!;
}
