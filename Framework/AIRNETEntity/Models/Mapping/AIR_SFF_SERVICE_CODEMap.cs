using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SFF_SERVICE_CODEMap : EntityTypeConfiguration<AIR_SFF_SERVICE_CODE>
    {
        public AIR_SFF_SERVICE_CODEMap()
        {
            this.HasKey(t => new { t.PRODUCT_NAME, t.UPD_DTM });

            this.Property(t => t.PRODUCT_NAME)
               .HasMaxLength(50);

            this.Property(t => t.SERVICE_CODE)
              .HasMaxLength(20);

            this.Property(t => t.UPD_BY)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("AIR_SFF_SERVICE_CODE", "AIR_ADMIN");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.SERVICE_CODE).HasColumnName("SERVICE_CODE");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");

        }
    }
}

