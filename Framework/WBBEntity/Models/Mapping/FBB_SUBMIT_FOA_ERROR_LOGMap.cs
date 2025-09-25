using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SUBMIT_FOA_ERROR_LOGMap : EntityTypeConfiguration<FBB_SUBMIT_FOA_ERROR_LOG>
    {
        public FBB_SUBMIT_FOA_ERROR_LOGMap()
        {
            this.HasKey(t => new { t.ACCESS_NUMBER });
            //  this.Property(t => t.ACCESS_NUMBER).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.ToTable("FBB_SUBMIT_FOA_ERROR_LOG", "WBB");
            this.Property(t => t.ACCESS_NUMBER).HasColumnName("ACCESS_NUMBER");
            this.Property(t => t.IN_XML_FOA).HasColumnName("IN_XML_FOA");
            this.Property(t => t.RESEND_STATUS).HasColumnName("RESEND_STATUS");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
