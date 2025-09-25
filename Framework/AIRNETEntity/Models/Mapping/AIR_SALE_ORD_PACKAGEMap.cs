using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_PACKAGEMap : EntityTypeConfiguration<AIR_SALE_ORD_PACKAGE>
    {
        public AIR_SALE_ORD_PACKAGEMap()
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
            this.Property(t => t.IA_NO)
                                    .HasMaxLength(30);
            this.Property(t => t.PACKAGE_CODE)
                                    .HasMaxLength(6);

            this.Property(t => t.PARTIAL_INVOICE)
                                    .HasMaxLength(1);
            this.Property(t => t.OLD_PACKAGE_CODE)
                                    .HasMaxLength(6);

            this.Property(t => t.UPD_BY)
                                    .HasMaxLength(15);
            this.Property(t => t.PAYMENT_TYPE)
                                    .HasMaxLength(10);
            this.Property(t => t.PACKAGE_GROUP)
                                    .HasMaxLength(100);


            this.ToTable("AIR_SALE_ORD_PACKAGE", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.IA_NO).HasColumnName("IA_NO");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.EFFECTIVE_DATE).HasColumnName("EFFECTIVE_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.QUANTITY).HasColumnName("QUANTITY");
            this.Property(t => t.PARTIAL_MONTH).HasColumnName("PARTIAL_MONTH");
            this.Property(t => t.PARTIAL_MNY).HasColumnName("PARTIAL_MNY");
            this.Property(t => t.PARTIAL_INVOICE).HasColumnName("PARTIAL_INVOICE");
            this.Property(t => t.OLD_PACKAGE_CODE).HasColumnName("OLD_PACKAGE_CODE");
            this.Property(t => t.OLD_PARTIAL_MNY).HasColumnName("OLD_PARTIAL_MNY");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.PAYMENT_TYPE).HasColumnName("PAYMENT_TYPE");
            this.Property(t => t.PACKAGE_GROUP).HasColumnName("PACKAGE_GROUP");


        }
    }
}
