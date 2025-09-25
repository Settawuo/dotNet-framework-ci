using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_IA_HISMap : EntityTypeConfiguration<AIR_SALE_ORD_IA_HIS>
    {
        public AIR_SALE_ORD_IA_HISMap()
        {

            // Primary Key
            this.HasKey(t => t.ORDER_NO);
            // Properties
            this.Property(t => t.ORDER_NO)
            .HasMaxLength(30);
            this.Property(t => t.PRODUCT_TYPE)
            .HasMaxLength(30);
            this.Property(t => t.PRODUCT_SUBTYPE)
            .HasMaxLength(30);
            this.Property(t => t.NON_MOBILE_NO)
            .HasMaxLength(50);

            this.ToTable("AIR_SALE_ORD_IA_HIS", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");

        }
    }
}
