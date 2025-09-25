using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.WebService
{
    public class GetListPackageDisplayQuery : IQuery<List<ListPackageDisplayModel>>
    {
        public string non_mobile_no { get; set; }
        public string relate_mobile { get; set; }
        public string ref_row_id { get; set; }
        public string owner_product { get; set; }
        public string serenate_flag { get; set; }

        private List<AirChangePromotionCode> _AirChangePromotionCodeArray;
        public List<AirChangePromotionCode> AirChangePromotionCodeArray
        {
            get { return _AirChangePromotionCodeArray; }
            set { _AirChangePromotionCodeArray = value; }
        }

        // Update 17.2
        public string client_ip { get; set; }

        // Update 17.3
        private List<AirPackageDisplay> _AirPackageDisplayArray;
        public List<AirPackageDisplay> AirPackageDisplayArray
        {
            get { return _AirPackageDisplayArray; }
            set { _AirPackageDisplayArray = value; }
        }

        // Update 17.5
        public string FullUrl { get; set; }
    }

    public class AirChangePromotionCode
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string startDt { get; set; }
        public string endDt { get; set; }
        public string PRODUCT_SEQ { get; set; }
    }

    public class AirPackageDisplay
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string promotionName { get; set; }
        public string productClass { get; set; }
        public string productGroup { get; set; }
        public string productPkg { get; set; }
        public string descThai { get; set; }
        public string descEng { get; set; }
        public string inStatementThai { get; set; }
        public string inStatementEng { get; set; }
        public string startDt { get; set; }
        public string endDt { get; set; }
        public string PRODUCT_SEQ { get; set; }
    }

    public class GetListPackageChangeQuery : IQuery<List<ListPackageChangeModel>>
    {
        public string sale_channel { get; set; }
        public string owner_product { get; set; }
        public string package_for { get; set; }
        public string customer_type { get; set; }
        public string partner_type { get; set; }
        public string partner_subtype { get; set; }
        public string location_code { get; set; }
        public string asc_code { get; set; }
        public string employee_id { get; set; }
        public string region { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string sub_district { get; set; }
        public string address_id { get; set; }
        public string serenade_flag { get; set; }
        public string penalty_flag { get; set; }
        public string package_main { get; set; }
        public string product_subtype { get; set; }
        //15.02.21
        public string distribution_channel { get; set; }
        public string channel_sales_group { get; set; }
        public string shop_segment { get; set; }


        public string non_mobile_no { get; set; }
        // Update 17.2
        public string client_ip { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }

        //R20.6 ChangePromotionCheckRight
        public List<ProjectCondFlag> ProjectCondFlagArray { get; set; }
        //R21.5 Pool Villa
        public string location_Province { get; set; }
    }

    public class GetListPackageChangeOnlineQuery : IQuery<List<ListPackageChangeModel>>
    {
        public string SALE_CHANNEL { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string PACKAGE_FOR { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string PARTNER_TYPE { get; set; }
        public string PARTNER_SUBTYPE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string REGION { get; set; }
        public string PROVINCE { get; set; }
        public string DISTRICT { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string ADDRESS_ID { get; set; }
        public string SERENADE_FLAG { get; set; }
        public string PENALTY_FLAG { get; set; }
        public string PACKAGE_MAIN { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        //15.02.21
        public string DISTRIBUTION_CHANNEL { get; set; }
        public string CHANNEL_SALES_GROUP { get; set; }
        public string SHOP_SEGMENT { get; set; }


        //R20.6 ChangePromotionCheckRight
        public List<AirProjectCondFlagArray> AIR_PROJECT_COND_FLAG_ARRAY { get; set; }
        //R21.5 Pool Villa
        public string LOCATION_PROVINCE { get; set; }

    }

    public class AirProjectCondFlagArray
    {
        public string Flag { get; set; }
        public string Value { get; set; }
    }
    public class GetListChangePackageQuery : IQuery<List<ListChangePackageModel>>
    {
        public string non_mobile_no { get; set; }
        public string relate_mobile { get; set; }
        public string network_type { get; set; }
        public string current_project_name { get; set; }

        private List<AirChangePromotionCode> _AirChangePromotionCodeOldArray;
        public List<AirChangePromotionCode> AirChangePromotionCodeOldArray
        {
            get { return _AirChangePromotionCodeOldArray; }
            set { _AirChangePromotionCodeOldArray = value; }
        }

        private List<AirChangePromotionCode> _AirChangePromotionCodeNewArray;
        public List<AirChangePromotionCode> AirChangePromotionCodeNewArray
        {
            get { return _AirChangePromotionCodeNewArray; }
            set { _AirChangePromotionCodeNewArray = value; }
        }

        public string oldRelateMobile { get; set; }
        public string acTion { get; set; }
        public string mobileNumberContact { get; set; }

        // Update 17.2
        public string client_ip { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }

        // Update 18.10
        private List<AirChangePromotionCode> _AirChangePlayBoxPromotionCodeNewArray;
        public List<AirChangePromotionCode> AirChangePlayBoxPromotionCodeNewArray
        {
            get { return _AirChangePlayBoxPromotionCodeNewArray; }
            set { _AirChangePlayBoxPromotionCodeNewArray = value; }
        }

        public string existing_mobile_flag { get; set; }
        public string current_project_name_opt { get; set; }
        public string current_mobile_chk_right { get; set; }
        public string current_mobile_chk_right_opt { get; set; }
        public string current_mobile_get_benefit { get; set; }
        public string current_mobile_get_benefit_opt { get; set; }
        public string new_mobile_chk_right { get; set; }
        public string new_mobile_get_benefit { get; set; }

        public string Language { get; set; }
    }

    public class ProjectCondFlag
    {
        public string projectCondFlag { get; set; }
        public string projectCondValue { get; set; }
    }

}
