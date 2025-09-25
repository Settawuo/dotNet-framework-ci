using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_PACKAGE_LOCATIONmap : EntityTypeConfiguration<AIR_PACKAGE_LOCATION>
    {
        public AIR_PACKAGE_LOCATIONmap()
        {
            // Primary Key
            this.HasKey(t => t.PACKAGE_CODE);
            this.HasKey(t => t.ADDRESS_TYPE);
            this.HasKey(t => t.BUILDING_NAME);
            this.HasKey(t => t.EFFECTIVE_DTM);

            // Properties
            this.Property(t => t.PACKAGE_CODE)
         .HasMaxLength(6);
            this.Property(t => t.REGION)
         .HasMaxLength(10);
            this.Property(t => t.PROVINCE)
         .HasMaxLength(50);
            this.Property(t => t.DISTRICT)
         .HasMaxLength(50);
            this.Property(t => t.SUB_DISTRICT)
         .HasMaxLength(50);
            this.Property(t => t.ADDRESS_TYPE)
         .HasMaxLength(5);
            this.Property(t => t.BUILDING_NAME)
         .HasMaxLength(250);
            this.Property(t => t.BUILDING_NO)
         .HasMaxLength(15);
            this.Property(t => t.UPD_BY)
         .HasMaxLength(15);
            this.Property(t => t.ADDRESS_ID)
         .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AIR_PACKAGE_LOCATION", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.REGION).HasColumnName("REGION");
            this.Property(t => t.PROVINCE).HasColumnName("PROVINCE");
            this.Property(t => t.DISTRICT).HasColumnName("DISTRICT");
            this.Property(t => t.SUB_DISTRICT).HasColumnName("SUB_DISTRICT");
            this.Property(t => t.ADDRESS_TYPE).HasColumnName("ADDRESS_TYPE");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");
            this.Property(t => t.BUILDING_NO).HasColumnName("BUILDING_NO");
            this.Property(t => t.EFFECTIVE_DTM).HasColumnName("EFFECTIVE_DTM");
            this.Property(t => t.EXPIRE_DTM).HasColumnName("EXPIRE_DTM");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
        }
    }
}
