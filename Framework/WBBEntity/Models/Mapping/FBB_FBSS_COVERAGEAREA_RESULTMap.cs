using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_FBSS_COVERAGEAREA_RESULTMap : EntityTypeConfiguration<FBB_FBSS_COVERAGEAREA_RESULT>
    {
        public FBB_FBSS_COVERAGEAREA_RESULTMap()
        {
            this.HasKey(t => t.RESULTID);
            this.Property(t => t.RESULTID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.ADDRRESS_TYPE)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.POSTAL_CODE)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.SUB_DISTRICT_NAME)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.LANGUAGE)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.BUILDING_NAME).HasMaxLength(250);
            this.Property(t => t.BUILDING_NO).HasMaxLength(20);
            this.Property(t => t.PHONE_FLAG)
                .HasMaxLength(1);
            this.Property(t => t.FLOOR_NO).HasMaxLength(20);
            this.Property(t => t.ADDRESS_NO)
                .HasMaxLength(20);
            this.Property(t => t.SOI).HasMaxLength(50);
            this.Property(t => t.ROAD).HasMaxLength(50);
            this.Property(t => t.LATITUDE).HasMaxLength(50);
            this.Property(t => t.LONGITUDE).HasMaxLength(50);
            this.Property(t => t.UNIT_NO).HasMaxLength(20);
            this.Property(t => t.COVERAGE).HasMaxLength(10);
            this.Property(t => t.ADDRESS_ID).HasMaxLength(100);
            this.Property(t => t.ACCESS_MODE_LIST).HasMaxLength(2000);
            this.Property(t => t.PLANNING_SITE_LIST).HasMaxLength(2000);
            this.Property(t => t.IS_PARTNER).HasMaxLength(1);
            this.Property(t => t.PARTNER_NAME).HasMaxLength(100);
            this.Property(t => t.PREFIXNAME).HasMaxLength(20);
            this.Property(t => t.FIRSTNAME).HasMaxLength(50);
            this.Property(t => t.LASTNAME).HasMaxLength(50);
            this.Property(t => t.CONTACTNUMBER).HasMaxLength(100);
            this.Property(t => t.PRODUCTTYPE).HasMaxLength(20);
            this.Property(t => t.ZIPCODE_ROWID)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.RETURN_MESSAGE).HasMaxLength(500);
            this.Property(t => t.RETURN_ORDER).HasMaxLength(30);
            this.Property(t => t.OWNER_PRODUCT).HasMaxLength(20);
            this.Property(t => t.TRANSACTION_ID).HasMaxLength(100);
            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.CREATED_DATE)
                .IsRequired();
            this.Property(t => t.UPDATED_BY).HasMaxLength(50);
            this.Property(t => t.CONTACT_EMAIL).HasMaxLength(100);
            this.Property(t => t.CONTACT_LINE_ID).HasMaxLength(50);
            this.Property(t => t.LOCATION_CODE).HasMaxLength(100);
            this.Property(t => t.ASC_CODE).HasMaxLength(100);
            this.Property(t => t.EMPLOYEE_ID).HasMaxLength(100);
            this.Property(t => t.SALE_NAME).HasMaxLength(3000);
            this.Property(t => t.LOCATION_NAME).HasMaxLength(3000);
            this.Property(t => t.SUB_REGION).HasMaxLength(10);
            this.Property(t => t.REGION_NAME).HasMaxLength(10);
            this.Property(t => t.ASC_NAME).HasMaxLength(3000);
            this.Property(t => t.CHANNEL_NAME).HasMaxLength(3000);
            this.Property(t => t.SALE_CHANNEL).HasMaxLength(1000);
            this.Property(t => t.ADDRESS_TYPE_DTL).HasMaxLength(1000);
            this.Property(t => t.REMARK).HasMaxLength(4000);
            this.Property(t => t.TECHNOLOGY).HasMaxLength(200);
            this.Property(t => t.PROJECTNAME).HasMaxLength(200);
            // onservice special
            this.Property(t => t.COVERAGE_AREA).HasMaxLength(10);
            this.Property(t => t.COVERAGE_STATUS).HasMaxLength(100);
            this.Property(t => t.COVERAGE_SUBSTATUS).HasMaxLength(100);
            this.Property(t => t.COVERAGE_CONTACTEMAIL).HasMaxLength(100);
            this.Property(t => t.COVERAGE_CONTACTTEL).HasMaxLength(100);
            this.Property(t => t.COVERAGE_GROUPOWNER).HasMaxLength(100);
            this.Property(t => t.COVERAGE_CONTACTNAME).HasMaxLength(255);
            this.Property(t => t.COVERAGE_NETWORKPROVIDER).HasMaxLength(50);
            this.Property(t => t.COVERAGE_FTTHDISPLAYMESSAGE).HasMaxLength(1000);
            this.Property(t => t.COVERAGE_WTTXDISPLAYMESSAGE).HasMaxLength(1000);
            //------------------------------------------------------
            this.ToTable("FBB_FBSS_COVERAGEAREA_RESULT", "WBB");
            this.Property(t => t.RESULTID).HasColumnName("RESULTID");
            this.Property(t => t.ADDRRESS_TYPE).HasColumnName("ADDRRESS_TYPE");
            this.Property(t => t.POSTAL_CODE).HasColumnName("POSTAL_CODE");
            this.Property(t => t.SUB_DISTRICT_NAME).HasColumnName("SUB_DISTRICT_NAME");
            this.Property(t => t.LANGUAGE).HasColumnName("LANGUAGE");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");
            this.Property(t => t.BUILDING_NO).HasColumnName("BUILDING_NO");
            this.Property(t => t.PHONE_FLAG).HasColumnName("PHONE_FLAG");
            this.Property(t => t.FLOOR_NO).HasColumnName("FLOOR_NO");
            this.Property(t => t.ADDRESS_NO).HasColumnName("ADDRESS_NO");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.ROAD).HasColumnName("ROAD");
            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
            this.Property(t => t.UNIT_NO).HasColumnName("UNIT_NO");
            this.Property(t => t.COVERAGE).HasColumnName("COVERAGE");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.ACCESS_MODE_LIST).HasColumnName("ACCESS_MODE_LIST");
            this.Property(t => t.PLANNING_SITE_LIST).HasColumnName("PLANNING_SITE_LIST");
            this.Property(t => t.IS_PARTNER).HasColumnName("IS_PARTNER");
            this.Property(t => t.PARTNER_NAME).HasColumnName("PARTNER_NAME");
            this.Property(t => t.PREFIXNAME).HasColumnName("PREFIXNAME");
            this.Property(t => t.FIRSTNAME).HasColumnName("FIRSTNAME");
            this.Property(t => t.LASTNAME).HasColumnName("LASTNAME");
            this.Property(t => t.CONTACTNUMBER).HasColumnName("CONTACTNUMBER");
            this.Property(t => t.PRODUCTTYPE).HasColumnName("PRODUCTTYPE");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.RETURN_CODE).HasColumnName("RETURN_CODE");
            this.Property(t => t.RETURN_MESSAGE).HasColumnName("RETURN_MESSAGE");
            this.Property(t => t.RETURN_ORDER).HasColumnName("RETURN_ORDER");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.CONTACT_EMAIL).HasColumnName("CONTACT_EMAIL");
            this.Property(t => t.CONTACT_LINE_ID).HasColumnName("CONTACT_LINE_ID");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.ASC_CODE).HasColumnName("ASC_CODE");
            this.Property(t => t.EMPLOYEE_ID).HasColumnName("EMPLOYEE_ID");
            this.Property(t => t.SALE_NAME).HasColumnName("SALE_NAME");
            this.Property(t => t.LOCATION_NAME).HasColumnName("LOCATION_NAME");
            this.Property(t => t.SUB_REGION).HasColumnName("SUB_REGION");
            this.Property(t => t.REGION_NAME).HasColumnName("REGION_NAME");
            this.Property(t => t.ASC_NAME).HasColumnName("ASC_NAME");
            this.Property(t => t.CHANNEL_NAME).HasColumnName("CHANNEL_NAME");
            this.Property(t => t.SALE_CHANNEL).HasColumnName("SALE_CHANNEL");
            this.Property(t => t.ADDRESS_TYPE_DTL).HasColumnName("ADDRESS_TYPE_DTL");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.PROJECTNAME).HasColumnName("PROJECTNAME");
            // onservice special
            this.Property(t => t.COVERAGE_AREA).HasColumnName("COVERAGE_AREA");
            this.Property(t => t.COVERAGE_STATUS).HasColumnName("COVERAGE_STATUS");
            this.Property(t => t.COVERAGE_SUBSTATUS).HasColumnName("COVERAGE_SUBSTATUS");
            this.Property(t => t.COVERAGE_CONTACTEMAIL).HasColumnName("COVERAGE_CONTACTEMAIL");
            this.Property(t => t.COVERAGE_CONTACTTEL).HasColumnName("COVERAGE_CONTACTTEL");
            this.Property(t => t.COVERAGE_GROUPOWNER).HasColumnName("COVERAGE_GROUPOWNER");
            this.Property(t => t.COVERAGE_CONTACTNAME).HasColumnName("COVERAGE_CONTACTNAME");
            this.Property(t => t.COVERAGE_NETWORKPROVIDER).HasColumnName("COVERAGE_NETWORKPROVIDER");
            this.Property(t => t.COVERAGE_FTTHDISPLAYMESSAGE).HasColumnName("COVERAGE_FTTHDISPLAYMESSAGE");
            this.Property(t => t.COVERAGE_WTTXDISPLAYMESSAGE).HasColumnName("COVERAGE_WTTXDISPLAYMESSAGE");
        }
    }
}
