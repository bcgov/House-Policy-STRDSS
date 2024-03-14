using System.Text.Json.Serialization;

namespace StrDss.Model.DelistingDtos
{
    public class DelistingWarningCreateDto
    {
        public long PlatformId { get; set; }
        public long ListingId { get; set; }
        public string ListingUrl { get; set; } = "";
        public string HostEmail { get; set; } = "";
        public bool HostEmailSent { get; set; }
        public long ReasonId { get; set; }
        public bool SendCopy { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
        public string LgContactEmail { get; set; }
        public string LgContactPhone { get; set; }
        public string StrBylawUrl { get; set; }
        public string Comment { get; set; } = "";
        [JsonIgnore]
        public List<string> ToList { get; set; } = new List<string>();
    }
}
