using System;

namespace WBBEntity.Models
{
    public class FBB_INTERFACE_LOG_PAYG
    {
        public long INTERFACE_ID { get; set; }
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string IN_XML_PARAM { get; set; }
        public string OUT_RESULT { get; set; }
        public string OUT_ERROR_RESULT { get; set; }
        public string OUT_XML_PARAM { get; set; }
        public string REQUEST_STATUS { get; set; }
        public string INTERFACE_NODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
