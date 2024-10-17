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

    public bool? IsActive { get; set; }

    public string? PlatformType { get; set; }

    public string? PlatformTypeNm { get; set; }

    public DateTime? UpdDtm { get; set; }

    public Guid? UpdUserGuid { get; set; }

    public string? UpdUserDisplayNm { get; set; }

    public string? UpdUserGivenNm { get; set; }

    public string? UpdUserFamilyNm { get; set; }

    public long? PrimaryNoticeOfTakedownContactId { get; set; }

    public string? PrimaryNoticeOfTakedownContactEmail { get; set; }

    public long? PrimaryTakedownRequestContactId { get; set; }

    public string? PrimaryTakedownRequestContactEmail { get; set; }

    public long? SecondaryNoticeOfTakedownContactId { get; set; }

    public string? SecondaryNoticeOfTakedownContactEmail { get; set; }

    public long? SecondaryTakedownRequestContactId { get; set; }

    public string? SecondaryTakedownRequestContactEmail { get; set; }
}
