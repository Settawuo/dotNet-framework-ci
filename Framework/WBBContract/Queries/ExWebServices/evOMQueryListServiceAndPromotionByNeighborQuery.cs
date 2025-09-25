using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{

    public class evOMQueryListServiceAndPromotionByNeighborQuery : IQuery<evOMQueryListServiceAndPromotionByNeighborModel>
    {
        //InputParameter 
        public string mobileNo { get; set; }
        //public string idCard { get; set; }
        //public string promotionType { get; set; }
        //public string serviceType { get; set; }

        // Update 17.6
        public string FullUrl { get; set; }
    }
}
