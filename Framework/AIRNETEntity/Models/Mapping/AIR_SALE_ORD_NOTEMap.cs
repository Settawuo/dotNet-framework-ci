using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_NOTEMap : EntityTypeConfiguration<AIR_SALE_ORD_NOTE>
    {
        public AIR_SALE_ORD_NOTEMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_NO);

            // Properties
            this.Property(t => t.ORDER_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.NOTE_USER_NAME)
                .HasMaxLength(15);

            this.Property(t => t.NOTE_DETAIL)
                .HasMaxLength(500);

            this.Property(t => t.UPD_BY)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORD_NOTE", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.NOTE_DATE).HasColumnName("NOTE_DATE");
            this.Property(t => t.NOTE_USER_NAME).HasColumnName("NOTE_USER_NAME");
            this.Property(t => t.NOTE_DETAIL).HasColumnName("NOTE_DETAIL");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
        }
    }
}
