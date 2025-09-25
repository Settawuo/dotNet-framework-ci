using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CARD_INFOMap : EntityTypeConfiguration<FBB_CARD_INFO>
    {


        public FBB_CARD_INFOMap()
        {
            // Primary Key
            this.HasKey(t => t.CARDID);

            // Properties
            this.Property(t => t.CARDID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.RESERVE_TECHNOLOGY)
                .HasMaxLength(50);

            this.Property(t => t.BUILDING)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_CARD_INFO", "WBB");
            this.Property(t => t.CARDID).HasColumnName("CARDID");
            this.Property(t => t.DSLAMID).HasColumnName("DSLAMID");
            this.Property(t => t.CARDNUMBER).HasColumnName("CARDNUMBER");
            this.Property(t => t.CARDMODELID).HasColumnName("CARDMODELID");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.RESERVE_TECHNOLOGY).HasColumnName("RESERVE_TECHNOLOGY");
            this.Property(t => t.BUILDING).HasColumnName("BUILDING");

        }
    }
}
