using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_REGISTER_PENDING_PAYMENTMap : EntityTypeConfiguration<FBB_REGISTER_PENDING_PAYMENT>
    {
        public FBB_REGISTER_PENDING_PAYMENTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ROW_ID });

            // Properties
            this.Property(t => t.ROW_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AIS_NON_MOBILE)
                .HasMaxLength(50);

            this.Property(t => t.REGISTER_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.PAYMENT_STATUS)
                .HasMaxLength(100);

            this.Property(t => t.WEB_PAYMENT_STATUS)
                .HasMaxLength(100);

            this.Property(t => t.PAYMENT_TRANSACTION_ID_IN)
                .HasMaxLength(50);

            this.Property(t => t.PAYMENT_METHOD)
                .HasMaxLength(50);

            this.Property(t => t.CONTACT_MOBILE_PHONE1)
                .HasMaxLength(50);

            this.Property(t => t.RETURN_ORDER)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_REGISTER_PENDING_PAYMENT", "WBB");
            this.Property(t => t.AIS_NON_MOBILE).HasColumnName("AIS_NON_MOBILE");
            this.Property(t => t.REGISTER_TYPE).HasColumnName("REGISTER_TYPE");
            this.Property(t => t.PAYMENT_STATUS).HasColumnName("PAYMENT_STATUS");
            this.Property(t => t.WEB_PAYMENT_STATUS).HasColumnName("WEB_PAYMENT_STATUS");
            this.Property(t => t.PAYMENT_TRANSACTION_ID_IN).HasColumnName("PAYMENT_TRANSACTION_ID_IN");
            this.Property(t => t.PAYMENT_METHOD).HasColumnName("PAYMENT_METHOD");
            this.Property(t => t.CONTACT_MOBILE_PHONE1).HasColumnName("CONTACT_MOBILE_PHONE1");
            this.Property(t => t.CREATED).HasColumnName("CREATED");
            this.Property(t => t.RETURN_ORDER).HasColumnName("RETURN_ORDER");

        }
    }
}
