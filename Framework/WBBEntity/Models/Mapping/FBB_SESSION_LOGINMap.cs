using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SESSION_LOGINMap : EntityTypeConfiguration<FBB_SESSION_LOGIN>
    {
        public FBB_SESSION_LOGINMap()
        {
            this.HasKey(t => t.CUST_INTERNET_NUM);

            this.Property(t => t.CUST_INTERNET_NUM)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CUST_INTERNET_NUM)
                .HasMaxLength(50);

            this.Property(t => t.SESSION_ID)
                .HasMaxLength(50);

            this.ToTable("FBB_SESSION_LOGIN", "WBB");
            this.Property(t => t.CUST_INTERNET_NUM).HasColumnName("CUST_INTERNET_NUM");
            this.Property(t => t.SESSION_ID).HasColumnName("SESSION_ID");
            this.Property(t => t.LOGIN_DATE).HasColumnName("LOGIN_DATE");
        }
    }
}
