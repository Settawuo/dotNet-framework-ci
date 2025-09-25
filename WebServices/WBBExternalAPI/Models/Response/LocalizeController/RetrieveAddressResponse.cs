using System.Collections.Generic;

namespace WBBExternalAPI.Models.Response.LocalizeController
{
    public class RetrieveAddressResponse
    {
        public string RESULT_CODE { set; get; }
        public string RESULT_DESC { set; get; }
        public string TRANSACTION_ID { set; get; }
        public List<DataLocalizeResponse> AddressList { set; get; } = new List<DataLocalizeResponse>();
    }
    public class DataLocalizeResponse
    {
        public string TECHNOLOGY { set; get; }
        public string ADDRESS_ID { set; get; }
        public string LISTBV_ID { set; get; }
        public string POSTAL_CODE { set; get; }
        public string BUILDING_NAME_TH { set; get; }
        public string BUILDING_NO_TH { set; get; }
        public string SUB_DISTRICT_TH { set; get; }
        public string BUILDING_NAME_EN { set; get; }
        public string BUILDING_NO_EN { set; get; }
        public string SUB_DISTRICT_EN { set; get; }
        public string PARTNER { set; get; }
        public string LATITUDE { set; get; }
        public string LONGTITUDE { set; get; }
    }
    public enum ResultMessageEnumLocalize
    {
        SUCCESS = 10000,
        FilenameDuplicate = 20001,
        DataNotFound = 20002,
        FileOverSize = 20003,
        SystemNotExit = 20004,
        IncorrectRequest = 20005
    }
}