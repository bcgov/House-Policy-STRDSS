using System.Text.Json.Serialization;

namespace StrDss.Model.DelistingDtos
{
    public class TakedownNoticesFromListingDto
    {
        public long RentalListingId { get; set; }
        public bool HostEmailSent { get; set; } = false;
        public List<string> CcList { get; set; } = new List<string>();
        public string Comment { get; set; } = "";
        [JsonIgnore]
        public List<string> HostEmails { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> PlatformEmails { get; set; } = new List<string>();
        [JsonIgnore]
        public long ProvidingPlatformId { get; set; }
    }
}
