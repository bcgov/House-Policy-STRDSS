using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A type of action that can be recorded against a rental listing
/// </summary>
public partial class DssListingActionType
{
    /// <summary>
    /// System-consistent code for the listing action type
    /// </summary>
    public string ListingActionType { get; set; } = null!;

    /// <summary>
    /// Business term for the listing action type
    /// </summary>
    public string ListingActionTypeNm { get; set; } = null!;

    /// <summary>
    /// Relative order in which the business prefers to see the action type listed
    /// </summary>
    public short ListingActionSortNo { get; set; }

    /// <summary>
    /// Indicates who or what typically originates this action type (User, Platform, System)
    /// </summary>
    public string ActionSourceType { get; set; } = null!;

    public virtual ICollection<DssRentalListingAction> DssRentalListingActions { get; set; } = new List<DssRentalListingAction>();
}
