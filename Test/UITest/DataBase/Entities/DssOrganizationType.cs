using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A level of government or business category
/// </summary>
public partial class DssOrganizationType
{
    /// <summary>
    /// System-consistent code for a level of government or business category
    /// </summary>
    public string OrganizationType { get; set; } = null!;

    /// <summary>
    /// Business term for a level of government or business category
    /// </summary>
    public string OrganizationTypeNm { get; set; } = null!;

    public virtual ICollection<DssOrganization> DssOrganizations { get; } = new List<DssOrganization>();
}
