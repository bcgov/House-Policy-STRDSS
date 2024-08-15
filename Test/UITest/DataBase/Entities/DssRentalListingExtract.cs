using System;
using System.Collections.Generic;

namespace DataBase.Entities;

/// <summary>
/// A prebuilt report that is specific to a subset of rental listings
/// </summary>
public partial class DssRentalListingExtract
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long RentalListingExtractId { get; set; }

    /// <summary>
    /// A description of the information contained in the extract
    /// </summary>
    public string RentalListingExtractNm { get; set; } = null!;

    /// <summary>
    /// Indicates whether the report is filtered by jurisdictional principal residence requirement
    /// </summary>
    public bool IsPrRequirementFiltered { get; set; }

    /// <summary>
    /// The binary image of the information in the report
    /// </summary>
    public byte[]? SourceBin { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? FilteringOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssOrganization? FilteringOrganization { get; set; }
}
