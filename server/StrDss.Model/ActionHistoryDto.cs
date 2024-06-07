using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class ActionHistoryDto
    {
        public string Action { get; set; } = "";
        public DateTime? Date { get; set; }
        public string? User { get; set; } = "";
        [JsonIgnore]
        public Guid? UserGuid { get; set; }
    }
}
