using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_VENDORMap : EntityTypeConfiguration<FBBPAYG_VENDOR>
    {
        public FBBPAYG_VENDORMap()
        {
            // Primary Key
            this.HasKey(t => t.MATERIAL_CODE);

            // Table & Column Mappings
            this.ToTable("FBBPAYG_VENDOR", "WBB");
            this.Property(t => t.MATERIAL_CODE).HasColumnName("MATERIAL_CODE");
            this.Property(t => t.VENDOR_ID).HasColumnName("VENDOR_ID");
            this.Property(t => t.SPARE_PARTS_TYPE).HasColumnName("SPARE_PARTS_TYPE");
            this.Property(t => t.MODEL_NAME).HasColumnName("MODEL_NAME");
            this.Property(t => t.CPE_TYPE).HasColumnName("CPE_TYPE");
        }
    }
}
