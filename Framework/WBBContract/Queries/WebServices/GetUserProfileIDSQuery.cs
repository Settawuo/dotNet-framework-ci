
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetUserProfileIDSQuery : IQuery<GetProfileQueryModel>
    {
        public string access_token { get; set; }
    }
}
