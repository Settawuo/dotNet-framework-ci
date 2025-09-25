using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_VOUCHER_MASTERMap : EntityTypeConfiguration<FBB_VOUCHER_MASTER>
    {
        public FBB_VOUCHER_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => t.VOUCHER_MASTER_ID);

            // Properties
            this.Property(t => t.VOUCHER_MASTER_ID);

            this.Property(t => t.VOUCHER_PROJECT_GROUP)
                .HasMaxLength(100);

            this.Property(t => t.VOUCHER_PROJECT_CODE)
                .HasMaxLength(1000);

            this.Property(t => t.VOUCHER_PROJECT_DES)
                .HasMaxLength(1000);

            this.Property(t => t.PROJECT_STATUS)
                .HasMaxLength(50);

            this.Property(t => t.TRANSACTION_ID);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_DATE);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(100);

            this.Property(t => t.UPDATED_DATE);

            // Table & Column Mappings
            this.ToTable("FBB_VOUCHER_MASTER", "WBB");
            this.Property(t => t.VOUCHER_MASTER_ID).HasColumnName("VOUCHER_MASTER_ID");
            this.Property(t => t.VOUCHER_PROJECT_GROUP).HasColumnName("VOUCHER_PROJECT_GROUP");
            this.Property(t => t.VOUCHER_PROJECT_CODE).HasColumnName("VOUCHER_PROJECT_CODE");
            this.Property(t => t.VOUCHER_PROJECT_DES).HasColumnName("VOUCHER_PROJECT_DES");
            this.Property(t => t.PROJECT_STATUS).HasColumnName("PROJECT_STATUS");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
