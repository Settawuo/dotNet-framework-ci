using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FOA_SUBMIT_ORDERMap : EntityTypeConfiguration<FBSS_FOA_SUBMIT_ORDER>
    {
        public FBSS_FOA_SUBMIT_ORDERMap()
        {
            this.HasKey(t => new { t.ACCESS_NUMBER, t.ORDER_NO });

            this.Property(t => t.ACCESS_NUMBER)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ORDER_NO)
                .IsRequired()
                .HasMaxLength(50);

            this.ToTable("FBSS_FOA_SUBMIT_ORDER", "WBB");
            this.Property(t => t.ACCESS_NUMBER).HasColumnName("ACCESS_NUMBER");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.ORDER_TYPE).HasColumnName("ORDER_TYPE");
            this.Property(t => t.FLAG_TYPE).HasColumnName("FLAG_TYPE");
            this.Property(t => t.SUBCONTRACT_CODE).HasColumnName("SUBCONTRACT_CODE");
            this.Property(t => t.SUBCONTRACT_NAME).HasColumnName("SUBCONTRACT_NAME");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.SERVICE_LIST).HasColumnName("SERVICE_LIST");
            this.Property(t => t.SUBMIT_FLAG).HasColumnName("SUBMIT_FLAG");
            this.Property(t => t.REJECT_REASON).HasColumnName("REJECT_REASON");
            this.Property(t => t.OLT_NAME).HasColumnName("OLT_NAME");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");
            this.Property(t => t.MOBILE_CONTACT).HasColumnName("MOBILE_CONTACT");
            this.Property(t => t.FOA_SUBMIT_DATE).HasColumnName("FOA_SUBMIT_DATE");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");
            this.Property(t => t.INSTALLATION_COST).HasColumnName("INSTALLATION_COST");
            this.Property(t => t.ORDER_NO_SFF).HasColumnName("ORDER_NO_SFF");

        }
    }
}
