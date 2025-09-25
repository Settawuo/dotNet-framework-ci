using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_EMAIL_TRANSACTIONMap : EntityTypeConfiguration<FBB_EMAIL_TRANSACTION>
    {
        public FBB_EMAIL_TRANSACTIONMap()
        {
            this.HasKey(t => new
            {
                t.PROCESS_NAME
            });


            // Properties
            this.Property(t => t.PROCESS_NAME).HasMaxLength(100);
            this.Property(t => t.EMAIL_TO).HasMaxLength(2000);
            this.Property(t => t.EMAIL_CONTENT).HasMaxLength(4000);
            this.Property(t => t.EMAIL_ATTACH).HasMaxLength(2000);
            this.Property(t => t.STATUS).HasMaxLength(10);
            this.Property(t => t.STATUS_DESC).HasMaxLength(4000);
            this.Property(t => t.CREATE_BY).HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("FBB_EMAIL_TRANSACTION", "WBB");
            this.Property(t => t.PROCESS_NAME).HasColumnName("PROCESS_NAME");
            this.Property(t => t.EMAIL_TO).HasColumnName("EMAIL_TO");
            this.Property(t => t.EMAIL_CONTENT).HasColumnName("EMAIL_CONTENT");
            this.Property(t => t.EMAIL_ATTACH).HasColumnName("EMAIL_ATTACH");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATUS_DESC).HasColumnName("STATUS_DESC");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
        }
    }
}
