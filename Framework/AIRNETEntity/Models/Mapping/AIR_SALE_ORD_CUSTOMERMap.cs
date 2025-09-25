using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_CUSTOMERMap : EntityTypeConfiguration<AIR_SALE_ORD_CUSTOMER>
    {
        public AIR_SALE_ORD_CUSTOMERMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_NO);

            // Properties
            this.Property(t => t.CUSTOMER_ID)
            .HasMaxLength(10);
            this.Property(t => t.TITLE_CODE)
            .HasMaxLength(3);
            this.Property(t => t.FIRST_NAME)
            .HasMaxLength(100);
            this.Property(t => t.LAST_NAME)
            .HasMaxLength(100);
            this.Property(t => t.CUSTOMER_TYPE)
            .HasMaxLength(1);
            this.Property(t => t.CUSTOMER_SUBTYPE)
            .HasMaxLength(1);
            this.Property(t => t.TAX_ID)
            .HasMaxLength(20);
            this.Property(t => t.STATUS)
            .HasMaxLength(2);
            this.Property(t => t.ID_CARD_TYPE)
            .HasMaxLength(3);
            this.Property(t => t.ID_CARD_NO)
            .HasMaxLength(20);
            this.Property(t => t.REGISTRATION_NO)
            .HasMaxLength(20);
            this.Property(t => t.GENDER)
            .HasMaxLength(1);
            this.Property(t => t.NATIONALITY_CODE)
            .HasMaxLength(3);
            this.Property(t => t.EDUCATION_CODE)
            .HasMaxLength(3);
            this.Property(t => t.OCCUPATION_CODE)
            .HasMaxLength(3);
            this.Property(t => t.SALARY_CODE)
            .HasMaxLength(3);
            this.Property(t => t.EMAIL_ADDRESS)
            .HasMaxLength(50);
            this.Property(t => t.WEBSITE)
            .HasMaxLength(50);
            this.Property(t => t.BUSINESS_TYPE_CODE)
            .HasMaxLength(3);
            this.Property(t => t.UPD_BY)
            .IsRequired()
            .HasMaxLength(15);
            this.Property(t => t.CA_BRANCH_NO)
            .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORD_CUSTOMER", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.CUSTOMER_ID).HasColumnName("CUSTOMER_ID");
            this.Property(t => t.TITLE_CODE).HasColumnName("TITLE_CODE");
            this.Property(t => t.FIRST_NAME).HasColumnName("FIRST_NAME");
            this.Property(t => t.LAST_NAME).HasColumnName("LAST_NAME");
            this.Property(t => t.CUSTOMER_TYPE).HasColumnName("CUSTOMER_TYPE");
            this.Property(t => t.CUSTOMER_SUBTYPE).HasColumnName("CUSTOMER_SUBTYPE");
            this.Property(t => t.TAX_ID).HasColumnName("TAX_ID");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.ID_CARD_TYPE).HasColumnName("ID_CARD_TYPE");
            this.Property(t => t.ID_CARD_NO).HasColumnName("ID_CARD_NO");
            this.Property(t => t.REGISTRATION_NO).HasColumnName("REGISTRATION_NO");
            this.Property(t => t.ID_EXPIRED_DATE).HasColumnName("ID_EXPIRED_DATE");
            this.Property(t => t.GENDER).HasColumnName("GENDER");
            this.Property(t => t.NATIONALITY_CODE).HasColumnName("NATIONALITY_CODE");
            this.Property(t => t.BIRTH_DATE).HasColumnName("BIRTH_DATE");
            this.Property(t => t.EDUCATION_CODE).HasColumnName("EDUCATION_CODE");
            this.Property(t => t.OCCUPATION_CODE).HasColumnName("OCCUPATION_CODE");
            this.Property(t => t.SALARY_CODE).HasColumnName("SALARY_CODE");
            this.Property(t => t.EMAIL_ADDRESS).HasColumnName("EMAIL_ADDRESS");
            this.Property(t => t.WEBSITE).HasColumnName("WEBSITE");
            this.Property(t => t.BUSINESS_TYPE_CODE).HasColumnName("BUSINESS_TYPE_CODE");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.CA_BRANCH_NO).HasColumnName("CA_BRANCH_NO");
        }
    }
}
