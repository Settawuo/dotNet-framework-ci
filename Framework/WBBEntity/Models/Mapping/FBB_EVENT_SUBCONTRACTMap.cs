using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_EVENT_SUBCONTRACTMap : EntityTypeConfiguration<FBB_EVENT_SUBCONTRACT>
    {
        public FBB_EVENT_SUBCONTRACTMap()
        {
            //Primary Key
            this.HasKey(t => new
            {
                t.EVENT_CODE
            });

            // Properties
            this.Property(t => t.SUB_LOCATION_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SUB_CONTRACT_NAME)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.SUB_TEAM_ID)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SUB_TEAM_NAME)
                 .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.INSTALL_STAFF_ID)
                 .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.INSTALL_STAFF_NAME)
                 .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CREATED_BY)
                 .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_BY)
                 .IsRequired()
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("FBB_EVENT_SUBCONTRACT", "WBB");
            this.Property(t => t.EVENT_CODE).HasColumnName("EVENT_CODE");
            this.Property(t => t.SUB_LOCATION_ID).HasColumnName("SUB_LOCATION_ID");
            this.Property(t => t.SUB_CONTRACT_NAME).HasColumnName("SUB_CONTRACT_NAME");
            this.Property(t => t.SUB_TEAM_ID).HasColumnName("SUB_TEAM_ID");
            this.Property(t => t.SUB_TEAM_NAME).HasColumnName("SUB_TEAM_NAME");
            this.Property(t => t.INSTALL_STAFF_ID).HasColumnName("INSTALL_STAFF_ID");
            this.Property(t => t.INSTALL_STAFF_NAME).HasColumnName("INSTALL_STAFF_NAME");
            this.Property(t => t.EVENT_START_DATE).HasColumnName("EVENT_START_DATE");
            this.Property(t => t.EVENT_END_DATE).HasColumnName("EVENT_END_DATE");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");

        }

    }
}
