namespace StrDss.Model.UserDtos
{
    public class UserCreateDto
    {
        public Guid UserGuid { get; set; }

        public string DisplayNm { get; set; } = null!;

        public string IdentityProviderNm { get; set; } = null!;

        public bool IsEnabled { get; set; }

        public string AccessRequestStatusDsc { get; set; } = null!;

        public DateTime? AccessRequestDtm { get; set; }

        public string? AccessRequestJustificationTxt { get; set; }

        public string? GivenNm { get; set; }

        public string? FamilyNm { get; set; }

        public string? EmailAddressDsc { get; set; }

        public string? BusinessNm { get; set; }

        public DateTime? TermsAcceptanceDtm { get; set; }

        public long? RepresentedByOrganizationId { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }
    }
}
