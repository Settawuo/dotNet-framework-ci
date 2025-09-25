using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public partial class FBB_CFG_QUERY_REPORTMap : EntityTypeConfiguration<FBB_CFG_QUERY_REPORT>
    {
        public FBB_CFG_QUERY_REPORTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.QUERY_ID });

            // Properties
            this.Property(t => t.QUERY_ID)
                .IsRequired();
            this.Property(t => t.REPORT_ID)
                .IsRequired();
            this.Property(t => t.SHEET_NAME)
               .IsRequired()
               .HasMaxLength(255);
            this.Property(t => t.QUERY_1)
               .IsRequired()
               .HasMaxLength(4000);
            this.Property(t => t.QUERY_2)
               .IsRequired()
               .HasMaxLength(4000);
            this.Property(t => t.QUERY_3)
               .IsRequired()
               .HasMaxLength(4000);
            this.Property(t => t.QUERY_4)
               .IsRequired()
               .HasMaxLength(4000);
            this.Property(t => t.QUERY_5)
              .IsRequired()
              .HasMaxLength(4000);
            this.Property(t => t.OWNER_DB)
             .IsRequired()
             .HasMaxLength(20);

            this.ToTable("FBB_CFG_QUERY_REPORT", "WBB");
            this.Property(t => t.QUERY_ID).HasColumnName("QUERY_ID");
            this.Property(t => t.REPORT_ID).HasColumnName("REPORT_ID");
            this.Property(t => t.SHEET_NAME).HasColumnName("SHEET_NAME");
            this.Property(t => t.QUERY_1).HasColumnName("QUERY_1");
            this.Property(t => t.QUERY_2).HasColumnName("QUERY_2");
            this.Property(t => t.QUERY_3).HasColumnName("QUERY_3");
            this.Property(t => t.QUERY_4).HasColumnName("QUERY_4");
            this.Property(t => t.QUERY_5).HasColumnName("QUERY_5");
            this.Property(t => t.OWNER_DB).HasColumnName("OWNER_DB");

        }

    }

}
