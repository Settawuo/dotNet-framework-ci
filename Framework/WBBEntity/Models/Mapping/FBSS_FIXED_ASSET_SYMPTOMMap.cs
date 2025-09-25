using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_ASSET_SYMPTOMMap : EntityTypeConfiguration<FBSS_FIXED_ASSET_SYMPTOM>
    {
        public FBSS_FIXED_ASSET_SYMPTOMMap()
        {
            this.HasKey(t => new { t.SYMPTOM_CODE });
            this.Property(t => t.SYMPTOM_CODE).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.ToTable("FBSS_FIXED_ASSET_SYMPTOM", "WBB");
            this.Property(t => t.SYMPTOM_CODE).HasColumnName("SYMPTOM_CODE");
            this.Property(t => t.CATEGORY).HasColumnName("CATEGORY");
            this.Property(t => t.SUB_CATEGORY).HasColumnName("SUB_CATEGORY");
            this.Property(t => t.SYMPTOM_NAME).HasColumnName("SYMPTOM_NAME");
            this.Property(t => t.SR_TT).HasColumnName("SR_TT");
            this.Property(t => t.SLA_HOURS).HasColumnName("SLA_HOURS");
            this.Property(t => t.SEARCH_SPEC_EXPR).HasColumnName("SEARCH_SPEC_EXPR");
            this.Property(t => t.SYMPTOM_TYPE).HasColumnName("SYMPTOM_TYPE");
            this.Property(t => t.SYMPTOM_GROUP).HasColumnName("SYMPTOM_GROUP");
            this.Property(t => t.OWNER).HasColumnName("OWNER");
            this.Property(t => t.REQUIRE_TYPE).HasColumnName("REQUIRE_TYPE");
            this.Property(t => t.OLD_SN_OLD_TYPE).HasColumnName("OLD_SN_OLD_TYPE");
            this.Property(t => t.NEW_SN_OLD_TYPE).HasColumnName("NEW_SN_OLD_TYPE");
            this.Property(t => t.NEW_SN_NEW_TYPE).HasColumnName("NEW_SN_NEW_TYPE");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");


        }
    }
}
