using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCancelOrderQuery : IQuery<SaveOrderResp>
    {
        public string ID_Card_No { get; set; }
        public List<string> ListOrder { get; set; }
    }
}
