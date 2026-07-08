using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A user-facing action recorded against a rental listing
/// </summary>
public partial class DssRentalListingAction
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingActionId { get; set; }

    /// <summary>
    /// Foreign key to the master rental listing
    /// </summary>
    public long RentalListingId { get; set; }

    /// <summary>
    /// Foreign key to the listing action type
    /// </summary>
    public string ListingActionType { get; set; } = null!;

    /// <summary>
    /// Timestamp when the action occurred
    /// </summary>
    public DateTime ActionDtm { get; set; }

    /// <summary>
    /// Short-form display label for listing and aggregate pages
    /// </summary>
    public string ActionShortNm { get; set; } = null!;

    /// <summary>
    /// Long-form display label for detail page and data download
    /// </summary>
    public string ActionLongNm { get; set; } = null!;

    /// <summary>
    /// Platform takedown reason when listing_action_type is PlatformTakedown
    /// </summary>
    public string? TakedownReason { get; set; }

    /// <summary>
    /// User who initiated the action, when applicable
    /// </summary>
    public long? InitiatingUserIdentityId { get; set; }

    /// <summary>
    /// Optional link to the originating email message when an email was sent
    /// </summary>
    public long? SourceEmailMessageId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    public virtual DssRentalListing RentalListing { get; set; } = null!;

    public virtual DssListingActionType ListingActionTypeNavigation { get; set; } = null!;

    public virtual DssUserIdentity? InitiatingUserIdentity { get; set; }

    public virtual DssEmailMessage? SourceEmailMessage { get; set; }
}
