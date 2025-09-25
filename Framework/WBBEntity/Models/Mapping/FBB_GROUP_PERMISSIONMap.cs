using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_GROUP_PERMISSIONMap : EntityTypeConfiguration<FBB_GROUP_PERMISSION>
    {
        public FBB_GROUP_PERMISSIONMap()
        {
            this.HasKey(t => new { t.USER_ID, t.GROUP_ID });

            this.Property(t => t.USER_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ACTIVE_FLG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_GROUP_PERMISSION", "WBB");
            this.Property(t => t.USER_ID).HasColumnName("USER_ID");
            this.Property(t => t.ACTIVE_FLG).HasColumnName("ACTIVE_FLG");
            this.Property(t => t.GROUP_ID).HasColumnName("GROUP_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
