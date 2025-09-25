using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SALE_ORD_INTERFACEMap : EntityTypeConfiguration<AIR_SALE_ORD_INTERFACE>
    {

        public AIR_SALE_ORD_INTERFACEMap()
        {
            // Primary Key
            this.HasKey(t => t.ORDER_NO);

            // Properties
            this.Property(t => t.ORDER_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.INTERFACE_BY)
                .HasMaxLength(15);

            this.Property(t => t.INTERFACE_STATUS)
                .HasMaxLength(20);

            this.Property(t => t.SFF_CA_NUMBER)
                .HasMaxLength(30);

            this.Property(t => t.NON_MOBILE_NO)
                .HasMaxLength(30);

            this.Property(t => t.MAIN_PROMOTION)
                .HasMaxLength(100);

            this.Property(t => t.ONTOP_PROMOTION)
                .HasMaxLength(100);

            this.Property(t => t.ERROR_REASON)
                .HasMaxLength(255);

            this.Property(t => t.REMARK)
                .HasMaxLength(500);

            this.Property(t => t.INTERFACE_RESULT)
                .HasMaxLength(10);

            this.Property(t => t.UPD_BY)
                .HasMaxLength(15);

            this.Property(t => t.SFF_SA_NUMBER)
                .HasMaxLength(30);

            this.Property(t => t.SFF_BA_NUMBER)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("AIR_SALE_ORD_INTERFACE", "AIR_ADMIN");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
            this.Property(t => t.INTERFACE_DATE).HasColumnName("INTERFACE_DATE");
            this.Property(t => t.INTERFACE_BY).HasColumnName("INTERFACE_BY");
            this.Property(t => t.INTERFACE_STATUS).HasColumnName("INTERFACE_STATUS");
            this.Property(t => t.SFF_CA_NUMBER).HasColumnName("SFF_CA_NUMBER");
            this.Property(t => t.NON_MOBILE_NO).HasColumnName("NON_MOBILE_NO");
            this.Property(t => t.MAIN_PROMOTION).HasColumnName("MAIN_PROMOTION");
            this.Property(t => t.ONTOP_PROMOTION).HasColumnName("ONTOP_PROMOTION");
            this.Property(t => t.ERROR_REASON).HasColumnName("ERROR_REASON");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.INTERFACE_RESULT).HasColumnName("INTERFACE_RESULT");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.SFF_SA_NUMBER).HasColumnName("SFF_SA_NUMBER");
            this.Property(t => t.SFF_BA_NUMBER).HasColumnName("SFF_BA_NUMBER");
        }
    }
}
