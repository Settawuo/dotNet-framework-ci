using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class SearchInterfaceLogData
    {
        public SearchInterfaceLogData()
        {
            this.InterfaceLogData = new List<SearchInterfaceLog>();
        }

        public List<SearchInterfaceLog> InterfaceLogData { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class SearchInterfaceLog
    {
        public decimal INTERFACE_ID { get; set; }
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string IN_XML_PARAM { get; set; }
        public string OUT_RESULT { get; set; }
        public string OUT_ERROR_RESULT { get; set; }
        public string OUT_XML_PARAM { get; set; }
        public string REQUEST_STATUS { get; set; }
        public string INTERFACE_NODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public decimal ALL_RECORDS { get; set; }
    }

    public class MethodNameLog
    {
        public string METHOD_NAME { get; set; }
    }
}
