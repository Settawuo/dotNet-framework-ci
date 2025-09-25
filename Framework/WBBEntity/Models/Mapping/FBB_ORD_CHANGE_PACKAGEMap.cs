namespace WBBEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class FBB_ORD_CHANGE_PACKAGEMap : EntityTypeConfiguration<FBB_ORD_CHANGE_PACKAGE>
    {
        public FBB_ORD_CHANGE_PACKAGEMap()
        {
            this.HasKey(t => new { t.ORDER_NO, t.SFF_PROMOTION_CODE, t.ACTION_STATUS, t.PRODUCT_SEQ });

            this.Property(t => t.ORDER_NO)
                .HasMaxLength(30);

            this.Property(t => t.NON_MOBILE_NO)
                .HasMaxLength(30);

            this.Property(t => t.RELATE_MOBILE)
                .HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.ACTION_STATUS)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_STATE)
                .HasMaxLength(30);

            this.Property(t => t.PROJECT_NAME)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_DATE);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.UPDATED_DATE);

            this.Property(t => t.PRODUCT_SEQ)
                .HasMaxLength(100);

            this.Property(t => t.BUNDLING_ACTION)
                .HasMaxLength(100);

            this.Property(t => t.OLD_RELATE_MOBILE)
                .HasMaxLength(100);

            this.Property(t => t.MOBILE_CONTACT)
                .HasMaxLength(100);

            this.ToTable("FBB_ORD_CHANGE_PACKAGE", "WBB");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");
            this.Property(t => t.RELATE_MOBILE).HasColumnName("RELATE_MOBILE");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.ACTION_STATUS).HasColumnName("ACTION_STATUS");
            this.Property(t => t.PACKAGE_STATE).HasColumnName("PACKAGE_STATE");
            this.Property(t => t.PROJECT_NAME).HasColumnName("PROJECT_NAME");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.PRODUCT_SEQ).HasColumnName("PRODUCT_SEQ");
            this.Property(t => t.BUNDLING_ACTION).HasColumnName("BUNDLING_ACTION");
            this.Property(t => t.OLD_RELATE_MOBILE).HasColumnName("OLD_RELATE_MOBILE");
            this.Property(t => t.MOBILE_CONTACT).HasColumnName("MOBILE_CONTACT");
        }
    }
}
