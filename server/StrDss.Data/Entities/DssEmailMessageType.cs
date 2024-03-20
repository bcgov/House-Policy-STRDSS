using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssEmailMessageType
{
    public string EmailMessageType { get; set; } = null!;

    public string EmailMessageTypeNm { get; set; } = null!;

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; set; } = new List<DssEmailMessage>();
}
