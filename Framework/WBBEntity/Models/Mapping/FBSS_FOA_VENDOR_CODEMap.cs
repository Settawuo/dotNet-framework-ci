using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FOA_VENDOR_CODEMap : EntityTypeConfiguration<FBSS_FOA_VENDOR_CODE>
    {
        public FBSS_FOA_VENDOR_CODEMap()
        {
            // Primary Key
            this.HasKey(t => t.ORG_ID);

            // Table & Column Mappings
            this.ToTable("FBSS_FOA_VENDOR_CODE", "WBB");
            this.Property(t => t.ORG_ID).HasColumnName("ORG_ID");
            this.Property(t => t.VENDOR_CODE).HasColumnName("VENDOR_CODE");
        }
    }
}
