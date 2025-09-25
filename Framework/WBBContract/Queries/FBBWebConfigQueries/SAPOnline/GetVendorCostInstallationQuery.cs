using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class GetVendorCostInstallationQuery : IQuery<List<LovModel>>
    {
        public string VENDER_MODE { get; set; }
    }
}
