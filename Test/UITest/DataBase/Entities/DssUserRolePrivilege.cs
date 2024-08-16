using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// The association of a granular application privilege to a role
/// </summary>
public partial class DssUserRolePrivilege
{
    /// <summary>
    /// Foreign key
    /// </summary>
    public string UserPrivilegeCd { get; set; } = null!;

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

    public virtual DssUserPrivilege UserPrivilegeCdNavigation { get; set; } = null!;

    public virtual DssUserRole UserRoleCdNavigation { get; set; } = null!;
}
