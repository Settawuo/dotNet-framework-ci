using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_DSLAMMODELMap : EntityTypeConfiguration<FBB_DSLAMMODEL>
    {
        public FBB_DSLAMMODELMap()
        {
            // Primary Key
            //this.HasKey(t => new { t.DSLAMMODELID, t.SLOTSTARTINDEX, t.MAXSLOT, t.CREATED_BY, t.CREATED_DATE, t.MODEL, t.BRAND });

            this.HasKey(t => t.DSLAMMODELID);
            // Properties
            //this.Property(t => t.DSLAMMODELID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //this.Property(t => t.SLOTSTARTINDEX)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //this.Property(t => t.MAXSLOT)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.MODEL)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BRAND)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SH_BRAND)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FBB_DSLAMMODEL", "WBB");
            this.Property(t => t.DSLAMMODELID).HasColumnName("DSLAMMODELID");
            this.Property(t => t.SLOTSTARTINDEX).HasColumnName("SLOTSTARTINDEX");
            this.Property(t => t.MAXSLOT).HasColumnName("MAXSLOT");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.MODEL).HasColumnName("MODEL");
            this.Property(t => t.BRAND).HasColumnName("BRAND");
            this.Property(t => t.SH_BRAND).HasColumnName("SH_BRAND");
        }
    }
}
