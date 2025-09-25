using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_REGISTER_PAYMENT_LOG_SPDPMap : EntityTypeConfiguration<FBB_REGISTER_PAYMENT_LOG_SPDP>
    {
        public FBB_REGISTER_PAYMENT_LOG_SPDPMap()
        {
            this.HasKey(t => t.ORDER_TRANSACTION_ID);

            this.Property(t => t.ORDER_TRANSACTION_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NON_MOBILE_NO)
                .HasMaxLength(50);

            this.Property(t => t.SERVICE_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.ENDPOINT)
                .HasMaxLength(1000);

            this.Property(t => t.ORDER_ID)
                .HasMaxLength(1000);

            this.Property(t => t.TXN_ID)
                .HasMaxLength(1000);

            this.Property(t => t.STATUS)
                .HasMaxLength(1000);

            this.Property(t => t.STATUS_CODE)
                .HasMaxLength(1000);

            this.Property(t => t.STATUS_MESSAGE)
                .HasMaxLength(1000);

            this.Property(t => t.CHANNEL)
                .HasMaxLength(1000);

            this.Property(t => t.AMOUNT)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.ORDER_TRANSACTION_ID)
                .HasMaxLength(100);


            // Table & Column Mappings
            this.ToTable("FBB_REGISTER_PAYMENT_LOG_SPDP", "WBB");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");
            this.Property(t => t.SERVICE_NAME).HasColumnName("SERVICE_NAME");
            this.Property(t => t.ENDPOINT).HasColumnName("ENDPOINT");
            this.Property(t => t.ORDER_ID).HasColumnName("ORDER_ID");
            this.Property(t => t.TXN_ID).HasColumnName("TXN_ID");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATUS_CODE).HasColumnName("STATUS_CODE");
            this.Property(t => t.STATUS_MESSAGE).HasColumnName("STATUS_MESSAGE");
            this.Property(t => t.CHANNEL).HasColumnName("CHANNEL");
            this.Property(t => t.AMOUNT).HasColumnName("AMOUNT");
            this.Property(t => t.REQ_XML_PARAM).HasColumnName("REQ_XML_PARAM");
            this.Property(t => t.RES_XML_PARAM).HasColumnName("RES_XML_PARAM");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.ORDER_TRANSACTION_ID).HasColumnName("ORDER_TRANSACTION_ID");
        }
    }
}