using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_CFG_IDSMap : EntityTypeConfiguration<FBB_CFG_IDS>
    {
        public FBB_CFG_IDSMap()
        {
            // Primary Key
            this.HasKey(t => t.CREATED_DATE);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(30);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(30);
            this.Property(t => t.SERVICE_PROVIDER_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CHANNEL)
                .HasMaxLength(50);

            this.Property(t => t.CALLBACK_URL)
                .HasMaxLength(4000);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(1);

            this.Property(t => t.CLIENT_ID)
                .HasMaxLength(200);

            this.Property(t => t.CLIENT_SECRET)
                .IsRequired()
                .HasMaxLength(200);



            // Table & Column Mappings
            this.ToTable("FBB_CFG_IDS", "WBB");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.SERVICE_PROVIDER_NAME).HasColumnName("SERVICE_PROVIDER_NAME");
            this.Property(t => t.CHANNEL).HasColumnName("CHANNEL");
            this.Property(t => t.CALLBACK_URL).HasColumnName("CALLBACK_URL");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CLIENT_ID).HasColumnName("CLIENT_ID");

            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
