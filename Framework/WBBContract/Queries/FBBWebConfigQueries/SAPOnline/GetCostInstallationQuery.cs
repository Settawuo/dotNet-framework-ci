using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class GetCostInstallationQuery : IQuery<List<CostInstallation>>
    {
        public string SERVICE { get; set; }
        public string VENDOR { get; set; }
        public string ORDER_TYPE { get; set; }
        public string INS_OPTION { get; set; }
    }

}
