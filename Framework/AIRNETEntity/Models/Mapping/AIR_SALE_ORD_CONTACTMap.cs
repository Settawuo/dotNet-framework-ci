using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_CONTACTMap : EntityTypeConfiguration<AIR_SALE_ORD_CONTACT>
    {
        public AIR_SALE_ORD_CONTACTMap()
        {

            // Primary Key
            this.HasKey(t => t.ORDER_NO);
            // Properties  
            this.Property(t => t.PRIMARY_CONTACT_TYPE)
            .HasMaxLength(1);
            this.Property(t => t.CONTACT_TITLE_CODE)
            .HasMaxLength(3);
            this.Property(t => t.CONTACT_FIRST_NAME)
            .HasMaxLength(100);
            this.Property(t => t.CONTACT_LAST_NAME)
            .HasMaxLength(100);
            this.Property(t => t.CONTACT_DEPARTMENT)
            .HasMaxLength(200);
            this.Property(t => t.CONTACT_POSITION_CODE)
            .HasMaxLength(3);
            this.Property(t => t.HOUSE_NO)
            .HasMaxLength(20);
            this.Property(t => t.MOO_NO)
            .HasMaxLength(50);
            this.Property(t => t.BUILDING)
            .HasMaxLength(250);
            this.Property(t => t.SOI)
            .HasMaxLength(50);
            this.Property(t => t.ROAD)
            .HasMaxLength(50);
            this.Property(t => t.TUMBON)
            .HasMaxLength(50);
            this.Property(t => t.ZIPCODE)
            .HasMaxLength(10);
            this.Property(t => t.ZIPCODE_THAI_ENG)
            .HasMaxLength(1);
            this.Property(t => t.WORK_TEL_NO)
            .HasMaxLength(30);
            this.Property(t => t.HOME_TEL_NO)
            .HasMaxLength(30);
            this.Property(t => t.MOBILE_NO)
            .HasMaxLength(30);
            this.Property(t => t.FAX_NO)
            .HasMaxLength(30);
            this.Property(t => t.EMAIL_ADDRESS)
            .HasMaxLength(50);
            this.Property(t => t.UPD_BY)
            .IsRequired()
            .HasMaxLength(15);
            this.Property(t => t.ZIPCODE_ROWID)
            .HasMaxLength(50);
            this.Property(t => t.CONTACT_TIME)
            .HasMaxLength(30);
            this.Property(t => t.CONTACT_FOR_BILL)
            .HasMaxLength(1);
            this.Property(t => t.MOOBAN)
            .HasMaxLength(50);
            this.Property(t => t.FLOOR)
            .HasMaxLength(50);
            this.Property(t => t.ROOM)
            .HasMaxLength(50);
            this.Property(t => t.MOBILE_NO_2)
            .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORD_CONTACT", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.CONTACT_SEQ).HasColumnName("CONTACT_SEQ");
            this.Property(t => t.PRIMARY_CONTACT_TYPE).HasColumnName("PRIMARY_CONTACT_TYPE");
            this.Property(t => t.CONTACT_TITLE_CODE).HasColumnName("CONTACT_TITLE_CODE");
            this.Property(t => t.CONTACT_FIRST_NAME).HasColumnName("CONTACT_FIRST_NAME");
            this.Property(t => t.CONTACT_LAST_NAME).HasColumnName("CONTACT_LAST_NAME");
            this.Property(t => t.CONTACT_DEPARTMENT).HasColumnName("CONTACT_DEPARTMENT");
            this.Property(t => t.CONTACT_POSITION_CODE).HasColumnName("CONTACT_POSITION_CODE");
            this.Property(t => t.HOUSE_NO).HasColumnName("HOUSE_NO");
            this.Property(t => t.MOO_NO).HasColumnName("MOO_NO");
            this.Property(t => t.BUILDING).HasColumnName("BUILDING");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.ROAD).HasColumnName("ROAD");
            this.Property(t => t.TUMBON).HasColumnName("TUMBON");
            this.Property(t => t.ZIPCODE).HasColumnName("ZIPCODE");
            this.Property(t => t.ZIPCODE_THAI_ENG).HasColumnName("ZIPCODE_THAI_ENG");
            this.Property(t => t.WORK_TEL_NO).HasColumnName("WORK_TEL_NO");
            this.Property(t => t.HOME_TEL_NO).HasColumnName("HOME_TEL_NO");
            this.Property(t => t.MOBILE_NO).HasColumnName("MOBILE_NO");
            this.Property(t => t.FAX_NO).HasColumnName("FAX_NO");
            this.Property(t => t.EMAIL_ADDRESS).HasColumnName("EMAIL_ADDRESS");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.CONTACT_TIME).HasColumnName("CONTACT_TIME");
            this.Property(t => t.CONTACT_FOR_BILL).HasColumnName("CONTACT_FOR_BILL");
            this.Property(t => t.MOOBAN).HasColumnName("MOOBAN");
            this.Property(t => t.FLOOR).HasColumnName("FLOOR");
            this.Property(t => t.ROOM).HasColumnName("ROOM");
            this.Property(t => t.MOBILE_NO_2).HasColumnName("MOBILE_NO_2");
        }
    }
}
