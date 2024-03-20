namespace StrDss.Model.OrganizationDtos
{
    public class ContactPersonDto
    {
        public long OrganizationContactPersonId { get; set; }

        public bool IsPrimary { get; set; }

        public string GivenNm { get; set; } = null!;

        public string FamilyNm { get; set; } = null!;

        public string PhoneNo { get; set; } = null!;

        public string EmailAddressDsc { get; set; } = null!;

        public long ContactedThroughOrganizationId { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }
    }
}
