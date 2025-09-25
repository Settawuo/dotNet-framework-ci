using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    //R23.05 CheckFraud
    public class GetOnlineAuthenRequestTokenQuery : IQuery<GetOnlineAuthenRequestTokenQueryModel>
    {
        public string Transaction_Id { get; set; }
    }
}
