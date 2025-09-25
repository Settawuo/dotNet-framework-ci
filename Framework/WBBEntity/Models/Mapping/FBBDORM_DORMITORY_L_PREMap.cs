using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBBDORM_DORMITORY_L_PREMap : EntityTypeConfiguration<FBBDORM_DORMITORY_L_PRE>
    {
        public FBBDORM_DORMITORY_L_PREMap()
        {

            // Properties
            this.Property(t => t.ROW_ID)
              .HasMaxLength(50);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_DATE)
                .IsRequired();

            this.Property(t => t.UPDATED_BY)
                 .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UPDATED_DATE)
                 .IsRequired();

            this.Property(t => t.FIRST_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.LAST_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.CONTACT_PHONE)
                .HasMaxLength(50);

            this.Property(t => t.ID_CARD_TYPE)
              .HasMaxLength(50);

            this.Property(t => t.ID_CARD_NO)
             .HasMaxLength(50);

            this.Property(t => t.DORMITORY_NAME)
                .HasMaxLength(1000);

            this.Property(t => t.BUILDING_NO)
            .HasMaxLength(1000);

            this.Property(t => t.FLOOR_NO)
              .HasMaxLength(50);

            this.Property(t => t.ROOM_NO)
             .HasMaxLength(50);

            this.Property(t => t.FIBRE_NET_ID)
               .HasMaxLength(50);

            this.Property(t => t.REGION_CODE)
               .HasMaxLength(50);

            this.Property(t => t.FLAG)
              .HasMaxLength(10);

            this.Property(t => t.SEND_EMAIL_DATE)
                 .IsRequired()
              .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FBBDORM_DORMITORY_PRE", "WBB");
            this.Property(t => t.ROW_ID).HasColumnName("ROW_ID");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");
            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.FIRST_NAME).HasColumnName("FIRST_NAME");
            this.Property(t => t.LAST_NAME).HasColumnName("LAST_NAME");
            this.Property(t => t.CONTACT_PHONE).HasColumnName("CONTACT_PHONE");
            this.Property(t => t.ID_CARD_TYPE).HasColumnName("ID_CARD_TYPE");
            this.Property(t => t.ID_CARD_NO).HasColumnName("ID_CARD_NO");
            this.Property(t => t.DORMITORY_NAME).HasColumnName("DORMITORY_NAME");
            this.Property(t => t.BUILDING_NO).HasColumnName("BUILDING_NO");
            this.Property(t => t.FLOOR_NO).HasColumnName("FLOOR_NO");
            this.Property(t => t.ROOM_NO).HasColumnName("ROOM_NO");
            this.Property(t => t.FIBRE_NET_ID).HasColumnName("FIBRE_NET_ID");
            this.Property(t => t.REGION_CODE).HasColumnName("REGION_CODE");
            this.Property(t => t.FLAG).HasColumnName("FLAG");
            this.Property(t => t.SEND_EMAIL_DATE).HasColumnName("SEND_EMAIL_DATE");

        }
    }
}
