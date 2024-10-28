using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A geographic classification of LOCAL GOVERNMENT SUBDIVISION used for sorting and grouping of members
/// </summary>
public partial class DssEconomicRegion
{
    /// <summary>
    /// System-consistent code (e.g. Northeast, Cariboo)
    /// </summary>
    public string EconomicRegionDsc { get; set; } = null!;

    /// <summary>
    /// Business term for the ECONOMIC REGION (e.g. Northeast, Cariboo)
    /// </summary>
    public string EconomicRegionNm { get; set; } = null!;

    /// <summary>
    /// Relative order in which the business prefers to see the ECONOMIC REGION listed
    /// </summary>
    public short? EconomicRegionSortNo { get; set; }

    public virtual ICollection<DssOrganization> DssOrganizations { get; set; } = new List<DssOrganization>();
}
