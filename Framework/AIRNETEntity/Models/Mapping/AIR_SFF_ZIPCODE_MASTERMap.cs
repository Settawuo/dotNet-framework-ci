using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SFF_ZIPCODE_MASTERMap : EntityTypeConfiguration<AIR_SFF_ZIPCODE_MASTER>
    {
        public AIR_SFF_ZIPCODE_MASTERMap()
        {

            // Primary Key
            this.HasKey(t => t.ZIPCODE_ROWID);
            // Properties
            this.Property(t => t.ZIPCODE_ROWID)
                        .HasMaxLength(50);
            this.Property(t => t.ZIPCODE)
                        .HasMaxLength(30);
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
            this.Property(t => t.UPD_BY)
                        .HasMaxLength(15);


            this.ToTable("AIR_SFF_ZIPCODE_MASTER", "AIR_ADMIN");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.ZIPCODE).HasColumnName("ZIPCODE");
            this.Property(t => t.LANG_FLAG).HasColumnName("LANG_FLAG");
            this.Property(t => t.TUMBON).HasColumnName("TUMBON");
            this.Property(t => t.AMPHUR).HasColumnName("AMPHUR");
            this.Property(t => t.PROVINCE).HasColumnName("PROVINCE");
            this.Property(t => t.COUNTRY).HasColumnName("COUNTRY");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");

        }
    }
}
