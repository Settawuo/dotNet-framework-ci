using System;

namespace WBBEntity.Models
{
    public class FBB_CFG_PAYMENT
    {
        public string PRODUCT_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string ENDPOINT { get; set; }
        public string ATTR_NAME { get; set; }
        public string ATTR_VALUE { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
