using System.Collections.Generic;

namespace WBBExternalAPI.Models.Response.EappController
{
    public class GetListWebsiteConfigurationResponse
    {
        public string RESULT_CODE { set; get; }
        public string RESULT_DESC { set; get; }
        public string TRANSACTION_ID { set; get; }
        public List<LovList> LOV_LIST { set; get; } = new List<LovList>();
    }

    public class LovList
    {
        public string DISPLAY_VAL { set; get; }
        public string LOV_VAL1 { set; get; }
        public string LOV_VAL2 { set; get; }
        public decimal? ORDER_BY { set; get; }
        public byte[] IMAGE_BLOB { set; get; }
    }

    public enum ResultMessageEnum
    {
        SUCCESS = 10000,
        FilenameDuplicate = 20001,
        DataNotFound = 20002,
        FileOverSize = 20003,
        SystemNotExit = 20004
    }
}