using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class PlatformTypeDto
    {
        [JsonPropertyName("value")]
        public string PlatformType { get; set; }
        [JsonPropertyName("label")]
        public string PlatformTypeNm { get; set; }
    }
}
