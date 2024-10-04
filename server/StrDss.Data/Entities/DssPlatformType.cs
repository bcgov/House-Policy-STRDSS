using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A sub-type of rental platform organization used for sorting and grouping of members
/// </summary>
public partial class DssPlatformType
{
    /// <summary>
    /// System-consistent code (e.g. Major/Minor)
    /// </summary>
    public string PlatformType { get; set; } = null!;

    /// <summary>
    /// Business term for the platform type (e.g. Major/Minor)
    /// </summary>
    public string PlatformTypeNm { get; set; } = null!;

    public virtual ICollection<DssOrganization> DssOrganizations { get; set; } = new List<DssOrganization>();
}
