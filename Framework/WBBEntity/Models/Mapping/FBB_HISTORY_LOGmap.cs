using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_HISTORY_LOGmap : EntityTypeConfiguration<FBB_HISTORY_LOG>
    {
        public FBB_HISTORY_LOGmap()
        {
            // Primary Key
            this.HasKey(t => t.HISTORY_ID);

            this.Property(t => t.HISTORY_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties   
            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(2500);

            this.Property(t => t.ACTION)
               .HasMaxLength(50);

            this.Property(t => t.APPLICATION)
                .HasMaxLength(50);

            this.Property(t => t.REF_NAME)
                .HasMaxLength(50);

            this.Property(t => t.REF_KEY)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_HISTORY_LOG", "WBB");
            this.Property(t => t.HISTORY_ID).HasColumnName("HISTORY_ID");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
            this.Property(t => t.ACTION).HasColumnName("ACTION");
            this.Property(t => t.APPLICATION).HasColumnName("APPLICATION");
            this.Property(t => t.REF_NAME).HasColumnName("REF_NAME");
            this.Property(t => t.REF_KEY).HasColumnName("REF_KEY");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
        }
    }
}
