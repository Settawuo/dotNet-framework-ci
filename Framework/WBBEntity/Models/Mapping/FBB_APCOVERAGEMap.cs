using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_APCOVERAGEMap : EntityTypeConfiguration<FBB_APCOVERAGE>
    {
        public FBB_APCOVERAGEMap()
        {
            // Primary Key
            this.HasKey(t => t.APPID);

            // Properties
            this.Property(t => t.BASEL2)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SITENAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SUB_DISTRICT)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DISTRICT)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PROVINCE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ZONE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LAT)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LNG)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.TOWER_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.TOWER_HEIGHT)
            .HasMaxLength(50);

            this.Property(t => t.VLAN)
                .HasMaxLength(50);

            this.Property(t => t.SUBNET_MASK_26)
                .HasMaxLength(50);

            this.Property(t => t.GATEWAY)
                .HasMaxLength(50);

            this.Property(t => t.AP_COMMENT)
                .HasMaxLength(100);

            this.Property(t => t.APPID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.COVERAGE_STATUS)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_APCOVERAGE", "WBB");
            this.Property(t => t.BASEL2).HasColumnName("BASEL2");
            this.Property(t => t.SITENAME).HasColumnName("SITENAME");
            this.Property(t => t.SUB_DISTRICT).HasColumnName("SUB_DISTRICT");
            this.Property(t => t.DISTRICT).HasColumnName("DISTRICT");
            this.Property(t => t.PROVINCE).HasColumnName("PROVINCE");
            this.Property(t => t.ZONE).HasColumnName("ZONE");
            this.Property(t => t.LAT).HasColumnName("LAT");
            this.Property(t => t.LNG).HasColumnName("LNG");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.VLAN).HasColumnName("VLAN");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.APPID).HasColumnName("APPID");

            this.Property(t => t.TOWER_TYPE).HasColumnName("TOWER_TYPE");
            this.Property(t => t.TOWER_HEIGHT).HasColumnName("TOWER_HEIGHT");
            this.Property(t => t.SUBNET_MASK_26).HasColumnName("SUBNET_MASK_26");
            this.Property(t => t.GATEWAY).HasColumnName("GATEWAY");
            this.Property(t => t.AP_COMMENT).HasColumnName("AP_COMMENT");
            this.Property(t => t.VLAN).HasColumnName("VLAN");
            this.Property(t => t.COVERAGE_STATUS).HasColumnName("COVERAGE_STATUS");
            this.Property(t => t.ONTARGET_DATE_IN).HasColumnName("ONTARGET_DATE_IN");
            this.Property(t => t.ONTARGET_DATE_EX).HasColumnName("ONTARGET_DATE_EX");
        }
    }
}
