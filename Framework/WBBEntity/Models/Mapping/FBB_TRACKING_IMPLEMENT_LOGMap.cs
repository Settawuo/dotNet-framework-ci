using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_TRACKING_IMPLEMENT_LOGMap : EntityTypeConfiguration<FBB_TRACKING_IMPLEMENT_LOG>
    {
        public FBB_TRACKING_IMPLEMENT_LOGMap()
        {
            this.HasKey(t => t.IDCARD);

            //this.Property(t => t.IDCARD)
            //  .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Properties
            this.Property(t => t.IDCARD)
                .HasMaxLength(30);

            this.Property(t => t.FIRSTNAME)
                .HasMaxLength(50);

            this.Property(t => t.LASTNAME)
                .HasMaxLength(50);

            this.Property(t => t.RESULT_CODE)
                .HasMaxLength(2);

            this.Property(t => t.RETURN_ORDER)
                .HasMaxLength(2000);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("FBB_TRACKING_IMPLEMENT_LOG", "WBB");
            this.Property(t => t.IDCARD).HasColumnName("IDCARD");
            this.Property(t => t.FIRSTNAME).HasColumnName("FIRSTNAME");
            this.Property(t => t.LASTNAME).HasColumnName("LASTNAME");
            this.Property(t => t.RESULT_CODE).HasColumnName("RESULT_CODE");
            this.Property(t => t.RETURN_ORDER).HasColumnName("RETURN_ORDER");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
        }
    }
}
