using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_EMAIL_PROCESSINGMap : EntityTypeConfiguration<FBB_EMAIL_PROCESSING>
    {
        public FBB_EMAIL_PROCESSINGMap()
        {
            this.HasKey(t => new
            {
                t.PROCESS_NAME,
                t.SEND_TO,
                //t.SEND_CC,
                //t.SEND_BCC,
                t.EFFECTIVE_DATE,
                //t.EXPIRE_DATE,
                t.CREATE_BY,
                t.CREATE_DATE,
                t.SEND_FROM,
                //t.ATTACHED_FILE,
                //t.IP_MAIL_SERVER,
            });

            // Properties
            this.Property(t => t.PROCESS_NAME)
                    .HasMaxLength(100);

            this.Property(t => t.SEND_TO)
                    .HasMaxLength(2000);

            this.Property(t => t.SEND_CC)
                .HasMaxLength(2000);

            this.Property(t => t.SEND_BCC)
                .HasMaxLength(2000);

            this.Property(t => t.CREATE_BY)
                .HasMaxLength(10);

            this.Property(t => t.SEND_FROM)
                .HasMaxLength(100);

            this.Property(t => t.SEND_FROM)
                .HasMaxLength(2000);

            this.Property(t => t.ATTACHED_FILE)
                .HasMaxLength(2000);

            this.Property(t => t.IP_MAIL_SERVER)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FBB_EMAIL_PROCESSING", "WBB");
            this.Property(t => t.PROCESS_NAME).HasColumnName("PROCESS_NAME");
            this.Property(t => t.SEND_TO).HasColumnName("SEND_TO");
            this.Property(t => t.SEND_CC).HasColumnName("SEND_CC");
            this.Property(t => t.SEND_BCC).HasColumnName("SEND_BCC");
            this.Property(t => t.EFFECTIVE_DATE).HasColumnName("EFFECTIVE_DATE");
            this.Property(t => t.EXPIRE_DATE).HasColumnName("EXPIRE_DATE");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.SEND_FROM).HasColumnName("SEND_FROM");
            this.Property(t => t.ATTACHED_FILE).HasColumnName("ATTACHED_FILE");
            this.Property(t => t.IP_MAIL_SERVER).HasColumnName("IP_MAIL_SERVER");
        }
    }
}
