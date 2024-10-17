using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class PlatformTypeDto
    {
        [JsonPropertyName("label")]
        public string PlatformType { get; set; }
        [JsonPropertyName("value")]
        public string PlatformTypeNm { get; set; }
    }
}
