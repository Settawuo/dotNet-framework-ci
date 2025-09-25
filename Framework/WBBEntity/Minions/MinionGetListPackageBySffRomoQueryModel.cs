using System.Collections.Generic;

namespace WBBEntity.Minions
{
    public class MinionGetListPackageBySffPromoQueryModel
    {

        //for return
        public decimal o_return_code { get; set; }

        public List<PackageSffPromoModel> ListPackageSffPromoList { get; set; }

        public MinionGetListPackageBySffPromoQueryModel()
        {
            o_return_code = -1;
            ListPackageSffPromoList = new List<PackageSffPromoModel>();
        }
    }

    public class PackageSffPromoModel
    {
        public string MAPPING_CODE { get; set; }

        public string MAPPING_PRODUCT { get; set; }

        public string PACKAGE_CODE { get; set; }

        public string PACKAGE_NAME { get; set; }

        public string PACKAGE_GROUP { get; set; }

        public decimal? RECURRING_CHARGE { get; set; }

        public decimal? PRE_RECURRING_CHARGE { get; set; }

        public string SFF_PROMOTION_BILL_THA { get; set; }

        public string SFF_PROMOTION_BILL_ENG { get; set; }

        public string TECHNOLOGY { get; set; }

        public string DOWNLOAD_SPEED { get; set; }

        public string UPLOAD_SPEED { get; set; }

        public decimal? INITIATION_CHARGE { get; set; }

        public decimal? PRE_INITIATION_CHARGE { get; set; }

        public string PACKAGE_TYPE { get; set; }

        public string PRODUCT_SUBTYPE { get; set; }

        public string OWNER_PRODUCT { get; set; }

        public string ACCESS_MODE { get; set; }

        public string SERVICE_CODE { get; set; }

        public string PRODUCT_SUBTYPE3 { get; set; }

        public decimal? SEQ { get; set; }

        public string DISCOUNT_TYPE { get; set; }

        public decimal? DISCOUNT_VALUE { get; set; }

        public decimal? DISCOUNT_DAY { get; set; }

        public string SFF_PROMOTION_CODE { get; set; }

        public string AUTO_MAPPING { get; set; }

        public string DISPLAY_FLAG { get; set; }

        public decimal? DISPLAY_SEQ { get; set; }

    }


}