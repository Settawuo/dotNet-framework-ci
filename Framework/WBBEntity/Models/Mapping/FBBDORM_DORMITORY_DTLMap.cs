using System.Data.Entity.ModelConfiguration;


namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_DORMITORY_DTLMap : EntityTypeConfiguration<FBBDORM_DORMITORY_DTL>
    {
        public FBBDORM_DORMITORY_DTLMap()
        {

            // Primary Key
            this.HasKey(t => new { t.PREPAID_NON_MOBILE });

            // Properties          

            this.Property(t => t.PIN_CODE)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.DORMITORY_ROW_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ROOM_NO)
                .HasMaxLength(50);

            this.Property(t => t.FLOOR_NO)
                .HasMaxLength(50);

            this.Property(t => t.IA_NO)
                .HasMaxLength(50);

            this.Property(t => t.PASSWORD)
                .HasMaxLength(50);

            this.Property(t => t.DSLAM_NO)
                .HasMaxLength(50);

            this.Property(t => t.DSLAM_PORT)
                .HasMaxLength(50);

            this.Property(t => t.SERVICE_STATUS)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.REFERENCE_NO)
               .HasMaxLength(100);

            this.Property(t => t.REFERENCE_PRE)
              .HasMaxLength(100);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBBDORM_DORMITORY_DTL", "WBB");
            this.Property(t => t.PREPAID_NON_MOBILE).HasColumnName("PREPAID_NON_MOBILE");
            this.Property(t => t.PIN_CODE).HasColumnName("PIN_CODE");
            this.Property(t => t.DORMITORY_ROW_ID).HasColumnName("DORMITORY_ROW_ID");
            this.Property(t => t.ROOM_NO).HasColumnName("ROOM_NO");
            this.Property(t => t.FLOOR_NO).HasColumnName("FLOOR_NO");
            this.Property(t => t.REFERENCE_PRE).HasColumnName("REFERENCE_PRE");
            this.Property(t => t.DSLAM_PORT).HasColumnName("DSLAM_PORT");
            this.Property(t => t.IA_NO).HasColumnName("IA_NO");

            this.Property(t => t.PASSWORD).HasColumnName("PASSWORD");
            this.Property(t => t.SERVICE_STATUS).HasColumnName("SERVICE_STATUS");
            this.Property(t => t.REFERENCE_NO).HasColumnName("REFERENCE_NO");
            this.Property(t => t.REGISTER_DATE).HasColumnName("REGISTER_DATE");
            this.Property(t => t.COMPLETE_DATE).HasColumnName("COMPLETE_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
