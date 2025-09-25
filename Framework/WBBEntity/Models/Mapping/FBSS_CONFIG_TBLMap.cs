using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_CONFIG_TBLMap : EntityTypeConfiguration<FBSS_CONFIG_TBL>
    {
        public FBSS_CONFIG_TBLMap()
        {
            this.HasKey(t => new { t.CON_ID });
            this.Property(t => t.CON_ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.ToTable("FBSS_CONFIG_TBL", "WBB");
            this.Property(t => t.CON_ID).HasColumnName("CON_ID");
            this.Property(t => t.CON_NAME).HasColumnName("CON_NAME");
            this.Property(t => t.DISPLAY_VAL).HasColumnName("DISPLAY_VAL");
            this.Property(t => t.VAL1).HasColumnName("VAL1");
            this.Property(t => t.VAL2).HasColumnName("VAL2");
            this.Property(t => t.VAL3).HasColumnName("VAL3");
            this.Property(t => t.VAL4).HasColumnName("VAL4");
            this.Property(t => t.VAL5).HasColumnName("VAL5");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.ORDER_BY).HasColumnName("ORDER_BY");
            this.Property(t => t.DEFAULT_VALUE).HasColumnName("DEFAULT_VALUE");
            this.Property(t => t.IMAGE_BLOB).HasColumnName("IMAGE_BLOB");
            this.Property(t => t.XML_BLOB).HasColumnName("XML_BLOB");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
