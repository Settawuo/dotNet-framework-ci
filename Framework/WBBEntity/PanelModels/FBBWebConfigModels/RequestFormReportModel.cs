using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{

    public class RequestFormReportModel
    {
        public string P_DATE_FROM { get; set; }
        public string P_DATE_TO { get; set; }

        public string P_REGION_CODE { get; set; }
        public string P_PROVINCE { get; set; }

        public string P_PROCESS_STATUS { get; set; }

        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }
    }

    public class RequestFormReturn
    {
        public List<ReportRequestFormListDetail> P_RES_DATA { get; set; }
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
        public int P_PAGE_COUNT { get; set; }
    }
    public class ReportRequestFormListDetail
    {
        public int RowNumber { get; set; }
        public string TYPE_CUSTOMER_REQUEST { get; set; }
        public string CREATED_DT { get; set; }
        public string CUSTOMER_FIRST_NAME { get; set; }
        public string CUSTOMER_LAST_NAME { get; set; }
        public string CONTRACT_PHONE { get; set; }
        public string DORMITORY_NAME { get; set; }
        public string TYPE_DORMITORY { get; set; }
        public string HOME_NO { get; set; }
        public string MOO { get; set; }
        public string SOI { get; set; }
        public string STREET { get; set; }
        public string Tumbon { get; set; }
        public string AMPHUR { get; set; }
        public string province { get; set; }
        public string ZIPCODE { get; set; }
        public string Region_Code { get; set; }
        public string A_BUILDING { get; set; }
        public string A_UNIT { get; set; }
        public string A_LIVING { get; set; }
        public string PHONE_CABLE { get; set; }
        public string PROBLEM_INTERNET { get; set; }
        public string A_UNIT_USE_INTERNET { get; set; }
        public string OLD_SYSTEM { get; set; }
        public string OLD_VENDOR_SERVICE { get; set; }
        public string SEND_TO_SALE_DT { get; set; }
        //public string USER_APPROVE { get; set; }
        //public string USER_APPROVE_DT { get; set; }
        public string PROCESS_STATUS { get; set; }
        //public string DORMITORY_NAME_TH { get; set; }
        //public string REMARK { get; set; }


    }
}

