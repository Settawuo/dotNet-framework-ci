using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_IMPLEMENT_REGIS_RABBITMap : EntityTypeConfiguration<FBB_IMPLEMENT_REGIS_RABBIT>
    {
        public FBB_IMPLEMENT_REGIS_RABBITMap()
        {
            this.HasKey(t => t.CUST_NON_MOBILE);

            //this.Property(t => t.CUST_NON_MOBILE)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CUST_NON_MOBILE)
               .HasMaxLength(50);

            this.Property(t => t.CUST_ID_CARD_NUM)
                .HasMaxLength(30);

            this.Property(t => t.CUST_NAME)
                .HasMaxLength(100);

            this.Property(t => t.CUST_SURNAME)
                .HasMaxLength(100);

            this.Property(t => t.CUST_GENDER)
                .HasMaxLength(50);

            this.Property(t => t.RABBIT_EMAIL)
                .HasMaxLength(100);

            this.Property(t => t.CONTACT_MOBILE_PHONE)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("FBB_IMPLEMENT_REGIS_RABBIT", "WBB");
            this.Property(t => t.CUST_NON_MOBILE).HasColumnName("CUST_NON_MOBILE");
            this.Property(t => t.CUST_ID_CARD_NUM).HasColumnName("CUST_ID_CARD_NUM");
            this.Property(t => t.CUST_NAME).HasColumnName("CUST_NAME");
            this.Property(t => t.CUST_SURNAME).HasColumnName("CUST_SURNAME");
            this.Property(t => t.CUST_BIRTHDAY).HasColumnName("CUST_BIRTHDAY");
            this.Property(t => t.CUST_GENDER).HasColumnName("CUST_GENDER");
            this.Property(t => t.RABBIT_REGISTER_DATE).HasColumnName("RABBIT_REGISTER_DATE");
            this.Property(t => t.RABBIT_EMAIL).HasColumnName("RABBIT_EMAIL");
            this.Property(t => t.CONTACT_MOBILE_PHONE).HasColumnName("CONTACT_MOBILE_PHONE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
