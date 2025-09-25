
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckQueryReportQuery : IQuery<ConfigurationCheckQueryModel>
    {
        public string Query { get; set; }
        public string Owner { get; set; }
    }
}
