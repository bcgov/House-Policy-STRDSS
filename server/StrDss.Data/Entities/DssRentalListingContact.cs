using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A person who has been identified as a notable contact for a particular rental listing
/// </summary>
public partial class DssRentalListingContact
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingContactId { get; set; }

    /// <summary>
    /// Indicates a person with the legal right to the unit being short-term rental
    /// </summary>
    public bool IsPropertyOwner { get; set; }

    /// <summary>
    /// Indicates which of the five possible supplier hosts is represented by this contact
    /// </summary>
    public short? ListingContactNbr { get; set; }

    /// <summary>
    /// The platform identifier for the supplier host
    /// </summary>
    public string? SupplierHostNo { get; set; }

    /// <summary>
    /// The full name of the contact person as inluded in the listing
    /// </summary>
    public string? FullNm { get; set; }

    /// <summary>
    /// Phone number given for the contact
    /// </summary>
    public string? PhoneNo { get; set; }

    /// <summary>
    /// Facsimile numbrer given for the contact
    /// </summary>
    public string? FaxNo { get; set; }

    /// <summary>
    /// Mailing address given for the contact
    /// </summary>
    public string? FullAddressTxt { get; set; }

    /// <summary>
    /// E-mail address given for the contact
    /// </summary>
    public string? EmailAddressDsc { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long ContactedThroughRentalListingId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssRentalListing ContactedThroughRentalListing { get; set; } = null!;
}
