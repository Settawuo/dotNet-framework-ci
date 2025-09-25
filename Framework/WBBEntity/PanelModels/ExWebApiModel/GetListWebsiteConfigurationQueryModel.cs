using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebApiModel
{
    public class GetListWebsiteConfigurationQueryModel
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
        public static LovList CreateLovList(LovExternalWebApiList lov)
        {
            LovList lovResult = new LovList();
            lovResult.DISPLAY_VAL = lov.DISPLAY_VAL;
            lovResult.LOV_VAL1 = lov.LOV_VAL1;
            lovResult.LOV_VAL2 = lov.LOV_VAL2;
            lovResult.ORDER_BY = lov.ORDER_BY;
            lovResult.IMAGE_BLOB = lov.IMAGE_BLOB;
            return lovResult;
        }
    }
    public class LovExternalWebApiList
    {
        public string DISPLAY_VAL { set; get; }
        public string LOV_VAL1 { set; get; }
        public string LOV_VAL2 { set; get; }
        public decimal? ORDER_BY { set; get; }
        public byte[] IMAGE_BLOB { set; get; }
        public double FORMAT_TYPE { set; get; }
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
