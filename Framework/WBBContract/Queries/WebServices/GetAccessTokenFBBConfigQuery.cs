using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetAccessTokenFBBConfigQuery : IQuery<GetAccessTokenQueryModel>
    {
        public string grant_type { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
    }
}
