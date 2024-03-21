using StrDss.Common;
using System.Text.Json.Serialization;

namespace StrDss.Model.UserDtos
{
    public class AccessRequestApproveDto
    {
        public long UserIdentityId { get; set; }
        public long RepresentedByOrganizationId { get; set; }
        [JsonIgnore]
        public string AccessRequestStatusCd { get; set; } = AccessRequestStatuses.Approved;
        public bool IsEnabled { get; set; } = true;
        public DateTime UpdDtm { get; set; }
    }
}
