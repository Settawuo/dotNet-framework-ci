using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPackageSequenceQuery : IQuery<GetPackageSequenceModel>
    {
        public string P_FIBRENET_ID { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string P_ACCESS_MODE { get; set; }
        public string P_PROMOTION_CODE { get; set; }
    }

    public class CheckUsePoinBySFFPromotionCodeQuery : IQuery<CheckUsePoinBySFFPromotionCodeModel>
    {
        public string SFF_PROMOTION_CODE { get; set; }
    }
}
