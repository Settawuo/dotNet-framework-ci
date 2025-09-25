using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class QueryPortModel
    {
        public string return_code { get; set; }
        public string return_message { get; set; }
        public QueryPortResponse Data { get; set; }
    }

    public class QueryPortResponse
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string RESOURCE_NO { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_ALIAS { get; set; }
        public string RESOURCE_SITE_NO { get; set; }
        public string RESOURCE_LATITUDE { get; set; }
        public string RESOURCE_LONGITUDE { get; set; }
        public List<QueryPortNo> QueryPortNoList { get; set; }
    }

    public class QueryPortNo
    {
        public string PORT_NO { get; set; }
        public string PSTN { get; set; }
        public string RELATE_DEVICE { get; set; }
        public string MDF_VOICE { get; set; }
        public string MDF_DATA { get; set; }
        public string SERVICE_STATE { get; set; }
        public string CUSTOMER_SERVICE_NO { get; set; }
        public string REMARK { get; set; }
    }
}
