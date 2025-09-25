namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPackageSequenceModel
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string PACKAGE_TYPE_DESC { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PRODUCT_SUBTYPE1 { get; set; }
        public decimal PACKAGE_SEQ { get; set; }
        public string PACKAGE_SUBSEQ { get; set; }
        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public string DESCTHAI { get; set; }
        public string DESCENG { get; set; }
        public decimal? SUB_SEQ { get; set; }


    }

    public class CheckUsePoinBySFFPromotionCodeModel
    {
        public decimal? PRE_PRICE_CHARGE { get; set; }
    }
}
