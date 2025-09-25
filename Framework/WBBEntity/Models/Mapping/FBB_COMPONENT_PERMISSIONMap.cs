using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COMPONENT_PERMISSIONMap : EntityTypeConfiguration<FBB_COMPONENT_PERMISSION>
    {
        public FBB_COMPONENT_PERMISSIONMap()
        {
            // Primary Key
            this.HasKey(t => new { t.COMPONENT_PERMISSION_ID });

            // Properties           
            this.Property(t => t.ENABLE_FLG)
                .HasMaxLength(1);

            this.Property(t => t.ACTIVE_FLG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.READ_ONLY_FLG)
                .HasMaxLength(2);


            // Table & Column Mappings
            this.ToTable("FBB_COMPONENT_PERMISSION", "WBB");
            this.Property(t => t.COMPONENT_PERMISSION_ID).HasColumnName("COMPONENT_PERMISSION_ID");
            this.Property(t => t.COMPONENT_ID).HasColumnName("COMPONENT_ID");
            this.Property(t => t.GROUP_ID).HasColumnName("GROUP_ID");
            this.Property(t => t.ENABLE_FLG).HasColumnName("ENABLE_FLG");
            this.Property(t => t.ACTIVE_FLG).HasColumnName("ACTIVE_FLG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.READ_ONLY_FLG).HasColumnName("READ_ONLY_FLG");
        }
    }
}
