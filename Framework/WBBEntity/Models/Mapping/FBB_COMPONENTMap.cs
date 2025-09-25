using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COMPONENTMap : EntityTypeConfiguration<FBB_COMPONENT>
    {
        public FBB_COMPONENTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.COMPONENT_ID });

            // Properties
            this.Property(t => t.PROGRAM_ID)
                .HasMaxLength(50);

            this.Property(t => t.COMPONENT_NAME)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.COMPONENT_TYPE)
                .HasMaxLength(100);


            // Table & Column Mappings
            this.ToTable("FBB_COMPONENT", "WBB");
            this.Property(t => t.COMPONENT_ID).HasColumnName("COMPONENT_ID");
            this.Property(t => t.PROGRAM_ID).HasColumnName("PROGRAM_ID");
            this.Property(t => t.COMPONENT_NAME).HasColumnName("COMPONENT_NAME");
            this.Property(t => t.ACTIVE_FLG).HasColumnName("ACTIVE_FLG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.COMPONENT_TYPE).HasColumnName("COMPONENT_TYPE");
        }
    }
}
