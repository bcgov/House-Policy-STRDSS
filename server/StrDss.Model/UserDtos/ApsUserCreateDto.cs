using System.Text.Json.Serialization;

namespace StrDss.Model.UserDtos
{
    public class ApsUserCreateDto : IOrgRoles
    {
        [JsonIgnore]
        public Guid UserGuid { get; set; }

        public string DisplayNm { get; set; } = "";

        [JsonIgnore]
        public string IdentityProviderNm { get; set; } = "aps";

        public bool IsEnabled { get; set; } = true;

        [JsonIgnore]
        public string AccessRequestStatusCd { get; set; } = "Approved";

        [JsonIgnore]
        public DateTime? AccessRequestDtm { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public string? AccessRequestJustificationTxt { get; set; } = "";

        [JsonIgnore]
        public string? GivenNm { get; set; } = "";

        [JsonIgnore]
        public string? FamilyNm { get; set; } = "";

        [JsonIgnore]
        public string? EmailAddressDsc { get; set; } = "";

        [JsonIgnore]
        public string? BusinessNm { get; set; } = "";

        [JsonIgnore]
        public DateTime? TermsAcceptanceDtm { get; set; } = DateTime.UtcNow;

        public long RepresentedByOrganizationId { get; set; }

        public List<string> RoleCds { get; set; } = new List<string>();
    }
}
