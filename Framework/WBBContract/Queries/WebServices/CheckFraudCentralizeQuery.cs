using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    //R23.05 CheckFraud
    public class CheckFraudCentralizeQuery : IQuery<CheckFraudCentralizeQueryModel>
    {
        public string Customer_type { get; set; }
        public string Customer_name { get; set; }
        public string Id_card_no { get; set; }
        public string Install_address { get; set; }
        public string Product_subtype { get; set; }
        public int Entry_fee { get; set; }
        public string Operator { get; set; }
        public string Promotion_name { get; set; }
        public string Promotion_ontop { get; set; }
        public int Promotion_price { get; set; }
        public int Price_net { get; set; }
        public string Cs_note { get; set; }
        public string Location_code { get; set; }
        public string Location_name { get; set; }
        public string Channel_sales { get; set; }
        public string Asc_code { get; set; }
        public string Asc_name { get; set; }
        public string Region_customer { get; set; }
        public string Region_sale { get; set; }
        public int Fraud_score { get; set; }
        public string Waiting_time_slot_flag { get; set; }
        public string Project_name { get; set; }
        public string Address_duplicated_flag { get; set; }
        public string Id_duplicated_flag { get; set; }
        public string Contact_duplicated_flag { get; set; }
        public string Contact_not_active_flag { get; set; }
        public string Contact_no_fmc_flag { get; set; }
        public string Watch_list_dealer_flag { get; set; }
        public string Sale_dealer_direct_sale_flag { get; set; }
        public string Relate_mobile_segment { get; set; }
        public string Charge_type { get; set; }
        public string Service_month { get; set; }
        public string Use_id_card_address_flag { get; set; }
        public string Reason_verify { get; set; }
    }
}