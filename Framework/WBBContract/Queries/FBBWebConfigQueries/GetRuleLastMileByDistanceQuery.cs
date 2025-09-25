using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetRuleLastMileByDistanceQuery : IQuery<List<RuleLastMileByDistanceModel>>
    {
        public string p_table_name { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
