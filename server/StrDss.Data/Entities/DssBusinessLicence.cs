using System;
using System.Collections.Generic;

namespace StrDss.Data.Entities;

public partial class DssBusinessLicence
{
    /// <summary>
    /// Unique generated key
    /// </summary>
    public long BusinessLicenceId { get; set; }

    /// <summary>
    /// The local government issued licence number that applies to the rental offering
    /// </summary>
    public string BusinessLicenceNo { get; set; } = null!;

    /// <summary>
    /// The date on which the business licence expires
    /// </summary>
    public DateOnly ExpiryDt { get; set; }

    /// <summary>
    /// The full physical address of the location that is licenced as a short-term rental business
    /// </summary>
    public string? PhysicalRentalAddressTxt { get; set; }

    /// <summary>
    /// Free form description of the type of business licence issued (e.g. short-term rental, bed and breakfast, boarding and lodging, tourist accommodation)
    /// </summary>
    public string? LicenceTypeTxt { get; set; }

    /// <summary>
    /// Notes related to any restrictions associated with the licence
    /// </summary>
    public string? RestrictionTxt { get; set; }

    /// <summary>
    /// Official name of the business
    /// </summary>
    public string? BusinessNm { get; set; }

    /// <summary>
    /// Street address component of the business mailing address
    /// </summary>
    public string? MailingStreetAddressTxt { get; set; }

    /// <summary>
    /// City component of the business mailing address
    /// </summary>
    public string? MailingCityNm { get; set; }

    /// <summary>
    /// Province component of the business mailing address
    /// </summary>
    public string? MailingProvinceCd { get; set; }

    /// <summary>
    /// Postal code component of the business mailing address
    /// </summary>
    public string? MailingPostalCd { get; set; }

    /// <summary>
    /// Full name of the registered business owner
    /// </summary>
    public string? BusinessOwnerNm { get; set; }

    /// <summary>
    /// Phone number of the business owner
    /// </summary>
    public string? BusinessOwnerPhoneNo { get; set; }

    /// <summary>
    /// Email address of the business owner
    /// </summary>
    public string? BusinessOwnerEmailAddressDsc { get; set; }

    /// <summary>
    /// Full name of the business operator or property manager
    /// </summary>
    public string? BusinessOperatorNm { get; set; }

    /// <summary>
    /// Phone number of the business operator
    /// </summary>
    public string? BusinessOperatorPhoneNo { get; set; }

    /// <summary>
    /// Email address of the business operator
    /// </summary>
    public string? BusinessOperatorEmailAddressDsc { get; set; }

    /// <summary>
    /// Description of an infraction
    /// </summary>
    public string? InfractionTxt { get; set; }

    /// <summary>
    /// The date on which the described infraction occurred
    /// </summary>
    public DateOnly? InfractionDt { get; set; }

    /// <summary>
    /// Description or name of the property zoning
    /// </summary>
    public string? PropertyZoneTxt { get; set; }

    /// <summary>
    /// The number of bedrooms in the dwelling unit that are available for short term rental
    /// </summary>
    public short? AvailableBedroomsQty { get; set; }

    /// <summary>
    /// The number of guests that can be accommodated
    /// </summary>
    public short? MaxGuestsAllowedQty { get; set; }

    /// <summary>
    /// Indicates whether the short term rental property is a principal residence
    /// </summary>
    public bool? IsPrincipalResidence { get; set; }

    /// <summary>
    /// Indicates whether the owner lives on the property
    /// </summary>
    public bool? IsOwnerLivingOnsite { get; set; }

    /// <summary>
    /// Indicates whether the business owner rents the property
    /// </summary>
    public bool? IsOwnerPropertyTenant { get; set; }

    /// <summary>
    /// The number used to identify the property
    /// </summary>
    public string? PropertyFolioNo { get; set; }

    /// <summary>
    /// The PID number assigned by the Land Title and Survey Authority that identifies the piece of land
    /// </summary>
    public string? PropertyParcelIdentifierNo { get; set; }

    /// <summary>
    /// The physical description of the property as it is registered with the Land Title and Survey Authority
    /// </summary>
    public string? PropertyLegalDescriptionTxt { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public string LicenceStatusType { get; set; } = null!;

    /// <summary>
    /// Foreign key
    /// </summary>
    public long ProvidingOrganizationId { get; set; }

    /// <summary>
    /// Foreign key
    /// </summary>
    public long? AffectedByPhysicalAddressId { get; set; }

    /// <summary>
    /// Trigger-updated timestamp of last change
    /// </summary>
    public DateTime UpdDtm { get; set; }

    /// <summary>
    /// The globally unique identifier (assigned by the identity provider) for the most recent user to record a change
    /// </summary>
    public Guid? UpdUserGuid { get; set; }

    public virtual DssPhysicalAddress? AffectedByPhysicalAddress { get; set; }

    public virtual ICollection<DssRentalListing> DssRentalListings { get; set; } = new List<DssRentalListing>();

    public virtual DssBusinessLicenceStatusType LicenceStatusTypeNavigation { get; set; } = null!;

    public virtual DssOrganization ProvidingOrganization { get; set; } = null!;
}
