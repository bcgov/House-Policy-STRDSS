using System;
using System.Collections.Generic;

namespace DataBase.Entities;

public partial class DssAccessRequestStatus
{
    public string AccessRequestStatusCd { get; set; } = null!;

    public string AccessRequestStatusNm { get; set; } = null!;

    public virtual ICollection<DssUserIdentity> DssUserIdentities { get; } = new List<DssUserIdentity>();
}
