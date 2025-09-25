using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_WFM_SUBCONTRACTORMap : EntityTypeConfiguration<FBBPAYG_WFM_SUBCONTRACTOR>
    {
        public FBBPAYG_WFM_SUBCONTRACTORMap()
        {
            // Primary Key
            this.HasKey(t => t.ROW_ID);

            // Properties
            this.Property(t => t.REGION)
                .HasMaxLength(30);

            this.Property(t => t.SUB_CONTRACTOR_NAME_TH)
                .HasMaxLength(250);

            this.Property(t => t.SUB_CONTRACTOR_NAME_EN)
                .HasMaxLength(250);

            this.Property(t => t.ACTION_FLAG)
                .HasMaxLength(5);

            this.Property(t => t.STORAGE_LOCATION)
                .HasMaxLength(10);


            this.ToTable("FBBPAYG_WFM_SUBCONTRACTOR", "WBB");
            this.Property(t => t.ORG_ID).HasColumnName("ORG_ID");
            this.Property(t => t.REGION).HasColumnName("REGION");
            this.Property(t => t.SUB_CONTRACTOR_NAME_TH).HasColumnName("SUB_CONTRACTOR_NAME_TH");
            this.Property(t => t.SUB_CONTRACTOR_NAME_EN).HasColumnName("SUB_CONTRACTOR_NAME_EN");
            this.Property(t => t.MODIFY_DT).HasColumnName("MODIFY_DT");
            this.Property(t => t.CREATE_DT).HasColumnName("CREATE_DT");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.ACTION_FLAG).HasColumnName("ACTION_FLAG");
            this.Property(t => t.STORAGE_LOCATION).HasColumnName("STORAGE_LOCATION");
        }
    }
}
