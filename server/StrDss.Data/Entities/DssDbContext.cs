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

    public virtual DbSet<DssEmailMessage> DssEmailMessages { get; set; }

    public virtual DbSet<DssOrganization> DssOrganizations { get; set; }

    public virtual DbSet<DssOrganizationContactPerson> DssOrganizationContactPeople { get; set; }

    public virtual DbSet<DssUserIdentity> DssUserIdentities { get; set; }

    public virtual DbSet<DssUserPrivilege> DssUserPrivileges { get; set; }

    public virtual DbSet<DssUserRole> DssUserRoles { get; set; }

    public virtual DbSet<DssUserRoleAssignment> DssUserRoleAssignments { get; set; }

    public virtual DbSet<DssUserRolePrivilege> DssUserRolePrivileges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<DssEmailMessage>(entity =>
        {
            entity.HasKey(e => e.EmailMessageId).HasName("dss_email_message_pk");

            entity.ToTable("dss_email_message", "dss", tb => tb.HasComment("A message that is sent to one or more recipients via email"));

            entity.Property(e => e.EmailMessageId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("email_message_id");
            entity.Property(e => e.AffectedByUserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("affected_by_user_identity_id");
            entity.Property(e => e.CcEmailAddressDsc)
                .HasMaxLength(250)
                .HasComment("E-mail address of a secondary message recipient (directly entered by the user)")
                .HasColumnName("cc_email_address_dsc");
            entity.Property(e => e.EmailMessageType)
                .HasMaxLength(50)
                .HasComment("Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)")
                .HasColumnName("email_message_type");
            entity.Property(e => e.HostEmailAddressDsc)
                .HasMaxLength(250)
                .HasComment("E-mail address of a short term rental host (directly entered by the user as a message recipient)")
                .HasColumnName("host_email_address_dsc");
            entity.Property(e => e.InitiatingUserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("initiating_user_identity_id");
            entity.Property(e => e.InvolvedInOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("involved_in_organization_id");
            entity.Property(e => e.MessageDeliveryDtm)
                .HasComment("A timestamp indicating when the message delivery was initiated")
                .HasColumnName("message_delivery_dtm");
            entity.Property(e => e.MessageReasonDsc)
                .HasMaxLength(250)
                .HasComment("A description of the justification for initiating the message")
                .HasColumnName("message_reason_dsc");
            entity.Property(e => e.MessageTemplateDsc)
                .HasMaxLength(4000)
                .HasComment("The full text or template for the message that is sent")
                .HasColumnName("message_template_dsc");
            entity.Property(e => e.UnreportedListingUrl)
                .HasMaxLength(250)
                .HasComment("User-provided URL for a short-term rental platform listing that is the subject of the message")
                .HasColumnName("unreported_listing_url");

            entity.HasOne(d => d.AffectedByUserIdentity).WithMany(p => p.DssEmailMessageAffectedByUserIdentities)
                .HasForeignKey(d => d.AffectedByUserIdentityId)
                .HasConstraintName("dss_email_message_fk_affecting");

            entity.HasOne(d => d.InitiatingUserIdentity).WithMany(p => p.DssEmailMessageInitiatingUserIdentities)
                .HasForeignKey(d => d.InitiatingUserIdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_email_message_fk_initiated_by");

            entity.HasOne(d => d.InvolvedInOrganization).WithMany(p => p.DssEmailMessages)
                .HasForeignKey(d => d.InvolvedInOrganizationId)
                .HasConstraintName("dss_email_message_fk_involving");
        });

        modelBuilder.Entity<DssOrganization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("dss_organization_pk");

            entity.ToTable("dss_organization", "dss", tb => tb.HasComment("A private company or governing body that plays a role in short term rental reporting or enforcement"));

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
                .HasComment("a level of government or business category")
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
        });

        modelBuilder.Entity<DssOrganizationContactPerson>(entity =>
        {
            entity.HasKey(e => e.OrganizationContactPersonId).HasName("dss_organization_contact_person_pk");

            entity.ToTable("dss_organization_contact_person", "dss", tb => tb.HasComment("A person who has been identified as a notable contact for a particular organization"));

            entity.Property(e => e.OrganizationContactPersonId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("organization_contact_person_id");
            entity.Property(e => e.ContactedThroughOrganizationId)
                .HasComment("Foreign key")
                .HasColumnName("contacted_through_organization_id");
            entity.Property(e => e.EmailAddressDsc)
                .HasMaxLength(250)
                .HasComment("E-mail address given for the contact by the organization")
                .HasColumnName("email_address_dsc");
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
                .HasMaxLength(13)
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
        });

        modelBuilder.Entity<DssUserIdentity>(entity =>
        {
            entity.HasKey(e => e.UserIdentityId).HasName("dss_user_identity_pk");

            entity.ToTable("dss_user_identity", "dss", tb => tb.HasComment("An externally defined domain directory object representing a potential application user or group"));

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
            entity.Property(e => e.AccessRequestStatusDsc)
                .HasMaxLength(25)
                .HasComment("The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied)")
                .HasColumnName("access_request_status_dsc");
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

            entity.HasOne(d => d.RepresentedByOrganization).WithMany(p => p.DssUserIdentities)
                .HasForeignKey(d => d.RepresentedByOrganizationId)
                .HasConstraintName("dss_user_identity_fk_representing");
        });

        modelBuilder.Entity<DssUserPrivilege>(entity =>
        {
            entity.HasKey(e => e.UserPrivilegeId).HasName("dss_user_privilege_pk");

            entity.ToTable("dss_user_privilege", "dss", tb => tb.HasComment("A granular access right or privilege within the application that may be granted to a role"));

            entity.Property(e => e.UserPrivilegeId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_privilege_id");
            entity.Property(e => e.PrivilegeCd)
                .HasMaxLength(25)
                .HasComment("The immutable system code that identifies the privilege")
                .HasColumnName("privilege_cd");
            entity.Property(e => e.PrivilegeNm)
                .HasMaxLength(250)
                .HasComment("The human-readable name that is given for the role")
                .HasColumnName("privilege_nm");
        });

        modelBuilder.Entity<DssUserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("dss_user_role_pk");

            entity.ToTable("dss_user_role", "dss", tb => tb.HasComment("A set of access rights and privileges within the application that may be granted to users"));

            entity.Property(e => e.UserRoleId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_role_id");
            entity.Property(e => e.UserRoleCd)
                .HasMaxLength(25)
                .HasComment("The immutable system code that identifies the role")
                .HasColumnName("user_role_cd");
            entity.Property(e => e.UserRoleNm)
                .HasMaxLength(250)
                .HasComment("The human-readable name that is given for the role")
                .HasColumnName("user_role_nm");
        });

        modelBuilder.Entity<DssUserRoleAssignment>(entity =>
        {
            entity.HasKey(e => e.UserRoleAssignmentId).HasName("dss_user_role_assignment_pk");

            entity.ToTable("dss_user_role_assignment", "dss", tb => tb.HasComment("The association of a grantee credential to a role for the purpose of conveying application privileges"));

            entity.Property(e => e.UserRoleAssignmentId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_role_assignment_id");
            entity.Property(e => e.UserIdentityId)
                .HasComment("Foreign key")
                .HasColumnName("user_identity_id");
            entity.Property(e => e.UserRoleId)
                .HasComment("Foreign key")
                .HasColumnName("user_role_id");

            entity.HasOne(d => d.UserIdentity).WithMany(p => p.DssUserRoleAssignments)
                .HasForeignKey(d => d.UserIdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_assignment_fk_granted_to");

            entity.HasOne(d => d.UserRole).WithMany(p => p.DssUserRoleAssignments)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_assignment_fk_granted");
        });

        modelBuilder.Entity<DssUserRolePrivilege>(entity =>
        {
            entity.HasKey(e => e.UserRolePrivilegeId).HasName("dss_user_role_privilege_pk");

            entity.ToTable("dss_user_role_privilege", "dss", tb => tb.HasComment("The association of a granular application privilege to a role"));

            entity.Property(e => e.UserRolePrivilegeId)
                .HasComment("Unique generated key")
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_role_privilege_id");
            entity.Property(e => e.UserPrivilegeId)
                .HasComment("Foreign key")
                .HasColumnName("user_privilege_id");
            entity.Property(e => e.UserRoleId)
                .HasComment("Foreign key")
                .HasColumnName("user_role_id");

            entity.HasOne(d => d.UserPrivilege).WithMany(p => p.DssUserRolePrivileges)
                .HasForeignKey(d => d.UserPrivilegeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_privilege_fk_conferring");

            entity.HasOne(d => d.UserRole).WithMany(p => p.DssUserRolePrivileges)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dss_user_role_privilege_fk_conferred_by");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
