namespace WBBEntity.Models.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class FBB_MINION_SERVICEMap : EntityTypeConfiguration<FBB_MINION_SERVICE>
    {
        public FBB_MINION_SERVICEMap()
        {
            this.HasKey(t => t.SERVICE_ID);
            this.Property(t => t.SERVICE_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SERVICE_MAIN_NAME)
                .HasMaxLength(100);

            this.Property(t => t.DEV_SERVICE_MAIN_URL)
                .HasMaxLength(255);

            this.Property(t => t.STG_SERVICE_MAIN_URL)
                .HasMaxLength(255);

            this.Property(t => t.PRD_SERVICE_MAIN_URL)
                .HasMaxLength(255);

            this.Property(t => t.SERVICE_PARENT_NAME)
                .HasMaxLength(100);

            this.Property(t => t.ACTIVE_FLAG)
                .HasMaxLength(2);

            this.Property(t => t.CREATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBB_MINION_SERVICE", "WBB");
            this.Property(t => t.SERVICE_ID).HasColumnName("SERVICE_ID");
            this.Property(t => t.SERVICE_MAIN_ID).HasColumnName("SERVICE_MAIN_ID");
            this.Property(t => t.SERVICE_MAIN_NAME).HasColumnName("SERVICE_MAIN_NAME");
            this.Property(t => t.DEV_SERVICE_MAIN_URL).HasColumnName("DEV_SERVICE_MAIN_URL");
            this.Property(t => t.STG_SERVICE_MAIN_URL).HasColumnName("STG_SERVICE_MAIN_URL");
            this.Property(t => t.PRD_SERVICE_MAIN_URL).HasColumnName("PRD_SERVICE_MAIN_URL");
            this.Property(t => t.SERVICE_PARENT_ID).HasColumnName("SERVICE_PARENT_ID");
            this.Property(t => t.SERVICE_PARENT_NAME).HasColumnName("SERVICE_PARENT_NAME");
            this.Property(t => t.REQUET_SOAP_XML).HasColumnName("REQUET_SOAP_XML");
            this.Property(t => t.ACTIVE_FLAG).HasColumnName("ACTIVE_FLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
        }
    }
}