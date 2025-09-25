using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_ASSET_SN_ACTMap : EntityTypeConfiguration<FBSS_FIXED_ASSET_SN_ACT>
    {

        public FBSS_FIXED_ASSET_SN_ACTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ACCESS_NUMBER, t.SN });

            // Properties
            this.Property(t => t.ACCESS_NUMBER)
                .HasMaxLength(50);

            this.Property(t => t.SN)
                .HasMaxLength(50);

            this.Property(t => t.COMPANY_CODE)
                .HasMaxLength(250);

            this.Property(t => t.MATERIAL_CODE)
                         .HasMaxLength(50);

            this.Property(t => t.PLANT)
                         .HasMaxLength(250);

            this.Property(t => t.STORAGE_LOCATION)
                         .HasMaxLength(250);

            //this.Property(t => t.SERVICE_SEQ)
            //             .HasMaxLength(50);

            this.Property(t => t.SNPATTERN)
                         .HasMaxLength(1);

            this.Property(t => t.SN_STATUS)
                         .HasMaxLength(50);

            //this.Property(t => t.CREATE_DATE)
            //             .HasMaxLength(50);

            //this.Property(t => t.MODIFY_DATE)
            //             .HasMaxLength(50);

            this.Property(t => t.ASSET_CODE)
                         .HasMaxLength(12);
            this.Property(t => t.MATERIAL_DOC)
                         .HasMaxLength(10);
            this.Property(t => t.DOC_YEAR)
                         .HasMaxLength(4);
            this.Property(t => t.MATERIAL_DOC_MT)
                         .HasMaxLength(10);
            this.Property(t => t.DOC_YEAR_MT)
                         .HasMaxLength(4);
            this.Property(t => t.MATERIAL_DOC_RET)
                         .HasMaxLength(10);
            this.Property(t => t.DOC_YEAR_RET)
                         .HasMaxLength(50);



            // Table & Column Mappings
            this.ToTable("FBSS_FIXED_ASSET_SN_ACT", "WBB");
            this.Property(t => t.ACCESS_NUMBER).HasColumnName("ACCESS_NUMBER");
            this.Property(t => t.SN).HasColumnName("SN");
            this.Property(t => t.COMPANY_CODE).HasColumnName("COMPANY_CODE");
            this.Property(t => t.MATERIAL_CODE).HasColumnName("MATERIAL_CODE");
            this.Property(t => t.PLANT).HasColumnName("PLANT");
            this.Property(t => t.STORAGE_LOCATION).HasColumnName("STORAGE_LOCATION");
            this.Property(t => t.MOVEMENT_TYPE).HasColumnName("MOVEMENT_TYPE");
            this.Property(t => t.SERVICE_SEQ).HasColumnName("SERVICE_SEQ");
            this.Property(t => t.SNPATTERN).HasColumnName("SNPATTERN");
            this.Property(t => t.SN_STATUS).HasColumnName("SN_STATUS");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");
            this.Property(t => t.ASSET_CODE).HasColumnName("ASSET_CODE");
            this.Property(t => t.MATERIAL_DOC).HasColumnName("MATERIAL_DOC");
            this.Property(t => t.DOC_YEAR).HasColumnName("DOC_YEAR");
            this.Property(t => t.MATERIAL_DOC_MT).HasColumnName("MATERIAL_DOC_MT");
            this.Property(t => t.DOC_YEAR_MT).HasColumnName("DOC_YEAR_MT");
            this.Property(t => t.MATERIAL_DOC_RET).HasColumnName("MATERIAL_DOC_RET");
            this.Property(t => t.DOC_YEAR_RET).HasColumnName("DOC_YEAR_RET");
        }
    }
}
