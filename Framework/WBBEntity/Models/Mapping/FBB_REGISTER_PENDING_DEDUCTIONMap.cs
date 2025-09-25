using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_REGISTER_PENDING_DEDUCTIONMap : EntityTypeConfiguration<FBB_REGISTER_PENDING_DEDUCTION>
    {
        public FBB_REGISTER_PENDING_DEDUCTIONMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TRANSACTION_ID, t.NON_MOBILE_NO });

            // Properties


            // Table & Column Mappings
            this.ToTable("FBB_REGISTER_PENDING_DEDUCTION", "WBB");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");
            this.Property(t => t.BA_NO).HasColumnName("BA_NO");
            this.Property(t => t.PAID_AMT).HasColumnName("PAID_AMT");
            this.Property(t => t.CHANNEL).HasColumnName("CHANNEL");
            this.Property(t => t.DEDUCTION_STATUS).HasColumnName("DEDUCTION_STATUS");
            this.Property(t => t.PAYMENT_STATUS).HasColumnName("PAYMENT_STATUS");
            this.Property(t => t.SEND_SMS_FLAG).HasColumnName("SEND_SMS_FLAG");
            this.Property(t => t.PAYMENT_METHOD_ID).HasColumnName("PAYMENT_METHOD_ID");
            this.Property(t => t.RECEIPT_NO).HasColumnName("RECEIPT_NO");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");

        }
    }
}
