using System.Text.Json.Serialization;

namespace StrDss.Model.DelistingDtos
{
    public class TakedownRequestCreateDto
    {
        public long PlatformId { get; set; }
        public string? ListingId { get; set; }
        public string ListingUrl { get; set; } = "";
        public bool SendCopy { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> ToList { get; set; } = new List<string>();
    }
}
