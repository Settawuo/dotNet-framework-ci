using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckAROrderQuery : IQuery<List<CheckAROrderModel>>
    {
        public string BroadbandId { get; set; }

        public string Option { get; set; }

        public string FullUrl { get; set; }
    }
}
