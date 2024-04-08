using System;
using System.Collections.Generic;

namespace DataBase.Entities;

public partial class DssEmailMessageType
{
    public string EmailMessageType { get; set; } = null!;

    public string EmailMessageTypeNm { get; set; } = null!;

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; } = new List<DssEmailMessage>();

    public virtual ICollection<DssMessageReason> DssMessageReasons { get; } = new List<DssMessageReason>();
}
