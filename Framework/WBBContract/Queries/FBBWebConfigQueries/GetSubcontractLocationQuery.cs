using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetSubcontractLocationQuery : IQuery<List<SubcontractLocationModel>>
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string cur { get; set; }
    }
}
