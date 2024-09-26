namespace StrDss.Model.OrganizationDtos
{
    public class PlatformViewDto
    {
        public PlatformViewDto()
        {
            Subsidiaries = new List<PlatformViewDto>();
        }

        public long OrganizationId { get; set; }
        public string OrganizationType { get; set; } = null!;
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public Guid? UpdUserGuid { get; set; }
        public long? NoticeOfTakedownContactId1 { get; set; }
        public string? NoticeOfTakedownContactEmail1 { get; set; }
        public long? TakedownRequestContactId1 { get; set; }
        public string? TakedownRequestContactEmail1 { get; set; }
        public long? NoticeOfTakedownContactId2 { get; set; }
        public string? NoticeOfTakedownContactEmail2 { get; set; }
        public long? TakedownRequestContactId2 { get; set; }
        public string? TakedownRequestContactEmail2 { get; set; }
        public virtual ICollection<PlatformViewDto> Subsidiaries { get; set; }
    }
}
