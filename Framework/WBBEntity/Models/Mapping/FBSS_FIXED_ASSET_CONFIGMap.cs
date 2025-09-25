using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_ASSET_CONFIGMap : EntityTypeConfiguration<FBSS_FIXED_ASSET_CONFIG>
    {
        public FBSS_FIXED_ASSET_CONFIGMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PROGRAM_CODE, t.COM_CODE });

            // Properties
            this.Property(t => t.PROGRAM_CODE)
                .HasMaxLength(30);

            this.Property(t => t.PROGRAM_NAME)
                .HasMaxLength(50);

            this.Property(t => t.COM_CODE)
                .HasMaxLength(30);

            this.Property(t => t.ASSET_CLASS_GI)
                .HasMaxLength(500);

            this.Property(t => t.ASSET_CLASS_INS)
                .HasMaxLength(30);

            this.Property(t => t.COST_CENTER)
                .HasMaxLength(30);

            this.Property(t => t.MOVEMENT_TYPE_OUT)
                .HasMaxLength(30);

            this.Property(t => t.XREF1_HD)
                .HasMaxLength(30);

            this.Property(t => t.EVA4_GI)
                .HasMaxLength(30);

            this.Property(t => t.EVA4_INS)
                .HasMaxLength(30);

            this.Property(t => t.DOCUMENT_TYPE)
                .HasMaxLength(30);

            this.Property(t => t.CURRENCY)
                .HasMaxLength(30);

            this.Property(t => t.ACCOUNT)
                .HasMaxLength(30);

            this.Property(t => t.MOVEMENT_TYPE_IN)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("FBSS_FIXED_ASSET_CONFIG", "WBB");
            this.Property(t => t.PROGRAM_CODE).HasColumnName("PROGRAM_CODE");
            this.Property(t => t.PROGRAM_NAME).HasColumnName("PROGRAM_NAME");
            this.Property(t => t.COM_CODE).HasColumnName("COM_CODE");
            this.Property(t => t.ASSET_CLASS_GI).HasColumnName("ASSET_CLASS_GI");
            this.Property(t => t.ASSET_CLASS_INS).HasColumnName("ASSET_CLASS_INS");
            this.Property(t => t.COST_CENTER).HasColumnName("COST_CENTER");
            this.Property(t => t.QUANTITY).HasColumnName("QUANTITY");
            this.Property(t => t.MOVEMENT_TYPE_OUT).HasColumnName("MOVEMENT_TYPE_OUT");
            this.Property(t => t.XREF1_HD).HasColumnName("XREF1_HD");
            this.Property(t => t.EVA4_GI).HasColumnName("EVA4_GI");
            this.Property(t => t.EVA4_INS).HasColumnName("EVA4_INS");
            this.Property(t => t.DOCUMENT_TYPE).HasColumnName("DOCUMENT_TYPE");
            this.Property(t => t.CURRENCY).HasColumnName("CURRENCY");
            this.Property(t => t.RATE).HasColumnName("RATE");
            this.Property(t => t.ACCOUNT).HasColumnName("ACCOUNT");
            this.Property(t => t.CREATE_DATETIME).HasColumnName("CREATE_DATETIME");
            this.Property(t => t.MODIFY_DATETIME).HasColumnName("MODIFY_DATETIME");
            this.Property(t => t.MOVEMENT_TYPE_IN).HasColumnName("MOVEMENT_TYPE_IN");
        }
    }
}
