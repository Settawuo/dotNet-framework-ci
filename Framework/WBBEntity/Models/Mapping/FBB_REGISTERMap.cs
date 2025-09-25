using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_REGISTERMap : EntityTypeConfiguration<FBB_REGISTER>
    {
        public FBB_REGISTERMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ROW_ID, t.CREATED, t.CREATED_BY, t.LAST_UPD, t.LAST_UPD_BY, t.CUST_NAME });

            // Properties
            this.Property(t => t.ROW_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.LAST_UPD_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.CUST_NAME)
                .IsRequired()
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

            this.Property(t => t.CONTACT_FIRST_NAME)
                .HasMaxLength(100);

            this.Property(t => t.CONTACT_LAST_NAME)
                .HasMaxLength(30);

            this.Property(t => t.CONTACT_HOME_PHONE)
                .HasMaxLength(50);

            this.Property(t => t.CONTACT_MOBILE_PHONE1)
                .HasMaxLength(50);

            this.Property(t => t.CONTACT_MOBILE_PHONE2)
                .HasMaxLength(50);

            this.Property(t => t.CONTACT_EMAIL)
                .HasMaxLength(100);

            this.Property(t => t.CONTACT_TIME)
                .HasMaxLength(50);

            this.Property(t => t.SALES_REP)
                .HasMaxLength(30);

            this.Property(t => t.ASC_CODE)
                .HasMaxLength(50);

            this.Property(t => t.EMPLOYEE_ID)
                .HasMaxLength(50);

            this.Property(t => t.LOCATION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.CS_NOTE)
                .HasMaxLength(50);

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

            this.Property(t => t.REMARK)
                .HasMaxLength(1000);

            this.Property(t => t.CVR_ID)
                .HasMaxLength(50);

            this.Property(t => t.CVR_NODE)
                .HasMaxLength(50);

            this.Property(t => t.CVR_TOWER)
                .HasMaxLength(50);

            this.Property(t => t.RETURN_CODE)
                .HasMaxLength(20);

            this.Property(t => t.RETURN_MESSAGE)
                .HasMaxLength(100);

            this.Property(t => t.RETURN_ORDER)
                .HasMaxLength(50);

            this.Property(t => t.CA_ID)
                .HasMaxLength(50);

            this.Property(t => t.SA_ID)
                .HasMaxLength(50);

            this.Property(t => t.BA_ID)
                .HasMaxLength(50);

            this.Property(t => t.AIS_MOBILE)
                .HasMaxLength(50);

            this.Property(t => t.AIS_NON_MOBILE)
                .HasMaxLength(50);

            this.Property(t => t.NETWORK_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.SERVICE_YEAR)
                .HasMaxLength(50);

            this.Property(t => t.REGISTER_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.PHONE_FLAG)
                .HasMaxLength(5);

            this.Property(t => t.TIME_SLOT)
                .HasMaxLength(50);

            this.Property(t => t.INSTALLATION_CAPACITY)
                .HasMaxLength(15);

            this.Property(t => t.ADDRESS_ID)
                .HasMaxLength(100);

            this.Property(t => t.ACCESS_MODE)
                .HasMaxLength(20);

            this.Property(t => t.SERVICE_CODE)
               .HasMaxLength(20);

            this.Property(t => t.ONEBILL_FLAG)
               .HasMaxLength(5);

            this.Property(t => t.EVENT_CODE)
               .HasMaxLength(50);

            this.Property(t => t.NUMBER_OF_PLAYBOX)
              .HasMaxLength(50);

            this.Property(t => t.CONVERGENCE_FLAG)
              .HasMaxLength(50);

            this.Property(t => t.TIME_SLOT_ID)
              .HasMaxLength(50);

            this.Property(t => t.GUID)
              .HasMaxLength(50);

            this.Property(t => t.ADDRESS_FLAG)
              .HasMaxLength(10);

            this.Property(t => t.VOUCHER_PIN)
            .HasMaxLength(100);

            this.Property(t => t.SUB_LOCATION_ID)
              .HasMaxLength(50);

            this.Property(t => t.SUB_CONTRACT_NAME)
              .HasMaxLength(500);

            this.Property(t => t.INSTALL_STAFF_ID)
              .HasMaxLength(15);

            this.Property(t => t.INSTALL_STAFF_NAME)
              .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FBB_REGISTER", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.CREATED).HasColumnName("CREATED");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.LAST_UPD).HasColumnName("LAST_UPD");
            this.Property(t => t.LAST_UPD_BY).HasColumnName("LAST_UPD_BY");
            this.Property(t => t.CUST_NAME).HasColumnName("CUST_NAME");
            this.Property(t => t.CUST_ID_CARD_TYPE).HasColumnName("CUST_ID_CARD_TYPE");
            this.Property(t => t.CUST_ID_CARD_NUM).HasColumnName("CUST_ID_CARD_NUM");
            this.Property(t => t.CUST_CATEGORY).HasColumnName("CUST_CATEGORY");
            this.Property(t => t.CUST_SUB_CATEGORY).HasColumnName("CUST_SUB_CATEGORY");
            this.Property(t => t.CUST_GENDER).HasColumnName("CUST_GENDER");
            this.Property(t => t.CUST_BIRTH_DT).HasColumnName("CUST_BIRTH_DT");
            this.Property(t => t.CUST_NATIONALITY).HasColumnName("CUST_NATIONALITY");
            this.Property(t => t.CUST_TITLE).HasColumnName("CUST_TITLE");
            this.Property(t => t.CONTACT_FIRST_NAME).HasColumnName("CONTACT_FIRST_NAME");
            this.Property(t => t.CONTACT_LAST_NAME).HasColumnName("CONTACT_LAST_NAME");
            this.Property(t => t.CONTACT_HOME_PHONE).HasColumnName("CONTACT_HOME_PHONE");
            this.Property(t => t.CONTACT_MOBILE_PHONE1).HasColumnName("CONTACT_MOBILE_PHONE1");
            this.Property(t => t.CONTACT_MOBILE_PHONE2).HasColumnName("CONTACT_MOBILE_PHONE2");
            this.Property(t => t.CONTACT_EMAIL).HasColumnName("CONTACT_EMAIL");
            this.Property(t => t.CONTACT_TIME).HasColumnName("CONTACT_TIME");
            this.Property(t => t.SALES_REP).HasColumnName("SALES_REP");

            this.Property(t => t.ASC_CODE).HasColumnName("ASC_CODE");
            this.Property(t => t.EMPLOYEE_ID).HasColumnName("EMPLOYEE_ID");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.CS_NOTE).HasColumnName("CS_NOTE");

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
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.CVR_ID).HasColumnName("CVR_ID");
            this.Property(t => t.CVR_NODE).HasColumnName("CVR_NODE");
            this.Property(t => t.CVR_TOWER).HasColumnName("CVR_TOWER");
            this.Property(t => t.RETURN_CODE).HasColumnName("RETURN_CODE");
            this.Property(t => t.RETURN_MESSAGE).HasColumnName("RETURN_MESSAGE");
            this.Property(t => t.RETURN_ORDER).HasColumnName("RETURN_ORDER");
            this.Property(t => t.RESULTID).HasColumnName("RESULTID");
            this.Property(t => t.CA_ID).HasColumnName("CA_ID");
            this.Property(t => t.SA_ID).HasColumnName("SA_ID");
            this.Property(t => t.BA_ID).HasColumnName("BA_ID");

            this.Property(t => t.AIS_MOBILE).HasColumnName("AIS_MOBILE");
            this.Property(t => t.AIS_NON_MOBILE).HasColumnName("AIS_NON_MOBILE");
            this.Property(t => t.NETWORK_TYPE).HasColumnName("NETWORK_TYPE");
            this.Property(t => t.SERVICE_YEAR).HasColumnName("SERVICE_YEAR");
            this.Property(t => t.REQUEST_INSTALL_DATE).HasColumnName("REQUEST_INSTALL_DATE");
            this.Property(t => t.REGISTER_TYPE).HasColumnName("REGISTER_TYPE");
            this.Property(t => t.PHONE_FLAG).HasColumnName("PHONE_FLAG");
            this.Property(t => t.TIME_SLOT).HasColumnName("TIME_SLOT");
            this.Property(t => t.INSTALLATION_CAPACITY).HasColumnName("INSTALLATION_CAPACITY");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.ACCESS_MODE).HasColumnName("ACCESS_MODE");
            this.Property(t => t.SERVICE_CODE).HasColumnName("SERVICE_CODE");
            this.Property(t => t.ONEBILL_FLAG).HasColumnName("ONEBILL_FLAG");
            this.Property(t => t.EVENT_CODE).HasColumnName("EVENT_CODE");
            this.Property(t => t.EVENT_CODE).HasColumnName("NUMBER_OF_PLAYBOX");
            this.Property(t => t.EVENT_CODE).HasColumnName("CONVERGENCE_FLAG");

            this.Property(t => t.TIME_SLOT_ID).HasColumnName("TIME_SLOT_ID");
            this.Property(t => t.GUID).HasColumnName("GUID");
            this.Property(t => t.ADDRESS_FLAG).HasColumnName("ADDRESS_FLAG");

            this.Property(t => t.VOUCHER_PIN).HasColumnName("VOUCHER_PIN");
            this.Property(t => t.SUB_LOCATION_ID).HasColumnName("SUB_LOCATION_ID");
            this.Property(t => t.SUB_CONTRACT_NAME).HasColumnName("SUB_CONTRACT_NAME");
            this.Property(t => t.INSTALL_STAFF_ID).HasColumnName("INSTALL_STAFF_ID");
            this.Property(t => t.INSTALL_STAFF_NAME).HasColumnName("INSTALL_STAFF_NAME");

        }
    }
}
