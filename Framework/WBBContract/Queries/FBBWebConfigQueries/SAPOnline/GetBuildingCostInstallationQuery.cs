using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class GetBuildingCostInstallationQuery : IQuery<List<LovModel>>
    {
        public string SERVICE_TYPE { get; set; }
        public string TYPE { get; set; }
        public List<string> ADDRESS_ID { get; set; }
    }
}
