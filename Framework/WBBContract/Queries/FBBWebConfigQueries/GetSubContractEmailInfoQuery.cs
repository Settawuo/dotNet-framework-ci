using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetSubContractEmailInfoQuery : IQuery<List<SubContractEmailInfoModel>>
    {
        public string p_subcontract_name { get; set; }
        public string p_storage { get; set; }
        public string p_subcontract_code { get; set; }
        public string p_Action_flag { get; set; }
        // return
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
