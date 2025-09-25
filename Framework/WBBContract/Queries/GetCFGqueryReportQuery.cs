using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetCFGqueryReportQuery : IQuery<List<CFGqueryReportModel>>
    {
        public string Sheet_Name { get; set; }
    }
}
