using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_ZIPCODEMap : EntityTypeConfiguration<FBB_ZIPCODE>
    {
        public FBB_ZIPCODEMap()
        {
            // Primary Key
            this.HasKey(t => t.ZIPCODE_ID);

            // Properties
            this.Property(t => t.ZIPCODE_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ZIPCODE_ROWID)
                .HasMaxLength(50);

            this.Property(t => t.ZIPCODE)
                .HasMaxLength(5);

            this.Property(t => t.LANG_FLAG)
                .HasMaxLength(1);

            this.Property(t => t.TUMBON)
                .HasMaxLength(50);

            this.Property(t => t.AMPHUR)
                .HasMaxLength(50);

            this.Property(t => t.PROVINCE)
                .HasMaxLength(50);

            this.Property(t => t.COUNTRY)
                .HasMaxLength(50);

            this.Property(t => t.STATUS)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(15);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(15);

            this.Property(t => t.REGION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.GROUP_AMPHUR)
                .HasMaxLength(50);

            this.Property(t => t.SUB_REGION)
             .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FBB_ZIPCODE", "WBB");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("ZIPCODE_ID");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.ZIPCODE).HasColumnName("ZIPCODE");
            this.Property(t => t.LANG_FLAG).HasColumnName("LANG_FLAG");
            this.Property(t => t.TUMBON).HasColumnName("TUMBON");
            this.Property(t => t.AMPHUR).HasColumnName("AMPHUR");
            this.Property(t => t.PROVINCE).HasColumnName("PROVINCE");
            this.Property(t => t.COUNTRY).HasColumnName("COUNTRY");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.REGION_CODE).HasColumnName("REGION_CODE");
            this.Property(t => t.GROUP_AMPHUR).HasColumnName("GROUP_AMPHUR");
            this.Property(t => t.SUB_REGION).HasColumnName("SUB_REGION");
        }
    }
}
