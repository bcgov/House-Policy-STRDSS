namespace StrDss.Model.OrganizationDtos
{
    public class PlatformCreateDto
    {
        public long OrganizationId { get; set; }
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public Guid? UpdUserGuid { get; set; }
        //public virtual ICollection<ContactPersonDto> ContactPeople { get; set; } = new List<ContactPersonDto>();
    }
}
