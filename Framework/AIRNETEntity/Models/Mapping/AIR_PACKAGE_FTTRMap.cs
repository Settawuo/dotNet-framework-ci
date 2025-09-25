using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_PACKAGE_FTTRMap : EntityTypeConfiguration<AIR_PACKAGE_FTTR>
    {
        public AIR_PACKAGE_FTTRMap()
        {
            // Primary Key
            this
         .HasKey(t => t.ADDRESS_ID);

            // Properties  
            this.Property(t => t.ADDRESS_ID)
         .HasMaxLength(60);
            this.Property(t => t.SFF_PROMOTION_CODE)
         .HasMaxLength(50);
            this.Property(t => t.SFF_PROMOTION_BILL_THA)
         .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_BILL_ENG)
         .HasMaxLength(255);
            this.Property(t => t.PRODUCT_SUBTYPE)
         .HasMaxLength(30);
            this.Property(t => t.OWNER_PRODUCT)
         .HasMaxLength(255);
            this.Property(t => t.CUSTOMER_TYPE)
        .HasMaxLength(20);
            this.Property(t => t.UPD_BY)
         .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("AIR_PACKAGE_FTTR", "AIR_ADMIN");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.SFF_PROMOTION_BILL_THA).HasColumnName("SFF_PROMOTION_BILL_THA");
            this.Property(t => t.SFF_PROMOTION_BILL_ENG).HasColumnName("SFF_PROMOTION_BILL_ENG");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.CUSTOMER_TYPE).HasColumnName("CUSTOMER_TYPE");
            this.Property(t => t.EFFECTIVE_DTM).HasColumnName("EFFECTIVE_DTM");
            this.Property(t => t.EXPIRE_DTM).HasColumnName("EXPIRE_DTM");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
        }
    }
}
