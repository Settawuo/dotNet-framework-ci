using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_LOAD_CONFIG_LOVMap : EntityTypeConfiguration<FBB_LOAD_CONFIG_LOV>
    {
        public FBB_LOAD_CONFIG_LOVMap()
        {
            // Primary Key
            this.HasKey(t => t.ROW_ID);

            // Properties
            this.Property(t => t.ROW_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.EVENT_NAME)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.FLAG)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.FLAG2)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FBB_LOAD_CONFIG_LOV", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.EVENT_NAME).HasColumnName("EVENT_NAME");
            this.Property(t => t.FLAG).HasColumnName("FLAG");
            this.Property(t => t.FLAG2).HasColumnName("FLAG2");
        }
    }
}
