namespace StrDss.Model.DelistingDtos
{
    public class DelistingRequestCreateDto
    {
        public long LgId { get; set; }
        public long PlatformId { get; set; }
        public long ListingId { get; set; }
        public string ListingUrl { get; set; } = "";
        public bool SendCopy { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
    }
}
