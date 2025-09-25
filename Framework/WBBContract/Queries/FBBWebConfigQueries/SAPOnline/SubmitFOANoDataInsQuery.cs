using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOANoDataInsQuery : IQuery<List<SubmitFOAInstallation>>
    {
        public string orderNo { get; set; }
        public string internetNo { get; set; }
        public string productName { get; set; }
        public string serviceName { get; set; }
        public string status { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
