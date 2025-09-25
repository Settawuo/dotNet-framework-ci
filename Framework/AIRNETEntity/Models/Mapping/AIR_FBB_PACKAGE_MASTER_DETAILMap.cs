namespace AIRNETEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class AIR_FBB_PACKAGE_MASTER_DETAILMap : EntityTypeConfiguration<AIR_FBB_PACKAGE_MASTER_DETAIL>
    {
        public AIR_FBB_PACKAGE_MASTER_DETAILMap()
        {
            this.HasKey(t => new { t.SFF_PROMOTION_CODE });


            this.Property(t => t.SFF_PROMOTION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.OWNER_PRODUCT)
                .HasMaxLength(50);

            this.Property(t => t.PRODUCT_SUBTYPE)
                .HasMaxLength(50);

            this.ToTable("AIR_FBB_PACKAGE_MASTER_DETAIL", "AIR_ADMIN");

            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
        }
    }
}
