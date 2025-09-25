using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.FBBShareplexModels.Mapping
{
    public class WFS_TEAM_ATTRMap : EntityTypeConfiguration<WFS_TEAM_ATTR>
    {
        public WFS_TEAM_ATTRMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TEAM_ID });

            // Table & Column Mappings
            this.ToTable("WFS_TEAM_ATTR", "WFM_R8");
            this.Property(t => t.TEAM_ID).HasColumnName("TEAM_ID");
            this.Property(t => t.STAGE_LOCAL).HasColumnName("STAGE_LOCAL");
            this.Property(t => t.SHIP_TO).HasColumnName("SHIP_TO");
            this.Property(t => t.TEAM_ID).HasColumnName("TEAM_ID");
            this.Property(t => t.SEQ).HasColumnName("SEQ");
            this.Property(t => t.LOCATION_CODE).HasColumnName("LOCATION_CODE");
            this.Property(t => t.VENDOR_CODE).HasColumnName("VENDOR_CODE");
            this.Property(t => t.JOB_TYPE).HasColumnName("JOB_TYPE");
            this.Property(t => t.SUBCONTRACT_EMAIL).HasColumnName("SUBCONTRACT_EMAIL");
            this.Property(t => t.ALIAS_COMPANY_NAME).HasColumnName("ALIAS_COMPANY_NAME");
            this.Property(t => t.OOS_STAGE_LOCAL).HasColumnName("OOS_STAGE_LOCAL");
            this.Property(t => t.SUBCONTRACT_TYPE).HasColumnName("SUBCONTRACT_TYPE");
            this.Property(t => t.SUBCONTRACT_SUB_TYPE).HasColumnName("SUBCONTRACT_SUB_TYPE");
            this.Property(t => t.WARRANTY_MA).HasColumnName("WARRANTY_MA");
            this.Property(t => t.WARRANTY_INSTALL).HasColumnName("WARRANTY_INSTALL");
            this.Property(t => t.PRI_IN_WARRANTY).HasColumnName("PRI_IN_WARRANTY");
            this.Property(t => t.PRI_OUT_WARRANTY).HasColumnName("PRI_OUT_WARRANTY");
            this.Property(t => t.SERVICE_SKILL).HasColumnName("SERVICE_SKILL");
            this.Property(t => t.SUB_PHASE).HasColumnName("SUB_PHASE");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.CREATE_USER).HasColumnName("CREATE_USER");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.MODIFY_USER).HasColumnName("MODIFY_USER");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");

        }
    }
}
