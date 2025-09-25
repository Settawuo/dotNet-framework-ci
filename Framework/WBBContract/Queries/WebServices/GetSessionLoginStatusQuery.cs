using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSessionLoginStatusQuery : IQuery<SessionLoginStatusModel>
    {
        public string CustInternetNum { get; set; }
        public string SessionId { get; set; }
    }
    public class GetSessionLoginDateQuery : IQuery<SessionLoginStatusModel>
    {
        public string CustInternetNum { get; set; }

    }
}

