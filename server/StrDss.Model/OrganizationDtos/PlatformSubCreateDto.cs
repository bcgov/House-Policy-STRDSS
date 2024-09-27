namespace StrDss.Model.OrganizationDtos
{
    public class PlatformSubCreateDto : IPlatformCreateDto
    {
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public long ManagingOrganizationId { get; set; }
        public DateTime UpdDtm { get; set; }
    }
}
