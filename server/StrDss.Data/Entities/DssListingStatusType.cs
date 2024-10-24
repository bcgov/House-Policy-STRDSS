﻿using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A potential status for a CURRENT RENTAL LISTING (e.g. New, Active, Inactive, Reassigned, Taken Down)
/// </summary>
public partial class DssListingStatusType
{
    /// <summary>
    /// System-consistent code for the listing status (e.g. N, A, I, R, T)
    /// </summary>
    public string ListingStatusType { get; set; } = null!;

    /// <summary>
    /// Business term for the listing status (e.g. New, Active, Inactive, Reassigned, Taken Down)
    /// </summary>
    public string ListingStatusTypeNm { get; set; } = null!;

    /// <summary>
    /// Relative order in which the business prefers to see the status listed
    /// </summary>
    public short ListingStatusSortNo { get; set; }

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();
}
