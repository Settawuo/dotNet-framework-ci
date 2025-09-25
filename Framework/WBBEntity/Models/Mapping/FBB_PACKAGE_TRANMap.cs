using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_PACKAGE_TRANMap : EntityTypeConfiguration<FBB_PACKAGE_TRAN>
    {
        public FBB_PACKAGE_TRANMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CREATED, t.CREATED_BY, t.LAST_UPD, t.LAST_UPD_BY });

            // Properties
            this.Property(t => t.ROW_ID)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.LAST_UPD_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.CUST_ROW_ID)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_CODE)
                .HasMaxLength(6);

            this.Property(t => t.PACKAGE_CLASS)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_GROUP)
                .HasMaxLength(50);

            this.Property(t => t.PRODUCT_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.PRODUCT_SUBTYPE)
                .HasMaxLength(50);

            this.Property(t => t.TECHNOLOGY)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_NAME)
                .HasMaxLength(255);

            this.Property(t => t.PACKAGE_BILL_THA)
                .HasMaxLength(255);

            this.Property(t => t.PACKAGE_BILL_ENG)
                .HasMaxLength(255);

            this.Property(t => t.DOWNLOAD_SPEED)
                .HasMaxLength(255);

            this.Property(t => t.UPLOAD_SPEED)
                .HasMaxLength(255);

            this.Property(t => t.OWNER_PRODUCT)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_PACKAGE_TRAN", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.CREATED).HasColumnName("CREATED");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.LAST_UPD).HasColumnName("LAST_UPD");
            this.Property(t => t.LAST_UPD_BY).HasColumnName("LAST_UPD_BY");
            this.Property(t => t.CUST_ROW_ID).HasColumnName("CUST_ROW_ID");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PACKAGE_CLASS).HasColumnName("PACKAGE_CLASS");
            this.Property(t => t.PACKAGE_GROUP).HasColumnName("PACKAGE_GROUP");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.PACKAGE_NAME).HasColumnName("PACKAGE_NAME");
            this.Property(t => t.RECURRING_CHARGE).HasColumnName("RECURRING_CHARGE");
            this.Property(t => t.INITIATION_CHARGE).HasColumnName("INITIATION_CHARGE");
            this.Property(t => t.DISCOUNT_INITIATION).HasColumnName("DISCOUNT_INITIATION");
            this.Property(t => t.PACKAGE_BILL_THA).HasColumnName("PACKAGE_BILL_THA");
            this.Property(t => t.PACKAGE_BILL_ENG).HasColumnName("PACKAGE_BILL_ENG");
            this.Property(t => t.DOWNLOAD_SPEED).HasColumnName("DOWNLOAD_SPEED");
            this.Property(t => t.UPLOAD_SPEED).HasColumnName("UPLOAD_SPEED");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
        }
    }
}
