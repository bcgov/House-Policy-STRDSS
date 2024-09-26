using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssPlatformVw
{
    public long? OrganizationId { get; set; }

    public string? OrganizationType { get; set; }

    public string? OrganizationCd { get; set; }

    public string? OrganizationNm { get; set; }

    public long? ManagingOrganizationId { get; set; }

    public DateTime? UpdDtm { get; set; }

    public Guid? UpdUserGuid { get; set; }

    public long? NoticeOfTakedownContactId1 { get; set; }

    public string? NoticeOfTakedownContactEmail1 { get; set; }

    public long? TakedownRequestContactId1 { get; set; }

    public string? TakedownRequestContactEmail1 { get; set; }

    public long? NoticeOfTakedownContactId2 { get; set; }

    public string? NoticeOfTakedownContactEmail2 { get; set; }

    public long? TakedownRequestContactId2 { get; set; }

    public string? TakedownRequestContactEmail2 { get; set; }
}
