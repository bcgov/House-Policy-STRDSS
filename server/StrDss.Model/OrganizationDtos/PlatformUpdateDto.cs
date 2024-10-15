namespace StrDss.Model.OrganizationDtos
{
    public class PlatformUpdateDto : IPlatformUpdateDto
    {
        public string OrganizationCd { get; }
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public string? PrimaryNoticeOfTakedownContactEmail { get; set; }
        public string? PrimaryTakedownRequestContactEmail { get; set; }
        public string? SecondaryNoticeOfTakedownContactEmail { get; set; }
        public string? SecondaryTakedownRequestContactEmail { get; set; }
        public bool? IsActive { get; set; }
        public string? PlatformType { get; set; }
    }
}
