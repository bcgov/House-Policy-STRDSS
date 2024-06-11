using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version
/// </summary>
public partial class DssRentalListing
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingId { get; set; }

    /// <summary>
    /// The platform issued identification number for the listing
    /// </summary>
    public string PlatformListingNo { get; set; } = null!;

    /// <summary>
    /// URL for the short-term rental platform listing
    /// </summary>
    public string? PlatformListingUrl { get; set; }

    /// <summary>
    /// The local government issued licence number that applies to the rental offering
    /// </summary>
    public string? BusinessLicenceNo { get; set; }

    /// <summary>
    /// The Short Term Registry issued permit number
    /// </summary>
    public string? BcRegistryNo { get; set; }

    /// <summary>
    /// Indicates whether the listing version is the most current one (within the same listing number for the same offering platform)
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Indicates whether a current listing is no longer considered active
    /// </summary>
    public bool? IsTakenDown { get; set; }

    /// <summary>
    /// Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)
    /// </summary>
    public bool? IsEntireUnit { get; set; }

    /// <summary>
    /// The number of bedrooms in the dwelling unit that are available for short term rental
    /// </summary>
    public short? AvailableBedroomsQty { get; set; }

    /// <summary>
    /// The number of nights that short term rental accommodation services were provided during the reporting period
    /// </summary>
    public short? NightsBookedQty { get; set; }

    /// <summary>
    /// The number of separate reservations that were taken during the reporting period
    /// </summary>
    public short? SeparateReservationsQty { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long OfferingOrganizationId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? IncludingRentalListingReportId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? DerivedFromRentalListingId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? LocatingPhysicalAddressId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssRentalListing? DerivedFromRentalListing { get; set; }

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; } = new List<DssEmailMessage>();

    public virtual ICollection<DssRentalListingContact> DssRentalListingContacts { get; } = new List<DssRentalListingContact>();

    public virtual DssRentalListingReport? IncludingRentalListingReport { get; set; }

    public virtual ICollection<DssRentalListing> InverseDerivedFromRentalListing { get; } = new List<DssRentalListing>();

    public virtual DssPhysicalAddress? LocatingPhysicalAddress { get; set; }

    public virtual DssOrganization OfferingOrganization { get; set; } = null!;
}
