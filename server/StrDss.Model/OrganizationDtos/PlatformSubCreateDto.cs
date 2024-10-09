namespace StrDss.Model.OrganizationDtos
{
    public class PlatformSubCreateDto : IPlatformCreateDto
    {
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public long ManagingOrganizationId { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime UpdDtm { get; set; }
    }
}
