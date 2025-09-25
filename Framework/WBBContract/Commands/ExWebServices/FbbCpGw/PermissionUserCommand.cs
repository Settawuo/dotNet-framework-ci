using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class PermissionUserCommand
    {
        public string P_TRANSACTION_NO { get; set; }
        public string P_ACTION { get; set; }
        //Return
        public string P_RETURN_CODE { get; set; }
        public string P_RETURN_MESSAGE { get; set; }

        public List<DETAIL_USER_RESPONSE> IO_PROCESS_FAIL { get; set; }
        public List<DETAIL_USER> P_FBBOR045_ACIM_ARRAY { get; set; }
    }
}
