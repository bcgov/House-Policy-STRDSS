using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class DropdownNumDto
    {
        [JsonPropertyName("value")]
        public long Id { get; set; }
        [JsonPropertyName("label")]
        public string Description { get; set; } = "";

        /// <summary>Populated for LG dropdown; used by listings filters grouped by local government type.</summary>
        [JsonPropertyName("localGovernmentType")]
        public string? LocalGovernmentType { get; set; }
    }
}
