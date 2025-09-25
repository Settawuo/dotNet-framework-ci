namespace WBBEntity.Models.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class FBB_FIXED_ASSET_HISTORY_LOGMap : EntityTypeConfiguration<FBB_FIXED_ASSET_HISTORY_LOG>
    {
        public FBB_FIXED_ASSET_HISTORY_LOGMap()
        {
            this.HasKey(t => t.HISTORY_ID);

            this.Property(t => t.HISTORY_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TRANSACTION_ID)
                .HasMaxLength(100);

            //this.Property(t => t.IN_FOA)
            //    .HasMaxLength(100);

            //this.Property(t => t.OUT_FOA)
            //    .HasMaxLength(100);

            //this.Property(t => t.INSTALLATION)
            //    .HasMaxLength(50);

            //this.Property(t => t.IN_SAP)
            //    .HasMaxLength(30);

            //this.Property(t => t.OUT_SAP)
            //    .HasMaxLength(100);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            //this.Property(t => t.CREATED_DATE)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBSS_FIXED_ASSET_HISTORY_LOG", "WBB");
            this.Property(t => t.HISTORY_ID).HasColumnName("HISTORY_ID");
            this.Property(t => t.TRANSACTION_ID).HasColumnName("TRANSACTION_ID");
            this.Property(t => t.IN_FOA).HasColumnName("IN_FOA");
            this.Property(t => t.OUT_FOA).HasColumnName("OUT_FOA");
            this.Property(t => t.INSTALLATION).HasColumnName("INSTALLATION");
            this.Property(t => t.IN_SAP).HasColumnName("IN_SAP");
            this.Property(t => t.OUT_SAP).HasColumnName("OUT_SAP");
            this.Property(t => t.REQUEST_STATUS).HasColumnName("REQUEST_STATUS");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
        }
    }
}
