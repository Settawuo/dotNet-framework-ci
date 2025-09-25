using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_ORDER_PACKAGEMap : EntityTypeConfiguration<FBBPAYG_ORDER_PACKAGE>
    {
        public FBBPAYG_ORDER_PACKAGEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ORDER_PKG_ID});

            // Properties
            this.Property(t => t.PACKAGE_CODE)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_NAME)
                .HasMaxLength(500);

            this.Property(t => t.PACKAGE_CLASS)
                .HasMaxLength(50);

            this.Property(t => t.IS_NEW)
                .HasMaxLength(1);

            this.Property(t => t.ORDER_NO)
                .HasMaxLength(50);

            this.Property(t => t.ACCESS_NO)
                .HasMaxLength(20);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("FBBPAYG_ORDER_PACKAGE", "WBB");
            this.Property(t => t.ORDER_PKG_ID).HasColumnName("ORDER_PKG_ID");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PACKAGE_NAME).HasColumnName("PACKAGE_NAME");
            this.Property(t => t.PACKAGE_CLASS).HasColumnName("PACKAGE_CLASS");
            this.Property(t => t.IS_NEW).HasColumnName("IS_NEW");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.ACCESS_NO).HasColumnName("ACCESS_NO");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
  
        }
    }
}
