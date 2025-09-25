using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGEAREA_RESULTMap : EntityTypeConfiguration<FBB_COVERAGEAREA_RESULT>
    {
        public FBB_COVERAGEAREA_RESULTMap()
        {
            // Primary Key
            this.HasKey(t => t.RESULTID);

            // Properties
            this.Property(t => t.RESULTID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CVRID)
                .IsRequired();

            this.Property(t => t.NODENAME)
                //.IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.TOWER)
                .HasMaxLength(50);

            this.Property(t => t.PREFIXNAME)
                .HasMaxLength(10);

            this.Property(t => t.FIRSTNAME)
                .HasMaxLength(50);

            this.Property(t => t.LASTNAME)
                .HasMaxLength(50);

            this.Property(t => t.CONTACTNUMBER)
                .HasMaxLength(50);

            this.Property(t => t.ISONLINENUMBER)
                .HasMaxLength(1);

            this.Property(t => t.ADDRESS_NO)
                //.IsRequired()
                .HasMaxLength(20);

            //this.Property(t => t.MOO)
            //.IsRequired();

            this.Property(t => t.SOI)
                //.IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ROAD)
                //.IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.COVERAGETYPE)
                //.IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.COVERAGERESULT)
                .HasMaxLength(1);

            this.Property(t => t.LATITUDE)
                .HasMaxLength(20);

            this.Property(t => t.LONGITUDE)
                .HasMaxLength(20);

            this.Property(t => t.PRODUCTTYPE)
                //.IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ZIPCODE_ROWID)
                //.IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_DATE)
                .IsRequired();

            this.Property(t => t.RETURN_MESSAGE)
                .HasMaxLength(500);

            this.Property(t => t.RETURN_ORDER)
                .HasMaxLength(30);

            this.Property(t => t.OWNER_PRODUCT)
                .HasMaxLength(20);

            //this.Property(t => t.SELECTPRODUCT)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_COVERAGEAREA_RESULT", "WBB");
            this.Property(t => t.CVRID).HasColumnName("CVRID");
            this.Property(t => t.NODENAME).HasColumnName("NODENAME");
            this.Property(t => t.TOWER).HasColumnName("TOWER");
            this.Property(t => t.PREFIXNAME).HasColumnName("PREFIXNAME");
            this.Property(t => t.FIRSTNAME).HasColumnName("FIRSTNAME");
            this.Property(t => t.LASTNAME).HasColumnName("LASTNAME");
            this.Property(t => t.CONTACTNUMBER).HasColumnName("CONTACTNUMBER");
            this.Property(t => t.FLOOR).HasColumnName("FLOOR");
            this.Property(t => t.ISONLINENUMBER).HasColumnName("ISONLINENUMBER");
            this.Property(t => t.ADDRESS_NO).HasColumnName("ADDRESS_NO");
            this.Property(t => t.MOO).HasColumnName("MOO");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.ROAD).HasColumnName("ROAD");
            this.Property(t => t.COVERAGETYPE).HasColumnName("COVERAGETYPE");
            this.Property(t => t.COVERAGERESULT).HasColumnName("COVERAGERESULT");
            this.Property(t => t.LATITUDE).HasColumnName("LATITUDE");
            this.Property(t => t.LONGITUDE).HasColumnName("LONGITUDE");
            this.Property(t => t.PRODUCTTYPE).HasColumnName("PRODUCTTYPE");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            //this.Property(t => t.SELECTPRODUCT).HasColumnName("SELECTPRODUCT");
            this.Property(t => t.RESULTID).HasColumnName("RESULTID");
            this.Property(t => t.RETURN_CODE).HasColumnName("RETURN_CODE");
            this.Property(t => t.RETURN_MESSAGE).HasColumnName("RETURN_MESSAGE");
            this.Property(t => t.RETURN_ORDER).HasColumnName("RETURN_ORDER");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");

            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
        }
    }
}
