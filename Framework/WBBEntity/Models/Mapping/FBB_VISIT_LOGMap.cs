using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_VISIT_LOGMap : EntityTypeConfiguration<FBB_VISIT_LOG>
    {
        public FBB_VISIT_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.VISITER_ID);

            this.Property(t => t.VISITER_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.USERNAME)
                .HasMaxLength(50);

            this.Property(t => t.VISIT_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.SELECT_PAGE)
                .HasMaxLength(50);

            this.Property(t => t.REQ_IPADDRESS)
                .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.HOST)
                .HasMaxLength(250);

            this.Property(t => t.LC)
            .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_VISIT_LOG", "WBB");
            this.Property(t => t.VISITER_ID).HasColumnName("VISITER_ID");
            this.Property(t => t.USERNAME).HasColumnName("USERNAME");
            this.Property(t => t.SELECT_PAGE).HasColumnName("SELECT_PAGE");
            this.Property(t => t.VISIT_TYPE).HasColumnName("VISIT_TYPE");
            this.Property(t => t.REQ_IPADDRESS).HasColumnName("REQ_IPADDRESS");
            this.Property(t => t.HOST).HasColumnName("HOST");
            this.Property(t => t.LC).HasColumnName("LC");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");

        }
    }
}
