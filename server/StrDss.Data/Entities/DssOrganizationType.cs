using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssOrganizationType
{
    public string OrganizationType { get; set; } = null!;

    public string OrganizationTypeNm { get; set; } = null!;

    public virtual ICollection<DssOrganization> DssOrganizations { get; set; } = new List<DssOrganization>();
}
