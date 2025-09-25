using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListPackageSellRouterV2Query : IQuery<GetListPackageSellRouterV2Model>
    {
        public string P_SALE_CHANNEL { get; set; }
        public string TransactionID { get; set; }
        public string FullUrl { get; set; }
    }

    public class GetListPackageSellRouterV2OnlineQuery : IQuery<GetListPackageSellRouterV2Model>
    {
        public string SALE_CHANNEL { get; set; }
    }
}
