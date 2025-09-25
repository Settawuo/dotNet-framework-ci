using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGE_REGIONMap : EntityTypeConfiguration<FBB_COVERAGE_REGION>
    {
        public FBB_COVERAGE_REGIONMap()
        {
            this.HasKey(t => new
            {
                t.FTTX_ID,
            });

            this.Property(t => t.OWNER_PRODUCT)
                .HasMaxLength(20);

            this.Property(t => t.OWNER_TYPE)
                .HasMaxLength(20);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_DATE)
                .IsRequired();

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.GROUP_AMPHUR)
                .HasMaxLength(50);

            this.Property(t => t.TOWER_TH)
                .HasMaxLength(250);

            this.Property(t => t.TOWER_EN)
                .HasMaxLength(250);

            this.Property(t => t.SERVICE_TYPE)
              .HasMaxLength(50);

            this.Property(t => t.COVERAGE_STATUS)
            .HasMaxLength(50);

            this.Property(t => t.LATITUDE)
            .HasMaxLength(20);

            this.Property(t => t.LONGITUDE)
            .HasMaxLength(20);

            this.Property(t => t.ZIPCODE_ROWID_TH)
            .HasMaxLength(50);

            this.Property(t => t.ZIPCODE_ROWID_EN)
            .HasMaxLength(50);

            this.ToTable("FBB_COVERAGE_REGION", "WBB");

            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.OWNER_TYPE).HasColumnName("OWNER_TYPE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.GROUP_AMPHUR).HasColumnName("GROUP_AMPHUR");
            this.Property(t => t.FTTX_ID).HasColumnName("FTTX_ID");
            this.Property(t => t.SERVICE_TYPE).HasColumnName("SERVICE_TYPE");
            this.Property(t => t.TOWER_TH).HasColumnName("TOWER_TH");
            this.Property(t => t.TOWER_EN).HasColumnName("TOWER_EN");
            this.Property(t => t.ONTARGET_DATE_EX).HasColumnName("ONTARGET_DATE_EX");
            this.Property(t => t.ONTARGET_DATE_IN).HasColumnName("ONTARGET_DATE_IN");
            this.Property(t => t.COVERAGE_STATUS).HasColumnName("COVERAGE_STATUS");
            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
            this.Property(t => t.ZIPCODE_ROWID_TH).HasColumnName("ZIPCODE_ROWID_TH");
            this.Property(t => t.ZIPCODE_ROWID_EN).HasColumnName("ZIPCODE_ROWID_EN");
        }
    }
}

