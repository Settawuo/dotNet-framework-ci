using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_USERMap : EntityTypeConfiguration<FBB_USER>
    {
        public FBB_USERMap()
        {
            this.HasKey(t => t.USER_ID);

            this.Property(t => t.USER_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.USER_NAME)
                .HasMaxLength(15);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.FIRST_NAME)
                .HasMaxLength(50);

            this.Property(t => t.LAST_NAME)
                .HasMaxLength(50);

            this.Property(t => t.PIN_CODE)
                .HasMaxLength(10);

            this.Property(t => t.POSITION)
                .HasMaxLength(50);

            this.Property(t => t.MOBILE_NUMBER)
                .HasMaxLength(10);

            this.Property(t => t.EMAIL)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(2);

            this.ToTable("FBB_USER", "WBB");
            this.Property(t => t.USER_ID).HasColumnName("USER_ID");
            this.Property(t => t.USER_NAME).HasColumnName("USER_NAME");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.FIRST_NAME).HasColumnName("FIRST_NAME");
            this.Property(t => t.LAST_NAME).HasColumnName("LAST_NAME");
            this.Property(t => t.PIN_CODE).HasColumnName("PIN_CODE");
            this.Property(t => t.POSITION).HasColumnName("POSITION");
            this.Property(t => t.MOBILE_NUMBER).HasColumnName("MOBILE_NUMBER");
            this.Property(t => t.EMAIL).HasColumnName("EMAIL");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");

        }

    }
}
