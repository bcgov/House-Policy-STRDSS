using System;
using System.Collections.Generic;

namespace DataBase.Entities;

public partial class DssUserIdentityView
{
    public long? UserIdentityId { get; set; }

    public bool? IsEnabled { get; set; }

    public string? AccessRequestStatusCd { get; set; }

    public DateTime? AccessRequestDtm { get; set; }

    public string? AccessRequestJustificationTxt { get; set; }

    public string? IdentityProviderNm { get; set; }

    public string? GivenNm { get; set; }

    public string? FamilyNm { get; set; }

    public string? EmailAddressDsc { get; set; }

    public string? BusinessNm { get; set; }

    public DateTime? TermsAcceptanceDtm { get; set; }

    public long? RepresentedByOrganizationId { get; set; }

    public string? OrganizationType { get; set; }

    public string? OrganizationCd { get; set; }

    public string? OrganizationNm { get; set; }

    public DateTime? UpdDtm { get; set; }
}
