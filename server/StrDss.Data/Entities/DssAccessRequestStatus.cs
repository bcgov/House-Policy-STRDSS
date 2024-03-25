using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssAccessRequestStatus
{
    public string AccessRequestStatusCd { get; set; } = null!;

    public string AccessRequestStatusNm { get; set; } = null!;

    public virtual ICollection<DssUserIdentity> DssUserIdentities { get; set; } = new List<DssUserIdentity>();
}
