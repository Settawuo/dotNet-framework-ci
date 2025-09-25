using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CFG_PAYMENTMap : EntityTypeConfiguration<FBB_CFG_PAYMENT>
    {
        public FBB_CFG_PAYMENTMap()
        {
            // Primary Key
            this.HasKey(t => new
            {
                t.PRODUCT_NAME,
                t.SERVICE_NAME,
                t.ENDPOINT,
                t.ATTR_NAME,
                t.ATTR_VALUE
            });

            // Properties
            this.Property(t => t.PRODUCT_NAME)
                .HasMaxLength(100);

            this.Property(t => t.SERVICE_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.ENDPOINT)
                .HasMaxLength(1000);

            this.Property(t => t.ATTR_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.ATTR_VALUE)
                .HasMaxLength(4000);

            this.Property(t => t.REMARK)
                .HasMaxLength(1000);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
               .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("FBB_CFG_PAYMENT", "WBB");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.SERVICE_NAME).HasColumnName("SERVICE_NAME");
            this.Property(t => t.ENDPOINT).HasColumnName("ENDPOINT");
            this.Property(t => t.ATTR_NAME).HasColumnName("ATTR_NAME");
            this.Property(t => t.ATTR_VALUE).HasColumnName("ATTR_VALUE");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
