using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FIXED_ASSET_TRAN_LOG
    {
        public string TRANS_ID { get; set; }
        public string REC_TYPE { get; set; }
        public string RUN_GROUP { get; set; }
        public string INTERNET_NO { get; set; }
        public string COM_CODE { get; set; }
        public string MAIN_ASSET { get; set; }
        public string SUB_NUMBER { get; set; }
        public string MATERIAL_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string ERR_CODE { get; set; }
        public string ERR_MSG { get; set; }
        public string TRAN_STATUS { get; set; }
        public string PREV_TRAN_ID { get; set; }
        public string NEXT_TRAN_ID { get; set; }
        public Nullable<System.DateTime> LOG_DATE { get; set; }
        public Nullable<System.DateTime> MODIFY_DATE { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string ORDER_NO { get; set; }
    }
}
