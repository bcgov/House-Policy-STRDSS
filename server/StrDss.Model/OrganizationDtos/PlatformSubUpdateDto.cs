namespace StrDss.Model.OrganizationDtos
{
    public class PlatformSubUpdateDto : IPlatformUpdateDto
    {
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = null!;
        public long ManagingOrganizationId { get; set; }
        public string? PrimaryNoticeOfTakedownContactEmail { get; set; }
        public string? PrimaryTakedownRequestContactEmail { get; set; }
        public string? SecondaryNoticeOfTakedownContactEmail { get; set; }
        public string? SecondaryTakedownRequestContactEmail { get; set; }
        public bool? IsActive { get; set; }
        public DateTime UpdDtm { get; set; }
    }
}
