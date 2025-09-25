using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_AP_INFOMap : EntityTypeConfiguration<FBB_AP_INFO>
    {
        public FBB_AP_INFOMap()
        {
            // Primary Key
            this.HasKey(t => t.AP_ID);

            this.Property(t => t.AP_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.AP_NAME)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.SECTOR)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.IP_ADDRESS)
                .HasMaxLength(50);

            this.Property(t => t.STATUS)
                .HasMaxLength(50);

            this.Property(t => t.IMPLEMENT_PHASE)
                .HasMaxLength(50);

            this.Property(t => t.PO_NUMBER)
                .HasMaxLength(50);

            this.Property(t => t.AP_COMPANY)
                .HasMaxLength(50);

            this.Property(t => t.AP_LOT)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_AP_INFO", "WBB");
            this.Property(t => t.AP_ID).HasColumnName("AP_ID");
            this.Property(t => t.AP_NAME).HasColumnName("AP_NAME");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.SECTOR).HasColumnName("SECTOR");
            this.Property(t => t.SITE_ID).HasColumnName("SITE_ID");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");

            this.Property(t => t.IP_ADDRESS).HasColumnName("IP_ADDRESS");
            this.Property(t => t.IMPLEMENT_PHASE).HasColumnName("IMPLEMENT_PHASE");
            this.Property(t => t.PO_NUMBER).HasColumnName("PO_NUMBER");
            this.Property(t => t.AP_COMPANY).HasColumnName("AP_COMPANY");
            this.Property(t => t.AP_LOT).HasColumnName("AP_LOT");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.IMPLEMENT_DATE).HasColumnName("IMPLEMENT_DATE");
            this.Property(t => t.ON_SERVICE_DATE).HasColumnName("ON_SERVICE_DATE");

        }
    }
}
