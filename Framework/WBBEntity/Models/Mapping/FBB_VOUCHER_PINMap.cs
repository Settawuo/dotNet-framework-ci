using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_VOUCHER_PINMap : EntityTypeConfiguration<FBB_VOUCHER_PIN>
    {
        public FBB_VOUCHER_PINMap()
        {
            // Primary Key
            this.HasKey(t => t.VOUCHER_PIN_ID);

            // Properties
            this.Property(t => t.VOUCHER_PIN_ID);

            this.Property(t => t.VOUCHER_PIN)
                .HasMaxLength(1000);

            this.Property(t => t.PIN_STATUS)
                .HasMaxLength(100);

            this.Property(t => t.START_DATE);

            this.Property(t => t.EXPIRE_DATE);

            this.Property(t => t.TRANSACTION_ID);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_DATE);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.UPDATED_DATE);

            this.Property(t => t.VOUCHER_MASTER_ID);

            this.Property(t => t.LOT);

            // Table & Column Mappings
            this.ToTable("FBB_VOUCHER_PIN", "WBB");
            this.Property(t => t.VOUCHER_PIN_ID).HasColumnName("VOUCHER_PIN_ID");
            this.Property(t => t.VOUCHER_PIN).HasColumnName("VOUCHER_PIN");
            this.Property(t => t.PIN_STATUS).HasColumnName("PIN_STATUS");
            this.Property(t => t.START_DATE).HasColumnName("START_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.VOUCHER_MASTER_ID).HasColumnName("VOUCHER_MASTER_ID");
            this.Property(t => t.LOT).HasColumnName("LOT");
        }
    }
}
