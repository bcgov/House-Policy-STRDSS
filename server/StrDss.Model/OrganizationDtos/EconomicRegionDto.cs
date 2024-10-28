using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class EconomicRegionDto
    {
        [JsonPropertyName("value")]
        public string EconomicRegionDsc { get; set; } = null!;
        [JsonPropertyName("label")]
        public string EconomicRegionNm { get; set; } = null!;
        [JsonPropertyName("sort")]
        public short? EconomicRegionSortNo { get; set; }
    }
}
