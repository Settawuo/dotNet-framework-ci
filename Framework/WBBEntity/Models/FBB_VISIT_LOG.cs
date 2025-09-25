using System;

namespace WBBEntity.Models
{
    public partial class FBB_VISIT_LOG
    {
        public decimal VISITER_ID { get; set; }
        public string USERNAME { get; set; }
        public string VISIT_TYPE { get; set; }
        public string REQ_IPADDRESS { get; set; }
        public string HOST { get; set; }
        public string LC { get; set; }


        public string SELECT_PAGE { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
    }
}
