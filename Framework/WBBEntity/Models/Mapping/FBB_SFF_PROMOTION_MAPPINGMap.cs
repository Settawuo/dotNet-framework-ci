using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SFF_PROMOTION_MAPPINGMap : EntityTypeConfiguration<FBB_SFF_PROMOTION_MAPPING>
    {
        public FBB_SFF_PROMOTION_MAPPINGMap()
        {
            this.HasKey(t => new
            {
                t.CONDITION_TYPE,
                t.PROJECT_NAME
            });

            // Properties
            this.Property(t => t.CONDITION_TYPE).HasMaxLength(30);

            this.Property(t => t.PROJECT_NAME).HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE).HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE_1).HasMaxLength(30);

            this.Property(t => t.SFF_PROMOTION_CODE_2).HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE_3).HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE_4).HasMaxLength(50);

            this.Property(t => t.SFF_PROMOTION_CODE_5).HasMaxLength(50);

            this.Property(t => t.UPD_BY).HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("FBB_SFF_PROMOTION_MAPPING", "WBB");
            this.Property(t => t.CONDITION_TYPE).HasColumnName("CONDITION_TYPE");
            this.Property(t => t.PROJECT_NAME).HasColumnName("PROJECT_NAME");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.SFF_PROMOTION_CODE_1).HasColumnName("SFF_PROMOTION_CODE_1");
            this.Property(t => t.SFF_PROMOTION_CODE_2).HasColumnName("SFF_PROMOTION_CODE_2");
            this.Property(t => t.SFF_PROMOTION_CODE_3).HasColumnName("SFF_PROMOTION_CODE_3");
            this.Property(t => t.SFF_PROMOTION_CODE_4).HasColumnName("SFF_PROMOTION_CODE_4");
            this.Property(t => t.SFF_PROMOTION_CODE_5).HasColumnName("SFF_PROMOTION_CODE_5");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
        }
    }
}
