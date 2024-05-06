using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StrDss.Data.Entities;

public partial class DssDbContext : DbContext
{
    public DssDbContext(DbContextOptions<DssDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DssAccessRequestStatus> DssAccessRequestStatuses { get; set; }

    public virtual DbSet<DssEmailMessage> DssEmailMessages { get; set; }

    public virtual DbSet<DssEmailMessageType> DssEmailMessageTypes { get; set; }

    public virtual DbSet<DssMessageReason> DssMessageReasons { get; set; }

    public virtual DbSet<DssOrganization> DssOrganizations { get; set; }

    public virtual DbSet<DssOrganizationContactPerson> DssOrganizationContactPeople { get; set; }

    public virtual DbSet<DssOrganizationType> DssOrganizationTypes { get; set; }

    public virtual DbSet<DssPhysicalAddress> DssPhysicalAddresses { get; set; }

    public virtual DbSet<DssRentalListing> DssRentalListings { get; set; }

    public virtual DbSet<DssRentalListingContact> DssRentalListingContacts { get; set; }

    public virtual DbSet<DssRentalListingReport> DssRentalListingReports { get; set; }

    public virtual DbSet<DssUploadDelivery> DssUploadDeliveries { get; set; }

    public virtual DbSet<DssUploadLine> DssUploadLines { get; set; }

    public virtual DbSet<DssUserIdentity> DssUserIdentities { get; set; }

    public virtual DbSet<DssUserIdentityView> DssUserIdentityViews { get; set; }

    public virtual DbSet<DssUserPrivilege> DssUserPrivileges { get; set; }

    public virtual DbSet<DssUserRole> DssUserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<DssAccessRequestStatus>(entity =>
        {
            entity.HasKey(e => e.AccessRequestStatusCd).HasName("dss_access_request_status_pk");

            entity.ToTable("dss_access_request_status", tb => tb.HasComment("A potential status for a user access request (e.g. Requested, Approved, or Denied)"));

            entity.Property(e => e.AccessRequestStatusCd)
                .HasMaxLength(25)
                .HasComment("System-consistent code for the request status")
                .HasColumnName("access_request_status_cd");
            entity.Property(e => e.AccessRequestStatusNm)
                .HasMaxLength(250)
                .HasComment("Business term for the request status")
                .HasColumnName("access_request_status_nm");
        });

        modelBuilder.Entity<DssEmailMessage>(entity =>
        {
            entity.HasKey(e => e.EmailMessageId).HasName("dss_email_message_pk");

            entity.ToTable("dss_email_message", tb => tb.HasComment("A message that is sent to one or more recipients via email"));

            entity.Property(e => e.EmailMessageId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("email_message_id");
            entity.Property(e => e.AffectedByUserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("affected_by_user_identity_id");
            entity.Property(e => e.BatchingEmailMessageId)
                .HasComment("Foreign key")
                .HasColumnName("batching_email_message_id");
            entity.Property(e => e.CcEmailAddressDsc)
                .HasMaxLength(4000)
                .HasComment("E-mail address of a secondary message recipient (directly entered by the user)")
                .HasColumnName("cc_email_address_dsc");
            entity.Property(e => e.ConcernedWithRentalListingId)
                .HasComment("Foreign key")
                .HasColumnName("concerned_with_rental_listing_id");
            entity.Property(e => e.EmailMessageType)
                .HasMaxLength(50)
                .HasComment("Foreign key")
                .HasColumnName("email_message_type");
            entity.Property(e => e.ExternalMessageNo)
                .HasMaxLength(50)
                .HasComment("External identifier for tracking the message delivery progress")
                .HasColumnName("external_message_no");
            entity.Property(e => e.HostEmailAddressDsc)
                .HasMaxLength(320)
                .HasComment("E-mail address of a short term rental host (directly entered by the user as a message recipient)")
                .HasColumnName("host_email_address_dsc");
            entity.Property(e => e.InitiatingUserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("initiating_user_identity_id");
            entity.Property(e => e.InvolvedInOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("involved_in_organization_id");
            entity.Property(e => e.IsHostContactedExternally)
                .HasComment("Indicates whether the the property host has already been contacted by external means")
                .HasColumnName("is_host_contacted_externally");
            entity.Property(e => e.IsSubmitterCcRequired)
                .HasComment("Indicates whether the user initiating the message should receive a copy of the email")
                .HasColumnName("is_submitter_cc_required");
            entity.Property(e => e.LgEmailAddressDsc)
                .HasMaxLength(320)
                .HasComment("E-mail address of a local government contact (directly entered by the user as a message recipient)")
                .HasColumnName("lg_email_address_dsc");
            entity.Property(e => e.LgPhoneNo)
                .HasMaxLength(30)
                .HasComment("A phone number associated with a Local Government contact")
                .HasColumnName("lg_phone_no");
            entity.Property(e => e.LgStrBylawUrl)
                .HasMaxLength(4000)
                .HasComment("User-provided URL for a local government bylaw that is the subject of the message")
                .HasColumnName("lg_str_bylaw_url");
            entity.Property(e => e.MessageDeliveryDtm)
                .HasComment("A timestamp indicating when the message delivery was initiated")
                .HasColumnName("message_delivery_dtm");
            entity.Property(e => e.MessageReasonId)
                .HasComment("Foreign key")
                .HasColumnName("message_reason_id");
            entity.Property(e => e.MessageTemplateDsc)
                .HasMaxLength(4000)
                .HasComment("The full text or template for the message that is sent")
                .HasColumnName("message_template_dsc");
            entity.Property(e => e.RequestingOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("requesting_organization_id");
            entity.Property(e => e.UnreportedListingNo)
                .HasMaxLength(50)
                .HasComment("The platform issued identification number for the listing (if not included in a rental listing report)")
                .HasColumnName("unreported_listing_no");
            entity.Property(e => e.UnreportedListingUrl)
                .HasMaxLength(4000)
                .HasComment("User-provided URL for a short-term rental platform listing that is the subject of the message")
                .HasColumnName("unreported_listing_url");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.AffectedByUserIdentity).WithMany(p => p.DssEmailMessageAffectedByUserIdentities)
                .HasForeignKey(d => d.AffectedByUserIdentityId)
                .HasConstraintName("dss_email_message_fk_affecting");

            entity.HasOne(d => d.BatchingEmailMessage).WithMany(p => p.InverseBatchingEmailMessage)
                .HasForeignKey(d => d.BatchingEmailMessageId)
                .HasConstraintName("dss_email_message_fk_batched_in");

            entity.HasOne(d => d.ConcernedWithRentalListing).WithMany(p => p.DssEmailMessages)
                .HasForeignKey(d => d.ConcernedWithRentalListingId)
                .HasConstraintName("dss_email_message_fk_included_in");

            entity.HasOne(d => d.EmailMessageTypeNavigation).WithMany(p => p.DssEmailMessages)
                .HasForeignKey(d => d.EmailMessageType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_email_message_fk_communicating");

            entity.HasOne(d => d.InitiatingUserIdentity).WithMany(p => p.DssEmailMessageInitiatingUserIdentities)
                .HasForeignKey(d => d.InitiatingUserIdentityId)
                .HasConstraintName("dss_email_message_fk_initiated_by");

            entity.HasOne(d => d.InvolvedInOrganization).WithMany(p => p.DssEmailMessageInvolvedInOrganizations)
                .HasForeignKey(d => d.InvolvedInOrganizationId)
                .HasConstraintName("dss_email_message_fk_involving");

            entity.HasOne(d => d.MessageReason).WithMany(p => p.DssEmailMessages)
                .HasForeignKey(d => d.MessageReasonId)
                .HasConstraintName("dss_email_message_fk_justified_by");

            entity.HasOne(d => d.RequestingOrganization).WithMany(p => p.DssEmailMessageRequestingOrganizations)
                .HasForeignKey(d => d.RequestingOrganizationId)
                .HasConstraintName("dss_email_message_fk_requested_by");
        });

        modelBuilder.Entity<DssEmailMessageType>(entity =>
        {
            entity.HasKey(e => e.EmailMessageType).HasName("dss_email_message_type_pk");

            entity.ToTable("dss_email_message_type", tb => tb.HasComment("The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)"));

            entity.Property(e => e.EmailMessageType)
                .HasMaxLength(50)
                .HasComment("System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)")
                .HasColumnName("email_message_type");
            entity.Property(e => e.EmailMessageTypeNm)
                .HasMaxLength(250)
                .HasComment("Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)")
                .HasColumnName("email_message_type_nm");
        });

        modelBuilder.Entity<DssMessageReason>(entity =>
        {
            entity.HasKey(e => e.MessageReasonId).HasName("dss_message_reason_pk");

            entity.ToTable("dss_message_reason", tb => tb.HasComment("A description of the justification for initiating a message"));

            entity.Property(e => e.MessageReasonId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("message_reason_id");
            entity.Property(e => e.EmailMessageType)
                .HasMaxLength(50)
                .HasComment("Foreign key")
                .HasColumnName("email_message_type");
            entity.Property(e => e.MessageReasonDsc)
                .HasMaxLength(250)
                .HasComment("A description of the justification for initiating a message")
                .HasColumnName("message_reason_dsc");

            entity.HasOne(d => d.EmailMessageTypeNavigation).WithMany(p => p.DssMessageReasons)
                .HasForeignKey(d => d.EmailMessageType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_message_reason_fk_justifying");
        });

        modelBuilder.Entity<DssOrganization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("dss_organization_pk");

            entity.ToTable("dss_organization", tb => tb.HasComment("A private company or governing body that plays a role in short term rental reporting or enforcement"));

            entity.Property(e => e.OrganizationId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("organization_id");
            entity.Property(e => e.LocalGovernmentGeometry)
                .HasComment("the shape identifying the boundaries of a local government")
                .HasColumnName("local_government_geometry");
            entity.Property(e => e.ManagingOrganizationId)
                .HasComment("Self-referential hierarchical foreign key")
                .HasColumnName("managing_organization_id");
            entity.Property(e => e.OrganizationCd)
                .HasMaxLength(25)
                .HasComment("An immutable system code that identifies the organization (e.g. CEU, AIRBNB)")
                .HasColumnName("organization_cd");
            entity.Property(e => e.OrganizationNm)
                .HasMaxLength(250)
                .HasComment("A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria)")
                .HasColumnName("organization_nm");
            entity.Property(e => e.OrganizationType)
                .HasMaxLength(25)
                .HasComment("Foreign key")
                .HasColumnName("organization_type");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.ManagingOrganization).WithMany(p => p.InverseManagingOrganization)
                .HasForeignKey(d => d.ManagingOrganizationId)
                .HasConstraintName("dss_organization_fk_managed_by");

            entity.HasOne(d => d.OrganizationTypeNavigation).WithMany(p => p.DssOrganizations)
                .HasForeignKey(d => d.OrganizationType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_organization_fk_treated_as");
        });

        modelBuilder.Entity<DssOrganizationContactPerson>(entity =>
        {
            entity.HasKey(e => e.OrganizationContactPersonId).HasName("dss_organization_contact_person_pk");

            entity.ToTable("dss_organization_contact_person", tb => tb.HasComment("A person who has been identified as a notable contact for a particular organization"));

            entity.Property(e => e.OrganizationContactPersonId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("organization_contact_person_id");
            entity.Property(e => e.ContactedThroughOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("contacted_through_organization_id");
            entity.Property(e => e.EmailAddressDsc)
                .HasMaxLength(320)
                .HasComment("E-mail address given for the contact by the organization")
                .HasColumnName("email_address_dsc");
            entity.Property(e => e.EmailMessageType)
                .HasMaxLength(50)
                .HasComment("Foreign key")
                .HasColumnName("email_message_type");
            entity.Property(e => e.FamilyNm)
                .HasMaxLength(25)
                .HasComment("A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)")
                .HasColumnName("family_nm");
            entity.Property(e => e.GivenNm)
                .HasMaxLength(25)
                .HasComment("A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)")
                .HasColumnName("given_nm");
            entity.Property(e => e.IsPrimary)
                .HasComment("Indicates whether the contact should receive all communications directed at the organization")
                .HasColumnName("is_primary");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(30)
                .HasComment("Phone number given for the contact by the organization (contains only digits)")
                .HasColumnName("phone_no");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.ContactedThroughOrganization).WithMany(p => p.DssOrganizationContactPeople)
                .HasForeignKey(d => d.ContactedThroughOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_organization_contact_person_fk_contacted_for");

            entity.HasOne(d => d.EmailMessageTypeNavigation).WithMany(p => p.DssOrganizationContactPeople)
                .HasForeignKey(d => d.EmailMessageType)
                .HasConstraintName("dss_organization_contact_person_fk_subscribed_to");
        });

        modelBuilder.Entity<DssOrganizationType>(entity =>
        {
            entity.HasKey(e => e.OrganizationType).HasName("dss_organization_type_pk");

            entity.ToTable("dss_organization_type", tb => tb.HasComment("A level of government or business category"));

            entity.Property(e => e.OrganizationType)
                .HasMaxLength(25)
                .HasComment("System-consistent code for a level of government or business category")
                .HasColumnName("organization_type");
            entity.Property(e => e.OrganizationTypeNm)
                .HasMaxLength(250)
                .HasComment("Business term for a level of government or business category")
                .HasColumnName("organization_type_nm");
        });

        modelBuilder.Entity<DssPhysicalAddress>(entity =>
        {
            entity.HasKey(e => e.PhysicalAddressId).HasName("dss_physical_address_pk");

            entity.ToTable("dss_physical_address", tb => tb.HasComment("A property address that includes any verifiable BC attributes"));

            entity.Property(e => e.PhysicalAddressId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("physical_address_id");
            entity.Property(e => e.BlockNo)
                .HasMaxLength(50)
                .HasComment("The blockID returned by the address match")
                .HasColumnName("block_no");
            entity.Property(e => e.CivicNo)
                .HasMaxLength(50)
                .HasComment("The civicNumber (building number) returned by the address match (e.g. 1285)")
                .HasColumnName("civic_no");
            entity.Property(e => e.ContainingOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("containing_organization_id");
            entity.Property(e => e.IsExempt)
                .HasComment("Indicates whether the address has been identified as exempt from Short Term Rental regulations")
                .HasColumnName("is_exempt");
            entity.Property(e => e.LocalityNm)
                .HasMaxLength(50)
                .HasComment("The localityName (community) returned by the address match (e.g. Vancouver)")
                .HasColumnName("locality_nm");
            entity.Property(e => e.LocalityTypeDsc)
                .HasMaxLength(50)
                .HasComment("The localityType returned by the address match (e.g. City)")
                .HasColumnName("locality_type_dsc");
            entity.Property(e => e.LocationGeometry)
                .HasComment("The computed location point of the matched address")
                .HasColumnName("location_geometry");
            entity.Property(e => e.MatchAddressTxt)
                .HasMaxLength(250)
                .HasComment("The sanitized physical address (returned as fullAddress) that has been derived from the original")
                .HasColumnName("match_address_txt");
            entity.Property(e => e.MatchResultJson)
                .HasComment("Full JSON result of the source address matching attempt")
                .HasColumnType("json")
                .HasColumnName("match_result_json");
            entity.Property(e => e.MatchScoreAmt)
                .HasComment("The relative score returned from the address matching attempt")
                .HasColumnName("match_score_amt");
            entity.Property(e => e.OriginalAddressTxt)
                .HasMaxLength(250)
                .HasComment("The source-provided address of a short-term rental offering")
                .HasColumnName("original_address_txt");
            entity.Property(e => e.ProvinceCd)
                .HasMaxLength(5)
                .HasComment("The provinceCode returned by the address match")
                .HasColumnName("province_cd");
            entity.Property(e => e.ReplacingPhysicalAddressId)
                .HasComment("Foreign key")
                .HasColumnName("replacing_physical_address_id");
            entity.Property(e => e.SiteNo)
                .HasMaxLength(50)
                .HasComment("The siteID returned by the address match")
                .HasColumnName("site_no");
            entity.Property(e => e.StreetDirectionDsc)
                .HasMaxLength(50)
                .HasComment("The streetDirection returned by the address match (e.g. W or West)")
                .HasColumnName("street_direction_dsc");
            entity.Property(e => e.StreetNm)
                .HasMaxLength(50)
                .HasComment("The streetName returned by the address match (e.g. Pender)")
                .HasColumnName("street_nm");
            entity.Property(e => e.StreetTypeDsc)
                .HasMaxLength(50)
                .HasComment("The streetType returned by the address match (e.g. St or Street)")
                .HasColumnName("street_type_dsc");
            entity.Property(e => e.UnitNo)
                .HasMaxLength(50)
                .HasComment("The unitNumber (suite) returned by the address match (e.g. 100)")
                .HasColumnName("unit_no");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.ContainingOrganization).WithMany(p => p.DssPhysicalAddresses)
                .HasForeignKey(d => d.ContainingOrganizationId)
                .HasConstraintName("dss_physical_address_fk_contained_in");

            entity.HasOne(d => d.ReplacingPhysicalAddress).WithMany(p => p.InverseReplacingPhysicalAddress)
                .HasForeignKey(d => d.ReplacingPhysicalAddressId)
                .HasConstraintName("dss_physical_address_fk_replaced_by");
        });

        modelBuilder.Entity<DssRentalListing>(entity =>
        {
            entity.HasKey(e => e.RentalListingId).HasName("dss_rental_listing_pk");

            entity.ToTable("dss_rental_listing", tb => tb.HasComment("A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version"));

            entity.Property(e => e.RentalListingId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("rental_listing_id");
            entity.Property(e => e.AvailableBedroomsQty)
                .HasComment("The number of bedrooms in the dwelling unit that are available for short term rental")
                .HasColumnName("available_bedrooms_qty");
            entity.Property(e => e.BcRegistryNo)
                .HasMaxLength(50)
                .HasComment("The Short Term Registry issued permit number")
                .HasColumnName("bc_registry_no");
            entity.Property(e => e.BusinessLicenceNo)
                .HasMaxLength(50)
                .HasComment("The local government issued licence number that applies to the rental offering")
                .HasColumnName("business_licence_no");
            entity.Property(e => e.DerivedFromRentalListingId)
                .HasComment("Foreign key")
                .HasColumnName("derived_from_rental_listing_id");
            entity.Property(e => e.IncludingRentalListingReportId)
                .HasComment("Foreign key")
                .HasColumnName("including_rental_listing_report_id");
            entity.Property(e => e.IsCurrent)
                .HasComment("Indicates whether the listing version is the most current one (within the same listing number for the same offering platform)")
                .HasColumnName("is_current");
            entity.Property(e => e.IsEntireUnit)
                .HasComment("Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)")
                .HasColumnName("is_entire_unit");
            entity.Property(e => e.IsTakenDown)
                .HasComment("Indicates whether a current listing is no longer considered active")
                .HasColumnName("is_taken_down");
            entity.Property(e => e.LocatingPhysicalAddressId)
                .HasComment("Foreign key")
                .HasColumnName("locating_physical_address_id");
            entity.Property(e => e.NightsBookedQty)
                .HasComment("The number of nights that short term rental accommodation services were provided during the reporting period")
                .HasColumnName("nights_booked_qty");
            entity.Property(e => e.OfferingOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("offering_organization_id");
            entity.Property(e => e.PlatformListingNo)
                .HasMaxLength(50)
                .HasComment("The platform issued identification number for the listing")
                .HasColumnName("platform_listing_no");
            entity.Property(e => e.PlatformListingUrl)
                .HasMaxLength(4000)
                .HasComment("URL for the short-term rental platform listing")
                .HasColumnName("platform_listing_url");
            entity.Property(e => e.SeparateReservationsQty)
                .HasComment("The number of separate reservations that were taken during the reporting period")
                .HasColumnName("separate_reservations_qty");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.DerivedFromRentalListing).WithMany(p => p.InverseDerivedFromRentalListing)
                .HasForeignKey(d => d.DerivedFromRentalListingId)
                .HasConstraintName("dss_rental_listing_fk_generating");

            entity.HasOne(d => d.IncludingRentalListingReport).WithMany(p => p.DssRentalListings)
                .HasForeignKey(d => d.IncludingRentalListingReportId)
                .HasConstraintName("dss_rental_listing_fk_included_in");

            entity.HasOne(d => d.LocatingPhysicalAddress).WithMany(p => p.DssRentalListings)
                .HasForeignKey(d => d.LocatingPhysicalAddressId)
                .HasConstraintName("dss_rental_listing_fk_located_at");

            entity.HasOne(d => d.OfferingOrganization).WithMany(p => p.DssRentalListings)
                .HasForeignKey(d => d.OfferingOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_rental_listing_fk_offered_by");
        });

        modelBuilder.Entity<DssRentalListingContact>(entity =>
        {
            entity.HasKey(e => e.RentalListingContactId).HasName("dss_rental_listing_contact_pk");

            entity.ToTable("dss_rental_listing_contact", tb => tb.HasComment("A person who has been identified as a notable contact for a particular rental listing"));

            entity.Property(e => e.RentalListingContactId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("rental_listing_contact_id");
            entity.Property(e => e.ContactedThroughRentalListingId)
                .HasComment("Foreign key")
                .HasColumnName("contacted_through_rental_listing_id");
            entity.Property(e => e.EmailAddressDsc)
                .HasMaxLength(320)
                .HasComment("E-mail address given for the contact")
                .HasColumnName("email_address_dsc");
            entity.Property(e => e.FaxNo)
                .HasMaxLength(30)
                .HasComment("Facsimile numbrer given for the contact")
                .HasColumnName("fax_no");
            entity.Property(e => e.FullAddressTxt)
                .HasMaxLength(250)
                .HasComment("Mailing address given for the contact")
                .HasColumnName("full_address_txt");
            entity.Property(e => e.FullNm)
                .HasMaxLength(50)
                .HasComment("The full name of the contact person as inluded in the listing")
                .HasColumnName("full_nm");
            entity.Property(e => e.IsPropertyOwner)
                .HasComment("Indicates a person with the legal right to the unit being short-term rental")
                .HasColumnName("is_property_owner");
            entity.Property(e => e.ListingContactNbr)
                .HasComment("Indicates which of the five possible supplier hosts is represented by this contact")
                .HasColumnName("listing_contact_nbr");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(30)
                .HasComment("Phone number given for the contact")
                .HasColumnName("phone_no");
            entity.Property(e => e.SupplierHostNo)
                .HasMaxLength(50)
                .HasComment("The platform identifier for the supplier host")
                .HasColumnName("supplier_host_no");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.ContactedThroughRentalListing).WithMany(p => p.DssRentalListingContacts)
                .HasForeignKey(d => d.ContactedThroughRentalListingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_rental_listing_contact_fk_contacted_for");
        });

        modelBuilder.Entity<DssRentalListingReport>(entity =>
        {
            entity.HasKey(e => e.RentalListingReportId).HasName("dss_rental_listing_report_pk");

            entity.ToTable("dss_rental_listing_report", tb => tb.HasComment("A platform-specific collection of rental listing information that is relevant to a specific month"));

            entity.Property(e => e.RentalListingReportId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("rental_listing_report_id");
            entity.Property(e => e.IsCurrent)
                .HasComment("Indicates whether the rental listing version is the most recent one reported by the platform")
                .HasColumnName("is_current");
            entity.Property(e => e.ProvidingOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("providing_organization_id");
            entity.Property(e => e.ReportPeriodYm)
                .HasComment("The month to which the listing information is relevant (always set to the first day of the month)")
                .HasColumnName("report_period_ym");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.ProvidingOrganization).WithMany(p => p.DssRentalListingReports)
                .HasForeignKey(d => d.ProvidingOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_rental_listing_report_fk_provided_by");
        });

        modelBuilder.Entity<DssUploadDelivery>(entity =>
        {
            entity.HasKey(e => e.UploadDeliveryId).HasName("dss_upload_delivery_pk");

            entity.ToTable("dss_upload_delivery", tb => tb.HasComment("A delivery of uploaded information that is relevant to a specific month"));

            entity.Property(e => e.UploadDeliveryId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("upload_delivery_id");
            entity.Property(e => e.ProvidingOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("providing_organization_id");
            entity.Property(e => e.ReportPeriodYm)
                .HasComment("The month to which the delivery batch is relevant (always set to the first day of the month)")
                .HasColumnName("report_period_ym");
            entity.Property(e => e.SourceBin)
                .HasComment("The binary image of the information that was uploaded")
                .HasColumnName("source_bin");
            entity.Property(e => e.SourceHashDsc)
                .HasMaxLength(256)
                .HasComment("The hash value of the information that was uploaded")
                .HasColumnName("source_hash_dsc");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");
            entity.Property(e => e.UploadDeliveryType)
                .HasMaxLength(25)
                .HasComment("Identifies the treatment applied to ingesting the uploaded information")
                .HasColumnName("upload_delivery_type");

            entity.HasOne(d => d.ProvidingOrganization).WithMany(p => p.DssUploadDeliveries)
                .HasForeignKey(d => d.ProvidingOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_upload_delivery_fk_provided_by");
        });

        modelBuilder.Entity<DssUploadLine>(entity =>
        {
            entity.HasKey(e => e.UploadLineId).HasName("dss_upload_line_pk");

            entity.ToTable("dss_upload_line", tb => tb.HasComment("An upload delivery line that has been extracted from the source"));

            entity.Property(e => e.UploadLineId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("upload_line_id");
            entity.Property(e => e.ErrorTxt)
                .HasMaxLength(32000)
                .HasComment("Freeform description of the problem found while attempting to interpret the report line")
                .HasColumnName("error_txt");
            entity.Property(e => e.IncludingUploadDeliveryId)
                .HasComment("Foreign key")
                .HasColumnName("including_upload_delivery_id");
            entity.Property(e => e.IsProcessed)
                .HasComment("Indicates that no further ingestion attempt is required for the upload line")
                .HasColumnName("is_processed");
            entity.Property(e => e.IsSystemFailure)
                .HasComment("Indicates that a system fault has prevented complete ingestion of the upload line")
                .HasColumnName("is_system_failure");
            entity.Property(e => e.IsValidationFailure)
                .HasComment("Indicates that there has been a validation problem that prevents successful ingestion of the upload line")
                .HasColumnName("is_validation_failure");
            entity.Property(e => e.SourceLineTxt)
                .HasMaxLength(32000)
                .HasComment("Full text of the uploaod line")
                .HasColumnName("source_line_txt");
            entity.Property(e => e.SourceOrganizationCd)
                .HasMaxLength(25)
                .HasComment("An immutable system code identifying the organization who created the information in the upload line (e.g. AIRBNB)")
                .HasColumnName("source_organization_cd");
            entity.Property(e => e.SourceRecordNo)
                .HasMaxLength(50)
                .HasComment("The immutable identification number for the source record, such as a rental listing number")
                .HasColumnName("source_record_no");

            entity.HasOne(d => d.IncludingUploadDelivery).WithMany(p => p.DssUploadLines)
                .HasForeignKey(d => d.IncludingUploadDeliveryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_upload_line_fk_included_in");
        });

        modelBuilder.Entity<DssUserIdentity>(entity =>
        {
            entity.HasKey(e => e.UserIdentityId).HasName("dss_user_identity_pk");

            entity.ToTable("dss_user_identity", tb => tb.HasComment("An externally defined domain directory object representing a potential application user or group"));

            entity.Property(e => e.UserIdentityId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_identity_id");
            entity.Property(e => e.AccessRequestDtm)
                .HasComment("A timestamp indicating when the most recent access request was made by the user")
                .HasColumnName("access_request_dtm");
            entity.Property(e => e.AccessRequestJustificationTxt)
                .HasMaxLength(250)
                .HasComment("The most recent user-provided reason for requesting application access")
                .HasColumnName("access_request_justification_txt");
            entity.Property(e => e.AccessRequestStatusCd)
                .HasMaxLength(25)
                .HasComment("The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)")
                .HasColumnName("access_request_status_cd");
            entity.Property(e => e.BusinessNm)
                .HasMaxLength(250)
                .HasComment("A human-readable organization name that is associated with the user by the identity provider")
                .HasColumnName("business_nm");
            entity.Property(e => e.DisplayNm)
                .HasMaxLength(250)
                .HasComment("A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name)")
                .HasColumnName("display_nm");
            entity.Property(e => e.EmailAddressDsc)
                .HasMaxLength(320)
                .HasComment("E-mail address associated with the user by the identity provider")
                .HasColumnName("email_address_dsc");
            entity.Property(e => e.FamilyNm)
                .HasMaxLength(25)
                .HasComment("A name that is often shared amongst members of the same family (commonly known as a surname within some cultures)")
                .HasColumnName("family_nm");
            entity.Property(e => e.GivenNm)
                .HasMaxLength(25)
                .HasComment("A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name)")
                .HasColumnName("given_nm");
            entity.Property(e => e.IdentityProviderNm)
                .HasMaxLength(25)
                .HasComment("A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness)")
                .HasColumnName("identity_provider_nm");
            entity.Property(e => e.IsEnabled)
                .HasComment("Indicates whether access is currently permitted using this identity")
                .HasColumnName("is_enabled");
            entity.Property(e => e.RepresentedByOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("represented_by_organization_id");
            entity.Property(e => e.TermsAcceptanceDtm)
                .HasComment("A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access")
                .HasColumnName("terms_acceptance_dtm");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");
            entity.Property(e => e.UserGuid)
                .HasComment("An immutable unique identifier assigned by the identity provider")
                .HasColumnName("user_guid");

            entity.HasOne(d => d.AccessRequestStatusCdNavigation).WithMany(p => p.DssUserIdentities)
                .HasForeignKey(d => d.AccessRequestStatusCd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_identity_fk_given");

            entity.HasOne(d => d.RepresentedByOrganization).WithMany(p => p.DssUserIdentities)
                .HasForeignKey(d => d.RepresentedByOrganizationId)
                .HasConstraintName("dss_user_identity_fk_representing");

            entity.HasMany(d => d.UserRoleCds).WithMany(p => p.UserIdentities)
                .UsingEntity<Dictionary<string, object>>(
                    "DssUserRoleAssignment",
                    r => r.HasOne<DssUserRole>().WithMany()
                        .HasForeignKey("UserRoleCd")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("dss_user_role_assignment_fk_granted"),
                    l => l.HasOne<DssUserIdentity>().WithMany()
                        .HasForeignKey("UserIdentityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("dss_user_role_assignment_fk_granted_to"),
                    j =>
                    {
                        j.HasKey("UserIdentityId", "UserRoleCd").HasName("dss_user_role_assignment_pk");
                        j.ToTable("dss_user_role_assignment", tb => tb.HasComment("The association of a grantee credential to a role for the purpose of conveying application privileges"));
                        j.IndexerProperty<long>("UserIdentityId")
                            .HasComment("Foreign key")
                            .HasColumnName("user_identity_id");
                        j.IndexerProperty<string>("UserRoleCd")
                            .HasMaxLength(25)
                            .HasComment("Foreign key")
                            .HasColumnName("user_role_cd");
                    });
        });

        modelBuilder.Entity<DssUserIdentityView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("dss_user_identity_view");

            entity.Property(e => e.AccessRequestDtm).HasColumnName("access_request_dtm");
            entity.Property(e => e.AccessRequestJustificationTxt)
                .HasMaxLength(250)
                .HasColumnName("access_request_justification_txt");
            entity.Property(e => e.AccessRequestStatusCd)
                .HasMaxLength(25)
                .HasColumnName("access_request_status_cd");
            entity.Property(e => e.BusinessNm)
                .HasMaxLength(250)
                .HasColumnName("business_nm");
            entity.Property(e => e.EmailAddressDsc)
                .HasMaxLength(320)
                .HasColumnName("email_address_dsc");
            entity.Property(e => e.FamilyNm)
                .HasMaxLength(25)
                .HasColumnName("family_nm");
            entity.Property(e => e.GivenNm)
                .HasMaxLength(25)
                .HasColumnName("given_nm");
            entity.Property(e => e.IdentityProviderNm)
                .HasMaxLength(25)
                .HasColumnName("identity_provider_nm");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.OrganizationCd)
                .HasMaxLength(25)
                .HasColumnName("organization_cd");
            entity.Property(e => e.OrganizationNm)
                .HasMaxLength(250)
                .HasColumnName("organization_nm");
            entity.Property(e => e.OrganizationType)
                .HasMaxLength(25)
                .HasColumnName("organization_type");
            entity.Property(e => e.RepresentedByOrganizationId).HasColumnName("represented_by_organization_id");
            entity.Property(e => e.TermsAcceptanceDtm).HasColumnName("terms_acceptance_dtm");
            entity.Property(e => e.UpdDtm).HasColumnName("upd_dtm");
            entity.Property(e => e.UserIdentityId).HasColumnName("user_identity_id");
        });

        modelBuilder.Entity<DssUserPrivilege>(entity =>
        {
            entity.HasKey(e => e.UserPrivilegeCd).HasName("dss_user_privilege_pk");

            entity.ToTable("dss_user_privilege", tb => tb.HasComment("A granular access right or privilege within the application that may be granted to a role"));

            entity.Property(e => e.UserPrivilegeCd)
                .HasMaxLength(25)
                .HasComment("The immutable system code that identifies the privilege")
                .HasColumnName("user_privilege_cd");
            entity.Property(e => e.UserPrivilegeNm)
                .HasMaxLength(250)
                .HasComment("The human-readable name that is given for the role")
                .HasColumnName("user_privilege_nm");

            entity.HasMany(d => d.UserRoleCds).WithMany(p => p.UserPrivilegeCds)
                .UsingEntity<Dictionary<string, object>>(
                    "DssUserRolePrivilege",
                    r => r.HasOne<DssUserRole>().WithMany()
                        .HasForeignKey("UserRoleCd")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("dss_user_role_privilege_fk_conferred_by"),
                    l => l.HasOne<DssUserPrivilege>().WithMany()
                        .HasForeignKey("UserPrivilegeCd")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("dss_user_role_privilege_fk_conferring"),
                    j =>
                    {
                        j.HasKey("UserPrivilegeCd", "UserRoleCd").HasName("dss_user_role_privilege_pk");
                        j.ToTable("dss_user_role_privilege", tb => tb.HasComment("The association of a granular application privilege to a role"));
                        j.IndexerProperty<string>("UserPrivilegeCd")
                            .HasMaxLength(25)
                            .HasComment("Foreign key")
                            .HasColumnName("user_privilege_cd");
                        j.IndexerProperty<string>("UserRoleCd")
                            .HasMaxLength(25)
                            .HasComment("Foreign key")
                            .HasColumnName("user_role_cd");
                    });
        });

        modelBuilder.Entity<DssUserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleCd).HasName("dss_user_role_pk");

            entity.ToTable("dss_user_role", tb => tb.HasComment("A set of access rights and privileges within the application that may be granted to users"));

            entity.Property(e => e.UserRoleCd)
                .HasMaxLength(25)
                .HasComment("The immutable system code that identifies the role")
                .HasColumnName("user_role_cd");
            entity.Property(e => e.UserRoleNm)
                .HasMaxLength(250)
                .HasComment("The human-readable name that is given for the role")
                .HasColumnName("user_role_nm");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
