using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace StrDss.Data.Entities;

/// <summary>
/// A property address that includes any verifiable BC attributes
/// </summary>
public partial class DssPhysicalAddress
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long PhysicalAddressId { get; set; }

    /// <summary>
    /// The source-provided address of a short-term rental offering
    /// </summary>
    public string OriginalAddressTxt { get; set; } = null!;

    /// <summary>
    /// Full JSON result of the source address matching attempt
    /// </summary>
    public string? MatchResultJson { get; set; }

    /// <summary>
    /// The sanitized physical address (returned as fullAddress) that has been derived from the original
    /// </summary>
    public string? MatchAddressTxt { get; set; }

    /// <summary>
    /// The relative score returned from the address matching attempt
    /// </summary>
    public short? MatchScoreAmt { get; set; }

    /// <summary>
    /// The siteID returned by the address match
    /// </summary>
    public string? SiteNo { get; set; }

    /// <summary>
    /// The blockID returned by the address match
    /// </summary>
    public string? BlockNo { get; set; }

    /// <summary>
    /// The computed location point of the matched address
    /// </summary>
    public Geometry? LocationGeometry { get; set; }

    /// <summary>
    /// Indicates whether the address has been identified as exempt from Short Term Rental regulations
    /// </summary>
    public bool? IsExempt { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? ContainingOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    /// <summary>
    /// The unitNumber (suite) returned by the address match (e.g. 100)
    /// </summary>
    public string? UnitNo { get; set; }

    /// <summary>
    /// The civicNumber (building number) returned by the address match (e.g. 1285)
    /// </summary>
    public string? CivicNo { get; set; }

    /// <summary>
    /// The streetName returned by the address match (e.g. Pender)
    /// </summary>
    public string? StreetNm { get; set; }

    /// <summary>
    /// The streetType returned by the address match (e.g. St or Street)
    /// </summary>
    public string? StreetTypeDsc { get; set; }

    /// <summary>
    /// The streetDirection returned by the address match (e.g. W or West)
    /// </summary>
    public string? StreetDirectionDsc { get; set; }

    /// <summary>
    /// The localityName (community) returned by the address match (e.g. Vancouver)
    /// </summary>
    public string? LocalityNm { get; set; }

    /// <summary>
    /// The localityType returned by the address match (e.g. City)
    /// </summary>
    public string? LocalityTypeDsc { get; set; }

    /// <summary>
    /// The provinceCode returned by the address match
    /// </summary>
    public string? ProvinceCd { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? ReplacingPhysicalAddressId { get; set; }

    public virtual DssOrganization? ContainingOrganization { get; set; }

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();

    public virtual ICollection<DssPhysicalAddress> InverseReplacingPhysicalAddress { get; set; } = new List<DssPhysicalAddress>();

    public virtual DssPhysicalAddress? ReplacingPhysicalAddress { get; set; }
}
