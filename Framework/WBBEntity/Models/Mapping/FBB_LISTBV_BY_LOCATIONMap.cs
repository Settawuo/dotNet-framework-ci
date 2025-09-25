using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_LISTBV_BY_LOCATIONMap : EntityTypeConfiguration<FBB_LISTBV_BY_LOCATION>
    {
        public FBB_LISTBV_BY_LOCATIONMap()
        {
            this.HasKey(t => new { t.LISTBV_LOC_ID });

            this.Property(t => t.LISTBV_LOC_ID);

            this.Property(t => t.LOCATION_CODE)
                .HasMaxLength(100);

            this.Property(t => t.ADDRESS_ID)
                .HasMaxLength(100);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_DATE);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.UPDATED_DATE);

            this.ToTable("FBB_LISTBV_BY_LOCATION", "WBB");
            this.Property(t => t.LISTBV_LOC_ID).HasColumnName("LISTBV_LOC_ID");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
