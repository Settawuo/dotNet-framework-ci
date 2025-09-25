using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class GetListPackageSellRouterV2Model
    {
        public decimal o_return_code { get; set; }

        public List<ListPackageSellRouterV2Model> ListPackageSellRouter { get; set; }

        public GetListPackageSellRouterV2Model()
        {
            o_return_code = -1;
            ListPackageSellRouter = new List<ListPackageSellRouterV2Model>();
        }
    }

    public class GetListPackageSellRouterV2OnlineModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }

        public List<ListPackageSellRouterV2Model> LIST_PACKAGE_SELL_ROUTER { get; set; }
    }

    public class ListPackageSellRouterV2Model
    {
        public string MAPPING_CODE { get; set; }
        public string PACKAGE_SERVICE_CODE { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string PACKAGE_GROUP { get; set; }
        public string PACKAGE_GROUP_DESC_THA { get; set; }
        public string PACKAGE_GROUP_DESC_ENG { get; set; }
        public string PACKAGE_REMARK_THA { get; set; }
        public string PACKAGE_REMARK_ENG { get; set; }
        public decimal? PACKAGE_GROUP_SEQ { get; set; }
        public decimal? PRICE_CHARGE { get; set; }
        public decimal? PRICE_CHARGE_VAT { get; set; }
        public decimal? PRE_PRICE_CHARGE { get; set; }
        public string SFF_WORD_IN_STATEMENT_THA { get; set; }
        public string SFF_WORD_IN_STATEMENT_ENG { get; set; }
        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public string DOWNLOAD_SPEED { get; set; }
        public string UPLOAD_SPEED { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_TYPE_DESC { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string ACCESS_MODE { get; set; }
        public string SERVICE_CODE { get; set; }
        public string PRODUCT_SUBTYPE3 { get; set; }
        public string DISCOUNT_TYPE { get; set; }
        public string DISCOUNT_VALUE { get; set; }
        public string DISCOUNT_DAY { get; set; }
        public string AUTO_MAPPING_CODE { get; set; }
        public string AUTO_MAPPING_PROMOTION_CODE { get; set; }
        public string DISPLAY_FLAG { get; set; }
        public decimal? DISPLAY_SEQ { get; set; }
        public decimal? SUB_SEQ { get; set; }
        public List<SGroupV2> S_GROUP { get; set; }
    }

    public class SGroupV2
    {
        public string PACKAGE_GROUP { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PACKAGE_TYPE { get; set; }
        public string PACKAGE_GROUP_DESC_THA { get; set; }
        public string PACKAGE_GROUP_DESC_ENG { get; set; }
        public string PACKAGE_REMARK_THA { get; set; }
        public string PACKAGE_REMARK_ENG { get; set; }
    }
}