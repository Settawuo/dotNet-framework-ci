using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_ORDER_INTERFACEMap : EntityTypeConfiguration<FBBDORM_ORDER_INTERFACE>
    {
        public FBBDORM_ORDER_INTERFACEMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_INTERFACE_ID);

            // Properties
            this.Property(t => t.ORDER_INTERFACE_ID)
              .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PREPAID_NON_MOBILE)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.REFERENCE_NO)
                .HasMaxLength(100);

            this.Property(t => t.SFF_ORDER_NO)
                .HasMaxLength(100);

            this.Property(t => t.ORDER_TYPE)
                .HasMaxLength(100);

            this.Property(t => t.EVENT_NAME)
                .HasMaxLength(100);

            this.Property(t => t.ERROR_CODE)
                .HasMaxLength(100);

            this.Property(t => t.ERROR_MESSAGE)
                .HasMaxLength(1000);

            this.Property(t => t.CREATED_BY)
               .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
              .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBBDORM_ORDER_INTERFACE", "WBB");
            this.Property(t => t.ORDER_INTERFACE_ID).HasColumnName("ORDER_INTERFACE_ID");
            this.Property(t => t.PREPAID_NON_MOBILE).HasColumnName("PREPAID_NON_MOBILE");
            this.Property(t => t.REFERENCE_NO).HasColumnName("REFERENCE_NO");
            this.Property(t => t.SFF_ORDER_NO).HasColumnName("SFF_ORDER_NO");
            this.Property(t => t.ORDER_TYPE).HasColumnName("ORDER_TYPE");
            this.Property(t => t.EVENT_NAME).HasColumnName("EVENT_NAME");
            this.Property(t => t.ERROR_CODE).HasColumnName("ERROR_CODE");
            this.Property(t => t.ERROR_MESSAGE).HasColumnName("ERROR_MESSAGE");
            this.Property(t => t.INTERFACE_ID).HasColumnName("INTERFACE_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
