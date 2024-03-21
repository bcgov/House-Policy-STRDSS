using StrDss.Common;
using System.Text.Json.Serialization;

namespace StrDss.Model.UserDtos
{
    public class AccessRequestDenyDto
    {
        public long UserIdentityId { get; set; }
        [JsonIgnore]
        public string AccessRequestStatusCd { get; set; } = AccessRequestStatuses.Denied;
        public DateTime UpdDtm { get; set; }
    }
}
