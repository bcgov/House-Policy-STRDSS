using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssMessageReason
{
    public long MessageReasonId { get; set; }

    public string EmailMessageType { get; set; } = null!;

    /// <summary>
    /// A description of the justification for initiating a message
    /// </summary>
    public string MessageReasonDsc { get; set; } = null!;

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; set; } = new List<DssEmailMessage>();

    public virtual DssEmailMessageType EmailMessageTypeNavigation { get; set; } = null!;
}
