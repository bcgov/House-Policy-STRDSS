using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssPlatformVw
{
    public long? OrganizationId { get; set; }

    public string? OrganizationCd { get; set; }

    public string? OrganizationNm { get; set; }

    public long? ManagingOrganizationId { get; set; }

    public DateTime? UpdDtm { get; set; }

    public Guid? UpdUserGuid { get; set; }

    public long? NoticeOfTakedownContactId { get; set; }

    public string? NoticeOfTakedownContactEmail { get; set; }

    public long? TakedownRequestContactId { get; set; }

    public string? TakedownRequestContactEmail { get; set; }
}
