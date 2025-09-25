using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOAMainAssetQuery : IQuery<List<SubmitFOAMainAsset>>
    {
        public string orderNo { get; set; }
        public string internetNo { get; set; }
        public string status { get; set; }
        public string companyCode { get; set; }
        public string assetClass { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
