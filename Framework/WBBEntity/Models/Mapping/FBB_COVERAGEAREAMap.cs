using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGEAREAMap : EntityTypeConfiguration<FBB_COVERAGEAREA>
    {
        public FBB_COVERAGEAREAMap()
        {
            // Primary Key
            this.HasKey(t => t.CVRID);

            // Properties
            this.Property(t => t.CVRID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.LOCATIONCODE)
                .HasMaxLength(50);

            this.Property(t => t.NODENAME_EN)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.NODENAME_TH)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.NODETYPE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NODESTATUS)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            //this.Property(t => t.ZIPCODE_ROWID_TH)
            //    .IsRequired()
            //    .HasMaxLength(50);

            //this.Property(t => t.ZIPCODE_ROWID_EN)
            //    .IsRequired()
            //    .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            //this.Property(t => t.REGION)
            //    .HasMaxLength(50);

            this.Property(t => t.BUILDINGCODE)
                .HasMaxLength(50);

            this.Property(t => t.IPRAN_CODE)
                .HasMaxLength(50);

            this.Property(t => t.CONTACT_NUMBER)
                .HasMaxLength(50);

            this.Property(t => t.FAX_NUMBER)
                .HasMaxLength(50);

            this.Property(t => t.REGION_CODE)
                .HasMaxLength(50);

            this.Property(t => t.LATITUDE)
                .HasMaxLength(20);

            this.Property(t => t.LONGITUDE)
                .HasMaxLength(20);

            this.Property(t => t.COMPLETE_FLAG)
                .HasMaxLength(1);


            this.Property(t => t.TIE_FLAG)
              .HasMaxLength(1);

            //this.Property(t => t.LATITUDE)
            //    .HasMaxLength(20);

            //this.Property(t => t.LONGITUDE)
            //    .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FBB_COVERAGEAREA", "WBB");
            this.Property(t => t.CVRID).HasColumnName("CVRID");
            this.Property(t => t.LOCATIONCODE).HasColumnName("LOCATIONCODE");
            this.Property(t => t.NODENAME_EN).HasColumnName("NODENAME_EN");
            this.Property(t => t.NODENAME_TH).HasColumnName("NODENAME_TH");
            this.Property(t => t.NODETYPE).HasColumnName("NODETYPE");
            this.Property(t => t.NODESTATUS).HasColumnName("NODESTATUS");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            //this.Property(t => t.ZIPCODE_ROWID_TH).HasColumnName("ZIPCODE_ROWID_TH");
            //this.Property(t => t.ZIPCODE_ROWID_EN).HasColumnName("ZIPCODE_ROWID_EN");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            //this.Property(t => t.LOCATIONCODE).HasColumnName("LOCATIONCODE");
            this.Property(t => t.BUILDINGCODE).HasColumnName("BUILDINGCODE");
            //this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            //this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.SOI_TH).HasColumnName("SOI_TH");
            this.Property(t => t.ROAD_TH).HasColumnName("ROAD_TH");
            this.Property(t => t.SOI_EN).HasColumnName("SOI_EN");
            this.Property(t => t.ROAD_EN).HasColumnName("ROAD_EN");
            this.Property(t => t.ZIPCODE).HasColumnName("ZIPCODE");
            this.Property(t => t.IPRAN_CODE).HasColumnName("IPRAN_CODE");
            this.Property(t => t.CONTACT_NUMBER).HasColumnName("CONTACT_NUMBER");
            this.Property(t => t.FAX_NUMBER).HasColumnName("FAX_NUMBER");
            this.Property(t => t.REGION_CODE).HasColumnName("REGION_CODE");
            this.Property(t => t.CONTACT_ID).HasColumnName("CONTACT_ID");
            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
            this.Property(t => t.COMPLETE_FLAG).HasColumnName("COMPLETE_FLAG");
            this.Property(t => t.TIE_FLAG).HasColumnName("TIE_FLAG");
            this.Property(t => t.ONTARGET_DATE_EX).HasColumnName("ONTARGET_DATE_EX");
            this.Property(t => t.ONTARGET_DATE_IN).HasColumnName("ONTARGET_DATE_IN");
        }
    }
}
