using WBBContract;
using WBBEntity.PanelModels.WebServices;

namespace iWorkflowsContract.Queries.WebServices
{
    public class GetDbAccountQuery : IQuery<WBBDbAccount>
    {
        public string ProjectCode { get; set; }
    }
}
