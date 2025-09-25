using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class QueryLOVForWebQuery : IQuery<QueryLOVForWebModel>
    {
        public string LovType { get; set; }
        public string LovName { get; set; }
    }
}
