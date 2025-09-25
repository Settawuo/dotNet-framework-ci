using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_FLOWMap : EntityTypeConfiguration<AIR_SALE_ORD_FLOW>
    {
        public AIR_SALE_ORD_FLOWMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_NO);

            // Properties 
            this.Property(t => t.REMARK)
            .HasMaxLength(500);
            this.Property(t => t.INPUT_COMPLETE_FLAG)
            .HasMaxLength(1);
            this.Property(t => t.UPD_BY)
            .IsRequired()
            .HasMaxLength(15);
            this.Property(t => t.CANCEL_CODE)
            .HasMaxLength(3);
            this.Property(t => t.OTHER_CANCEL_REASON)
            .HasMaxLength(200);
            this.Property(t => t.MENU_CODE)
            .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORD_FLOW", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.WORK_FLOW_ID).HasColumnName("WORK_FLOW_ID");
            this.Property(t => t.FLOW_SEQ).HasColumnName("FLOW_SEQ");
            this.Property(t => t.FLOW_CREATE_DTM).HasColumnName("FLOW_CREATE_DTM");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.INPUT_COMPLETE_FLAG).HasColumnName("INPUT_COMPLETE_FLAG");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.CANCEL_CODE).HasColumnName("CANCEL_CODE");
            this.Property(t => t.OTHER_CANCEL_REASON).HasColumnName("OTHER_CANCEL_REASON");
            this.Property(t => t.MENU_CODE).HasColumnName("MENU_CODE");
        }
    }
}
