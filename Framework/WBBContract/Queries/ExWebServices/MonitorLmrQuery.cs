using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.ExWebServices
{
    public class MonitorLmrQuery : IQuery<List<SubmitFOAEquipment>>
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
