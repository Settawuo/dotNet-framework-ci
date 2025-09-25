using System.Data.Entity.ModelConfiguration;


namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_DORMITORY_REQUESTMap : EntityTypeConfiguration<FBBDORM_DORMITORY_REQUEST>
    {
        public FBBDORM_DORMITORY_REQUESTMap()
        {

            // Primary Key
            this.HasKey(t => new { t.ROW_ID });

            // Properties          

            this.Property(t => t.CREATE_BY)
                .HasMaxLength(100);

            this.Property(t => t.LAST_UPDATE_BY)
                .HasMaxLength(100);

            this.Property(t => t.TYPE_CUSTOMER_REQUEST)
                .HasMaxLength(50);

            this.Property(t => t.CUSTOMER_FIRST_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CUSTOMER_LAST_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CONTRACT_PHONE)
                .HasMaxLength(50);

            this.Property(t => t.DORMITORY_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.TYPE_DORMITORY)
                .HasMaxLength(50);

            this.Property(t => t.HOME_NO)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.MOO)
               .HasMaxLength(100);

            this.Property(t => t.SOI)
              .HasMaxLength(100);

            this.Property(t => t.STREET)
                .HasMaxLength(1000);

            this.Property(t => t.ZIPCODE_ROW_ID)
                .HasMaxLength(50);

            this.Property(t => t.A_BUILDING)
                .HasMaxLength(10);

            this.Property(t => t.A_UNIT)
                .HasMaxLength(10);

            this.Property(t => t.A_LIVING)
                .HasMaxLength(10);

            this.Property(t => t.PHONE_CABLE)
                .HasMaxLength(10);

            this.Property(t => t.PROBLEM_INTERNET)
              .HasMaxLength(100);

            this.Property(t => t.A_UNIT_USE_INTERNET)
              .HasMaxLength(10);

            this.Property(t => t.OLD_SYSTEM)
              .HasMaxLength(50);

            this.Property(t => t.OLD_VENDOR_SERVICE)
              .HasMaxLength(50);

            this.Property(t => t.FLAG_LANGUAGE)
              .HasMaxLength(10);

            this.Property(t => t.FLAG_SEND_TO_SALE)
              .HasMaxLength(10);

            this.Property(t => t.USER_APPROVE)
              .HasMaxLength(100);

            this.Property(t => t.PROCESS_STATUS)
              .HasMaxLength(100);

            this.Property(t => t.DORMITORY_MASTER_ID)
              .HasMaxLength(50);

            this.Property(t => t.REMARK)
              .HasMaxLength(1000);


            // Table & Column Mappings
            this.ToTable("FBBDORM_DORMITORY_REQUEST", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.CREATED_DT).HasColumnName("CREATED_DT");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.LAST_UPDATE_DT).HasColumnName("LAST_UPDATE_DT");
            this.Property(t => t.LAST_UPDATE_BY).HasColumnName("LAST_UPDATE_BY");
            this.Property(t => t.TYPE_CUSTOMER_REQUEST).HasColumnName("TYPE_CUSTOMER_REQUEST");
            this.Property(t => t.CUSTOMER_FIRST_NAME).HasColumnName("CUSTOMER_FIRST_NAME");
            this.Property(t => t.CUSTOMER_LAST_NAME).HasColumnName("CUSTOMER_LAST_NAME");
            this.Property(t => t.CONTRACT_PHONE).HasColumnName("CONTRACT_PHONE");
            this.Property(t => t.DORMITORY_NAME).HasColumnName("DORMITORY_NAME");
            this.Property(t => t.TYPE_DORMITORY).HasColumnName("TYPE_DORMITORY");
            this.Property(t => t.HOME_NO).HasColumnName("HOME_NO");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.STREET).HasColumnName("CREATED_DATE");
            this.Property(t => t.ZIPCODE_ROW_ID).HasColumnName("ZIPCODE_ROW_ID");
            this.Property(t => t.A_BUILDING).HasColumnName("A_BUILDING");
            this.Property(t => t.A_UNIT).HasColumnName("A_UNIT");
            this.Property(t => t.A_LIVING).HasColumnName("A_LIVING");
            this.Property(t => t.PHONE_CABLE).HasColumnName("PHONE_CABLE");
            this.Property(t => t.PROBLEM_INTERNET).HasColumnName("PROBLEM_INTERNET");
            this.Property(t => t.A_UNIT_USE_INTERNET).HasColumnName("A_UNIT_USE_INTERNET");
            this.Property(t => t.OLD_SYSTEM).HasColumnName("OLD_SYSTEM");
            this.Property(t => t.OLD_VENDOR_SERVICE).HasColumnName("OLD_VENDOR_SERVICE");
            this.Property(t => t.FLAG_LANGUAGE).HasColumnName("FLAG_LANGUAGE");
            this.Property(t => t.FLAG_SEND_TO_SALE).HasColumnName("FLAG_SEND_TO_SALE");
            this.Property(t => t.SEND_TO_SALE_DT).HasColumnName("SEND_TO_SALE_DT");
            this.Property(t => t.USER_APPROVE).HasColumnName("USER_APPROVE");
            this.Property(t => t.USER_APPROVE_DT).HasColumnName("USER_APPROVE_DT");
            this.Property(t => t.PROCESS_STATUS).HasColumnName("PROCESS_STATUS");
            this.Property(t => t.DORMITORY_MASTER_ID).HasColumnName("DORMITORY_MASTER_ID");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
        }
    }
}
