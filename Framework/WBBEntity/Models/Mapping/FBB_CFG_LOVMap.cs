using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CFG_LOVMap : EntityTypeConfiguration<FBB_CFG_LOV>
    {
        public FBB_CFG_LOVMap()
        {
            // Primary Key
            this.HasKey(t => t.LOV_ID);

            // Properties
            this.Property(t => t.LOV_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.LOV_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.LOV_NAME)
                .HasMaxLength(255);

            this.Property(t => t.DISPLAY_VAL)
                .HasMaxLength(1000);

            this.Property(t => t.LOV_VAL1)
                .HasMaxLength(1000);

            this.Property(t => t.LOV_VAL2)
                .HasMaxLength(1000);

            this.Property(t => t.LOV_VAL3)
                .HasMaxLength(1000);

            this.Property(t => t.LOV_VAL4)
                .HasMaxLength(1000);

            this.Property(t => t.LOV_VAL5)
                .HasMaxLength(1000);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.DEFAULT_VALUE)
                .HasMaxLength(10);


            // Table & Column Mappings
            this.ToTable("FBB_CFG_LOV", "WBB");
            this.Property(t => t.LOV_ID).HasColumnName("LOV_ID");
            this.Property(t => t.PAR_LOV_ID).HasColumnName("PAR_LOV_ID");
            this.Property(t => t.LOV_TYPE).HasColumnName("LOV_TYPE");
            this.Property(t => t.LOV_NAME).HasColumnName("LOV_NAME");
            this.Property(t => t.DISPLAY_VAL).HasColumnName("DISPLAY_VAL");
            this.Property(t => t.LOV_VAL1).HasColumnName("LOV_VAL1");
            this.Property(t => t.LOV_VAL2).HasColumnName("LOV_VAL2");
            this.Property(t => t.LOV_VAL3).HasColumnName("LOV_VAL3");
            this.Property(t => t.LOV_VAL4).HasColumnName("LOV_VAL4");
            this.Property(t => t.LOV_VAL5).HasColumnName("LOV_VAL5");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.ORDER_BY).HasColumnName("ORDER_BY");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.DEFAULT_VALUE).HasColumnName("DEFAULT_VALUE");
            this.Property(t => t.IMAGE_BLOB).HasColumnName("IMAGE_BLOB");
        }
    }
}
