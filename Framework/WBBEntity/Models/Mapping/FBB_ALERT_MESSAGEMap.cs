using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_ALERT_MESSAGEMap : EntityTypeConfiguration<FBB_ALERT_MESSAGE>
    {
        public FBB_ALERT_MESSAGEMap()
        {
            // Primary Key
            this.HasKey(t => t.ERROR_CODE);

            // Properties
            this.Property(t => t.ERROR_CODE)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.DESCRIPTION_TH)
                .HasMaxLength(100);

            this.Property(t => t.DESCRIPTION_EN)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FBB_ALERT_MESSAGE", "WBB");
            this.Property(t => t.ERROR_CODE).HasColumnName("ERROR_CODE");
            this.Property(t => t.DESCRIPTION_TH).HasColumnName("DESCRIPTION_TH");
            this.Property(t => t.DESCRIPTION_EN).HasColumnName("DESCRIPTION_EN");
        }
    }
}
