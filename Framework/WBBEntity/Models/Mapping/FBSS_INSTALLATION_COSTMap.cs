using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_INSTALLATION_COSTMap : EntityTypeConfiguration<FBSS_INSTALLATION_COST>
    {
        public FBSS_INSTALLATION_COSTMap()
        {
            // Primary Key
            //this.HasKey(t => new { t.SERVICE, t.VENDOR, t.ORDER_TYPE, t.INS_OPTION, t.EFFECTIVE_DATE });
            this.HasKey(t => t.ID);

            // Properties
            //this.Property(t => t.SERVICE)
            //    .IsRequired()
            //    .HasMaxLength(5);

            //this.Property(t => t.INS_OPTION)
            //    .IsRequired()
            //    .HasMaxLength(50);

            //this.Property(t => t.VENDOR)
            //    .IsRequired()
            //    .HasMaxLength(10);

            //this.Property(t => t.RATE)
            //    .HasMaxLength(5);

            //this.Property(t => t.PLAYBOX)
            //    .HasMaxLength(5);

            //this.Property(t => t.VOIP)
            //    .HasMaxLength(5);

            //this.Property(t => t.ORDER_TYPE)
            //    .IsRequired()
            //    .HasMaxLength(20);

            //this.Property(t => t.EFFECTIVE_DATE)
            //    .IsRequired()
            //    .HasMaxLength(30);

            //this.Property(t => t.EXPIRE_DATE)
            //    .HasMaxLength(50);

            //this.Property(t => t.REMARK)
            //    .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FBSS_INSTALLATION_COST", "WBB");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SERVICE).HasColumnName("SERVICE");
            this.Property(t => t.INS_OPTION).HasColumnName("INS_OPTION");
            this.Property(t => t.VENDOR).HasColumnName("VENDOR");
            this.Property(t => t.INTERNET).HasColumnName("INTERNET");
            this.Property(t => t.PLAYBOX).HasColumnName("PLAYBOX");
            this.Property(t => t.VOIP).HasColumnName("VOIP");
            this.Property(t => t.ORDER_TYPE).HasColumnName("ORDER_TYPE");
            this.Property(t => t.EFFECTIVE_DATE).HasColumnName("EFFECTIVE_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.LENGTH_FR).HasColumnName("LENGTH_FR");
            this.Property(t => t.LENGTH_TO).HasColumnName("LENGTH_TO");
            this.Property(t => t.OUT_DOOR_PRICE).HasColumnName("OUT_DOOR_PRICE");
            this.Property(t => t.IN_DOOR_PRICE).HasColumnName("IN_DOOR_PRICE");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.TOTAL_PRICE).HasColumnName("TOTAL_PRICE");
        }
    }
}
