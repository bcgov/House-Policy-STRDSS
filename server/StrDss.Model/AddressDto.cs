using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class AddressDto
    {
        public string Address { get; set; } = "";
        [JsonIgnore]
        public NetTopologySuite.Geometries.Point LocationGeometry { get; set; }
        public long? OrganizationId { get; set; }

    }
}
