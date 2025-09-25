using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetTableAirFbbQuery : IQuery<List<GetTableAirFbbModel>>
    {
        public string ACTION{ get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SERVICE_CODE { get; set; }
        public string PACKAGE_TYPE { get; set; }
    }
}
