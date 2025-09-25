using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_NATIONALITY_MASTERMap : EntityTypeConfiguration<FBB_NATIONALITY_MASTER>
    {
        public FBB_NATIONALITY_MASTERMap()
        {
            this.HasKey(t => new { t.ROW_ID, t.NATIONALITY_THA, t.NATIONALITY_ENG });

            // Properties
            this.Property(t => t.NATIONALITY_THA)
                .HasMaxLength(100);

            this.Property(t => t.NATIONALITY_ENG)
                .HasMaxLength(100);

            this.Property(t => t.STATUS)
                .HasMaxLength(50);

            this.Property(t => t.INTERFACE_SFF)
                .HasMaxLength(100);

            this.Property(t => t.UPD_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_NATIONALITY_MASTER", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.NATIONALITY_THA).HasColumnName("NATIONALITY_THA");
            this.Property(t => t.NATIONALITY_ENG).HasColumnName("NATIONALITY_ENG");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.INTERFACE_SFF).HasColumnName("INTERFACE_SFF");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
        }
    }
}
