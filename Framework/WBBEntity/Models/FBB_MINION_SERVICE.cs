using System;

namespace WBBEntity.Models
{
    public partial class FBB_MINION_SERVICE
    {
        public decimal SERVICE_ID { get; set; }
        public decimal SERVICE_MAIN_ID { get; set; }
        public string SERVICE_MAIN_NAME { get; set; }
        public string DEV_SERVICE_MAIN_URL { get; set; }
        public string STG_SERVICE_MAIN_URL { get; set; }
        public string PRD_SERVICE_MAIN_URL { get; set; }
        public decimal SERVICE_PARENT_ID { get; set; }
        public string SERVICE_PARENT_NAME { get; set; }
        public string REQUET_SOAP_XML { get; set; }
        public string ACTIVE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}