using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_IAMap : EntityTypeConfiguration<AIR_SALE_ORD_IA>
    {
        public AIR_SALE_ORD_IAMap()
        {

            // Primary Key
            this.HasKey(t => t.ORDER_NO);
            // Properties
            this.Property(t => t.ORDER_NO)
            .HasMaxLength(30);
            this.Property(t => t.PRODUCT_TYPE)
            .HasMaxLength(30);
            this.Property(t => t.PRODUCT_SUBTYPE)
            .HasMaxLength(30);
            this.Property(t => t.IA_NO)
            .HasMaxLength(30);

            this.Property(t => t.IA_STATUS)
            .HasMaxLength(2);

            this.Property(t => t.OLD_PORT_SERVICE_TYPE)
            .HasMaxLength(100);
            this.Property(t => t.OLD_PORT_FEATURE)
            .HasMaxLength(200);
            this.Property(t => t.OLD_PORT_INTERFACE)
            .HasMaxLength(200);
            this.Property(t => t.HOST_ADDRESS)
            .HasMaxLength(500);
            this.Property(t => t.HOST_CONTACT_NAME)
            .HasMaxLength(100);
            this.Property(t => t.HOST_EQUIPMENT_STATUS)
            .HasMaxLength(1);
            this.Property(t => t.TERMINAL_EQUIPMENT_STATUS)
            .HasMaxLength(1);
            this.Property(t => t.OLD_MEDIA_TYPE)
            .HasMaxLength(100);
            this.Property(t => t.CIRCUIT_ID)
            .HasMaxLength(30);

            this.Property(t => t.UPD_BY)
            .HasMaxLength(15);
            this.Property(t => t.HOST_LATITUDE)
            .HasMaxLength(50);
            this.Property(t => t.HOST_LONGTITUDE)
            .HasMaxLength(50);
            this.Property(t => t.COVERAGE_FLAG)
            .HasMaxLength(1);
            this.Property(t => t.SFF_CA_NO)
            .HasMaxLength(50);
            this.Property(t => t.NON_MOBILE_NO)
            .HasMaxLength(50);
            this.Property(t => t.PASSWORD)
            .HasMaxLength(10);
            this.Property(t => t.ASC_CODE)
            .HasMaxLength(50);
            this.Property(t => t.EMPLOYEE_ID)
            .HasMaxLength(30);
            this.Property(t => t.LOCATION_CODE)
            .HasMaxLength(30);
            this.Property(t => t.WIFI_ACCESS_POINT)
            .HasMaxLength(1);
            this.Property(t => t.INSTALL_STATUS)
            .HasMaxLength(1);
            this.Property(t => t.COVERAGE_REMARK)
            .HasMaxLength(500);
            this.Property(t => t.ADDRESS_TYPE)
            .HasMaxLength(100);
            this.Property(t => t.FLOOR_NUMBER)
            .HasMaxLength(50);
            this.Property(t => t.AP_COVERAGE)
            .HasMaxLength(100);
            this.Property(t => t.SECTOR)
            .HasMaxLength(100);
            this.Property(t => t.LINE_OF_SIGHT)
            .HasMaxLength(100);
            this.Property(t => t.DEGREE)
            .HasMaxLength(100);
            this.Property(t => t.NEAR_LOCATION)
            .HasMaxLength(200);
            this.Property(t => t.OBSERVATION_POINT)
            .HasMaxLength(200);
            this.Property(t => t.JOB_ORDER_REMARK)
            .HasMaxLength(500);
            this.Property(t => t.EXISTING_AIRNET_NO)
            .HasMaxLength(30);
            this.Property(t => t.GSM_MOBILE_NO)
            .HasMaxLength(30);
            this.Property(t => t.REMARK)
            .HasMaxLength(500);
            this.Property(t => t.HOST_NAME)
            .HasMaxLength(500);
            this.Property(t => t.SFF_SA_NO)
            .HasMaxLength(30);
            this.Property(t => t.SFF_BA_NO)
            .HasMaxLength(30);
            this.Property(t => t.JOB_ORDER_TYPE)
            .HasMaxLength(1);
            this.Property(t => t.SALE_REPRESENT)
            .HasMaxLength(100);
            this.Property(t => t.SUB_CONTRACT_ROW_ID)
            .HasMaxLength(15);
            this.Property(t => t.PORT)
            .HasMaxLength(100);
            this.Property(t => t.VOICE)
            .HasMaxLength(100);
            this.Property(t => t.DATA)
            .HasMaxLength(100);
            this.Property(t => t.JUMP_TYPE)
            .HasMaxLength(30);
            this.Property(t => t.JUMP_TELEPHONE)
            .HasMaxLength(100);
            this.Property(t => t.NODE_NAME)
            .HasMaxLength(100);
            this.Property(t => t.ROUTER_FLAG)
            .HasMaxLength(1);
            this.Property(t => t.APPOINTMENT_REASON)
            .HasMaxLength(200);
            this.Property(t => t.INSTALL_REASON)
            .HasMaxLength(200);
            this.Property(t => t.APPOINTMENT_STATUS)
            .HasMaxLength(10);
            this.Property(t => t.ADDRESS_TYPE_WIRE)
            .HasMaxLength(100);
            this.Property(t => t.CONDO_ROOF_TOP)
            .HasMaxLength(1);
            this.Property(t => t.CONDO_BALCONY)
            .HasMaxLength(1);
            this.Property(t => t.BALCONY_NORTH)
            .HasMaxLength(1);
            this.Property(t => t.BALCONY_SOUTH)
            .HasMaxLength(1);
            this.Property(t => t.BALCONY_EAST)
            .HasMaxLength(1);
            this.Property(t => t.BALCONY_WEST)
            .HasMaxLength(1);
            this.Property(t => t.CUSTOMER_REMARK)
            .HasMaxLength(2000);
            this.Property(t => t.CS_NOTE)
            .HasMaxLength(2000);
            this.Property(t => t.PORT_REASON)
            .HasMaxLength(200);
            this.Property(t => t.HIGH_BUILDING)
            .HasMaxLength(1);
            this.Property(t => t.HIGH_TREE)
            .HasMaxLength(1);
            this.Property(t => t.BILLBOARD)
            .HasMaxLength(1);
            this.Property(t => t.EXPRESSWAY)
            .HasMaxLength(1);
            this.Property(t => t.CONDO_FLOOR)
            .HasMaxLength(100);
            this.Property(t => t.CVR_ID)
            .HasMaxLength(20);
            this.Property(t => t.PORT_ID)
            .HasMaxLength(20);
            this.Property(t => t.JOB_GRADE)
            .HasMaxLength(1);
            this.Property(t => t.NODE_TOWER)
            .HasMaxLength(100);
            this.Property(t => t.SITE_CODE)
            .HasMaxLength(50);
            this.Property(t => t.DSLAM_NAME)
            .HasMaxLength(100);
            this.Property(t => t.DSLAM_MAX_PORT)
            .HasMaxLength(10);

            this.ToTable("AIR_SALE_ORD_IA", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.PRODUCT_SUBTYPE).HasColumnName("PRODUCT_SUBTYPE");
            this.Property(t => t.IA_NO).HasColumnName("IA_NO");
            this.Property(t => t.EFFECTIVE_DTM).HasColumnName("EFFECTIVE_DTM");
            this.Property(t => t.EXPIRE_DTM).HasColumnName("EXPIRE_DTM");
            this.Property(t => t.IA_STATUS).HasColumnName("IA_STATUS");
            this.Property(t => t.NO_OF_PORT).HasColumnName("NO_OF_PORT");
            this.Property(t => t.OLD_PORT_SERVICE_TYPE).HasColumnName("OLD_PORT_SERVICE_TYPE");
            this.Property(t => t.OLD_PORT_FEATURE).HasColumnName("OLD_PORT_FEATURE");
            this.Property(t => t.OLD_PORT_INTERFACE).HasColumnName("OLD_PORT_INTERFACE");
            this.Property(t => t.HOST_ADDRESS).HasColumnName("HOST_ADDRESS");
            this.Property(t => t.HOST_CONTACT_NAME).HasColumnName("HOST_CONTACT_NAME");
            this.Property(t => t.HOST_EQUIPMENT_STATUS).HasColumnName("HOST_EQUIPMENT_STATUS");
            this.Property(t => t.TERMINAL_EQUIPMENT_STATUS).HasColumnName("TERMINAL_EQUIPMENT_STATUS");
            this.Property(t => t.OLD_MEDIA_TYPE).HasColumnName("OLD_MEDIA_TYPE");
            this.Property(t => t.CIRCUIT_ID).HasColumnName("CIRCUIT_ID");
            this.Property(t => t.INSTALL_DATE).HasColumnName("INSTALL_DATE");
            this.Property(t => t.TEST_DATE).HasColumnName("TEST_DATE");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.HOST_LATITUDE).HasColumnName("HOST_LATITUDE");
            this.Property(t => t.HOST_LONGTITUDE).HasColumnName("HOST_LONGTITUDE");
            this.Property(t => t.COVERAGE_FLAG).HasColumnName("COVERAGE_FLAG");
            this.Property(t => t.SFF_CA_NO).HasColumnName("SFF_CA_NO");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");
            this.Property(t => t.PASSWORD).HasColumnName("PASSWORD");
            this.Property(t => t.ASC_CODE).HasColumnName("ASC_CODE");
            this.Property(t => t.EMPLOYEE_ID).HasColumnName("EMPLOYEE_ID");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.WIFI_ACCESS_POINT).HasColumnName("WIFI_ACCESS_POINT");
            this.Property(t => t.INSTALL_STATUS).HasColumnName("INSTALL_STATUS");
            this.Property(t => t.COVERAGE_REMARK).HasColumnName("COVERAGE_REMARK");
            this.Property(t => t.ADDRESS_TYPE).HasColumnName("ADDRESS_TYPE");
            this.Property(t => t.FLOOR_NUMBER).HasColumnName("FLOOR_NUMBER");
            this.Property(t => t.AP_COVERAGE).HasColumnName("AP_COVERAGE");
            this.Property(t => t.SECTOR).HasColumnName("SECTOR");
            this.Property(t => t.LINE_OF_SIGHT).HasColumnName("LINE_OF_SIGHT");
            this.Property(t => t.DEGREE).HasColumnName("DEGREE");
            this.Property(t => t.NEAR_LOCATION).HasColumnName("NEAR_LOCATION");
            this.Property(t => t.OBSERVATION_POINT).HasColumnName("OBSERVATION_POINT");
            this.Property(t => t.JOB_ORDER_REMARK).HasColumnName("JOB_ORDER_REMARK");
            this.Property(t => t.EXISTING_AIRNET_NO).HasColumnName("EXISTING_AIRNET_NO");
            this.Property(t => t.GSM_MOBILE_NO).HasColumnName("GSM_MOBILE_NO");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.HOST_NAME).HasColumnName("HOST_NAME");
            this.Property(t => t.SFF_SA_NO).HasColumnName("SFF_SA_NO");
            this.Property(t => t.SFF_BA_NO).HasColumnName("SFF_BA_NO");
            this.Property(t => t.JOB_ORDER_TYPE).HasColumnName("JOB_ORDER_TYPE");
            this.Property(t => t.SALE_REPRESENT).HasColumnName("SALE_REPRESENT");
            this.Property(t => t.SUB_CONTRACT_ROW_ID).HasColumnName("SUB_CONTRACT_ROW_ID");
            this.Property(t => t.PORT).HasColumnName("PORT");
            this.Property(t => t.VOICE).HasColumnName("VOICE");
            this.Property(t => t.DATA).HasColumnName("DATA");
            this.Property(t => t.JUMP_TYPE).HasColumnName("JUMP_TYPE");
            this.Property(t => t.JUMP_TELEPHONE).HasColumnName("JUMP_TELEPHONE");
            this.Property(t => t.NODE_NAME).HasColumnName("NODE_NAME");
            this.Property(t => t.ROUTER_FLAG).HasColumnName("ROUTER_FLAG");
            this.Property(t => t.APPOINTMENT_REASON).HasColumnName("APPOINTMENT_REASON");
            this.Property(t => t.INSTALL_REASON).HasColumnName("INSTALL_REASON");
            this.Property(t => t.APPOINTMENT_STATUS).HasColumnName("APPOINTMENT_STATUS");
            this.Property(t => t.ADDRESS_TYPE_WIRE).HasColumnName("ADDRESS_TYPE_WIRE");
            this.Property(t => t.CONDO_ROOF_TOP).HasColumnName("CONDO_ROOF_TOP");
            this.Property(t => t.CONDO_BALCONY).HasColumnName("CONDO_BALCONY");
            this.Property(t => t.BALCONY_NORTH).HasColumnName("BALCONY_NORTH");
            this.Property(t => t.BALCONY_SOUTH).HasColumnName("BALCONY_SOUTH");
            this.Property(t => t.BALCONY_EAST).HasColumnName("BALCONY_EAST");
            this.Property(t => t.BALCONY_WEST).HasColumnName("BALCONY_WEST");
            this.Property(t => t.CUSTOMER_REMARK).HasColumnName("CUSTOMER_REMARK");
            this.Property(t => t.CS_NOTE).HasColumnName("CS_NOTE");
            this.Property(t => t.PORT_REASON).HasColumnName("PORT_REASON");
            this.Property(t => t.HIGH_BUILDING).HasColumnName("HIGH_BUILDING");
            this.Property(t => t.HIGH_TREE).HasColumnName("HIGH_TREE");
            this.Property(t => t.BILLBOARD).HasColumnName("BILLBOARD");
            this.Property(t => t.EXPRESSWAY).HasColumnName("EXPRESSWAY");
            this.Property(t => t.CONDO_FLOOR).HasColumnName("CONDO_FLOOR");
            this.Property(t => t.CVR_ID).HasColumnName("CVR_ID");
            this.Property(t => t.PORT_ID).HasColumnName("PORT_ID");
            this.Property(t => t.JOB_GRADE).HasColumnName("JOB_GRADE");
            this.Property(t => t.NODE_TOWER).HasColumnName("NODE_TOWER");
            this.Property(t => t.SITE_CODE).HasColumnName("SITE_CODE");
            this.Property(t => t.DSLAM_NAME).HasColumnName("DSLAM_NAME");
            this.Property(t => t.DSLAM_MAX_PORT).HasColumnName("DSLAM_MAX_PORT");

        }
    }
}
