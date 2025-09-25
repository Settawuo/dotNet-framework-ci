using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGE_ZIPCODEMap : EntityTypeConfiguration<FBB_COVERAGE_ZIPCODE>
    {
        public FBB_COVERAGE_ZIPCODEMap()
        {
            //this.HasKey(t => new
            //{
            //    t.CVRID,
            //    t.CREATED_BY,
            //    t.CREATED_DATE,
            //    t.ZIPCODE_ROWID_EN,
            //    t.ZIPCODE_ROWID_TH,
            //});

            //this.HasKey(t => t.CVRID);

            this.HasKey(t => new
            {
                t.CVRID,
                t.ZIPCODE_ROWID_EN,
                t.ZIPCODE_ROWID_TH
            });

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.ZIPCODE_ROWID_EN)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ZIPCODE_ROWID_TH)
                .IsRequired()
                .HasMaxLength(50);


            this.ToTable("FBB_COVERAGE_ZIPCODE", "WBB");
            this.Property(t => t.CVRID).HasColumnName("CVRID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.ZIPCODE_ROWID_EN).HasColumnName("ZIPCODE_ROWID_EN");
            this.Property(t => t.ZIPCODE_ROWID_TH).HasColumnName("ZIPCODE_ROWID_TH");
        }
    }
}
