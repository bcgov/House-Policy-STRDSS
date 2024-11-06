using System.Text.Json.Serialization;

namespace StrDss.Model.DelistingDtos
{
    public class ComplianceOrderDto
    {
        public long RentalListingId { get; set; }
        public List<string> BccList { get; set; } = new List<string>();
        public string Comment { get; set; } = "";
        [JsonIgnore]
        public List<string> HostEmails { get; set; } = new List<string>();
    }
}
