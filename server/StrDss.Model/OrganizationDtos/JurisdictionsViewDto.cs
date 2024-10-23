using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class JurisdictionsViewDto
    {
        public long OrganizationId { get; set; }
        public string OrganizationNm { get; set; } = null!;
        [JsonPropertyName("shapeFileId")]
        public string OrganizationCd { get; set; } = null!;
        public bool? IsPrincipalResidenceRequired { get; set; }
        public bool? IsStrProhibited { get; set; }
        public bool? IsBusinessLicenceRequired { get; set; }
        public string? EconomicRegionDsc { get; set; }
        public long? ManagingOrganizationId { get; set; }
    }
}
