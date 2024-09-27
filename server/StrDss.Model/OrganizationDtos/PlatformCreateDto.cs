namespace StrDss.Model.OrganizationDtos
{
    public class PlatformCreateDto : IPlatformCreateDto
    {
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public string? PrimaryNoticeOfTakedownContactEmail { get; set; }
        public string? PrimaryTakedownRequestContactEmail { get; set; }
        public string? SecondaryNoticeOfTakedownContactEmail { get; set; }
        public string? SecondaryTakedownRequestContactEmail { get; set; }
    }
}
