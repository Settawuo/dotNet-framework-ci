using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SFF_PROMOTION_CONFIGMap : EntityTypeConfiguration<FBB_SFF_PROMOTION_CONFIG>
    {
        public FBB_SFF_PROMOTION_CONFIGMap()
        {
            this.HasKey(t => new
            {
                t.SFF_PROMOTION_CODE,
                t.PROMOTION_CLASS
            });

            // Properties

            this.Property(t => t.CREATED_BY).HasMaxLength(50);

            this.Property(t => t.UPDATED_BY).HasMaxLength(50);

            this.Property(t => t.TYPE_PROMOTION).HasMaxLength(30);

            this.Property(t => t.SFF_PROMOTION_CODE).HasMaxLength(50);

            this.Property(t => t.PROMOTION_CLASS).HasMaxLength(50);

            this.Property(t => t.FLAG).HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_SFF_PROMOTION_CONFIG", "WBB");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.TYPE_PROMOTION).HasColumnName("TYPE_PROMOTION");
            this.Property(t => t.SFF_PROMOTION_CODE).HasColumnName("SFF_PROMOTION_CODE");
            this.Property(t => t.PROMOTION_CLASS).HasColumnName("PROMOTION_CLASS");
            this.Property(t => t.FLAG).HasColumnName("FLAG");
        }
    }
}
