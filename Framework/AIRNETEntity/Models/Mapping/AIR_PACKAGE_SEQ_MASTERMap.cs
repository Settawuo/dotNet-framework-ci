namespace AIRNETEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class AIR_PACKAGE_SEQ_MASTERMap : EntityTypeConfiguration<AIR_PACKAGE_SEQ_MASTER>
    {
        public AIR_PACKAGE_SEQ_MASTERMap()
        {
            this.HasKey(t => new { t.PACKAGE_SUBSEQ });

            this.Property(t => t.PACKAGE_SEQ_TYPE)
                .HasMaxLength(50);

            this.Property(t => t.PACKAGE_SUBSEQ)
                .HasMaxLength(50);

            this.ToTable("AIR_PACKAGE_SEQ_MASTER", "AIR_ADMIN");

            this.Property(t => t.PACKAGE_SEQ_TYPE).HasColumnName("PACKAGE_SEQ_TYPE");
            this.Property(t => t.PACKAGE_SEQ).HasColumnName("PACKAGE_SEQ");
            this.Property(t => t.PACKAGE_SUBSEQ).HasColumnName("PACKAGE_SUBSEQ");

        }
    }
}
