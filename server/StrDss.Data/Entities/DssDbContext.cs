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

    public virtual DbSet<DssListingStatusType> DssListingStatusTypes { get; set; }

    public virtual DbSet<DssOrganization> DssOrganizations { get; set; }

    public virtual DbSet<DssOrganizationContactPerson> DssOrganizationContactPeople { get; set; }

    public virtual DbSet<DssOrganizationType> DssOrganizationTypes { get; set; }

    public virtual DbSet<DssPhysicalAddress> DssPhysicalAddresses { get; set; }

    public virtual DbSet<DssRentalListing> DssRentalListings { get; set; }

    public virtual DbSet<DssRentalListingContact> DssRentalListingContacts { get; set; }

    public virtual DbSet<DssRentalListingExtract> DssRentalListingExtracts { get; set; }

    public virtual DbSet<DssRentalListingReport> DssRentalListingReports { get; set; }

    public virtual DbSet<DssRentalListingVw> DssRentalListingVws { get; set; }

    public virtual DbSet<DssRentalUploadHistoryView> DssRentalUploadHistoryViews { get; set; }

    public virtual DbSet<DssUploadDelivery> DssUploadDeliveries { get; set; }

    public virtual DbSet<DssUploadLine> DssUploadLines { get; set; }

    public virtual DbSet<DssUserIdentity> DssUserIdentities { get; set; }

    public virtual DbSet<DssUserIdentityView> DssUserIdentityViews { get; set; }

    public virtual DbSet<DssUserPrivilege> DssUserPrivileges { get; set; }

    public virtual DbSet<DssUserRole> DssUserRoles { get; set; }

    public virtual DbSet<DssUserRoleAssignment> DssUserRoleAssignments { get; set; }

    public virtual DbSet<DssUserRolePrivilege> DssUserRolePrivileges { get; set; }

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

            entity.HasIndex(e => new { e.ConcernedWithRentalListingId, e.MessageDeliveryDtm }, "dss_email_message_i1");

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
            entity.Property(e => e.CustomDetailTxt)
                .HasMaxLength(4000)
                .HasComment("Free form text that should be included in the message body")
                .HasColumnName("custom_detail_txt");
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
            entity.Property(e => e.IsWithStandardDetail)
                .HasComment("Indicates whether message body should include text a block of detail text that is standard for the message type")
                .HasColumnName("is_with_standard_detail");
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

        modelBuilder.Entity<DssListingStatusType>(entity =>
        {
            entity.HasKey(e => e.ListingStatusType).HasName("dss_listing_status_type_pk");

            entity.ToTable("dss_listing_status_type", tb => tb.HasComment("A potential status for a CURRENT RENTAL LISTING (e.g. New, Active, Inactive, Reassigned, Taken Down)"));

            entity.Property(e => e.ListingStatusType)
                .HasMaxLength(2)
                .HasComment("System-consistent code for the listing status (e.g. N, A, I, R, T)")
                .HasColumnName("listing_status_type");
            entity.Property(e => e.ListingStatusSortNo).HasColumnName("listing_status_sort_no");
            entity.Property(e => e.ListingStatusTypeNm)
                .HasMaxLength(50)
                .HasComment("Business term for the listing status (e.g. New, Active, Inactive, Reassigned, Taken Down)")
                .HasColumnName("listing_status_type_nm");
        });

        modelBuilder.Entity<DssOrganization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("dss_organization_pk");

            entity.ToTable("dss_organization", tb => tb.HasComment("A private company or governing body component that plays a role in short term rental reporting or enforcement"));

            entity.HasIndex(e => e.OrganizationType, "dss_organization_i1");

            entity.HasIndex(e => e.ManagingOrganizationId, "dss_organization_i2");

            entity.HasIndex(e => e.OrganizationCd, "dss_organization_uk").IsUnique();

            entity.Property(e => e.OrganizationId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("organization_id");
            entity.Property(e => e.AreaGeometry)
                .HasComment("the multipolygon shape identifying the boundaries of a local government subdivision")
                .HasColumnName("area_geometry");
            entity.Property(e => e.EconomicRegionDsc)
                .HasMaxLength(100)
                .HasComment("A free form description of the economic region to which a Local Government Subdivision belongs")
                .HasColumnName("economic_region_dsc");
            entity.Property(e => e.IsBusinessLicenceRequired)
                .HasComment("Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation")
                .HasColumnName("is_business_licence_required");
            entity.Property(e => e.IsLgParticipating)
                .HasComment("Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing")
                .HasColumnName("is_lg_participating");
            entity.Property(e => e.IsPrincipalResidenceRequired)
                .HasComment("Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions")
                .HasColumnName("is_principal_residence_required");
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

            entity.HasIndex(e => e.OriginalAddressTxt, "dss_physical_address_i1");

            entity.HasIndex(e => e.MatchAddressTxt, "dss_physical_address_i2");

            entity.HasIndex(e => e.ContainingOrganizationId, "dss_physical_address_i3");

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
            entity.Property(e => e.IsChangedOriginalAddress)
                .HasComment("Indicates whether the original address has received a different property address from the platform in the last reporting period")
                .HasColumnName("is_changed_original_address");
            entity.Property(e => e.IsExempt)
                .HasComment("Indicates whether the address has been identified as exempt from Short Term Rental regulations")
                .HasColumnName("is_exempt");
            entity.Property(e => e.IsMatchCorrected)
                .HasComment("Indicates whether the matched address has been manually changed to one that is verified as correct for the listing")
                .HasColumnName("is_match_corrected");
            entity.Property(e => e.IsMatchVerified)
                .HasComment("Indicates whether the matched address has been verified as correct for the listing by the responsible authorities")
                .HasColumnName("is_match_verified");
            entity.Property(e => e.IsSystemProcessing)
                .HasComment("Indicates whether the physical address is being processed by the system and may not yet be in its final form")
                .HasColumnName("is_system_processing");
            entity.Property(e => e.LocalityNm)
                .HasMaxLength(100)
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
                .HasMaxLength(100)
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

            entity.HasIndex(e => new { e.OfferingOrganizationId, e.PlatformListingNo }, "dss_rental_listing_i1");

            entity.HasIndex(e => e.IncludingRentalListingReportId, "dss_rental_listing_i2");

            entity.HasIndex(e => e.DerivedFromRentalListingId, "dss_rental_listing_i3");

            entity.HasIndex(e => e.LocatingPhysicalAddressId, "dss_rental_listing_i4");

            entity.HasIndex(e => new { e.ListingStatusType, e.OfferingOrganizationId }, "dss_rental_listing_i5");

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
                .HasMaxLength(100)
                .HasComment("The local government issued licence number that applies to the rental offering")
                .HasColumnName("business_licence_no");
            entity.Property(e => e.DerivedFromRentalListingId)
                .HasComment("Foreign key")
                .HasColumnName("derived_from_rental_listing_id");
            entity.Property(e => e.IncludingRentalListingReportId)
                .HasComment("Foreign key")
                .HasColumnName("including_rental_listing_report_id");
            entity.Property(e => e.IsActive)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING was included in the most recent RENTAL LISTING REPORT")
                .HasColumnName("is_active");
            entity.Property(e => e.IsChangedAddress)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING has been subjected to address match changes by a user")
                .HasColumnName("is_changed_address");
            entity.Property(e => e.IsChangedOriginalAddress)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING has received a different property address in the last reporting period")
                .HasColumnName("is_changed_original_address");
            entity.Property(e => e.IsCurrent)
                .HasComment("Indicates whether the RENTAL LISTING VERSION is a CURRENT RENTAL LISTING (if it is a copy of the most current REPORTED RENTAL LISTING (having the same listing number for the same offering platform)")
                .HasColumnName("is_current");
            entity.Property(e => e.IsEntireUnit)
                .HasComment("Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom)")
                .HasColumnName("is_entire_unit");
            entity.Property(e => e.IsLgTransferred)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING has been transferred to a different Local Goverment Organization as a result of address changes")
                .HasColumnName("is_lg_transferred");
            entity.Property(e => e.IsNew)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING appeared for the first time in the last reporting period")
                .HasColumnName("is_new");
            entity.Property(e => e.IsTakenDown)
                .HasComment("Indicates whether a CURRENT RENTAL LISTING has been reported as taken down by the offering platform")
                .HasColumnName("is_taken_down");
            entity.Property(e => e.ListingStatusType)
                .HasMaxLength(2)
                .HasComment("Foreign key")
                .HasColumnName("listing_status_type");
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

            entity.HasOne(d => d.ListingStatusTypeNavigation).WithMany(p => p.DssRentalListings)
                .HasForeignKey(d => d.ListingStatusType)
                .HasConstraintName("dss_rental_listing_fk_classified_as");

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

            entity.HasIndex(e => e.ContactedThroughRentalListingId, "dss_rental_listing_contact_i1");

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
                .HasMaxLength(100)
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

        modelBuilder.Entity<DssRentalListingExtract>(entity =>
        {
            entity.HasKey(e => e.RentalListingExtractId).HasName("dss_rental_listing_extract_pk");

            entity.ToTable("dss_rental_listing_extract", tb => tb.HasComment("A prebuilt report that is specific to a subset of rental listings"));

            entity.Property(e => e.RentalListingExtractId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("rental_listing_extract_id");
            entity.Property(e => e.FilteringOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("filtering_organization_id");
            entity.Property(e => e.IsPrRequirementFiltered)
                .HasComment("Indicates whether the report is filtered by jurisdictional principal residence requirement")
                .HasColumnName("is_pr_requirement_filtered");
            entity.Property(e => e.RentalListingExtractNm)
                .HasMaxLength(250)
                .HasComment("A description of the information contained in the extract")
                .HasColumnName("rental_listing_extract_nm");
            entity.Property(e => e.SourceBin)
                .HasComment("The binary image of the information in the report")
                .HasColumnName("source_bin");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.FilteringOrganization).WithMany(p => p.DssRentalListingExtracts)
                .HasForeignKey(d => d.FilteringOrganizationId)
                .HasConstraintName("dss_rental_listing_extract_fk_filtered_by");
        });

        modelBuilder.Entity<DssRentalListingReport>(entity =>
        {
            entity.HasKey(e => e.RentalListingReportId).HasName("dss_rental_listing_report_pk");

            entity.ToTable("dss_rental_listing_report", tb => tb.HasComment("A platform-specific collection of rental listing information that is relevant to a specific month"));

            entity.HasIndex(e => new { e.ProvidingOrganizationId, e.ReportPeriodYm }, "dss_rental_listing_report_uk").IsUnique();

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

        modelBuilder.Entity<DssRentalListingVw>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("dss_rental_listing_vw");

            entity.Property(e => e.AddressSort1ProvinceCd)
                .HasMaxLength(5)
                .HasColumnName("address_sort_1_province_cd");
            entity.Property(e => e.AddressSort2LocalityNm)
                .HasMaxLength(100)
                .HasColumnName("address_sort_2_locality_nm");
            entity.Property(e => e.AddressSort3LocalityTypeDsc)
                .HasMaxLength(50)
                .HasColumnName("address_sort_3_locality_type_dsc");
            entity.Property(e => e.AddressSort4StreetNm)
                .HasMaxLength(100)
                .HasColumnName("address_sort_4_street_nm");
            entity.Property(e => e.AddressSort5StreetTypeDsc)
                .HasMaxLength(50)
                .HasColumnName("address_sort_5_street_type_dsc");
            entity.Property(e => e.AddressSort6StreetDirectionDsc)
                .HasMaxLength(50)
                .HasColumnName("address_sort_6_street_direction_dsc");
            entity.Property(e => e.AddressSort7CivicNo)
                .HasMaxLength(50)
                .HasColumnName("address_sort_7_civic_no");
            entity.Property(e => e.AddressSort8UnitNo)
                .HasMaxLength(50)
                .HasColumnName("address_sort_8_unit_no");
            entity.Property(e => e.AvailableBedroomsQty).HasColumnName("available_bedrooms_qty");
            entity.Property(e => e.BcRegistryNo)
                .HasMaxLength(50)
                .HasColumnName("bc_registry_no");
            entity.Property(e => e.BusinessLicenceNo)
                .HasMaxLength(100)
                .HasColumnName("business_licence_no");
            entity.Property(e => e.EconomicRegionDsc)
                .HasMaxLength(100)
                .HasColumnName("economic_region_dsc");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsBusinessLicenceRequired).HasColumnName("is_business_licence_required");
            entity.Property(e => e.IsChangedAddress).HasColumnName("is_changed_address");
            entity.Property(e => e.IsEntireUnit).HasColumnName("is_entire_unit");
            entity.Property(e => e.IsLgTransferred).HasColumnName("is_lg_transferred");
            entity.Property(e => e.IsMatchCorrected).HasColumnName("is_match_corrected");
            entity.Property(e => e.IsMatchVerified).HasColumnName("is_match_verified");
            entity.Property(e => e.IsNew).HasColumnName("is_new");
            entity.Property(e => e.IsPrincipalResidenceRequired).HasColumnName("is_principal_residence_required");
            entity.Property(e => e.IsTakenDown).HasColumnName("is_taken_down");
            entity.Property(e => e.LastActionDtm).HasColumnName("last_action_dtm");
            entity.Property(e => e.LastActionNm)
                .HasMaxLength(250)
                .HasColumnName("last_action_nm");
            entity.Property(e => e.LatestReportPeriodYm).HasColumnName("latest_report_period_ym");
            entity.Property(e => e.ListingContactNamesTxt).HasColumnName("listing_contact_names_txt");
            entity.Property(e => e.ListingStatusSortNo).HasColumnName("listing_status_sort_no");
            entity.Property(e => e.ListingStatusType)
                .HasMaxLength(2)
                .HasColumnName("listing_status_type");
            entity.Property(e => e.ListingStatusTypeNm)
                .HasMaxLength(50)
                .HasColumnName("listing_status_type_nm");
            entity.Property(e => e.ManagingOrganizationId).HasColumnName("managing_organization_id");
            entity.Property(e => e.ManagingOrganizationNm)
                .HasMaxLength(250)
                .HasColumnName("managing_organization_nm");
            entity.Property(e => e.MatchAddressTxt)
                .HasMaxLength(250)
                .HasColumnName("match_address_txt");
            entity.Property(e => e.MatchScoreAmt).HasColumnName("match_score_amt");
            entity.Property(e => e.NightsBookedYtdQty).HasColumnName("nights_booked_ytd_qty");
            entity.Property(e => e.OfferingOrganizationCd)
                .HasMaxLength(25)
                .HasColumnName("offering_organization_cd");
            entity.Property(e => e.OfferingOrganizationId).HasColumnName("offering_organization_id");
            entity.Property(e => e.OfferingOrganizationNm)
                .HasMaxLength(250)
                .HasColumnName("offering_organization_nm");
            entity.Property(e => e.OriginalAddressTxt)
                .HasMaxLength(250)
                .HasColumnName("original_address_txt");
            entity.Property(e => e.PlatformListingNo)
                .HasMaxLength(50)
                .HasColumnName("platform_listing_no");
            entity.Property(e => e.PlatformListingUrl)
                .HasMaxLength(4000)
                .HasColumnName("platform_listing_url");
            entity.Property(e => e.RentalListingId).HasColumnName("rental_listing_id");
            entity.Property(e => e.SeparateReservationsYtdQty).HasColumnName("separate_reservations_ytd_qty");
        });

        modelBuilder.Entity<DssRentalUploadHistoryView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("dss_rental_upload_history_view");

            entity.Property(e => e.Errors).HasColumnName("errors");
            entity.Property(e => e.FamilyNm)
                .HasMaxLength(25)
                .HasColumnName("family_nm");
            entity.Property(e => e.GivenNm)
                .HasMaxLength(25)
                .HasColumnName("given_nm");
            entity.Property(e => e.OrganizationNm)
                .HasMaxLength(250)
                .HasColumnName("organization_nm");
            entity.Property(e => e.Processed).HasColumnName("processed");
            entity.Property(e => e.ProvidingOrganizationId).HasColumnName("providing_organization_id");
            entity.Property(e => e.ReportPeriodYm).HasColumnName("report_period_ym");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Success).HasColumnName("success");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.UpdDtm).HasColumnName("upd_dtm");
            entity.Property(e => e.UploadDeliveryId).HasColumnName("upload_delivery_id");
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

            entity.HasIndex(e => new { e.IncludingUploadDeliveryId, e.SourceRecordNo }, "dss_upload_line_uk").IsUnique();

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
        });

        modelBuilder.Entity<DssUserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleCd).HasName("dss_user_role_pk");

            entity.ToTable("dss_user_role", tb => tb.HasComment("A set of access rights and privileges within the application that may be granted to users"));

            entity.Property(e => e.UserRoleCd)
                .HasMaxLength(25)
                .HasComment("The immutable system code that identifies the role")
                .HasColumnName("user_role_cd");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");
            entity.Property(e => e.UserRoleDsc)
                .HasMaxLength(200)
                .HasComment("The human-readable description that is given for the role")
                .HasColumnName("user_role_dsc");
            entity.Property(e => e.UserRoleNm)
                .HasMaxLength(250)
                .HasComment("The human-readable name that is given for the role")
                .HasColumnName("user_role_nm");
        });

        modelBuilder.Entity<DssUserRoleAssignment>(entity =>
        {
            entity.HasKey(e => new { e.UserIdentityId, e.UserRoleCd }).HasName("dss_user_role_assignment_pk");

            entity.ToTable("dss_user_role_assignment", tb => tb.HasComment("The association of a grantee credential to a role for the purpose of conveying application privileges"));

            entity.Property(e => e.UserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("user_identity_id");
            entity.Property(e => e.UserRoleCd)
                .HasMaxLength(25)
                .HasComment("Foreign key")
                .HasColumnName("user_role_cd");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.UserIdentity).WithMany(p => p.DssUserRoleAssignments)
                .HasForeignKey(d => d.UserIdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_assignment_fk_granted_to");

            entity.HasOne(d => d.UserRoleCdNavigation).WithMany(p => p.DssUserRoleAssignments)
                .HasForeignKey(d => d.UserRoleCd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_assignment_fk_granted");
        });

        modelBuilder.Entity<DssUserRolePrivilege>(entity =>
        {
            entity.HasKey(e => new { e.UserPrivilegeCd, e.UserRoleCd }).HasName("dss_user_role_privilege_pk");

            entity.ToTable("dss_user_role_privilege", tb => tb.HasComment("The association of a granular application privilege to a role"));

            entity.Property(e => e.UserPrivilegeCd)
                .HasMaxLength(25)
                .HasComment("Foreign key")
                .HasColumnName("user_privilege_cd");
            entity.Property(e => e.UserRoleCd)
                .HasMaxLength(25)
                .HasComment("Foreign key")
                .HasColumnName("user_role_cd");
            entity.Property(e => e.UpdDtm)
                .HasComment("Trigger-updated timestamp of last change")
                .HasColumnName("upd_dtm");
            entity.Property(e => e.UpdUserGuid)
                .HasComment("The globally unique identifier (assigned by the identity provider) for the most recent user to record a change")
                .HasColumnName("upd_user_guid");

            entity.HasOne(d => d.UserPrivilegeCdNavigation).WithMany(p => p.DssUserRolePrivileges)
                .HasForeignKey(d => d.UserPrivilegeCd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_privilege_fk_conferring");

            entity.HasOne(d => d.UserRoleCdNavigation).WithMany(p => p.DssUserRolePrivileges)
                .HasForeignKey(d => d.UserRoleCd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_privilege_fk_conferred_by");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
