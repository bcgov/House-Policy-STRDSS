namespace StrDss.Model.OrganizationDtos
{
    public class ContactPersonDto
    {
        public long OrganizationContactPersonId { get; set; }

        public bool IsPrimary { get; set; }

        public string? GivenNm { get; set; }

        public string? FamilyNm { get; set; }

        public string? PhoneNo { get; set; }

        public string EmailAddressDsc { get; set; } = null!;

        public long ContactedThroughOrganizationId { get; set; }

        public string? EmailMessageType { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }
    }
}
