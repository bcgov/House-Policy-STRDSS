using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A description of the justification for initiating a message
/// </summary>
public partial class DssMessageReason
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long MessageReasonId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public string EmailMessageType { get; set; } = null!;

    /// <summary>
    /// A description of the justification for initiating a message
    /// </summary>
    public string MessageReasonDsc { get; set; } = null!;

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; } = new List<DssEmailMessage>();

    public virtual DssEmailMessageType EmailMessageTypeNavigation { get; set; } = null!;
}
