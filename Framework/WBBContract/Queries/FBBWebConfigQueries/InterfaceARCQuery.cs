using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class InterfaceARCQuery : IQuery<List<InterfaceARCSendMailDetailList>>
    {
        public string p_type { get; set; }

        //return value
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
