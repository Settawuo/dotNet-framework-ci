using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_PACKAGE_MASTER_DETAILmap : EntityTypeConfiguration<AIR_PACKAGE_MASTER_DETAIL>
    {
        public AIR_PACKAGE_MASTER_DETAILmap()
        {
            // Primary Key
            this.HasKey(t => t.PACKAGE_CODE);
            // Properties  

            this.Property(t => t.PRODUCT_TYPE)
            .HasMaxLength(30);

            this.Property(t => t.PRODUCT_SUBTYPE)
            .HasMaxLength(30);

            this.Property(t => t.PRODUCT_SUBTYPE2)
            .HasMaxLength(30);

            this.Property(t => t.PRODUCT_SUBTYPE3)
            .HasMaxLength(30);

            this.Property(t => t.OWNER_PRODUCT)
            .HasMaxLength(255);

            this.Property(t => t.TECHNOLOGY)
            .HasMaxLength(500);

            this.Property(t => t.PACKAGE_GROUP)
            .HasMaxLength(100);

            this.Property(t => t.NETWORK_TYPE)
            .HasMaxLength(100);

            this.Property(t => t.UPD_BY)
            .HasMaxLength(15);

            this.Property(t => t.SERENADE_FLAG)
            .HasMaxLength(5);

            this.Property(t => t.SERENADE_PACKAGE_GROUP)
            .HasMaxLength(100);

            this.Property(t => t.NON_SERENADE_PACKAGE_GROUP)
            .HasMaxLength(100);

            this.Property(t => t.PLUG_AND_PLAY_FLAG)
            .HasMaxLength(5);

            this.Property(t => t.CUSTOMER_TYPE)
            .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("AIR_PACKAGE_MASTER_DETAIL", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.PRODUCT_SUBTYPE2).HasColumnName("PRODUCT_SUBTYPE2");
            this.Property(t => t.PRODUCT_SUBTYPE3).HasColumnName("PRODUCT_SUBTYPE3");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.PACKAGE_GROUP).HasColumnName("PACKAGE_GROUP");
            this.Property(t => t.NETWORK_TYPE).HasColumnName("NETWORK_TYPE");
            this.Property(t => t.SERVICE_DAY_START).HasColumnName("SERVICE_DAY_START");
            this.Property(t => t.SERVICE_DAY_END).HasColumnName("SERVICE_DAY_END");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.SERENADE_FLAG).HasColumnName("SERENADE_FLAG");
            this.Property(t => t.SERENADE_PACKAGE_GROUP).HasColumnName("SERENADE_PACKAGE_GROUP");
            this.Property(t => t.NON_SERENADE_PACKAGE_GROUP).HasColumnName("NON_SERENADE_PACKAGE_GROUP");
            this.Property(t => t.PLUG_AND_PLAY_FLAG).HasColumnName("PLUG_AND_PLAY_FLAG");
            this.Property(t => t.CUSTOMER_TYPE).HasColumnName("CUSTOMER_TYPE");
        }
    }
}
