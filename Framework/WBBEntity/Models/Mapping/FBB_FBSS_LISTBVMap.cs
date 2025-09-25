using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_FBSS_LISTBVMap : EntityTypeConfiguration<FBB_FBSS_LISTBV>
    {
        public FBB_FBSS_LISTBVMap()
        {
            this.HasKey(t => t.LISTBV_ID);

            this.Property(t => t.LISTBV_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.LANGUAGE)
                .HasMaxLength(5);

            this.Property(t => t.ADDRESS_TYPE)
                .HasMaxLength(5);

            this.Property(t => t.POSTAL_CODE)
                .HasMaxLength(10);

            this.Property(t => t.BUILDING_NAME)
                .HasMaxLength(250);

            this.Property(t => t.BUILDING_NO)
                .HasMaxLength(20);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(5);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_DATE)
                .IsRequired();

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.ADDRESS_ID)
                .HasMaxLength(100);

            this.Property(t => t.SUB_DISTRICT)
               .HasMaxLength(100);

            this.Property(t => t.SITE_CODE)
              .HasMaxLength(100);

            this.Property(t => t.ACCESS_MODE)
               .HasMaxLength(100);

            this.Property(t => t.PARTNER)
               .HasMaxLength(100);

            this.Property(t => t.LATITUDE)
               .HasMaxLength(100);

            this.Property(t => t.LONGTITUDE)
               .HasMaxLength(100);

            this.Property(t => t.FTTR_FLAG)
                .HasMaxLength(50);

            this.Property(t => t.SPECIFIC_TEAM_1)
                .HasMaxLength(4000);

            this.Property(t => t.SPECIFIC_TEAM_2)
                .HasMaxLength(4000);

            this.Property(t => t.FTTR_TYPE)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("FBB_FBSS_LISTBV", "WBB");
            this.Property(t => t.LISTBV_ID).HasColumnName("LISTBV_ID");
            this.Property(t => t.LANGUAGE).HasColumnName("LANGUAGE");
            this.Property(t => t.ADDRESS_TYPE).HasColumnName("ADDRESS_TYPE");
            this.Property(t => t.POSTAL_CODE).HasColumnName("POSTAL_CODE");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.SUB_DISTRICT).HasColumnName("SUB_DISTRICT");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.SITE_CODE).HasColumnName("SITE_CODE");
            this.Property(t => t.ACCESS_MODE).HasColumnName("ACCESS_MODE");
            this.Property(t => t.PARTNER).HasColumnName("PARTNER");
            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGTITUDE).HasColumnName("LONGTITUDE");
            this.Property(t => t.FTTR_FLAG).HasColumnName("FTTR_FLAG");
            this.Property(t => t.SPECIFIC_TEAM_1).HasColumnName("SPECIFIC_TEAM_1");
            this.Property(t => t.SPECIFIC_TEAM_2).HasColumnName("SPECIFIC_TEAM_2");
            this.Property(t => t.FTTR_TYPE).HasColumnName("FTTR_TYPE");

        }
    }
}