using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_MESSAGE_LOGMap : EntityTypeConfiguration<FBB_MESSAGE_LOG>
    {
        public FBB_MESSAGE_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.CUST_ROW_ID);

            this.Property(t => t.CUST_ROW_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PROCESS_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATE_USER)
                .HasMaxLength(50);

            this.Property(t => t.RETURN_DESC)
                .HasMaxLength(50);

            this.Property(t => t.REMARK)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("FBB_MESSAGE_LOG", "WBB");
            this.Property(t => t.CUST_ROW_ID).HasColumnName("CUST_ROW_ID");
            this.Property(t => t.PROCESS_NAME).HasColumnName("PROCESS_NAME");
            this.Property(t => t.CREATE_USER).HasColumnName("CREATE_USER");
            this.Property(t => t.RETURN_DESC).HasColumnName("RETURN_DESC");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.RETURN_CODE).HasColumnName("RETURN_CODE");

        }
    }
}
