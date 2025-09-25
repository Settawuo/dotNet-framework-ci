using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CFG_REPORTMap : EntityTypeConfiguration<FBB_CFG_REPORT>
    {
        public FBB_CFG_REPORTMap()
        {
            // Primary Key
            this.HasKey(t => t.REPORT_ID);

            // Properties
            this.Property(t => t.REPORT_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
               .HasMaxLength(50);

            this.Property(t => t.REPORT_NAME)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.MAIL_TYPE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DAY_OF_WEEK)
                .HasMaxLength(5);

            this.Property(t => t.EMAIL_TO)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.EMAIL_FROM)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.EMAIL_CC)
                .HasMaxLength(1000);

            this.Property(t => t.EMAIL_SUBJECT)
                .IsRequired()
                .HasMaxLength(2000);

            this.Property(t => t.EMAIL_CONTENT)
                .HasMaxLength(4000);

            this.Property(t => t.EMAIL_TO_NOTIFICATION)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.ACTIVE_FLAG)
             .IsRequired()
             .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("FBB_CFG_REPORT", "WBB");
            this.Property(t => t.REPORT_ID).HasColumnName("REPORT_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.REPORT_NAME).HasColumnName("REPORT_NAME");
            this.Property(t => t.MAIL_TYPE).HasColumnName("MAIL_TYPE");
            this.Property(t => t.DAY_OF_WEEK).HasColumnName("DAY_OF_WEEK");
            this.Property(t => t.EMAIL_TO).HasColumnName("EMAIL_TO");
            this.Property(t => t.EMAIL_FROM).HasColumnName("EMAIL_FROM");
            this.Property(t => t.EMAIL_CC).HasColumnName("EMAIL_CC");
            this.Property(t => t.EMAIL_SUBJECT).HasColumnName("EMAIL_SUBJECT");
            this.Property(t => t.EMAIL_CONTENT).HasColumnName("EMAIL_CONTENT");
            this.Property(t => t.EMAIL_TO_NOTIFICATION).HasColumnName("EMAIL_TO_NOTIFICATION");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
        }
    }
}
