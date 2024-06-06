using System.Text.Json.Serialization;

namespace StrDss.Model.DelistingDtos
{
    public class BulkTakedownRequestsDto
    {
        public long RentalListingId { get; set; }
        public List<string> CcList { get; set; } = new List<string>();
        public bool IsWithStandardDetail { get; set; }
        public string CustomDetailTxt { get; set; } = "";
        [JsonIgnore]
        public List<string> PlatformEmails { get; set; } = new List<string>();
        [JsonIgnore]
        public long ProvidingPlatformId { get; set; }
    }
}
