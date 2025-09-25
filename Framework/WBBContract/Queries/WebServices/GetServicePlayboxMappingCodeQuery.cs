using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetServicePlayboxMappingCodeQuery : IQuery<GetServicePlayboxMappingCodeModel>
    {
        public string INTERNET_NO { get; set; }
        public string HAVE_PLAY_FLAG { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string FULL_URL { get; set; }
    }
}