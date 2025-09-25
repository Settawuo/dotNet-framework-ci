using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CUST_CONTACTMap : EntityTypeConfiguration<FBB_CUST_CONTACT>
    {
        public FBB_CUST_CONTACTMap()
        {
            // Primary Key
            this.HasKey(t => t.CREATED_DATE);

            // Properties
            this.Property(t => t.CUST_NON_MOBILE)
               .HasMaxLength(50);

            this.Property(t => t.CONTACT_ADDR_ID)
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

            this.Property(t => t.CREATED_BY)
               .IsRequired()
               .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
               .IsRequired()
               .HasMaxLength(30);

            this.Property(t => t.BA_ID)
              .HasMaxLength(50);

            this.Property(t => t.CA_ID)
              .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_CUST_CONTACT", "WBB");
            this.Property(t => t.CUST_NON_MOBILE).HasColumnName("CUST_NON_MOBILE");
            this.Property(t => t.CONTACT_ADDR_ID).HasColumnName("CONTACT_ADDR_ID");
            this.Property(t => t.CONTACT_FIRST_NAME).HasColumnName("CONTACT_FIRST_NAME");
            this.Property(t => t.CONTACT_LAST_NAME).HasColumnName("CONTACT_LAST_NAME");
            this.Property(t => t.CONTACT_HOME_PHONE).HasColumnName("CONTACT_HOME_PHONE");
            this.Property(t => t.CONTACT_MOBILE_PHONE1).HasColumnName("CONTACT_MOBILE_PHONE1");
            this.Property(t => t.CONTACT_MOBILE_PHONE2).HasColumnName("CONTACT_MOBILE_PHONE2");
            this.Property(t => t.CONTACT_EMAIL).HasColumnName("CONTACT_EMAIL");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.CONTACT_SEQ).HasColumnName("CONTACT_SEQ");
            this.Property(t => t.BA_ID).HasColumnName("BA_ID");
            this.Property(t => t.CA_ID).HasColumnName("CA_ID");
        }
    }
}
