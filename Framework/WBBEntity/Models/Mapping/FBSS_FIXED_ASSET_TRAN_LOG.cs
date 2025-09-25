using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBSS_FIXED_ASSET_TRAN_LOGMap : EntityTypeConfiguration<FBSS_FIXED_ASSET_TRAN_LOG>
    {
        public FBSS_FIXED_ASSET_TRAN_LOGMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TRANS_ID });

            // Properties
            this.Property(t => t.TRANS_ID)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.REC_TYPE)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.INTERNET_NO)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("FBSS_FIXED_ASSET_TRAN_LOG", "WBB");
            this.Property(t => t.TRANS_ID).HasColumnName("TRANS_ID");
            this.Property(t => t.REC_TYPE).HasColumnName("REC_TYPE");
            this.Property(t => t.RUN_GROUP).HasColumnName("RUN_GROUP");
            this.Property(t => t.INTERNET_NO).HasColumnName("INTERNET_NO");
            this.Property(t => t.COM_CODE).HasColumnName("COM_CODE");
            this.Property(t => t.MAIN_ASSET).HasColumnName("MAIN_ASSET");
            this.Property(t => t.SUB_NUMBER).HasColumnName("SUB_NUMBER");
            this.Property(t => t.MATERIAL_NO).HasColumnName("MATERIAL_NO");
            this.Property(t => t.SERIAL_NO).HasColumnName("SERIAL_NO");
            this.Property(t => t.MATERIAL_DOC).HasColumnName("MATERIAL_DOC");
            this.Property(t => t.DOC_YEAR).HasColumnName("DOC_YEAR");
            this.Property(t => t.ERR_CODE).HasColumnName("ERR_CODE");
            this.Property(t => t.ERR_MSG).HasColumnName("ERR_MSG");
            this.Property(t => t.TRAN_STATUS).HasColumnName("TRAN_STATUS");
            this.Property(t => t.PREV_TRAN_ID).HasColumnName("PREV_TRAN_ID");
            this.Property(t => t.NEXT_TRAN_ID).HasColumnName("NEXT_TRAN_ID");
            this.Property(t => t.LOG_DATE).HasColumnName("LOG_DATE");
            this.Property(t => t.MODIFY_DATE).HasColumnName("MODIFY_DATE");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.ORDER_NO).HasColumnName("ORDER_NO");
        }
    }
}
