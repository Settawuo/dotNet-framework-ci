using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetListSubContractQuery : IQuery<List<ListSubcontractModel>>
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string cur { get; set; }

    }
}
