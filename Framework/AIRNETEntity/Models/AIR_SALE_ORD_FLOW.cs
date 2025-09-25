using System;

namespace AIRNETEntity.Models
{
    public class AIR_SALE_ORD_FLOW
    {
        public string ORDER_NO { get; set; }
        public int WORK_FLOW_ID { get; set; }
        public int FLOW_SEQ { get; set; }
        public DateTime? FLOW_CREATE_DTM { get; set; }
        public string REMARK { get; set; }
        public string INPUT_COMPLETE_FLAG { get; set; }
        public DateTime UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string CANCEL_CODE { get; set; }
        public string OTHER_CANCEL_REASON { get; set; }
        public string MENU_CODE { get; set; }
    }
}
