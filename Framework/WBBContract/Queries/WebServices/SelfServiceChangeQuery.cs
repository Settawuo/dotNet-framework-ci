using WBBEntity.PanelModels.WebServiceModels;
namespace WBBContract.Queries.WebServices
{
    public class SelfServiceChangeQuery : IQuery<SelfServiceChangeModel>
    {
        public string P_ID_CARD { get; set; }
        public string P_MOBILE_NO { get; set; }
        public string P_ID_CARD_TYPE { get; set; }
        public string P_ORDER_ID { get; set; }
    }
}
