using NetTopologySuite.Geometries;

namespace StrDss.Model.OrganizationDtos
{
    public class OrganizationDto
    {
        public long OrganizationId { get; set; }

        public string OrganizationType { get; set; } = null!;

        public string OrganizationCd { get; set; } = null!;

        public string OrganizationNm { get; set; } = null!;

        public Geometry? LocalGovernmentGeometry { get; set; }

        public long? ManagingOrganizationId { get; set; }

        public DateTime UpdDtm { get; set; }

        public Guid? UpdUserGuid { get; set; }
    }
}
