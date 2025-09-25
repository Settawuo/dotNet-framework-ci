namespace AIRNETEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class AIR_FBB_PACKAGE_MAPPINGMap : EntityTypeConfiguration<AIR_FBB_PACKAGE_MAPPING>
    {
        public AIR_FBB_PACKAGE_MAPPINGMap()
        {
            this.HasKey(t => new { t.SFF_PROMOTION_CODE });


            this.Property(t => t.SFF_PROMOTION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.MAPPING_CODE)
                .HasMaxLength(50);



            this.ToTable("AIR_FBB_PACKAGE_MAPPING", "AIR_ADMIN");

            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.MAPPING_CODE).HasColumnName("MAPPING_CODE");

        }
    }
}
