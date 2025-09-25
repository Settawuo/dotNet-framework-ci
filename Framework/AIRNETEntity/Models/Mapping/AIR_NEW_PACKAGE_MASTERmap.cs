using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_NEW_PACKAGE_MASTERmap : EntityTypeConfiguration<AIR_NEW_PACKAGE_MASTER>
    {
        public AIR_NEW_PACKAGE_MASTERmap()
        {
            // Primary Key
            this.HasKey(t => t.PACKAGE_CODE);
            // Properties            
            this.Property(t => t.PACKAGE_TYPE)
         .HasMaxLength(1);
            this.Property(t => t.PACKAGE_CLASS)
         .HasMaxLength(1);
            this.Property(t => t.PACKAGE_NAME_THA)
         .HasMaxLength(255);
            this.Property(t => t.PACKAGE_NAME_ENG)
         .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_CODE)
         .HasMaxLength(50);
            this.Property(t => t.SFF_PROMOTION_BILL_THA)
         .HasMaxLength(255);
            this.Property(t => t.SFF_PROMOTION_BILL_ENG)
         .HasMaxLength(255);
            this.Property(t => t.DOWNLOAD_SPEED)
         .HasMaxLength(50);
            this.Property(t => t.UPLOAD_SPEED)
         .HasMaxLength(50);
            this.Property(t => t.DISCOUNT_TYPE)
         .HasMaxLength(50);
            this.Property(t => t.VAS_SERVICE)
         .HasMaxLength(1);
            this.Property(t => t.UPD_BY)
         .HasMaxLength(15);
            this.Property(t => t.NO_PLAY_BOX_FLAG)
         .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("AIR_NEW_PACKAGE_MASTER", "AIR_ADMIN");
            this.Property(t => t.PACKAGE_CODE).HasColumnName("PACKAGE_CODE");
            this.Property(t => t.PACKAGE_TYPE).HasColumnName("PACKAGE_TYPE");
            this.Property(t => t.PACKAGE_CLASS).HasColumnName("PACKAGE_CLASS");
            this.Property(t => t.SALE_START_DATE).HasColumnName("SALE_START_DATE");
            this.Property(t => t.SALE_END_DATE).HasColumnName("SALE_END_DATE");
            this.Property(t => t.DURATION_MONTH).HasColumnName("DURATION_MONTH");
            this.Property(t => t.PRE_INITIATION_CHARGE).HasColumnName("PRE_INITIATION_CHARGE");
            this.Property(t => t.INITIATION_CHARGE).HasColumnName("INITIATION_CHARGE");
            this.Property(t => t.PRE_RECURRING_CHARGE).HasColumnName("PRE_RECURRING_CHARGE");
            this.Property(t => t.RECURRING_CHARGE).HasColumnName("RECURRING_CHARGE");
            this.Property(t => t.TERMINATION_CHARGE).HasColumnName("TERMINATION_CHARGE");
            this.Property(t => t.SUSPENSION_CHARGE).HasColumnName("SUSPENSION_CHARGE");
            this.Property(t => t.SUSPEND_RECURRING_CHARGE).HasColumnName("SUSPEND_RECURRING_CHARGE");
            this.Property(t => t.REACTIVATION_CHARGE).HasColumnName("REACTIVATION_CHARGE");
            this.Property(t => t.PACKAGE_NAME_THA).HasColumnName("PACKAGE_NAME_THA");
            this.Property(t => t.PACKAGE_NAME_ENG).HasColumnName("PACKAGE_NAME_ENG");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.SFF_PROMOTION_BILL_THA).HasColumnName("SFF_PROMOTION_BILL_THA");
            this.Property(t => t.SFF_PROMOTION_BILL_ENG).HasColumnName("SFF_PROMOTION_BILL_ENG");
            this.Property(t => t.DOWNLOAD_SPEED).HasColumnName("DOWNLOAD_SPEED");
            this.Property(t => t.UPLOAD_SPEED).HasColumnName("UPLOAD_SPEED");
            this.Property(t => t.DISCOUNT_TYPE).HasColumnName("DISCOUNT_TYPE");
            this.Property(t => t.DISCOUNT_VALUE).HasColumnName("DISCOUNT_VALUE");
            this.Property(t => t.DISCOUNT_DAY).HasColumnName("DISCOUNT_DAY");
            this.Property(t => t.VAS_SERVICE).HasColumnName("VAS_SERVICE");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.CHANGE_START_DATE).HasColumnName("CHANGE_START_DATE");
            this.Property(t => t.CHANGE_END_DATE).HasColumnName("CHANGE_END_DATE");
            this.Property(t => t.NO_PLAY_BOX_FLAG).HasColumnName("NO_PLAY_BOX_FLAG");
        }
    }
}
