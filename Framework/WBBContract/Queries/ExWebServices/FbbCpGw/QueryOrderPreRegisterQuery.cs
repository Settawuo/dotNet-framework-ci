using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class QueryOrderPreRegisterQuery : IQuery<List<PreRegisterModel>>
    {
        public string TransactionID { get; set; }
        public string LocationCode { get; set; }
        public string AscCode { get; set; }
        public string QMonth { get; set; }

    }
}
