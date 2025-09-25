using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_EMPLOYEEMap : EntityTypeConfiguration<FBB_EMPLOYEE>
    {
        public FBB_EMPLOYEEMap()
        {
            // Primary Key
            this.HasKey(t => t.EMP_PIN);

            // Properties

            this.Property(t => t.EMP_PIN)
                .HasMaxLength(50);

            this.Property(t => t.EMP_USER_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CREATE_BY)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("FBB_EMPLOYEE", "WBB");
            this.Property(t => t.EMP_PIN).HasColumnName("EMP_PIN");
            this.Property(t => t.EMP_USER_NAME).HasColumnName("EMP_USER_NAME");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.CREATE_DTM).HasColumnName("CREATE_DTM");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
        }
    }
}
