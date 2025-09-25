using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SENDMAIL_LOGMap : EntityTypeConfiguration<FBB_SENDMAIL_LOG>
    {
        public FBB_SENDMAIL_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.RUNNING_NO);

            // Properties
            this.Property(t => t.RUNNING_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CUST_ROW_ID)
                //.IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PROCESS_NAME)
                //.IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.RETURN_CODE)
                .HasMaxLength(50);

            this.Property(t => t.RETURN_DESC)
                .HasMaxLength(500);

            this.Property(t => t.CREATE_USER)
                //.IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.CREATE_USER)
                //.IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.FILE_NAME)
                //.IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("FBB_SENDMAIL_LOG", "WBB");
            this.Property(t => t.CUST_ROW_ID).HasColumnName("CUST_ROW_ID");
            this.Property(t => t.PROCESS_NAME).HasColumnName("PROCESS_NAME");
            this.Property(t => t.RETURN_CODE).HasColumnName("RETURN_CODE");
            this.Property(t => t.RETURN_DESC).HasColumnName("RETURN_DESC");
            this.Property(t => t.CREATE_USER).HasColumnName("CREATE_USER");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.RUNNING_NO).HasColumnName("RUNNING_NO");
            this.Property(t => t.FILE_NAME).HasColumnName("FILE_NAME");
        }
    }
}
