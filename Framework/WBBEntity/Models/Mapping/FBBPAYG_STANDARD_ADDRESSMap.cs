using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBPAYG_STANDARD_ADDRESSMap : EntityTypeConfiguration<FBBPAYG_STANDARD_ADDRESS>
    {
        public FBBPAYG_STANDARD_ADDRESSMap()
        {
            // Primary Key
            this.HasKey(t => t.ROW_ID);

            // Properties
            this.Property(t => t.ADDRESS_ID)
                .HasMaxLength(30);

            this.Property(t => t.SERVICE_TYPE)
                .HasMaxLength(250);

            this.Property(t => t.OWNER)
                .HasMaxLength(30);

            this.Property(t => t.POST_CODE)
                .HasMaxLength(30);

            this.Property(t => t.BUILDING_NAME_TH)
                .HasMaxLength(250);

            this.Property(t => t.BUILDING_NAME_EN)
                .HasMaxLength(250);

            this.Property(t => t.TYPE)
                .HasMaxLength(10);

            this.Property(t => t.BUILDING_NO_EN)
                .HasMaxLength(250);

            this.Property(t => t.BUILDING_NO_TH)
                .HasMaxLength(250);

            this.Property(t => t.COUNTRY)
                .HasMaxLength(80);

            this.Property(t => t.REGION)
                .HasMaxLength(80);

            this.Property(t => t.PROVINCE_TH)
                .HasMaxLength(250);

            this.Property(t => t.PROVINCE_EN)
                .HasMaxLength(250);

            this.Property(t => t.DISTRICT_TH)
                .HasMaxLength(250);

            this.Property(t => t.DISTRICT_EN)
                .HasMaxLength(80);

            this.Property(t => t.SUB_DISTRICT_TH)
                .HasMaxLength(250);

            this.Property(t => t.SUB_DISTRICT_EN)
                .HasMaxLength(80);

            this.Property(t => t.STATION_ID)
                .HasMaxLength(10);

            this.Property(t => t.STATION_NO)
                .HasMaxLength(50);

            this.Property(t => t.AREA_ID_NEW)
                .HasMaxLength(30);

            this.Property(t => t.SITE_ADDRESS)
                .HasMaxLength(500);

            this.Property(t => t.NETWORK_RESOURCE_ID)
                .HasMaxLength(2000);

            this.Property(t => t.TECHNOLOGY)
                .HasMaxLength(30);

            this.Property(t => t.ACTION_FLAG)
                .HasMaxLength(250);


            this.ToTable("FBBPAYG_STANDARD_ADDRESS", "WBB");
            this.Property(t => t.ADDRESS_ID).HasColumnName("ADDRESS_ID");
            this.Property(t => t.SERVICE_TYPE).HasColumnName("SERVICE_TYPE");
            this.Property(t => t.OWNER).HasColumnName("OwbbWNER");
            this.Property(t => t.POST_CODE).HasColumnName("POST_CODE");
            this.Property(t => t.BUILDING_NAME_TH).HasColumnName("BUILDING_NAME_TH");
            this.Property(t => t.BUILDING_NAME_EN).HasColumnName("BUILDING_NAME_EN");
            this.Property(t => t.TYPE).HasColumnName("TYPE");
            this.Property(t => t.BUILDING_NO_EN).HasColumnName("BUILDING_NO_EN");
            this.Property(t => t.BUILDING_NO_TH).HasColumnName("BUILDING_NO_TH");
            this.Property(t => t.COUNTRY).HasColumnName("COUNTRY");
            this.Property(t => t.REGION).HasColumnName("REGION");
            this.Property(t => t.PROVINCE_TH).HasColumnName("PROVINCE_TH");
            this.Property(t => t.PROVINCE_EN).HasColumnName("PROVINCE_EN");
            this.Property(t => t.DISTRICT_TH).HasColumnName("DISTRICT_TH");
            this.Property(t => t.DISTRICT_EN).HasColumnName("DISTRICT_EN");
            this.Property(t => t.SUB_DISTRICT_TH).HasColumnName("SUB_DISTRICT_TH");
            this.Property(t => t.SUB_DISTRICT_EN).HasColumnName("SUB_DISTRICT_EN");
            this.Property(t => t.STATION_ID).HasColumnName("STATION_ID");
            this.Property(t => t.STATION_NO).HasColumnName("STATION_NO");
            this.Property(t => t.AREA_ID_NEW).HasColumnName("AREA_ID_NEW");
            this.Property(t => t.SITE_ADDRESS).HasColumnName("SITE_ADDRESS");
            this.Property(t => t.NETWORK_RESOURCE_ID).HasColumnName("NETWORK_RESOURCE_ID");
            this.Property(t => t.TECHNOLOGY).HasColumnName("TECHNOLOGY");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.ACTION_FLAG).HasColumnName("ACTION_FLAG");
        }
    }
}
