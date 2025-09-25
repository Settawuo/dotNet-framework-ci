using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSessionLoginQuery : IQuery<List<SessionLoginModel>>
    {
        public string SessionId { get; set; }
    }
}
