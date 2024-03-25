using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class OrganizationTypeDto
    {
        [JsonPropertyName("value")]
        public string OrganizationType { get; set; } = "";
        [JsonPropertyName("label")]
        public string OrganizationTypeNm { get; set; } = "";
    }
}
