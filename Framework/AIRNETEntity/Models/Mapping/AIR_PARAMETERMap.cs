namespace AIRNETEntity.Models.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class AIR_PARAMETERMap : EntityTypeConfiguration<AIR_PARAMETER>
    {
        public AIR_PARAMETERMap()
        {
            this.HasKey(t => new { t.PARAM });


            this.Property(t => t.PARAM)
                .HasMaxLength(50);

            this.Property(t => t.VALUE)
                .HasMaxLength(50);
            this.Property(t => t.JOB_NAME)
                .HasMaxLength(50);

            this.ToTable("AIR_PARAMETER", "AIR_ADMIN");

            this.Property(t => t.PARAM).HasColumnName("PARAM");
            this.Property(t => t.VALUE).HasColumnName("VALUE");
            this.Property(t => t.JOB_NAME).HasColumnName("JOB_NAME");
        }
    }
}
