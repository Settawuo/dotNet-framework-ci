using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetDataCoverageAreaResultQuery : IQuery<DataCoverageAreaResultModel>
    {
        public decimal RESULTID { get; set; }
    }
}
