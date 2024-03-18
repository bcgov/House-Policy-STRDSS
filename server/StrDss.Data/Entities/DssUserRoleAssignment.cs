using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// The association of a grantee credential to a role for the purpose of conveying application privileges
/// </summary>
public partial class DssUserRoleAssignment
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UserRoleAssignmentId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long UserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long UserRoleId { get; set; }

    public virtual DssUserIdentity UserIdentity { get; set; } = null!;

    public virtual DssUserRole UserRole { get; set; } = null!;
}
