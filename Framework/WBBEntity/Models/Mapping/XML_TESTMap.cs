using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class XML_TESTMap : EntityTypeConfiguration<XML_TEST>
    {
        public XML_TESTMap()
        {
            this.HasKey(t => t.ORDER_NO);

            // Table & Column Mappings
            this.ToTable("XML_TEST", "WBB");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.XML_DATA).HasColumnName("XML_DATA");
            this.Property(t => t.FLAG).HasColumnName("FLAG");
        }

    }
}
