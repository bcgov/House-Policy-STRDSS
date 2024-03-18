using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// A person who has been identified as a notable contact for a particular organization
/// </summary>
public partial class DssOrganizationContactPerson
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long OrganizationContactPersonId { get; set; }

    /// <summary>
    /// Indicates whether the contact should receive all communications directed at the organization
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)
    /// </summary>
    public string GivenNm { get; set; } = null!;

    /// <summary>
    /// A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)
    /// </summary>
    public string FamilyNm { get; set; } = null!;

    /// <summary>
    /// Phone number given for the contact by the organization (contains only digits)
    /// </summary>
    public string PhoneNo { get; set; } = null!;

    /// <summary>
    /// E-mail address given for the contact by the organization
    /// </summary>
    public string EmailAddressDsc { get; set; } = null!;

    /// <summary>
    /// Foreign key
    /// </summary>
    public long ContactedThroughOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssOrganization ContactedThroughOrganization { get; set; } = null!;
}
