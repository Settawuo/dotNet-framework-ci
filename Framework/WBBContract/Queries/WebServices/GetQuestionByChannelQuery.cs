using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetQuestionByChannelQuery : IQuery<GetQuestionByChannelModel>
    {
        public string p_order_type { get; set; }
        public string p_channel { get; set; }
        public string p_technology { get; set; }
        public string MobileNo { get; set; }
    }
}
