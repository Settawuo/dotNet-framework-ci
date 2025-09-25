using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class DoServiceConfirmQuery : IQuery<DoServiceConfirmModel>
    {
        public string Url { get; set; }
        public DoServiceConfirmBody BodyData { get; set; }
        public string BodyStr { get; set; }
        public string FullUrl { get; set; }
    }

    public class DoServiceConfirmBody
    {
        public string FIBRENET_ID { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_NO { get; set; }
        public string ACTION { get; set; }
        public string REASON { get; set; }
        public string SUB_REASON { get; set; }
        public string TAG_CODE { get; set; }
        public string STAFF_ID { get; set; }
        public string REMARK { get; set; }
    }
}
