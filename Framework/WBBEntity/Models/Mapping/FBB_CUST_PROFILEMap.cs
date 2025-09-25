using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CUST_PROFILEMap : EntityTypeConfiguration<FBB_CUST_PROFILE>
    {
        public FBB_CUST_PROFILEMap()
        {
            // Primary Key
            this.HasKey(t => t.CUST_NON_MOBILE);

            // Properties
            this.Property(t => t.CUST_NON_MOBILE)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //this.Property(t => t.CUST_NON_MOBILE)
            //   .IsRequired()
            //   .HasMaxLength(50);

            this.Property(t => t.CA_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SA_ID)
                .HasMaxLength(50);

            this.Property(t => t.BA_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IA_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CUST_NAME)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CUST_SURNAME)
                .HasMaxLength(100);

            this.Property(t => t.CUST_ID_CARD_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.CUST_ID_CARD_NUM)
                .HasMaxLength(30);

            this.Property(t => t.CUST_CATEGORY)
                .HasMaxLength(50);

            this.Property(t => t.CUST_SUB_CATEGORY)
                .HasMaxLength(50);

            this.Property(t => t.CUST_GENDER)
                .HasMaxLength(50);

            this.Property(t => t.CUST_NATIONALITY)
                .HasMaxLength(50);

            this.Property(t => t.CUST_TITLE)
                .HasMaxLength(50);

            this.Property(t => t.ONLINE_NUMBER)
                .HasMaxLength(30);

            this.Property(t => t.CONDO_TYPE)
                .HasMaxLength(500);

            this.Property(t => t.CONDO_DIRECTION)
                .HasMaxLength(500);

            this.Property(t => t.CONDO_LIMIT)
                .HasMaxLength(500);

            this.Property(t => t.CONDO_AREA)
                .HasMaxLength(500);

            this.Property(t => t.HOME_TYPE)
                .HasMaxLength(500);

            this.Property(t => t.HOME_AREA)
                .HasMaxLength(500);

            this.Property(t => t.INSTALL_ADDR_ID)
                .HasMaxLength(50);

            this.Property(t => t.BILL_ADDR_ID)
                .HasMaxLength(50);

            this.Property(t => t.VAT_ADDR_ID)
                .HasMaxLength(50);

            this.Property(t => t.DOCUMENT_TYPE)
                .HasMaxLength(30);

            this.Property(t => t.CVR_ID)
                .HasMaxLength(50);

            this.Property(t => t.PORT_ID)
                .HasMaxLength(50);

            this.Property(t => t.ORDER_NO)
                .HasMaxLength(50);

            this.Property(t => t.REMARK)
                .HasMaxLength(1000);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.ADDRESS_ID)
                .HasMaxLength(100);

            this.Property(t => t.ACCESS_MODE)
                .HasMaxLength(20);

            this.Property(t => t.SERVICE_CODE)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FBB_CUST_PROFILE", "WBB");
            this.Property(t => t.CUST_NON_MOBILE).HasColumnName("CUST_NON_MOBILE");
            this.Property(t => t.CA_ID).HasColumnName("CA_ID");
            this.Property(t => t.SA_ID).HasColumnName("SA_ID");
            this.Property(t => t.BA_ID).HasColumnName("BA_ID");
            this.Property(t => t.IA_ID).HasColumnName("IA_ID");
            this.Property(t => t.CUST_NAME).HasColumnName("CUST_NAME");
            this.Property(t => t.CUST_SURNAME).HasColumnName("CUST_SURNAME");
            this.Property(t => t.CUST_ID_CARD_TYPE).HasColumnName("CUST_ID_CARD_TYPE");
            this.Property(t => t.CUST_ID_CARD_NUM).HasColumnName("CUST_ID_CARD_NUM");
            this.Property(t => t.CUST_CATEGORY).HasColumnName("CUST_CATEGORY");
            this.Property(t => t.CUST_SUB_CATEGORY).HasColumnName("CUST_SUB_CATEGORY");
            this.Property(t => t.CUST_GENDER).HasColumnName("CUST_GENDER");
            this.Property(t => t.CUST_BIRTHDAY).HasColumnName("CUST_BIRTHDAY");
            this.Property(t => t.CUST_NATIONALITY).HasColumnName("CUST_NATIONALITY");
            this.Property(t => t.CUST_TITLE).HasColumnName("CUST_TITLE");
            this.Property(t => t.ONLINE_NUMBER).HasColumnName("ONLINE_NUMBER");
            this.Property(t => t.CONDO_TYPE).HasColumnName("CONDO_TYPE");
            this.Property(t => t.CONDO_DIRECTION).HasColumnName("CONDO_DIRECTION");
            this.Property(t => t.CONDO_LIMIT).HasColumnName("CONDO_LIMIT");
            this.Property(t => t.CONDO_AREA).HasColumnName("CONDO_AREA");
            this.Property(t => t.HOME_TYPE).HasColumnName("HOME_TYPE");
            this.Property(t => t.HOME_AREA).HasColumnName("HOME_AREA");
            this.Property(t => t.INSTALL_ADDR_ID).HasColumnName("INSTALL_ADDR_ID");
            this.Property(t => t.BILL_ADDR_ID).HasColumnName("BILL_ADDR_ID");
            this.Property(t => t.VAT_ADDR_ID).HasColumnName("VAT_ADDR_ID");
            this.Property(t => t.DOCUMENT_TYPE).HasColumnName("DOCUMENT_TYPE");
            this.Property(t => t.CVR_ID).HasColumnName("CVR_ID");
            this.Property(t => t.PORT_ID).HasColumnName("PORT_ID");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.REGISTER_DATE).HasColumnName("REGISTER_DATE");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.RABBIT_REGISTER_DATE).HasColumnName("RABBIT_REGISTER_DATE");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.ACCESS_MODE).HasColumnName("ACCESS_MODE");
            this.Property(t => t.SERVICE_CODE).HasColumnName("SERVICE_CODE");
        }

    }
}
