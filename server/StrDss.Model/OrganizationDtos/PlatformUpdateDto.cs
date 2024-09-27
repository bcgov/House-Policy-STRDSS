namespace StrDss.Model.OrganizationDtos
{
    public class PlatformUpdateDto
    {
        public long OrganizationId { get; set; }
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public string? NoticeOfTakedownContactEmail1 { get; set; }
        public string? TakedownRequestContactEmail1 { get; set; }
        public string? NoticeOfTakedownContactEmail2 { get; set; }
        public string? TakedownRequestContactEmail2 { get; set; }
    }
}
