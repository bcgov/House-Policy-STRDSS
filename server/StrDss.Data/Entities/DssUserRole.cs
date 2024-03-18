using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A set of access rights and privileges within the application that may be granted to users
/// </summary>
public partial class DssUserRole
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UserRoleId { get; set; }

    /// <summary>
    /// The immutable system code that identifies the role
    /// </summary>
    public string UserRoleCd { get; set; } = null!;

    /// <summary>
    /// The human-readable name that is given for the role
    /// </summary>
    public string UserRoleNm { get; set; } = null!;

    public virtual ICollection<DssUserRoleAssignment> DssUserRoleAssignments { get; set; } = new List<DssUserRoleAssignment>();

    public virtual ICollection<DssUserRolePrivilege> DssUserRolePrivileges { get; set; } = new List<DssUserRolePrivilege>();
}
