using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class ReportInstallationSelectSubContractorNameQuery : IQuery<List<reportInstallSelectSubContractorModel>>
    {
        public string p_code { get; set; }
        public string p_name { get; set; }
        public bool p_code_distinct { get; set; }
    }
}
