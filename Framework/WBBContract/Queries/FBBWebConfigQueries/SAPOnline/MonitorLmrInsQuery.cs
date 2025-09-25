using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class MonitorLmrInsQuery : IQuery<List<SubmitFOAInstallation>>
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
