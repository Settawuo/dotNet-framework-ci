using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FOA_SUBMIT_ORDER_DTLMap : EntityTypeConfiguration<FBSS_FOA_SUBMIT_ORDER_DTL>
    {
        public FBSS_FOA_SUBMIT_ORDER_DTLMap()
        {
            this.HasKey(t => new { t.ORDER_NO });

            this.Property(t => t.ORDER_NO)
                .HasMaxLength(50);

            this.Property(t => t.SN)
                .HasMaxLength(50);

            this.ToTable("FBSS_FOA_SUBMIT_ORDER_DTL", "WBB");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.SN).HasColumnName("SN");
            this.Property(t => t.MATERIAL_CODE).HasColumnName("MATERIAL_CODE");
            this.Property(t => t.COMPANY_CODE).HasColumnName("COMPANY_CODE");
            this.Property(t => t.PLANT).HasColumnName("PLANT");
            this.Property(t => t.STORAGE_LOCATION).HasColumnName("STORAGE_LOCATION");
            this.Property(t => t.SNPATTERN).HasColumnName("SNPATTERN");
            this.Property(t => t.MOVEMENT_TYPE).HasColumnName("MOVEMENT_TYPE");
            this.Property(t => t.CREATE_DATETIME).HasColumnName("CREATE_DATETIME");
            this.Property(t => t.MODIFY_DATETIME).HasColumnName("MODIFY_DATETIME");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.SERVICE_NAME).HasColumnName("SERVICE_NAME");
            this.Property(t => t.SERVICE_SEQ).HasColumnName("SERVICE_SEQ");
        }
    }
}
