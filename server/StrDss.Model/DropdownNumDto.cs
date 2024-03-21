using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class DropdownNumDto
    {
        [JsonPropertyName("value")]
        public long Id { get; set; }
        [JsonPropertyName("label")]
        public string Description { get; set; } = "";
    }
}
