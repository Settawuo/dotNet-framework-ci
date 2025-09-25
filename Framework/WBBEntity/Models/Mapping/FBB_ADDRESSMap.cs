using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_ADDRESSMap : EntityTypeConfiguration<FBB_ADDRESS>
    {
        public FBB_ADDRESSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ROW_ID, t.CREATED, t.CREATED_BY, t.LAST_UPD, t.LAST_UPD_BY });

            // Properties
            this.Property(t => t.ROW_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.LAST_UPD_BY)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.ENG_FLG)
                .HasMaxLength(1);

            this.Property(t => t.HOUSE_NO)
                .HasMaxLength(30);

            this.Property(t => t.SOI)
                .HasMaxLength(50);

            this.Property(t => t.MOO)
                .HasMaxLength(30);

            this.Property(t => t.MOOBAN)
                .HasMaxLength(50);

            this.Property(t => t.BUILDING_NAME)
                .HasMaxLength(80);

            this.Property(t => t.FLOOR)
                .HasMaxLength(30);

            this.Property(t => t.ROOM)
                .HasMaxLength(30);

            this.Property(t => t.STREET_NAME)
                .HasMaxLength(80);

            this.Property(t => t.ZIPCODE_ID)
                .HasMaxLength(50);

            this.Property(t => t.INSTALL_ADDRESS_1)
                .HasMaxLength(255);

            this.Property(t => t.INSTALL_ADDRESS_2)
                .HasMaxLength(255);

            this.Property(t => t.INSTALL_ADDRESS_3)
                .HasMaxLength(255);

            this.Property(t => t.INSTALL_ADDRESS_4)
                .HasMaxLength(255);

            this.Property(t => t.INSTALL_ADDRESS_5)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("FBB_ADDRESS", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.CREATED).HasColumnName("CREATED");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.LAST_UPD).HasColumnName("LAST_UPD");
            this.Property(t => t.LAST_UPD_BY).HasColumnName("LAST_UPD_BY");
            this.Property(t => t.ENG_FLG).HasColumnName("ENG_FLG");
            this.Property(t => t.HOUSE_NO).HasColumnName("HOUSE_NO");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.MOOBAN).HasColumnName("MOOBAN");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");
            this.Property(t => t.FLOOR).HasColumnName("FLOOR");
            this.Property(t => t.ROOM).HasColumnName("ROOM");
            this.Property(t => t.STREET_NAME).HasColumnName("STREET_NAME");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("ZIPCODE_ID");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("INSTALL_ADDRESS_1");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("INSTALL_ADDRESS_2");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("INSTALL_ADDRESS_3");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("INSTALL_ADDRESS_4");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("INSTALL_ADDRESS_5");

        }
    }
}
