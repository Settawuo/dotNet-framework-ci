using System;

namespace WBBEntity.Models
{
    public class FBB_CUST_PROFILE
    {
        public string CUST_NON_MOBILE { get; set; }
        public string CA_ID { get; set; }
        public string SA_ID { get; set; }
        public string BA_ID { get; set; }
        public string IA_ID { get; set; }
        public string CUST_NAME { get; set; }
        public string CUST_SURNAME { get; set; }
        public string CUST_ID_CARD_TYPE { get; set; }
        public string CUST_ID_CARD_NUM { get; set; }
        public string CUST_CATEGORY { get; set; }
        public string CUST_SUB_CATEGORY { get; set; }
        public string CUST_GENDER { get; set; }
        public DateTime? CUST_BIRTHDAY { get; set; }
        public string CUST_NATIONALITY { get; set; }
        public string CUST_TITLE { get; set; }
        public string ONLINE_NUMBER { get; set; }
        public string CONDO_TYPE { get; set; }
        public string CONDO_DIRECTION { get; set; }
        public string CONDO_LIMIT { get; set; }
        public string CONDO_AREA { get; set; }
        public string HOME_TYPE { get; set; }
        public string HOME_AREA { get; set; }
        public string INSTALL_ADDR_ID { get; set; }
        public string BILL_ADDR_ID { get; set; }
        public string VAT_ADDR_ID { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string CVR_ID { get; set; }
        public string PORT_ID { get; set; }
        public string ORDER_NO { get; set; }
        public DateTime REGISTER_DATE { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public DateTime? RABBIT_REGISTER_DATE { get; set; }
        public Nullable<int> CUST_CURRENT_STATUS { get; set; }
        public DateTime? PORT_ACTIVE_DATE { get; set; }
        public DateTime? INSTALLATION_DATE { get; set; }
        public DateTime? CURRENT_STATUS_DATE { get; set; }
        public string RELATE_MOBILE { get; set; }
        public string RELATE_NON_MOBILE { get; set; }
        public string NETWORK_TYPE { get; set; }
        public Nullable<int> SERVICE_DAY { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ACCESS_MODE { get; set; }
        public string SERVICE_CODE { get; set; }
    }
}
