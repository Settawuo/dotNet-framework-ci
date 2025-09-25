using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{

    public class FBSS_HISTORY_LOGmap : EntityTypeConfiguration<FBSS_HISTORY_LOG>
    {
        public FBSS_HISTORY_LOGmap()
        {
            this.HasKey(t => t.INTERFACE_ID);

            this.Property(t => t.INTERFACE_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IN_TRANSACTION_ID)
                //.IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.IN_ACCESS_NUMBER)
               //.IsRequired()
               .HasMaxLength(50);

            this.Property(t => t.METHOD_NAME)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.SERVICE_NAME)
                .HasMaxLength(100);

            //this.Property(t => t.IN_ID_CARD_NO)
            //    .HasMaxLength(50);

            this.Property(t => t.IN_XML_PARAM)
               .HasMaxLength(4000);

            this.Property(t => t.OUT_RESULT)
                .HasMaxLength(30);

            this.Property(t => t.OUT_ERROR_RESULT)
                .HasMaxLength(100);

            this.Property(t => t.OUT_XML_PARAM)
              .HasMaxLength(4000);

            this.Property(t => t.REQUEST_STATUS)
                .HasMaxLength(50);

            this.Property(t => t.INTERFACE_NODE)
                .HasMaxLength(50);

            this.Property(t => t.URL_LINE)
                .HasMaxLength(1000);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBSS_HISTORY_LOG", "WBB");
            this.Property(t => t.INTERFACE_ID).HasColumnName("INTERFACE_ID");
            this.Property(t => t.IN_TRANSACTION_ID).HasColumnName("IN_TRANSACTION_ID");
            this.Property(t => t.IN_ACCESS_NUMBER).HasColumnName("IN_ACCESS_NUMBER");
            this.Property(t => t.METHOD_NAME).HasColumnName("METHOD_NAME");
            this.Property(t => t.SERVICE_NAME).HasColumnName("SERVICE_NAME");
            // this.Property(t => t.IN_ID_CARD_NO).HasColumnName("IN_ID_CARD_NO");
            this.Property(t => t.IN_XML_PARAM).HasColumnName("IN_XML_PARAM");
            this.Property(t => t.OUT_XML_PARAM).HasColumnName("OUT_XML_PARAM");
            this.Property(t => t.OUT_RESULT).HasColumnName("OUT_RESULT");
            this.Property(t => t.OUT_ERROR_RESULT).HasColumnName("OUT_ERROR_RESULT");
            this.Property(t => t.REQUEST_STATUS).HasColumnName("REQUEST_STATUS");
            this.Property(t => t.INTERFACE_NODE).HasColumnName("INTERFACE_NODE");
            this.Property(t => t.URL_LINE).HasColumnName("URL_LINE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}
