using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_EVENT_CODEMap : EntityTypeConfiguration<FBB_EVENT_CODE>
    {
        public FBB_EVENT_CODEMap()
        {
            this.HasKey(t => new
            {
                t.EVENT_CODE
            });

            // Properties
            this.Property(t => t.EVENT_CODE).HasMaxLength(50);

            this.Property(t => t.CREATED_BY).HasMaxLength(50);

            this.Property(t => t.UPDATED_BY).HasMaxLength(50);

            this.Property(t => t.TECHNOLOGY).HasMaxLength(100);

            this.Property(t => t.PLUG_AND_PLAY_FLAG).HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("FBB_EVENT_CODE", "WBB");
            this.Property(t => t.EVENT_CODE).HasColumnName("EVENT_CODE");
            this.Property(t => t.EFFECTIVE_DATE).HasColumnName("EFFECTIVE_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.PLUG_AND_PLAY_FLAG).HasColumnName("PLUG_AND_PLAY_FLAG");
        }
    }
}
