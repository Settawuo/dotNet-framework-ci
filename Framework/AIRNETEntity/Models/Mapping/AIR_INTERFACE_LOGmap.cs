using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_INTERFACE_LOGmap : EntityTypeConfiguration<AIR_INTERFACE_LOG>
    {
        public AIR_INTERFACE_LOGmap()
        {
            // Primary Key
            this
         .HasKey(t => t.INTERFACE_DTM);

            // Properties  
            this.Property(t => t.ORDER_NO)
         .HasMaxLength(30);
            this.Property(t => t.INTERFACE_EVENT)
         .HasMaxLength(100);
            this.Property(t => t.INTERFACE_BY)
         .HasMaxLength(20);
            this.Property(t => t.INTERFACE_BY)
         .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AIR_INTERFACE_LOG", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.INTERFACE_EVENT).HasColumnName("INTERFACE_EVENT");
            this.Property(t => t.INTERFACE_DTM).HasColumnName("INTERFACE_DTM");
            this.Property(t => t.INTERFACE_BY).HasColumnName("INTERFACE_BY");
            this.Property(t => t.INTERFACE_REQUEST).HasColumnName("INTERFACE_REQUEST");
            this.Property(t => t.INTERFACE_DATA).HasColumnName("INTERFACE_DATA");
        }
    }
}
