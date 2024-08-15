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
    /// Foreign key
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

    /// <summary>
    /// Indicates whether the the property host has already been contacted by external means
    /// </summary>
    public bool? IsHostContactedExternally { get; set; }

    /// <summary>
    /// Indicates whether the user initiating the message should receive a copy of the email
    /// </summary>
    public bool IsSubmitterCcRequired { get; set; }

    /// <summary>
    /// A phone number associated with a Local Government contact
    /// </summary>
    public string? LgPhoneNo { get; set; }

    /// <summary>
    /// The platform issued identification number for the listing (if not included in a rental listing report)
    /// </summary>
    public string? UnreportedListingNo { get; set; }

    /// <summary>
    /// E-mail address of a short term rental host (directly entered by the user as a message recipient)
    /// </summary>
    public string? HostEmailAddressDsc { get; set; }

    /// <summary>
    /// E-mail address of a local government contact (directly entered by the user as a message recipient)
    /// </summary>
    public string? LgEmailAddressDsc { get; set; }

    /// <summary>
    /// E-mail address of a secondary message recipient (directly entered by the user)
    /// </summary>
    public string? CcEmailAddressDsc { get; set; }

    /// <summary>
    /// User-provided URL for a short-term rental platform listing that is the subject of the message
    /// </summary>
    public string? UnreportedListingUrl { get; set; }

    /// <summary>
    /// User-provided URL for a local government bylaw that is the subject of the message
    /// </summary>
    public string? LgStrBylawUrl { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? InitiatingUserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? AffectedByUserIdentityId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? InvolvedInOrganizationId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? BatchingEmailMessageId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? RequestingOrganizationId { get; set; }

    /// <summary>
    /// External identifier for tracking the message delivery progress
    /// </summary>
    public string? ExternalMessageNo { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime? UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? ConcernedWithRentalListingId { get; set; }

    /// <summary>
    /// Indicates whether message body should include text a block of detail text that is standard for the message type
    /// </summary>
    public bool? IsWithStandardDetail { get; set; }

    /// <summary>
    /// Free form text that should be included in the message body
    /// </summary>
    public string? CustomDetailTxt { get; set; }

    public virtual DssUserIdentity? AffectedByUserIdentity { get; set; }

    public virtual DssEmailMessage? BatchingEmailMessage { get; set; }

    public virtual DssRentalListing? ConcernedWithRentalListing { get; set; }

    public virtual DssEmailMessageType EmailMessageTypeNavigation { get; set; } = null!;

    public virtual DssUserIdentity? InitiatingUserIdentity { get; set; }

    public virtual ICollection<DssEmailMessage> InverseBatchingEmailMessage { get; } = new List<DssEmailMessage>();

    public virtual DssOrganization? InvolvedInOrganization { get; set; }

    public virtual DssOrganization? RequestingOrganization { get; set; }
}
