using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A sub-type of local government organization used for sorting and grouping of members
/// </summary>
public partial class DssLocalGovernmentType
{
    /// <summary>
    /// System-consistent code (e.g. Municipality, First Nations Community)
    /// </summary>
    public string LocalGovernmentType { get; set; } = null!;

    /// <summary>
    /// Business term for for the local government type (e.g. Municipality, First Nations Community)
    /// </summary>
    public string LocalGovernmentTypeNm { get; set; } = null!;

    /// <summary>
    /// Relative order in which the business prefers to see the LOCAL GOVERNMENT TYPE listed
    /// </summary>
    public short? LocalGovernmentTypeSortNo { get; set; }

    public virtual ICollection<DssOrganization> DssOrganizations { get; set; } = new List<DssOrganization>();
}
