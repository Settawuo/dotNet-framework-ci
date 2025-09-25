using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGEAREA_RELATIONMap : EntityTypeConfiguration<FBB_COVERAGEAREA_RELATION>
    {
        public FBB_COVERAGEAREA_RELATIONMap()
        {
            // Primary Key
            this.HasKey(t => t.CVRRELATIONID);

            // Properties
            this.Property(t => t.CVRRELATIONID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TOWERNAME_EN)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TOWERNAME_TH)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.LATITUDE)
                .HasMaxLength(20);

            this.Property(t => t.LONGITUDE)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FBB_COVERAGEAREA_RELATION", "WBB");
            this.Property(t => t.CVRRELATIONID).HasColumnName("CVRRELATIONID");
            this.Property(t => t.CVRID).HasColumnName("CVRID");
            this.Property(t => t.DSLAMID).HasColumnName("DSLAMID");
            this.Property(t => t.TOWERNAME_EN).HasColumnName("TOWERNAME_EN");
            this.Property(t => t.TOWERNAME_TH).HasColumnName("TOWERNAME_TH");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");

            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
        }
    }
}
