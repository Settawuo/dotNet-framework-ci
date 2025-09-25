using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CARDMODELMap : EntityTypeConfiguration<FBB_CARDMODEL>
    {
        public FBB_CARDMODELMap()
        {
            // Primary Key
            //this.HasKey(t => new { t.CARDMODELID, t.MODEL, t.PORTSTARTINDEX, t.MAXPORT, t.RESERVEPORTSPARE, t.CREATED_BY, t.CREATED_DATE, t.BRAND });
            this.HasKey(t => t.CARDMODELID);

            // Properties
            //this.Property(t => t.CARDMODELID)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.MODEL)
                .IsRequired()
                .HasMaxLength(50);

            //this.Property(t => t.PORTSTARTINDEX)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //this.Property(t => t.MAXPORT)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //this.Property(t => t.RESERVEPORTSPARE)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.DATAONLY_FLAG)
                .HasMaxLength(1);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.BRAND)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_CARDMODEL", "WBB");
            this.Property(t => t.CARDMODELID).HasColumnName("CARDMODELID");
            this.Property(t => t.MODEL).HasColumnName("MODEL");
            this.Property(t => t.PORTSTARTINDEX).HasColumnName("PORTSTARTINDEX");
            this.Property(t => t.MAXPORT).HasColumnName("MAXPORT");
            this.Property(t => t.RESERVEPORTSPARE).HasColumnName("RESERVEPORTSPARE");
            this.Property(t => t.DATAONLY_FLAG).HasColumnName("DATAONLY_FLAG");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.BRAND).HasColumnName("BRAND");
        }
    }
}
