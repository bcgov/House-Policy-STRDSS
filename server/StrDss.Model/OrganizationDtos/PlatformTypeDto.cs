using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class PlatformTypeDto
    {
        [JsonPropertyName("label")]
        public string PlatformTypeNm { get; set; }

        [JsonPropertyName("value")]
        public string PlatformType { get; set; }
    } 

        
}
