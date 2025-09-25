using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels.InWebServices
{
    public class BatchRequestForServiceModel
    {
        public List<DetailSubjectRecListRequest> P_RES_DATA1 { get; set; }
        public List<DetailForExcelListRequest> P_RES_DATA2 { get; set; }

        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }

    }

    public class DetailSubjectRecListRequest
    {

        public string SEND_FROM { get; set; }
        public string SEND_TO { get; set; }
        public string SEND_CC { get; set; }
        public string SEND_BCC { get; set; }
        public string REGION { get; set; }
        public string SUBJECT_NAME { get; set; }
        public string BODY_DETAIL { get; set; }


    }

    public class DetailForExcelListRequest
    {
        public string TYPE_CUSTOMER_REQUEST { get; set; }
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
    }

    public class DetailResponseBatchListRequest
    {
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }
    }
}
