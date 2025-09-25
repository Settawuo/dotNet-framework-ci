using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_SPECIALISTMap : EntityTypeConfiguration<FBB_SPECIALIST>
    {
        public FBB_SPECIALISTMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.CHANNEL_SALES_CODE)
                .HasMaxLength(255);
            this.Property(t => t.CHANNEL_SALES_NAME)
                .HasMaxLength(255);
            this.Property(t => t.REMARK)
                .HasMaxLength(255);
            this.Property(t => t.CREATE_BY)
                .HasMaxLength(255);
            this.Property(t => t.UPDATE_BY)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("FBB_SPECIALIST", "WBB");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CHANNEL_SALES_CODE).HasColumnName("CHANNEL_SALES_CODE");
            this.Property(t => t.CHANNEL_SALES_NAME).HasColumnName("CHANNEL_SALES_NAME");
            this.Property(t => t.IS_STAFF).HasColumnName("IS_STAFF");
            this.Property(t => t.IS_PARTNER).HasColumnName("IS_PARTNER");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.UPDATE_DATE).HasColumnName("UPDATE_DATE");
            this.Property(t => t.UPDATE_BY).HasColumnName("UPDATE_BY");


    }
    }
}
