using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_PACKAGE_MASTERMap : EntityTypeConfiguration<AIR_PACKAGE_MASTER>
    {
        public AIR_PACKAGE_MASTERMap()
        {

            // Primary Key
            this.HasKey(t => t.PACKAGE_CODE);
            // Properties          
            this.Property(t => t.PACKAGE_CODE)
             .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PACKAGE_TYPE)
            .IsRequired()
            .HasMaxLength(1);

            this.Property(t => t.PACKAGE_CLASS)
            .IsRequired()
            .HasMaxLength(1);

            this.Property(t => t.RATE_PLAN_ID)
            .HasMaxLength(6);

            this.Property(t => t.UPD_BY)
            .IsRequired()
            .HasMaxLength(15);

            this.Property(t => t.PACKAGE_NAME_THA)
            .HasMaxLength(255);

            this.Property(t => t.PACKAGE_NAME_ENG)
            .HasMaxLength(255);

            this.Property(t => t.PRORATE_FLAG)
            .IsRequired()
            .HasMaxLength(1);

            this.Property(t => t.AGGREGATE_LEVEL)
            .IsRequired()
            .HasMaxLength(1);

            this.Property(t => t.PACKAGE_TARIFF_TYPE)
            .IsRequired()
            .HasMaxLength(2);

            this.Property(t => t.PRODUCT_TYPE)
            .HasMaxLength(30);

            this.Property(t => t.PRODUCT_SUBTYPE)
            .HasMaxLength(30);

            this.Property(t => t.SPEED)
            .HasMaxLength(100);

            this.Property(t => t.PRODUCT_SUBTYPE2)
            .HasMaxLength(30);
            this.Property(t => t.SFF_PROMOTION_CODE)
            .HasMaxLength(50);
            this.Property(t => t.SFF_PROMOTION_NAME_THA)
            .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_NAME_ENG)
            .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_BILL_THA)
            .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_BILL_ENG)
            .HasMaxLength(255);
            this.Property(t => t.TECHNOLOGY)
            .HasMaxLength(50);
            this.Property(t => t.DOWNLOAD_SPEED)
            .HasMaxLength(50);
            this.Property(t => t.UPLOAD_SPEED)
            .HasMaxLength(50);

            this.Property(t => t.OWNER_PRODUCT)
            .HasMaxLength(255);


            // Table & Column Mappings
            this.ToTable("AIR_PACKAGE_MASTER", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PACKAGE_TYPE).HasColumnName("PACKAGE_TYPE");
            this.Property(t => t.PACKAGE_CLASS).HasColumnName("PACKAGE_CLASS");
            this.Property(t => t.SALE_START_DATE).HasColumnName("SALE_START_DATE");
            this.Property(t => t.SALE_END_DATE).HasColumnName("SALE_END_DATE");
            this.Property(t => t.RATE_PLAN_ID).HasColumnName("RATE_PLAN_ID");
            this.Property(t => t.INITIATION_CHARGE).HasColumnName("INITIATION_CHARGE");
            this.Property(t => t.INC_REVENUE_CODE).HasColumnName("INC_REVENUE_CODE");
            this.Property(t => t.RECURRING_CHARGE).HasColumnName("RECURRING_CHARGE");
            this.Property(t => t.RCC_REVENUE_CODE).HasColumnName("RCC_REVENUE_CODE");
            this.Property(t => t.TERMINATION_CHARGE).HasColumnName("TERMINATION_CHARGE");
            this.Property(t => t.TMC_REVENUE_CODE).HasColumnName("TMC_REVENUE_CODE");
            this.Property(t => t.SUSPENSION_CHARGE).HasColumnName("SUSPENSION_CHARGE");
            this.Property(t => t.SSC_REVENUE_CODE).HasColumnName("SSC_REVENUE_CODE");
            this.Property(t => t.SUSPEND_RECURRING_CHARGE).HasColumnName("SUSPEND_RECURRING_CHARGE");
            this.Property(t => t.SRC_REVENUE_CODE).HasColumnName("SRC_REVENUE_CODE");
            this.Property(t => t.REACTIVATION_CHARGE).HasColumnName("REACTIVATION_CHARGE");
            this.Property(t => t.RAC_REVENUE_CODE).HasColumnName("RAC_REVENUE_CODE");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.PACKAGE_NAME_THA).HasColumnName("PACKAGE_NAME_THA");
            this.Property(t => t.PACKAGE_NAME_ENG).HasColumnName("PACKAGE_NAME_ENG");
            this.Property(t => t.DURATION_MONTH).HasColumnName("DURATION_MONTH");
            this.Property(t => t.PRORATE_FLAG).HasColumnName("PRORATE_FLAG");
            this.Property(t => t.AGGREGATE_LEVEL).HasColumnName("AGGREGATE_LEVEL");
            this.Property(t => t.PACKAGE_TARIFF_TYPE).HasColumnName("PACKAGE_TARIFF_TYPE");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.PRIORITY_NUM).HasColumnName("PRIORITY_NUM");
            this.Property(t => t.SPEED).HasColumnName("SPEED");
            this.Property(t => t.MINIMUM_CHARGE).HasColumnName("MINIMUM_CHARGE");
            this.Property(t => t.PRODUCT_SUBTYPE2).HasColumnName("PRODUCT_SUBTYPE2");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.SFF_PROMOTION_NAME_THA).HasColumnName("SFF_PROMOTION_NAME_THA");
            this.Property(t => t.SFF_PROMOTION_NAME_ENG).HasColumnName("SFF_PROMOTION_NAME_ENG");
            this.Property(t => t.SFF_PROMOTION_BILL_THA).HasColumnName("SFF_PROMOTION_BILL_THA");
            this.Property(t => t.SFF_PROMOTION_BILL_ENG).HasColumnName("SFF_PROMOTION_BILL_ENG");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.DOWNLOAD_SPEED).HasColumnName("DOWNLOAD_SPEED");
            this.Property(t => t.UPLOAD_SPEED).HasColumnName("UPLOAD_SPEED");
            this.Property(t => t.DISCOUNT_INITIATION_CHARGE).HasColumnName("DISCOUNT_INITIATION_CHARGE");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");


        }
    }
}
