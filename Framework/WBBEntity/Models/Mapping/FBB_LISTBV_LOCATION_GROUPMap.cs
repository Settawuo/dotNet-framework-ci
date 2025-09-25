using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_LISTBV_LOCATION_GROUPMap : EntityTypeConfiguration<FBB_LISTBV_LOCATION_GROUP>
    {
        public FBB_LISTBV_LOCATION_GROUPMap()
        {
            this.HasKey(t => new { t.LOCATION_GROUP_ID });

            this.Property(t => t.LOCATION_GROUP_ID);

            this.Property(t => t.LOCATION_CODE)
                .HasMaxLength(100);

            this.Property(t => t.LOCATION_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.SPECIFIC_FLAG)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_DATE);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.UPDATED_DATE);

            this.ToTable("FBB_LISTBV_LOCATION_GROUP", "WBB");
            this.Property(t => t.LOCATION_GROUP_ID).HasColumnName("LOCATION_GROUP_ID");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.LOCATION_NAME).HasColumnName("LOCATION_NAME");
            this.Property(t => t.SPECIFIC_FLAG).HasColumnName("SPECIFIC_FLAG");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
