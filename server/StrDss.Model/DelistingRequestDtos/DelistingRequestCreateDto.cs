namespace StrDss.Model.DelistingRequestDtos
{
    public class DelistingRequestCreateDto
    {
        public long PlatformId { get; set; }
        public string ListingUrl { get; set; } = "";
        public bool SendCopy { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
        public string Comment { get; set; } = "";
    }
}
