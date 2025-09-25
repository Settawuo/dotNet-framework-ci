using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_OM010_RPTMap : EntityTypeConfiguration<FBSS_FIXED_OM010_RPT>
    {

        public FBSS_FIXED_OM010_RPTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ACC_NBR });

            // Properties
            this.Property(t => t.ORD_NO)
                .HasMaxLength(64);

            this.Property(t => t.ORDER_SFF)
                .HasMaxLength(50);

            this.ToTable("FBSS_FIXED_OM010_RPT", "WBB");
            this.Property(t => t.ACC_NBR).HasColumnName("ACC_NBR");
            this.Property(t => t.ORD_NO).HasColumnName("ORD_NO");
            this.Property(t => t.ORDER_SFF).HasColumnName("ORDER_SFF");
            this.Property(t => t.SFF_ACTIVE_DATE).HasColumnName("SFF_ACTIVE_DATE");
        }

    }
}
