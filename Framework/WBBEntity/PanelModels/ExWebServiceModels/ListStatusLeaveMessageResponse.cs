using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class ListStatusLeaveMessageResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<StatusLeaveMessageList> SearchStatusList { get; set; }
    }

    public class StatusLeaveMessageList
    {
        public string RefferenceNo { get; set; }
        public string Language { get; set; }
        public string ServiceSpeed { get; set; }
        public string Name { get; set; }
        public string ContactMobile { get; set; }
        public string RecordDate { get; set; }
        public string Status { get; set; }
        public string ContactCustomer { get; set; }
        public string CheckCoverage { get; set; }
        public string CustomerRegister { get; set; }
    }

    public class StatusLeaveMessageModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        public string Refference_No { get; set; }
        public string Language { get; set; }
        public string Service_Speed { get; set; }
        public string Name { get; set; }
        public string Contact_Mobile { get; set; }
        public string Record_Date { get; set; }
        public string Status { get; set; }
        public string Contact_Customer { get; set; }
        public string Check_Coverage { get; set; }
        public string Customer_Register { get; set; }
    }

    public class ChkValidateLeaveMessageModel
    {
        public bool ReturnChkInputParam { get; set; }
        public string ReturnMessageChk { get; set; }
    }
}
