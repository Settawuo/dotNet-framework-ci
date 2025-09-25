using System;

namespace WBBEntity.FBBShareplexModels
{
    public partial class WFS_TEAM_ATTR
    {
        public decimal TEAM_ID { get; set; }
        public decimal SEQ { get; set; }
        public string LOCATION_CODE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string STAGE_LOCAL { get; set; }
        public string JOB_TYPE { get; set; }
        public string SHIP_TO { get; set; }
        public string SUBCONTRACT_EMAIL { get; set; }
        public string ALIAS_COMPANY_NAME { get; set; }
        public string ALIAS_MAIN_COMPANY_NAME { get; set; }
        public string OOS_STAGE_LOCAL { get; set; }
        public string SUBCONTRACT_TYPE { get; set; }
        public string SUBCONTRACT_SUB_TYPE { get; set; }
        public string WARRANTY_MA { get; set; }
        public string WARRANTY_INSTALL { get; set; }
        public string PRI_IN_WARRANTY { get; set; }
        public string PRI_OUT_WARRANTY { get; set; }
        public string SERVICE_SKILL { get; set; }
        public string SUB_PHASE { get; set; }
        public string STATE { get; set; }
        public decimal CREATE_USER { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public decimal MODIFY_USER { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
    }
}
