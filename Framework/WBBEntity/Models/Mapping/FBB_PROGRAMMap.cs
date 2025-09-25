using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_PROGRAMMap : EntityTypeConfiguration<FBB_PROGRAM>
    {
        public FBB_PROGRAMMap()
        {
            this.HasKey(t => t.PROGRAM_ID);

            this.Property(t => t.PROGRAM_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PROGRAM_CODE)
                .HasMaxLength(50);

            this.Property(t => t.PROGRAM_NAME)
                .HasMaxLength(100);

            this.Property(t => t.PROGRAM_DESCRIPTION)
                .HasMaxLength(100);

            this.Property(t => t.ACTIVE_FLG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_PROGRAM", "WBB");
            this.Property(t => t.PROGRAM_ID).HasColumnName("PROGRAM_ID");
            this.Property(t => t.PROGRAM_CODE).HasColumnName("PROGRAM_CODE");
            this.Property(t => t.PROGRAM_NAME).HasColumnName("PROGRAM_NAME");
            this.Property(t => t.PROGRAM_DESCRIPTION).HasColumnName("PROGRAM_DESCRIPTION");
            this.Property(t => t.ACTIVE_FLG).HasColumnName("ACTIVE_FLG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.PARENT_ID).HasColumnName("PARENT_ID");
            this.Property(t => t.ORDER_BY).HasColumnName("ORDER_BY");
        }

    }
}
