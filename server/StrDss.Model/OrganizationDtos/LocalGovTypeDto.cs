using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class LocalGovTypeDto
    {
        [JsonPropertyName("value")]
        public string LocalGovernmentType { get; set; } = null!;
        [JsonPropertyName("label")]
        public string LocalGovernmentTypeNm { get; set; } = null!;
        [JsonPropertyName("sort")]
        public short? LocalGovernmentTypeSortNo { get; set; }
    }
}
