using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_DSLAM_INFOMap : EntityTypeConfiguration<FBB_DSLAM_INFO>
    {
        public FBB_DSLAM_INFOMap()
        {
            // Primary Key
            this.HasKey(t => t.DSLAMID);

            // Properties
            this.Property(t => t.DSLAMID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
               .HasMaxLength(50);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.NODEID)
                .HasMaxLength(50);

            this.Property(t => t.REGION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.LOT_NUMBER)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_DSLAM_INFO", "WBB");
            this.Property(t => t.DSLAMID).HasColumnName("DSLAMID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.DSLAMNUMBER).HasColumnName("DSLAMNUMBER");
            this.Property(t => t.DSLAMMODELID).HasColumnName("DSLAMMODELID");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.NODEID).HasColumnName("NODEID");
            this.Property(t => t.REGION_CODE).HasColumnName("REGION_CODE");
            this.Property(t => t.LOT_NUMBER).HasColumnName("LOT_NUMBER");
            this.Property(t => t.CVRID).HasColumnName("CVRID");
        }
    }
}
