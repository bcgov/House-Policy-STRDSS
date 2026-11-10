using Microsoft.EntityFrameworkCore;

namespace StrDss.Data.Entities;

public partial class DssDbContext
{
    public virtual DbSet<DssRentalListingSummary> DssRentalListingSummaries { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DssRentalListingSummary>(entity =>
        {
            entity.HasKey(e => e.RentalListingId).HasName("dss_rental_listing_summary_pk");

            entity.ToTable("dss_rental_listing_summary", tb => tb.HasComment(
                "Precomputed search values for the current master rental listing."));

            entity.HasIndex(e => e.LatestReportPeriodYm, "dss_rental_listing_summary_i1");
            entity.HasIndex(e => e.LastActionDtm, "dss_rental_listing_summary_i2");

            entity.Property(e => e.RentalListingId)
                .ValueGeneratedNever()
                .HasColumnName("rental_listing_id");
            entity.Property(e => e.LastActionNm)
                .HasMaxLength(200)
                .HasColumnName("last_action_nm");
            entity.Property(e => e.LastActionDtm)
                .HasColumnName("last_action_dtm");
            entity.Property(e => e.LastRentalListingActionId)
                .HasColumnName("last_rental_listing_action_id");
            entity.Property(e => e.LatestReportPeriodYm)
                .HasColumnName("latest_report_period_ym");
            entity.Property(e => e.PrimaryHostFullNm)
                .HasMaxLength(320)
                .HasColumnName("primary_host_full_nm");
            entity.Property(e => e.BusinessLicenceDigits)
                .HasColumnName("business_licence_digits");
            entity.Property(e => e.EffectiveHostNm)
                .HasMaxLength(320)
                .HasColumnName("effective_host_nm");
            entity.Property(e => e.BcRegistryNo)
                .HasMaxLength(50)
                .HasColumnName("bc_registry_no");
            entity.Property(e => e.MatchAddressTxt)
                .HasMaxLength(250)
                .HasColumnName("match_address_txt");
            entity.Property(e => e.MatchUnitNo)
                .HasMaxLength(100)
                .HasColumnName("match_unit_no");
            entity.Property(e => e.ListingBusinessLicenceNo)
                .HasMaxLength(320)
                .HasColumnName("listing_business_licence_no");
            entity.Property(e => e.UpdDtm)
                .HasDefaultValueSql("now()")
                .HasColumnName("upd_dtm");
        });
    }
}
