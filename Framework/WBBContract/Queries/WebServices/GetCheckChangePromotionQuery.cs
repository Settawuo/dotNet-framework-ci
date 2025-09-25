using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCheckChangePromotionQuery : IQuery<CheckChangePromotionModelLine4>
    {
        public string mobileNo { get; set; }
        public string promotionType { get; set; }
        public string promotionCd { get; set; }
        public string orderChannel { get; set; }


    }
}
