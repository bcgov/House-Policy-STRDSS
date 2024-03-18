using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

/// <summary>
/// An externally defined domain directory object representing a potential application user or group
/// </summary>
public partial class DssUserIdentity
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long UserIdentityId { get; set; }

    /// <summary>
    /// An immutable unique identifier assigned by the identity provider
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)
    /// </summary>
    public string DisplayNm { get; set; } = null!;

    /// <summary>
    /// A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)
    /// </summary>
    public string IdentityProviderNm { get; set; } = null!;

    /// <summary>
    /// Indicates whether access is currently permitted using this identity
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)
    /// </summary>
    public string AccessRequestStatusDsc { get; set; } = null!;

    /// <summary>
    /// A timestamp indicating when the most recent access request was made by the user
    /// </summary>
    public DateTime? AccessRequestDtm { get; set; }

    /// <summary>
    /// The most recent user-provided reason for requesting application access
    /// </summary>
    public string? AccessRequestJustificationTxt { get; set; }

    /// <summary>
    /// A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)
    /// </summary>
    public string? GivenNm { get; set; }

    /// <summary>
    /// A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)
    /// </summary>
    public string? FamilyNm { get; set; }

    /// <summary>
    /// E-mail address associated with the user by the identity provider
    /// </summary>
    public string? EmailAddressDsc { get; set; }

    /// <summary>
    /// A human-readable organization name that is associated with the user by the identity provider
    /// </summary>
    public string? BusinessNm { get; set; }

    /// <summary>
    /// A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access
    /// </summary>
    public DateTime? TermsAcceptanceDtm { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? RepresentedByOrganizationId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual ICollection<DssEmailMessage> DssEmailMessageAffectedByUserIdentities { get; set; } = new List<DssEmailMessage>();

    public virtual ICollection<DssEmailMessage> DssEmailMessageInitiatingUserIdentities { get; set; } = new List<DssEmailMessage>();

    public virtual ICollection<DssUserRoleAssignment> DssUserRoleAssignments { get; set; } = new List<DssUserRoleAssignment>();

    public virtual DssOrganization? RepresentedByOrganization { get; set; }
}
