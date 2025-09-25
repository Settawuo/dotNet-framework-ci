using System.Data.Entity.ModelConfiguration;

namespace WBBEntity.Models.Mapping
{
    public class FBB_PRE_REGISTERMap : EntityTypeConfiguration<FBB_PRE_REGISTER>
    {
        public FBB_PRE_REGISTERMap()
        {

            this.HasKey(t => new
            {
                t.PRE_REG_ID,
                t.REFFERENCE_NO
            });

            this.ToTable("FBB_PRE_REGISTER", "WBB");
            this.Property(t => t.PRE_REG_ID).HasColumnName("PRE_REG_ID");
            this.Property(t => t.LANGUAGE).HasColumnName("LANGUAGE");
            this.Property(t => t.CUST_NAME).HasColumnName("CUST_NAME");
            this.Property(t => t.CUST_SURNAME).HasColumnName("CUST_SURNAME");
            this.Property(t => t.CONTACT_MOBILE_NO).HasColumnName("CONTACT_MOBILE_NO");

            this.Property(t => t.IS_AIS_MOBILE).HasColumnName("IS_AIS_MOBILE");
            this.Property(t => t.CONTACT_EMAIL).HasColumnName("CONTACT_EMAIL");
            this.Property(t => t.ADDRESS_TYPE).HasColumnName("ADDRESS_TYPE");
            this.Property(t => t.HOUSE_NO).HasColumnName("HOUSE_NO");
            this.Property(t => t.BUILDING_NAME).HasColumnName("BUILDING_NAME");

            this.Property(t => t.VILLAGE_NAME).HasColumnName("VILLAGE_NAME");
            this.Property(t => t.SOI).HasColumnName("SOI");
            this.Property(t => t.ROAD).HasColumnName("ROAD");
            this.Property(t => t.ZIPCODE_ROWID).HasColumnName("ZIPCODE_ROWID");
            this.Property(t => t.CONTACT_TIME).HasColumnName("CONTACT_TIME");

            this.Property(t => t.CONTACT_TIME).HasColumnName("CONTACT_TIME");
            this.Property(t => t.IS_CONTACT_CUST).HasColumnName("IS_CONTACT_CUST");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.IS_IN_COVERAGE).HasColumnName("IS_IN_COVERAGE");
            this.Property(t => t.CLOSING_SALE).HasColumnName("CLOSING_SALE");

            this.Property(t => t.INCENTIVE_PAY).HasColumnName("INCENTIVE_PAY");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATUS_DATE).HasColumnName("STATUS_DATE");
            this.Property(t => t.CREATE_BY).HasColumnName("CREATE_BY");
            this.Property(t => t.CREATE_DTM).HasColumnName("CREATE_DTM");

            this.Property(t => t.UPDATE_BY).HasColumnName("UPDATE_BY");
            this.Property(t => t.UPDATE_DTM).HasColumnName("UPDATE_DTM");
            this.Property(t => t.REMARK_FOR_NO_COV).HasColumnName("REMARK_FOR_NO_COV");
            this.Property(t => t.REMARK_FOR_NO_REG).HasColumnName("REMARK_FOR_NO_REG");
            this.Property(t => t.REFFERENCE_NO).HasColumnName("REFFERENCE_NO");
        }
    }
}
