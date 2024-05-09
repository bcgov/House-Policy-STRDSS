using System.Text.Json.Serialization;

namespace StrDss.Model.OrganizationDtos
{
    public class OrganizationDto
    {
        [JsonPropertyName("value")]
        public long OrganizationId { get; set; }

        public string OrganizationType { get; set; } = null!;

        public string OrganizationCd { get; set; } = null!;

        [JsonPropertyName("label")]
        public string OrganizationNm { get; set; } = null!;

        public Geometry? LocalGovernmentGeometry { get; set; }

        public long? ManagingOrganizationId { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }

        public virtual ICollection<ContactPersonDto> ContactPeople { get; set; } = new List<ContactPersonDto>();

    }
}
