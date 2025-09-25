using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_TITLE_MASTERMap : EntityTypeConfiguration<FBB_TITLE_MASTER>
    {
        public FBB_TITLE_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TITLE_CODE, t.TITLE_DESC, t.STATUS, t.UPD_DTM, t.CUSTOMER_TYPE });

            // Properties
            //this.Property(t => t.TITLE_ROWID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TITLE_CODE)
                .HasMaxLength(3);

            this.Property(t => t.TITLE_DESC)
                .HasMaxLength(50);

            this.Property(t => t.STATUS)
                .HasMaxLength(1);

            this.Property(t => t.UPD_BY)
                .HasMaxLength(15);

            this.Property(t => t.CUSTOMER_TYPE)
                .HasMaxLength(10);

            this.Property(t => t.FIRST_TITLE)
                .HasMaxLength(100);

            this.Property(t => t.LAST_TITLE)
                .HasMaxLength(100);

            this.Property(t => t.STATUS)
                .HasMaxLength(1);

            this.Property(t => t.ENG_FLAG)
                .HasMaxLength(1);

            this.Property(t => t.DEFAULT_VALUE_TH)
                .HasMaxLength(10);

            this.Property(t => t.DEFAULT_VALUE_EN)
               .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("FBB_TITLE_MASTER", "WBB");
            this.Property(t => t.TITLE_CODE).HasColumnName("TITLE_CODE");
            this.Property(t => t.TITLE_DESC).HasColumnName("TITLE_DESC");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.TITLE_ROWID).HasColumnName("TITLE_ROWID");
            this.Property(t => t.CUSTOMER_TYPE).HasColumnName("CUSTOMER_TYPE");
            this.Property(t => t.FIRST_TITLE).HasColumnName("FIRST_TITLE");
            this.Property(t => t.LAST_TITLE).HasColumnName("LAST_TITLE");
            this.Property(t => t.ENG_FLAG).HasColumnName("ENG_FLAG");
            this.Property(t => t.DEFAULT_VALUE_TH).HasColumnName("DEFAULT_VALUE_TH");
            this.Property(t => t.DEFAULT_VALUE_EN).HasColumnName("DEFAULT_VALUE_EN");
        }
    }
}
