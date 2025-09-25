using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_VSMP_LOGMap : EntityTypeConfiguration<FBB_VSMP_LOG>
    {


        public FBB_VSMP_LOGMap()
        {
            // Primary Key

            this.HasKey(t => t.VSMP_ID);

            this.Property(t => t.VSMP_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            // Properties
            this.Property(t => t.ORDER_REF)
             .HasMaxLength(20);

            this.Property(t => t.MOBILE_NO)
             .HasMaxLength(11);

            this.Property(t => t.USER_NAME)
             .HasMaxLength(30);

            this.Property(t => t.ORDER_DESC)
                .HasMaxLength(30);

            this.Property(t => t.VSMP_ISSUCCESS)
            .HasMaxLength(30);

            this.Property(t => t.VSMP_RETURN_CODE)
            .HasMaxLength(30);

            this.Property(t => t.VSMP_RETURN_DESC)
            .HasMaxLength(500);

            this.Property(t => t.VSMP_TRANSID)
           .HasMaxLength(50);

            this.Property(t => t.SPNAME)
          .HasMaxLength(15);

            this.Property(t => t.CHM)
          .HasMaxLength(1);

            this.Property(t => t.STATE)
          .HasMaxLength(2);

            this.Property(t => t.VSMP_TRANSID)
          .HasMaxLength(50);


            this.Property(t => t.CREATED_BY)
           .HasMaxLength(20);

            this.Property(t => t.UPDATED_BY)
           .HasMaxLength(20);

            // Table & Column Mappings

            this.ToTable("FBB_VSMP_LOG", "WBB");
            this.Property(t => t.VSMP_ID).HasColumnName("VSMP_ID");
            this.Property(t => t.ORDER_REF).HasColumnName("ORDER_REF");
            this.Property(t => t.MOBILE_NO).HasColumnName("MOBILE_NO");
            this.Property(t => t.USER_NAME).HasColumnName("USER_NAME");
            this.Property(t => t.ORDER_DESC).HasColumnName("ORDER_DESC");
            this.Property(t => t.VSMP_ISSUCCESS).HasColumnName("VSMP_ISSUCCESS");
            this.Property(t => t.VSMP_RETURN_CODE).HasColumnName("VSMP_RETURN_CODE");
            this.Property(t => t.VSMP_RETURN_DESC).HasColumnName("VSMP_RETURN_DESC");
            this.Property(t => t.VSMP_TRANSID).HasColumnName("VSMP_TRANSID");
            this.Property(t => t.SPNAME).HasColumnName("SPNAME");
            this.Property(t => t.CHM).HasColumnName("CHM");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");

        }
    }
}
