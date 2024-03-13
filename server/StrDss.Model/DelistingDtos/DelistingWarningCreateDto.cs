namespace StrDss.Model.DelistingDtos
{
    public class DelistingWarningCreateDto
    {
        public long PlatformId { get; set; }
        public string ListingUrl { get; set; } = "";
        public string HostEmail { get; set; } = "";
        public long ReasonId { get; set; }
        public bool SendCopy { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
        public string Comment { get; set; } = "";
    }
}
