﻿using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public NetTopologySuite.Geometries.Geometry? AreaGeometry { get; set; }
        public string? EconomicRegionDsc { get; set; }
        public string? LocalGovernmentType { get; set; }
        public long? ManagingOrganizationId { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }
        public bool? IsLgParticipating { get; set; }
        public bool? IsPrincipalResidenceRequired { get; set; }
        public bool? IsBusinessLicenceRequired { get; set; }
        public bool? IsStrProhibited { get; set; }
        public bool? IsStraaExempt { get; set; }
        public bool? IsActive { get; set; }
        public string? PlatformType { get; set; }
        public string? BusinessLicenceFormatTxt { get; set; }
        public virtual ICollection<ContactPersonDto> ContactPeople { get; set; } = new List<ContactPersonDto>();

    }
}
