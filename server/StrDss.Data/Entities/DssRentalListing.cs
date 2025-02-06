﻿using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

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
    /// The business licence number that is published for the rental offering
    /// </summary>
    public string? BusinessLicenceNo { get; set; }

    /// <summary>
    /// The Short Term Registry issued permit number
    /// </summary>
    public string? BcRegistryNo { get; set; }

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
    public long? IncludingRentalListingReportId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long OfferingOrganizationId { get; set; }

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

    /// <summary>
    /// Indicates whether the RENTAL LISTING VERSION is a CURRENT RENTAL LISTING (if it is a copy of the most current REPORTED RENTAL LISTING (having the same listing number for the same offering platform)
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING has been reported as taken down by the offering platform
    /// </summary>
    public bool? IsTakenDown { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? DerivedFromRentalListingId { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING was included in the most recent RENTAL LISTING REPORT
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING appeared for the first time in the last reporting period
    /// </summary>
    public bool? IsNew { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING has been subjected to address match changes by a user
    /// </summary>
    public bool? IsChangedAddress { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING has been transferred to a different Local Goverment Organization as a result of address changes
    /// </summary>
    public bool? IsLgTransferred { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING has received a different property address in the last reporting period
    /// </summary>
    public bool? IsChangedOriginalAddress { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public string? ListingStatusType { get; set; }

    /// <summary>
    /// The local government issued licence number that should be used for compariing the rental offering with others
    /// </summary>
    public string? EffectiveBusinessLicenceNo { get; set; }

    /// <summary>
    /// The full name of the rental offering host that should be used for comparison with others
    /// </summary>
    public string? EffectiveHostNm { get; set; }

    /// <summary>
    /// Indicates whether a CURRENT RENTAL LISTING has been subjected to business licence linking changes by a user
    /// </summary>
    public bool? IsChangedBusinessLicence { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? GoverningBusinessLicenceId { get; set; }

    /// <summary>
    /// Indicates when a CURRENT RENTAL LISTING was most recently transferred to a different Local Goverment Organization as a result of address changes
    /// </summary>
    public DateTime? LgTransferDtm { get; set; }

    /// <summary>
    /// Indicates the reason why the listing was taken down
    /// </summary>
    public string? TakeDownReason { get; set; }

    public virtual DssRentalListing? DerivedFromRentalListing { get; set; }

    public virtual ICollection<DssEmailMessage> DssEmailMessages { get; set; } = new List<DssEmailMessage>();

    public virtual ICollection<DssRentalListingContact> DssRentalListingContacts { get; set; } = new List<DssRentalListingContact>();

    public virtual DssBusinessLicence? GoverningBusinessLicence { get; set; }

    public virtual DssRentalListingReport? IncludingRentalListingReport { get; set; }

    public virtual ICollection<DssRentalListing> InverseDerivedFromRentalListing { get; set; } = new List<DssRentalListing>();

    public virtual DssListingStatusType? ListingStatusTypeNavigation { get; set; }

    public virtual DssPhysicalAddress? LocatingPhysicalAddress { get; set; }

    public virtual DssOrganization OfferingOrganization { get; set; } = null!;
}
