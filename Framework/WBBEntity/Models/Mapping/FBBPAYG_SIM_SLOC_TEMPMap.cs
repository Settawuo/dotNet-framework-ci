using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_SIM_SLOC_TEMPMap : EntityTypeConfiguration<FBBPAYG_SIM_SLOC_TEMP>
    {
        public FBBPAYG_SIM_SLOC_TEMPMap()
        {
            // Primary Key
            this.HasKey(t => t.SHIP_ID);

            this.Property(t => t.SHIP_ID)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.STORAGE_LOCATION)
                .HasMaxLength(30);

            this.Property(t => t.CREATE_BY)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("FBBPAYG_SIM_SLOC_TEMP", "WBB");
            this.Property(t => t.SHIP_ID).HasColumnName("SHIP_ID");
            this.Property(t => t.STORAGE_LOCATION).HasColumnName("STORAGE_LOCATION");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
        }
    }
}
