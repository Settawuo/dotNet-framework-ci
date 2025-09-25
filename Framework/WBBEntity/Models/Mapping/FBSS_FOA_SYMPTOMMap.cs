using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FOA_SYMPTOMMap : EntityTypeConfiguration<FBSS_FOA_SYMPTOM>
    {
        public FBSS_FOA_SYMPTOMMap()
        {
            this.HasKey(t => new { t.SYMPTOM_CODE });
            this.Property(t => t.SYMPTOM_CODE).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.ToTable("FBSS_FOA_SYMPTOM", "WBB");
            this.Property(t => t.SYMPTOM_CODE).HasColumnName("SYMPTOM_CODE");
            this.Property(t => t.DISPLAY_VALUE).HasColumnName("DISPLAY_VALUE");
            this.Property(t => t.LONG_VALUE).HasColumnName("LONG_VALUE");
            this.Property(t => t.SYMPTOM_GROUP).HasColumnName("SYMPTOM_GROUP");
            this.Property(t => t.LANGUAGE_INDEPENDENT_CODE).HasColumnName("LANGUAGE_INDEPENDENT_CODE");
            this.Property(t => t.LANGUAGE_NAME).HasColumnName("LANGUAGE_NAME");
            this.Property(t => t.PARENT_LIC).HasColumnName("PARENT_LIC");
            this.Property(t => t.SYMPTOM_ORDER).HasColumnName("SYMPTOM_ORDER");
            this.Property(t => t.ACTIVE).HasColumnName("ACTIVE");
            this.Property(t => t.TRANSLATE1).HasColumnName("TRANSLATE1");
            this.Property(t => t.MULTILINGUAL).HasColumnName("MULTILINGUAL");
            this.Property(t => t.DESCRIPTION_CAT).HasColumnName("DESCRIPTION_CAT");
            this.Property(t => t.SYMPTOM_REPLICATION).HasColumnName("SYMPTOM_REPLICATION");
            this.Property(t => t.SYMPTOM_LEVEL).HasColumnName("SYMPTOM_LEVEL");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");

        }
    }
}
