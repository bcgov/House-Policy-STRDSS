using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A message that is sent to one or more recipients via email
/// </summary>
public partial class DssEmailMessage
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long EmailMessageId { get; set; }

    /// <summary>
    /// Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)
    /// </summary>
    public string EmailMessageType { get; set; } = null!;

    /// <summary>
    /// A timestamp indicating when the message delivery was initiated
    /// </summary>
    public DateTime MessageDeliveryDtm { get; set; }

    /// <summary>
    /// The full text or template for the message that is sent
    /// </summary>
    public string MessageTemplateDsc { get; set; } = null!;

    public bool IsHostContactedExternally { get; set; }

    public bool IsSubmitterCcRequired { get; set; }

    public long? MessageReasonId { get; set; }

    public string? LgPhoneNo { get; set; }

    public string? UnreportedListingNo { get; set; }

    /// <summary>
    /// E-mail address of a short term rental host (directly entered by the user as a message recipient)
    /// </summary>
    public string? HostEmailAddressDsc { get; set; }

    public string? LgEmailAddressDsc { get; set; }

    /// <summary>
    /// E-mail address of a secondary message recipient (directly entered by the user)
    /// </summary>
    public string? CcEmailAddressDsc { get; set; }

    /// <summary>
    /// User-provided URL for a short-term rental platform listing that is the subject of the message
    /// </summary>
    public string? UnreportedListingUrl { get; set; }

    public string? LgStrBylawUrl { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long InitiatingUserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? AffectedByUserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? InvolvedInOrganizationId { get; set; }

    public virtual DssUserIdentity? AffectedByUserIdentity { get; set; }

    public virtual DssEmailMessageType EmailMessageTypeNavigation { get; set; } = null!;

    public virtual DssUserIdentity InitiatingUserIdentity { get; set; } = null!;

    public virtual DssOrganization? InvolvedInOrganization { get; set; }

    public virtual DssMessageReason? MessageReason { get; set; }
}
