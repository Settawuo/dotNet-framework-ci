using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_GROUPMap : EntityTypeConfiguration<FBB_GROUP>
    {
        public FBB_GROUPMap()
        {
            this.HasKey(t => t.GROUP_ID);

            this.Property(t => t.GROUP_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.GROUP_NAME)
                .HasMaxLength(50);

            this.Property(t => t.GROUP_DESC)
                .HasMaxLength(100);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_GROUP", "WBB");
            this.Property(t => t.GROUP_ID).HasColumnName("GROUP_ID");
            this.Property(t => t.GROUP_NAME).HasColumnName("GROUP_NAME");
            this.Property(t => t.GROUP_DESC).HasColumnName("GROUP_DESC");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
