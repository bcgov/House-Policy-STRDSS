﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace StrDss.Data.Entities;

/// <summary>
/// A private company or governing body component that plays a role in short term rental reporting or enforcement
/// </summary>
public partial class DssOrganization
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public string OrganizationType { get; set; } = null!;

    /// <summary>
    /// An immutable system code that identifies the organization (e.g. CEU, AIRBNB)
    /// </summary>
    public string OrganizationCd { get; set; } = null!;

    /// <summary>
    /// A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)
    /// </summary>
    public string OrganizationNm { get; set; } = null!;

    /// <summary>
    /// the multipolygon shape identifying the boundaries of a local government subdivision
    /// </summary>
    public Geometry? AreaGeometry { get; set; }

    /// <summary>
    /// Self-referential hierarchical foreign key
    /// </summary>
    public long? ManagingOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    /// <summary>
    /// Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing
    /// </summary>
    public bool? IsLgParticipating { get; set; }

    /// <summary>
    /// Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions
    /// </summary>
    public bool? IsPrincipalResidenceRequired { get; set; }

    /// <summary>
    /// Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation
    /// </summary>
    public bool? IsBusinessLicenceRequired { get; set; }

    /// <summary>
    /// Foreign key for a Local Government Subdivision
    /// </summary>
    public string? EconomicRegionDsc { get; set; }

    /// <summary>
    /// Foreign key for a LOCAL GOVERNMENT
    /// </summary>
    public string? LocalGovernmentType { get; set; }

    /// <summary>
    /// Indicates whether a LOCAL GOVERNMENT SUBDIVISION entirely prohibits short term housing rentals
    /// </summary>
    public bool? IsStrProhibited { get; set; }

    /// <summary>
    /// Indicates whether the ORGANIZATION is currently available for new associations
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Foreign key for a RENTAL PLATFORM
    /// </summary>
    public string? PlatformType { get; set; }

    /// <summary>
    /// A free form indication of how BUSINESS NUMBER is laid out for a LOCAL GOVERNMENT ORGANIZATION
    /// </summary>
    public string? BusinessLicenceFormatTxt { get; set; }

    public virtual ICollection<DssBusinessLicence> DssBusinessLicences { get; set; } = new List<DssBusinessLicence>();

    public virtual ICollection<DssEmailMessage> DssEmailMessageInvolvedInOrganizations { get; set; } = new List<DssEmailMessage>();

    public virtual ICollection<DssEmailMessage> DssEmailMessageRequestingOrganizations { get; set; } = new List<DssEmailMessage>();

    public virtual ICollection<DssOrganizationContactPerson> DssOrganizationContactPeople { get; set; } = new List<DssOrganizationContactPerson>();

    public virtual ICollection<DssPhysicalAddress> DssPhysicalAddresses { get; set; } = new List<DssPhysicalAddress>();

    public virtual ICollection<DssRentalListingExtract> DssRentalListingExtracts { get; set; } = new List<DssRentalListingExtract>();

    public virtual ICollection<DssRentalListingReport> DssRentalListingReports { get; set; } = new List<DssRentalListingReport>();

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();

    public virtual ICollection<DssUploadDelivery> DssUploadDeliveries { get; set; } = new List<DssUploadDelivery>();

    public virtual ICollection<DssUserIdentity> DssUserIdentities { get; set; } = new List<DssUserIdentity>();

    public virtual DssEconomicRegion? EconomicRegionDscNavigation { get; set; }

    public virtual ICollection<DssOrganization> InverseManagingOrganization { get; set; } = new List<DssOrganization>();

    public virtual DssLocalGovernmentType? LocalGovernmentTypeNavigation { get; set; }

    public virtual DssOrganization? ManagingOrganization { get; set; }

    public virtual DssOrganizationType OrganizationTypeNavigation { get; set; } = null!;

    public virtual DssPlatformType? PlatformTypeNavigation { get; set; }
}
