using System;
using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class FBBLastMileByDistanceJobCommand
    {
        public List<FBB_LastMileByDistanceJob> P_LIST_FIXED_OM0101 { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }
    public class FBB_LastMileByDistanceJob
    {
        public string ACC_NBR { get; set; }
        public string USER_NAME { get; set; }
        public string SBC_CPY { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ON_TOP1 { get; set; }
        public string ON_TOP2 { get; set; }
        public string VOIP_NUMBER { get; set; }
        public string SERVICE_PACK_NAME { get; set; }
        public string ORD_NO { get; set; }
        public string ORD_TYPE { get; set; }
        public string ORDER_SFF { get; set; }
        public DateTime? APPOINTMENT_DATE { get; set; }
        public DateTime? SFF_ACTIVE_DATE { get; set; }
        public DateTime? APPROVE_JOB_FBSS_DATE { get; set; }
        public DateTime? COMPLETED_DATE { get; set; }
        public string ORDER_STATUS { get; set; }
        public string REJECT_REASON { get; set; }
        public string MATERIAL_CODE_CPESN { get; set; }
        public string CPE_SN { get; set; }
        public string CPE_MODE { get; set; }
        public string MATERIAL_CODE_STBSN { get; set; }
        public string STB_SN { get; set; }
        public string MATERIAL_CODE_ATASN { get; set; }
        public string ATA_SN { get; set; }
        public string MATERIAL_CODE_WIFIROUTESN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string STO_LOCATION { get; set; }
        public string VENDOR_CODE { get; set; }
        public string FOA_REJECT_REASON { get; set; }
        public string RE_APPOINTMENT_REASON { get; set; }
        public string PHASE_PO { get; set; }
        public DateTime? SFF_SUBMITTED_DATE { get; set; }
        public string EVENT_CODE { get; set; }
        public string REGION { get; set; }
        public decimal? TOTAL_FEE { get; set; }
        public string FEE_CODE { get; set; }
        public string ADDR_ID { get; set; }
        public string ADDR_NAME_TH { get; set; }
        public DateTime? TRANSFER_DATE { get; set; }
        public decimal? TOTAL_ROW { get; set; }
        public string FILE_NAME { get; set; }
        public string USER_CODE { get; set; }


    }
}
