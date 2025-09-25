using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_DORMITORY_TRN_IPAYMap : EntityTypeConfiguration<FBBDORM_DORMITORY_TRN_IPAY>
    {
        public FBBDORM_DORMITORY_TRN_IPAYMap()
        {
            // Primary Key
            this.HasKey(t => t.CHANNEL_REF_ID);

            // Properties
            this.Property(t => t.RESPONSE_CODE)
                .HasMaxLength(10);

            this.Property(t => t.RESPONSE_MES)
                .HasMaxLength(500);

            this.Property(t => t.CHANNEL_NAME)
                .HasMaxLength(20);

            this.Property(t => t.COMMAND)
                .HasMaxLength(50);

            this.Property(t => t.MSISDN)
                .HasMaxLength(20);

            this.Property(t => t.AIS_REF_DATE)
                .HasMaxLength(22);

            this.Property(t => t.AIS_REF_NO)
                .HasMaxLength(20);

            this.Property(t => t.BANK_REF_NO)
                .HasMaxLength(20);

            this.Property(t => t.PAY_BANK_FLAG)
                .HasMaxLength(1);

            this.Property(t => t.ISSUER_BANK_CODE)
                .HasMaxLength(3);

            this.Property(t => t.ACQUIRER_BANK_CODE)
                .HasMaxLength(3);

            this.Property(t => t.CARD_TYPE)
                .HasMaxLength(20);

            this.Property(t => t.ISSUER_BANK_NAME)
                .HasMaxLength(50);

            this.Property(t => t.SHOP_ID)
                .HasMaxLength(2);

            this.Property(t => t.BANK_REPONSE_CODE)
                .HasMaxLength(20);

            this.Property(t => t.SESSION_ID)
                .HasMaxLength(20);

            this.Property(t => t.BOS_RESULT_CODE)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FBBDORM_DORMITORY_TRN_IPAY", "WBB");
            this.Property(t => t.RESPONSE_CODE).HasColumnName("RESPONSE_CODE");
            this.Property(t => t.RESPONSE_MES).HasColumnName("RESPONSE_MES");
            this.Property(t => t.CHANNEL_ID).HasColumnName("CHANNEL_ID");
            this.Property(t => t.CHANNEL_NAME).HasColumnName("CHANNEL_NAME");
            this.Property(t => t.CHANNEL_REF_ID).HasColumnName("CHANNEL_REF_ID");
            this.Property(t => t.COMMAND).HasColumnName("COMMAND");
            this.Property(t => t.MSISDN).HasColumnName("MSISDN");
            this.Property(t => t.AIS_REF_DATE).HasColumnName("AIS_REF_DATE");
            this.Property(t => t.AIS_REF_NO).HasColumnName("AIS_REF_NO");
            this.Property(t => t.BANK_REF_NO).HasColumnName("BANK_REF_NO");
            this.Property(t => t.PAY_BANK_FLAG).HasColumnName("PAY_BANK_FLAG");
            this.Property(t => t.ISSUER_BANK_CODE).HasColumnName("ISSUER_BANK_CODE");
            this.Property(t => t.ACQUIRER_BANK_CODE).HasColumnName("ACQUIRER_BANK_CODE");
            this.Property(t => t.CARD_NO).HasColumnName("CARD_NO");
            this.Property(t => t.CARD_TYPE).HasColumnName("CARD_TYPE");
            this.Property(t => t.ISSUER_BANK_NAME).HasColumnName("ISSUER_BANK_NAME");
            this.Property(t => t.SHOP_ID).HasColumnName("SHOP_ID");
            this.Property(t => t.PAY_TERM).HasColumnName("PAY_TERM");
            this.Property(t => t.INSTALLMENT_RATE).HasColumnName("INSTALLMENT_RATE");
            this.Property(t => t.AMOUNT_PER_MONTH).HasColumnName("AMOUNT_PER_MONTH");
            this.Property(t => t.TOTAL_AMOUNT).HasColumnName("TOTAL_AMOUNT");
            this.Property(t => t.BANK_REPONSE_CODE).HasColumnName("BANK_REPONSE_CODE");
            this.Property(t => t.SESSION_ID).HasColumnName("SESSION_ID");
            this.Property(t => t.BANK_RETURN_AMOUNT).HasColumnName("BANK_RETURN_AMOUNT");
            this.Property(t => t.IPAY_RETURN_AMOUNT).HasColumnName("IPAY_RETURN_AMOUNT");
            this.Property(t => t.BOS_RESULT_CODE).HasColumnName("BOS_RESULT_CODE");
        }
    }
}
