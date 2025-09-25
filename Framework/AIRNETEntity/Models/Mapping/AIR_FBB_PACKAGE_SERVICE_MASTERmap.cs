using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_FBB_PACKAGE_SERVICE_MASTERmap : EntityTypeConfiguration<AIR_FBB_PACKAGE_SERVICE_MASTER>
    {
        public AIR_FBB_PACKAGE_SERVICE_MASTERmap()
        {
            // Primary Key
            this.HasKey(t => t.PACKAGE_SERVICE_CODE);
            this.HasKey(t => t.PACKAGE_SERVICE_NAME);

            // Properties  
            this.Property(t => t.PACKAGE_SERVICE_CODE)
         .HasMaxLength(250);
            this.Property(t => t.PACKAGE_SERVICE_NAME)
         .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("AIR_FBB_PACKAGE_SERVICE_MASTER", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_SERVICE_CODE).HasColumnName("PACKAGE_SERVICE_CODE");
            this.Property(t => t.PACKAGE_SERVICE_NAME).HasColumnName("PACKAGE_SERVICE_NAME");
        }

    }
}
