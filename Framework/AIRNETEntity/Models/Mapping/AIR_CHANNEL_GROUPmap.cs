using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_CHANNEL_GROUPmap : EntityTypeConfiguration<AIR_CHANNEL_GROUP>
    {
        public AIR_CHANNEL_GROUPmap()
        {
            // Primary Key
            this
         .HasKey(t => t.PARTNER_TYPE);
            this
         .HasKey(t => t.PARTNER_SUBTYPE);
            this
         .HasKey(t => t.CATALOG_AUTHORIZE);
            this
         .HasKey(t => t.EFFECTIVE_DATE);

            // Properties  
            this.Property(t => t.PARTNER_TYPE)
         .HasMaxLength(250);
            this.Property(t => t.PARTNER_SUBTYPE)
         .HasMaxLength(250);
            this.Property(t => t.CATALOG_AUTHORIZE)
         .HasMaxLength(250);
            this.Property(t => t.UPD_BY)
         .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("AIR_CHANNEL_GROUP", "AIR_ADMIN");
            this.Property(t => t.PARTNER_TYPE).HasColumnName("PARTNER_TYPE");
            this.Property(t => t.PARTNER_SUBTYPE).HasColumnName("PARTNER_SUBTYPE");
            this.Property(t => t.CATALOG_AUTHORIZE).HasColumnName("CATALOG_AUTHORIZE");
            this.Property(t => t.EFFECTIVE_DATE).HasColumnName("EFFECTIVE_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
        }
    }
}
