using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A granular access right or privilege within the application that may be granted to a role
/// </summary>
public partial class DssUserPrivilege
{
    /// <summary>
    /// The immutable system code that identifies the privilege
    /// </summary>
    public string UserPrivilegeCd { get; set; } = null!;

    /// <summary>
    /// The human-readable name that is given for the role
    /// </summary>
    public string UserPrivilegeNm { get; set; } = null!;

    public virtual ICollection<DssUserRolePrivilege> DssUserRolePrivileges { get; set; } = new List<DssUserRolePrivilege>();
}
