using System;
using System.Collections.Generic;

namespace DataBase.Entities;

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

    /// <summary>
    /// The human-readable description that is given for the role
    /// </summary>
    public string? UserRoleDsc { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime? UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual ICollection<DssUserRoleAssignment> DssUserRoleAssignments { get; } = new List<DssUserRoleAssignment>();

    public virtual ICollection<DssUserRolePrivilege> DssUserRolePrivileges { get; } = new List<DssUserRolePrivilege>();
}
