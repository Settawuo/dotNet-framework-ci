using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORDERMap : EntityTypeConfiguration<AIR_SALE_ORDER>
    {
        public AIR_SALE_ORDERMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_NO);

            // Properties 
            this.Property(t => t.ORDER_TYPE)
            .HasMaxLength(3);
            this.Property(t => t.ORDER_CREATED_BY)
            .HasMaxLength(15);
            this.Property(t => t.CUSTOMER_ID)
            .HasMaxLength(10);
            this.Property(t => t.ACCOUNT_NO)
            .HasMaxLength(10);
            this.Property(t => t.SALE_USER_NAME)
            .HasMaxLength(15);
            this.Property(t => t.INVOICING_CO_ID)
            .IsRequired()
            .HasMaxLength(1);
            this.Property(t => t.ORDER_STATUS)
            .HasMaxLength(2);
            this.Property(t => t.CANCEL_ORDER_NO)
            .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORDER", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.LOCATION_ID).HasColumnName("LOCATION_ID");
            this.Property(t => t.ORDER_TYPE).HasColumnName("ORDER_TYPE");
            this.Property(t => t.ORDER_CREATED_DTM).HasColumnName("ORDER_CREATED_DTM");
            this.Property(t => t.ORDER_CREATED_BY).HasColumnName("ORDER_CREATED_BY");
            this.Property(t => t.CUSTOMER_ID).HasColumnName("CUSTOMER_ID");
            this.Property(t => t.ACCOUNT_NO).HasColumnName("ACCOUNT_NO");
            this.Property(t => t.SALE_USER_NAME).HasColumnName("SALE_USER_NAME");
            this.Property(t => t.SUBMIT_DATE).HasColumnName("SUBMIT_DATE");
            this.Property(t => t.INVOICING_CO_ID).HasColumnName("INVOICING_CO_ID");
            this.Property(t => t.ORDER_STATUS).HasColumnName("ORDER_STATUS");
            this.Property(t => t.CANCEL_ORDER_NO).HasColumnName("CANCEL_ORDER_NO");
        }
    }
}
