using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetAccessTokenQuery : IQuery<GetAccessTokenQueryModel>
    {
        public string authorization { get; set; }
        public string grant_type { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
    }
}
