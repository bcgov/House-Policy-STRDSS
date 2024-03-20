namespace StrDss.Model.UserDtos
{
    public class AccessRequestDto
    {
        public long UserIdentityId { get; set; }

        public bool IsEnabled { get; set; }

        public string AccessRequestStatusCd { get; set; } = null!;

        public DateTime? AccessRequestDtm { get; set; }

        public string? AccessRequestJustificationTxt { get; set; }

        public string? GivenNm { get; set; }

        public string? FamilyNm { get; set; }

        public string? EmailAddressDsc { get; set; }

        public string? BusinessNm { get; set; }

        public DateTime? TermsAcceptanceDtm { get; set; }

        public long? RepresentedByOrganizationId { get; set; }

        public string OrganizationType { get; set; } = null!;

        public string OrganizationCd { get; set; } = null!;

        public string OrganizationNm { get; set; } = null!;
    }
}
