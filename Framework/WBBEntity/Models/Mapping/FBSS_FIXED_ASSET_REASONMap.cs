using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_ASSET_REASONMap : EntityTypeConfiguration<FBSS_FIXED_ASSET_REASON>
    {
        public FBSS_FIXED_ASSET_REASONMap()
        {
            this.HasKey(t => new { t.REASON_CODE });
            this.Property(t => t.REASON_CODE).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.ToTable("FBSS_FIXED_ASSET_REASON", "WBB");
            this.Property(t => t.REASON_CODE).HasColumnName("REASON_CODE");
            this.Property(t => t.DISPLAY_VALUE).HasColumnName("DISPLAY_VALUE");
            this.Property(t => t.LONG_VALUE).HasColumnName("LONG_VALUE");
            this.Property(t => t.SYMPTOM_GROUP).HasColumnName("SYMPTOM_GROUP");
            this.Property(t => t.MOVEMENT_OLD).HasColumnName("MOVEMENT_OLD");
            this.Property(t => t.MOVEMENT_NEW).HasColumnName("MOVEMENT_NEW");
            this.Property(t => t.STATUS_OLD).HasColumnName("STATUS_OLD");
            this.Property(t => t.STATUS_NEW).HasColumnName("STATUS_NEW");
            this.Property(t => t.ACTIVE).HasColumnName("ACTIVE");
            this.Property(t => t.DESCRIPTION_CAT).HasColumnName("DESCRIPTION_CAT");
            this.Property(t => t.SYMPTOM_REPLICATION).HasColumnName("SYMPTOM_REPLICATION");
            this.Property(t => t.SYMPTOM_LEVEL).HasColumnName("SYMPTOM_LEVEL");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");
            this.Property(t => t.REASON_GROUP).HasColumnName("REASON_GROUP");

        }
    }
}
