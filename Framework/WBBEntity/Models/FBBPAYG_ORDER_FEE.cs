using System;

namespace WBBEntity.Models
{
    public partial class FBBPAYG_ORDER_FEE
    {
        public int ORDER_FEE_ID { get; set; }

        public string FEE_ID { get; set; }

        public string FEE_ID_TYPE { get; set; }

        public string FEE_NAME { get; set; }

        public decimal ORDER_FEE_PRICE { get; set; }
        public string FEE_ACTION {  get; set; }
        public string ORDER_NO { get; set; }
        public string ACCESS_NO { get; set; }      
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set;}
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_BY {  get; set; }
    }
}
