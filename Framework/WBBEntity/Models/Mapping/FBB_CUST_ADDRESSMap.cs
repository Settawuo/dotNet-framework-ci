using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CUST_ADDRESSMap : EntityTypeConfiguration<FBB_CUST_ADDRESS>
    {
        // Primary Key
        public FBB_CUST_ADDRESSMap()
        {
            this.HasKey(t => t.ROW_ID);

            this.Property(t => t.ROW_ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.HOUSE_NO).HasMaxLength(30);
            this.Property(t => t.SOI).HasMaxLength(50);
            this.Property(t => t.MOO).HasMaxLength(30);
            this.Property(t => t.MOOBAN).HasMaxLength(50);
            this.Property(t => t.BUILDING_NAME).HasMaxLength(80);
            this.Property(t => t.FLOOR).HasMaxLength(250);
            this.Property(t => t.ROOM).HasMaxLength(250);
            this.Property(t => t.STREET_NAME).HasMaxLength(250);
            this.Property(t => t.ZIPCODE_ID).HasMaxLength(250);
            this.Property(t => t.CREATED_BY).HasMaxLength(250);
            this.Property(t => t.UPDATED_BY).HasMaxLength(250);
            this.Property(t => t.BA_ID).HasMaxLength(50);
            this.Property(t => t.CA_ID).HasMaxLength(50);

            this.ToTable("FBB_CUST_ADDRESS", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.HOUSE_NO).HasColumnName("HOUSE_NO");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.MOOBAN).HasColumnName("MOOBAN");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");
            this.Property(t => t.FLOOR).HasColumnName("FLOOR");
            this.Property(t => t.ROOM).HasColumnName("ROOM");
            this.Property(t => t.STREET_NAME).HasColumnName("STREET_NAME");
            this.Property(t => t.ZIPCODE_ID).HasColumnName("ZIPCODE_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.CA_ID).HasColumnName("CA_ID");
            this.Property(t => t.BA_ID).HasColumnName("BA_ID");
        }
    }
}
