using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCardInfoPortPanelQuery : IQuery<List<CoveragePortPanelGrid>>
    {
        public decimal DSLAMID { get; set; }
    }
}
