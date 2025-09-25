using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_PORT_INFOMap : EntityTypeConfiguration<FBB_PORT_INFO>
    {
        public FBB_PORT_INFOMap()
        {
            // Primary Key
            this.HasKey(t => t.PORTID);

            // Properties
            this.Property(t => t.PORTID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PORTTYPE)
                .HasMaxLength(10);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_PORT_INFO", "WBB");
            this.Property(t => t.PORTID).HasColumnName("PORTID");
            this.Property(t => t.CARDID).HasColumnName("CARDID");
            this.Property(t => t.PORTNUMBER).HasColumnName("PORTNUMBER");
            this.Property(t => t.PORTSTATUSID).HasColumnName("PORTSTATUSID");
            this.Property(t => t.PORTTYPE).HasColumnName("PORTTYPE");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
