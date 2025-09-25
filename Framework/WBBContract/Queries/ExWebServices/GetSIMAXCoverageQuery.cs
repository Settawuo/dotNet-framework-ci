using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetSIMAXCoverageQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public string Province { get; set; }
        public string Aumphur { get; set; }
    }
}
