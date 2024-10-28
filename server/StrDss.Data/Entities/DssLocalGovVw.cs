using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssLocalGovVw
{
    public long? OrganizationId { get; set; }

    public string? OrganizationType { get; set; }

    public string? OrganizationCd { get; set; }

    public string? OrganizationNm { get; set; }

    public string? LocalGovernmentType { get; set; }

    public string? LocalGovernmentTypeNm { get; set; }

    public string? BusinessLicenceFormatTxt { get; set; }

    public DateTime? UpdDtm { get; set; }
}
