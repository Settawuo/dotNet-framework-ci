using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGEAREA_BUILDINGMap : EntityTypeConfiguration<FBB_COVERAGEAREA_BUILDING>
    {
        public FBB_COVERAGEAREA_BUILDINGMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CONTACT_ID, t.BUILDING, t.CREATED_BY, t.CREATED_DATE });

            // Properties
            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.BUILDING_EN)
                .HasMaxLength(50);

            this.Property(t => t.BUILDING_TH)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(2);

            this.Property(t => t.INSTALL_NOTE)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FBB_COVERAGEAREA_BUILDING", "WBB");
            this.Property(t => t.CONTACT_ID).HasColumnName("CONTACT_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.BUILDING).HasColumnName("BUILDING");
            this.Property(t => t.BUILDING_EN).HasColumnName("BUILDING_EN");
            this.Property(t => t.BUILDING_TH).HasColumnName("BUILDING_TH");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.INSTALL_NOTE).HasColumnName("INSTALL_NOTE");
        }
    }
}
