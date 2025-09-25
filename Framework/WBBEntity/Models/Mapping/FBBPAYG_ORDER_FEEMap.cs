using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_ORDER_FEEMap : EntityTypeConfiguration<FBBPAYG_ORDER_FEE>
    {
        public FBBPAYG_ORDER_FEEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ORDER_FEE_ID});

            // Properties
            this.Property(t => t.FEE_ID)
                .HasMaxLength(50);

            this.Property(t => t.FEE_ID_TYPE)
                .HasMaxLength(100);

            this.Property(t => t.FEE_NAME)
                .HasMaxLength(500);

            this.Property(t => t.ORDER_FEE_PRICE)
                .HasPrecision(7, 2);

            this.Property(t => t.FEE_ACTION)
                .HasMaxLength(5);

            this.Property(t => t.ORDER_NO)
                .HasMaxLength(50);

            this.Property(t => t.ACCESS_NO)
                .HasMaxLength(20);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("FBBPAYG_ORDER_FEE", "WBB");
            this.Property(t => t.ORDER_FEE_ID).HasColumnName("ORDER_FEE_ID");
            this.Property(t => t.FEE_ID).HasColumnName("FEE_ID");
            this.Property(t => t.FEE_ID_TYPE).HasColumnName("FEE_ID_TYPE");
            this.Property(t => t.FEE_NAME).HasColumnName("FEE_NAME");
            this.Property(t => t.ORDER_FEE_PRICE).HasColumnName("ORDER_FEE_PRICE");
            this.Property(t => t.FEE_ACTION).HasColumnName("FEE_ACTION");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.ACCESS_NO).HasColumnName("ACCESS_NO");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
  
        }
    }
}
