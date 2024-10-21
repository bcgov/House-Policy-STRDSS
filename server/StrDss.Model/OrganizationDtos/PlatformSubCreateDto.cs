namespace StrDss.Model.OrganizationDtos
{
    public class PlatformSubCreateDto : IPlatformCreateDto
    {
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public long ManagingOrganizationId { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime UpdDtm { get; set; }
        public string? PrimaryNoticeOfTakedownContactEmail { get; set; }
        public string? PrimaryTakedownRequestContactEmail { get; set; }
        public string? SecondaryNoticeOfTakedownContactEmail { get; set; }
        public string? SecondaryTakedownRequestContactEmail { get; set; }
    }
}
