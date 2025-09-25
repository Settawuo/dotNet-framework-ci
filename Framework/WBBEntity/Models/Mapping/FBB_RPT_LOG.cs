using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_RPT_LOGMap : EntityTypeConfiguration<FBB_RPT_LOG>
    {
        public FBB_RPT_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.REPORT_ID);

            // Properties
            this.Property(t => t.REPORT_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.REPORT_CODE)
                            .HasMaxLength(100);
            this.Property(t => t.REPORT_NAME)
                            .HasMaxLength(100);
            this.Property(t => t.REPORT_CONDITION)
                            .HasMaxLength(50);
            this.Property(t => t.REPORT_DESC)
                            .HasMaxLength(100);
            this.Property(t => t.CREATED_BY)
                            .HasMaxLength(50);
            this.Property(t => t.UPDATED_BY)
                            .HasMaxLength(50);
            this.Property(t => t.FILE_PATH)
                           .HasMaxLength(500);
            this.Property(t => t.REPORT_STATUS)
                            .HasMaxLength(50);
            this.Property(t => t.REPORT_STATUS_DESC)
                            .HasMaxLength(1000);
            this.Property(t => t.REPORT_PARAMETER)
                            .HasMaxLength(1000);
            this.Property(t => t.FILE_NAME)
                            .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("FBB_RPT_LOG", "WBB");

            this.Property(t => t.REPORT_ID).HasColumnName("REPORT_ID");
            this.Property(t => t.REPORT_CODE).HasColumnName("REPORT_CODE");
            this.Property(t => t.REPORT_NAME).HasColumnName("REPORT_NAME");
            this.Property(t => t.REPORT_CONDITION).HasColumnName("REPORT_CONDITION");
            this.Property(t => t.REPORT_DESC).HasColumnName("REPORT_DESC");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.FILE_PATH).HasColumnName("FILE_PATH");
            this.Property(t => t.REPORT_STATUS).HasColumnName("REPORT_STATUS");
            this.Property(t => t.REPORT_STATUS_DESC).HasColumnName("REPORT_STATUS_DESC");
            this.Property(t => t.REPORT_PARAMETER).HasColumnName("REPORT_PARAMETER");
            this.Property(t => t.FILE_NAME).HasColumnName("FILE_NAME");

        }
    }
}
