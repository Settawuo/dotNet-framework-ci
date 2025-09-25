using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class LogInterfaceModel
    {
        public int INTERFACE_ID { get; set; }
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string INPUT { get; set; }
        public string OUTPUT { get; set; }
        public string INTERFACE_NODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string ALL_RECORDS { get; set; }
    }

    public class LogInterfaceReportGridList
    {
        public int INTERFACE_ID { get; set; }
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string INPUT { get; set; }
        public string OUTPUT { get; set; }
        public string INTERFACE_NODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
    }

    public class LogInterfaceReportExportList
    {
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string INPUT { get; set; }
        public string OUTPUT { get; set; }
        public string INTERFACE_NODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
    }

    public class LogInterfaceReportResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
