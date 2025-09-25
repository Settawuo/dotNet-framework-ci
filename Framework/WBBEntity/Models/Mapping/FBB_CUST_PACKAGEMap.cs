using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CUST_PACKAGEMap : EntityTypeConfiguration<FBB_CUST_PACKAGE>
    {
        public FBB_CUST_PACKAGEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CUST_NON_MOBILE, t.PACKAGE_CODE });

            // Properties
            this.Property(t => t.CUST_NON_MOBILE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_CODE)
                .IsRequired()
                .HasMaxLength(6);

            this.Property(t => t.PACKAGE_CLASS).HasMaxLength(50);
            this.Property(t => t.PACKAGE_TYPE).HasMaxLength(50);
            this.Property(t => t.PACKAGE_GROUP).HasMaxLength(50);
            this.Property(t => t.PACKAGE_SUBTYPE).HasMaxLength(50);
            this.Property(t => t.PACKAGE_OWNER).HasMaxLength(100);
            this.Property(t => t.TECHNOLOGY).HasMaxLength(50);
            this.Property(t => t.PACKAGE_STATUS).HasMaxLength(20);
            this.Property(t => t.PACKAGE_NAME).HasMaxLength(255);
            this.Property(t => t.PACKAGE_BILL_THA).HasMaxLength(255);
            this.Property(t => t.DOWNLOAD_SPEED).HasMaxLength(255);
            this.Property(t => t.UPLOAD_SPEED).HasMaxLength(255);
            this.Property(t => t.CREATED_BY).HasMaxLength(30).IsRequired();
            this.Property(t => t.UPDATED_BY).HasMaxLength(30).IsRequired();
            this.Property(t => t.PRE_RECURRING_CHARGE).HasMaxLength(50);
            this.Property(t => t.PRE_INITIATION_CHARGE).HasMaxLength(50);
            this.Property(t => t.HOME_IP).HasMaxLength(50);
            this.Property(t => t.HOME_PORT).HasMaxLength(50);
            this.Property(t => t.IDD_FLAG).HasMaxLength(1);
            this.Property(t => t.FAX_FLAG).HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("FBB_CUST_PACKAGE", "WBB");
            this.Property(t => t.CUST_NON_MOBILE).HasColumnName("CUST_NON_MOBILE");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PACKAGE_CLASS).HasColumnName("PACKAGE_CLASS");
            this.Property(t => t.PACKAGE_TYPE).HasColumnName("PACKAGE_TYPE");
            this.Property(t => t.PACKAGE_GROUP).HasColumnName("PACKAGE_GROUP");
            this.Property(t => t.PACKAGE_SUBTYPE).HasColumnName("PACKAGE_SUBTYPE");
            this.Property(t => t.PACKAGE_OWNER).HasColumnName("PACKAGE_OWNER");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.PACKAGE_STATUS).HasColumnName("PACKAGE_STATUS");
            this.Property(t => t.PACKAGE_NAME).HasColumnName("PACKAGE_NAME");
            this.Property(t => t.RECURRING_CHARGE).HasColumnName("RECURRING_CHARGE");
            this.Property(t => t.RECURRING_DISCOUNT).HasColumnName("RECURRING_DISCOUNT");
            this.Property(t => t.RECURRING_DISCOUNT_EXP).HasColumnName("RECURRING_DISCOUNT_EXP");
            this.Property(t => t.RECURRING_START_DT).HasColumnName("RECURRING_START_DT");
            this.Property(t => t.RECURRING_END_DT).HasColumnName("RECURRING_END_DT");
            this.Property(t => t.INITIATION_CHARGE).HasColumnName("INITIATION_CHARGE");
            this.Property(t => t.INITIATION_DISCOUNT).HasColumnName("INITIATION_DISCOUNT");
            this.Property(t => t.PACKAGE_BILL_THA).HasColumnName("PACKAGE_BILL_THA");
            this.Property(t => t.DOWNLOAD_SPEED).HasColumnName("DOWNLOAD_SPEED");
            this.Property(t => t.UPLOAD_SPEED).HasColumnName("UPLOAD_SPEED");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.PRE_RECURRING_CHARGE).HasColumnName("PRE_RECURRING_CHARGE");
            this.Property(t => t.PRE_INITIATION_CHARGE).HasColumnName("PRE_INITIATION_CHARGE");
            this.Property(t => t.HOME_IP).HasColumnName("HOME_IP");
            this.Property(t => t.HOME_PORT).HasColumnName("HOME_PORT");
            this.Property(t => t.IDD_FLAG).HasColumnName("IDD_FLAG");
            this.Property(t => t.FAX_FLAG).HasColumnName("FAX_FLAG");
        }
    }
}
