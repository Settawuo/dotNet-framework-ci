using System;

namespace WBBEntity.Models
{
    public partial class FBBPAYG_ORDER_PACKAGE
    {

        public int ORDER_PKG_ID { get; set; }

        public string PACKAGE_CODE { get; set; }

        public string PACKAGE_NAME { get; set; }

        public string PACKAGE_CLASS { get; set; }

        public string IS_NEW { get; set; }

        public string ORDER_NO { get; set; }

        public string ACCESS_NO { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public string CREATED_BY { get; set; }

        public DateTime? UPDATED_DATE { get; set; }

        public string UPDATED_BY { get; set; }
        
    }
}
