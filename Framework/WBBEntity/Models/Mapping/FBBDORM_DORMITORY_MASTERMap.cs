using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_DORMITORY_MASTERMap : EntityTypeConfiguration<FBBDORM_DORMITORY_MASTER>
    {
        public FBBDORM_DORMITORY_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => new { t.DORMITORY_ROW_ID });

            // Properties
            this.Property(t => t.DORMITORY_ROW_ID)
              .IsRequired()
              .HasMaxLength(50);

            this.Property(t => t.ADDRESS_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DORMITORY_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DORMITORY_NAME_TH)
                .HasMaxLength(1000);

            this.Property(t => t.DORMITORY_NO_TH)
                .HasMaxLength(100);

            this.Property(t => t.DORMITORY_NO_EN)
                .HasMaxLength(100);


            this.Property(t => t.NUMBER_OF_ROOM)
                .HasMaxLength(50);

            this.Property(t => t.STATE)
                .HasMaxLength(50);

            this.Property(t => t.HOME_NO_TH)
              .HasMaxLength(50);

            this.Property(t => t.HOME_NO_EN)
             .HasMaxLength(50);

            this.Property(t => t.MOO_TH)
                .HasMaxLength(50);

            this.Property(t => t.MOO_EN)
            .HasMaxLength(50);

            this.Property(t => t.SOI_TH)
              .HasMaxLength(50);

            this.Property(t => t.SOI_EN)
             .HasMaxLength(50);

            this.Property(t => t.STREET_NAME_TH)
               .HasMaxLength(50);

            this.Property(t => t.STREET_NAME_EN)
               .HasMaxLength(50);

            this.Property(t => t.ZIPCODE_ROW_ID_TH)
              .HasMaxLength(50);

            this.Property(t => t.ZIPCODE_ROW_ID_EN)
              .HasMaxLength(50);

            this.Property(t => t.MKT_OWNER)
                .HasMaxLength(100);

            this.Property(t => t.SUB_CONTRACT_LOCATION_CODE)
              .HasMaxLength(100);

            this.Property(t => t.SUB_CONTRACT_NAME_TH)
                .HasMaxLength(100);

            this.Property(t => t.SUB_CONTRACT_NAME_EN)
                .HasMaxLength(100);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
              .HasMaxLength(50);

            this.Property(t => t.DORMITORY_NAME_EN)
                .HasMaxLength(1000);


            // Table & Column Mappings
            this.ToTable("FBBDORM_DORMITORY_MASTER", "WBB");
            this.Property(t => t.DORMITORY_ROW_ID).HasColumnName("DORMITORY_ROW_ID");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.DORMITORY_ID).HasColumnName("DORMITORY_ID");
            this.Property(t => t.DORMITORY_NAME_TH).HasColumnName("DORMITORY_NAME_TH");
            this.Property(t => t.NUMBER_OF_ROOM).HasColumnName("NUMBER_OF_ROOM");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.HOME_NO_TH).HasColumnName("HOME_NO_TH");
            this.Property(t => t.HOME_NO_EN).HasColumnName("HOME_NO_EN");
            this.Property(t => t.MOO_TH).HasColumnName("MOO_TH");
            this.Property(t => t.MOO_EN).HasColumnName("MOO_EN");
            this.Property(t => t.SOI_TH).HasColumnName("SOI_TH");
            this.Property(t => t.SOI_EN).HasColumnName("SOI_EN");
            this.Property(t => t.STREET_NAME_TH).HasColumnName("STREET_NAME_TH");
            this.Property(t => t.STREET_NAME_EN).HasColumnName("STREET_NAME_EN");
            this.Property(t => t.ZIPCODE_ROW_ID_TH).HasColumnName("ZIPCODE_ROW_ID_TH");
            this.Property(t => t.ZIPCODE_ROW_ID_EN).HasColumnName("ZIPCODE_ROW_ID_EN");
            this.Property(t => t.MKT_OWNER).HasColumnName("MKT_OWNER");
            this.Property(t => t.SUB_CONTRACT_LOCATION_CODE).HasColumnName("SUB_CONTRACT_LOCATION_CODE");
            this.Property(t => t.SUB_CONTRACT_NAME_TH).HasColumnName("SUB_CONTRACT_NAME_TH");
            this.Property(t => t.SUB_CONTRACT_NAME_EN).HasColumnName("SUB_CONTRACT_NAME_EN");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_BY).HasColumnName("CREATED_DATE");
            this.Property(t => t.CREATED_DATE).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.DORMITORY_NAME_EN).HasColumnName("DORMITORY_NAME_EN");
        }
    }
}
