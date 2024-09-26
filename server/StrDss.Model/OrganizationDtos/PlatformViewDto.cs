namespace StrDss.Model.OrganizationDtos
{
    public class PlatformViewDto
    {
        public PlatformViewDto()
        {
            Subsidiaries = new List<PlatformViewDto>();
        }

        public long OrganizationId { get; set; }
        public string OrganizationCd { get; set; } = null!;
        public string OrganizationNm { get; set; } = null!;
        public DateTime UpdDtm { get; set; }
        public Guid? UpdUserGuid { get; set; }
        public long NoticeOfTakedownContactId { get; set; }
        public string NoticeOfTakedownContactEmail { get; set; }
        public long TakedownRequestContactId { get; set; }
        public string TakedownRequestContactEmail { get; set; }
        public virtual ICollection<PlatformViewDto> Subsidiaries { get; set; }
    }
}
