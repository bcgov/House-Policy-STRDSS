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
    /// The sanitized physical address that has been derived from the original
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

    public virtual DssOrganization? ContainingOrganization { get; set; }

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();
}
