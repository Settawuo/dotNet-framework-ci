using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOARevalueBatchQuery : IQuery<List<SubmitFOARevalue>>
    {
        public string orderNo { get; set; }
        public string internetNo { get; set; }
        public string status { get; set; }
        public string companyCode { get; set; }
        public string mainasset { get; set; }
        public string errormessage { get; set; }
        public string action { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
