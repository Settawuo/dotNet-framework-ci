using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_PORT_NOTEMap : EntityTypeConfiguration<FBB_PORT_NOTE>
    {
        public FBB_PORT_NOTEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PORTID, t.NOTE, t.CREATED_BY, t.CREATED_DATE });

            // Properties
            //this.Property(t => t.PORTID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.NOTE)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.REF_KEY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_PORT_NOTE", "WBB");
            this.Property(t => t.PORTID).HasColumnName("PORTID");
            this.Property(t => t.NOTE).HasColumnName("NOTE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.REF_KEY).HasColumnName("REF_KEY");
        }
    }
}
