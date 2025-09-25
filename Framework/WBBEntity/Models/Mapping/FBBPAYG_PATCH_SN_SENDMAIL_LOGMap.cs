using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_PATCH_SN_SENDMAIL_LOGMap : EntityTypeConfiguration<FBBPAYG_PATCH_SN_SENDMAIL_LOG>
    {
        public FBBPAYG_PATCH_SN_SENDMAIL_LOGMap()
        {
            this.HasKey(t => new { t.FILE_NAME });
            this.Property(t => t.FILE_NAME)
                .HasMaxLength(200);
            this.Property(t => t.MAIL_TO)
                .HasMaxLength(1000);
            this.Property(t => t.MAIL_CONTENT)
                .HasMaxLength(4000);
            this.Property(t => t.MAIL_STATUS)
                .HasMaxLength(50);
            this.Property(t => t.CREATE_BY)
                .HasMaxLength(20);
            this.Property(t => t.UPDATE_BY)
                .HasMaxLength(20);
            this.Property(t => t.ERROR_MESSAGE)
                .HasMaxLength(2000);

            this.ToTable("FBBPAYG_PATCH_SN_SENDMAIL_LOG", "WBB");
            this.Property(t => t.FILE_NAME).HasColumnName("FILE_NAME");
            this.Property(t => t.MAIL_TO).HasColumnName("MAIL_TO");
            this.Property(t => t.MAIL_CONTENT).HasColumnName("MAIL_CONTENT");
            this.Property(t => t.MAIL_STATUS).HasColumnName("MAIL_STATUS");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.UPDATE_DATE).HasColumnName("UPDATE_DATE");
            this.Property(t => t.UPDATE_BY).HasColumnName("UPDATE_BY");
            this.Property(t => t.ERROR_MESSAGE).HasColumnName("ERROR_MESSAGE");

        }
    }
}
