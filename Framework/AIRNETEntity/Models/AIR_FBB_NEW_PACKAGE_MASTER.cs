namespace AIRNETEntity.Models
{
    public class AIR_FBB_NEW_PACKAGE_MASTER
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string PACKAGE_TYPE_DESC { get; set; }
        public string PACKAGE_SERVICE_CODE { get; set; }
        public string SFF_PRODUCT_CLASS { get; set; }

        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public decimal? SUB_SEQ { get; set; }

        public string DOWNLOAD_SPEED { get; set; }
        public decimal? PRE_PRICE_CHARGE { get; set; }

        //AWARE_R20.02
        public string SFF_PRODUCT_NAME { get; set; }
        public decimal PRICE_CHARGE { get; set; }
    }
}
