namespace AIRNETEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class AIR_FBB_NEW_PACKAGE_MASTERMap : EntityTypeConfiguration<AIR_FBB_NEW_PACKAGE_MASTER>
    {
        public AIR_FBB_NEW_PACKAGE_MASTERMap()
        {
            this.HasKey(t => new { t.SFF_PROMOTION_CODE });


            this.Property(t => t.SFF_PROMOTION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_TYPE_DESC)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_SERVICE_CODE)
                .HasMaxLength(50);

            this.Property(t => t.SFF_PRODUCT_CLASS)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_DISPLAY_THA)
                .HasMaxLength(50);

            this.Property(t => t.DOWNLOAD_SPEED)
                .HasMaxLength(50);

            this.ToTable("AIR_FBB_NEW_PACKAGE_MASTER", "AIR_ADMIN");

            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.PACKAGE_TYPE_DESC).HasColumnName("PACKAGE_TYPE_DESC");
            this.Property(t => t.PACKAGE_SERVICE_CODE).HasColumnName("PACKAGE_SERVICE_CODE");
            this.Property(t => t.SFF_PRODUCT_CLASS).HasColumnName("SFF_PRODUCT_CLASS");
            this.Property(t => t.PACKAGE_DISPLAY_THA).HasColumnName("PACKAGE_DISPLAY_THA");
            this.Property(t => t.SUB_SEQ).HasColumnName("SUB_SEQ");
            this.Property(t => t.DOWNLOAD_SPEED).HasColumnName("DOWNLOAD_SPEED");
            this.Property(t => t.PRE_PRICE_CHARGE).HasColumnName("PRE_PRICE_CHARGE");

        }
    }
}
