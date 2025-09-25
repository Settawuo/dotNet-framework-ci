using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_PACKAGE_USER_GROUPmap : EntityTypeConfiguration<AIR_PACKAGE_USER_GROUP>
    {
        public AIR_PACKAGE_USER_GROUPmap()
        {

            // Primary Key
            this
         .HasKey(t => t.PACKAGE_CODE);
            this
         .HasKey(t => t.USER_GROUP);
            this
         .HasKey(t => t.EFFECTIVE_DTM);

            // Properties  
            this.Property(t => t.PACKAGE_CODE)
         .HasMaxLength(6);
            this.Property(t => t.LOCATION_CODE)
         .HasMaxLength(30);
            this.Property(t => t.ASC_CODE)
         .HasMaxLength(50);
            this.Property(t => t.USER_GROUP)
         .HasMaxLength(300);
            this.Property(t => t.UPD_BY)
         .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("AIR_PACKAGE_USER_GROUP", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.ASC_CODE).HasColumnName("ASC_CODE");
            this.Property(t => t.USER_GROUP).HasColumnName("USER_GROUP");
            this.Property(t => t.EFFECTIVE_DTM).HasColumnName("EFFECTIVE_DTM");
            this.Property(t => t.EXPIRE_DTM).HasColumnName("EXPIRE_DTM");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
        }
    }
}
