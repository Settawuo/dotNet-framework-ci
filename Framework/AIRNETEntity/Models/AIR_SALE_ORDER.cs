using System;

namespace AIRNETEntity.Models
{
    public class AIR_SALE_ORDER
    {
        public string ORDER_NO { get; set; }
        public decimal? LOCATION_ID { get; set; }
        public string ORDER_TYPE { get; set; }
        public DateTime? ORDER_CREATED_DTM { get; set; }
        public string ORDER_CREATED_BY { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string SALE_USER_NAME { get; set; }
        public DateTime? SUBMIT_DATE { get; set; }
        public string INVOICING_CO_ID { get; set; }
        public string ORDER_STATUS { get; set; }
        public string CANCEL_ORDER_NO { get; set; }
    }
}
