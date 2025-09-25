using System.Data.Entity.ModelConfiguration;

namespace AIRNETEntity.Models.Mapping
{
    public class AIR_SFF_PROVINCE_MASTERMap : EntityTypeConfiguration<AIR_SFF_PROVINCE_MASTER>
    {
        public AIR_SFF_PROVINCE_MASTERMap()
        {

            // Primary Key
            this.HasKey(t => t.PROVINCE_ROWID);
            // Properties

            this.Property(t => t.PROVINCE_ROWID)
                                    .HasMaxLength(50);
            this.Property(t => t.PROVINCE_CODE)
                                    .HasMaxLength(30);
            this.Property(t => t.PROVINCE_DESC)
                                    .HasMaxLength(30);
            this.Property(t => t.STATUS)
                                    .HasMaxLength(1);
            this.Property(t => t.UPD_BY)
                                    .HasMaxLength(15);
            this.Property(t => t.REGION)
                                    .HasMaxLength(10);



            this.ToTable("AIR_SFF_PROVINCE_MASTER", "AIR_ADMIN");
            this.Property(t => t.PROVINCE_ROWID).HasColumnName("PROVINCE_ROWID");
            this.Property(t => t.PROVINCE_CODE).HasColumnName("PROVINCE_CODE");
            this.Property(t => t.PROVINCE_DESC).HasColumnName("PROVINCE_DESC");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.UPD_DTM).HasColumnName("UPD_DTM");
            this.Property(t => t.UPD_BY).HasColumnName("UPD_BY");
            this.Property(t => t.REGION).HasColumnName("REGION");


        }
    }
}
