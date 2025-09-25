using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class FBBReconcileInvRPTBatchModel
    {
        public FBBReconcileInvRPTBatchModel()
        {
            if (cur == null)
            {
                cur = new List<FBBReconcileInvRPTBatchModel_MSG>();
            }
            if (cur2 == null)
            {
                cur2 = new List<FBBReconcileInvRPTBatchModel_Cur2>();
            }
        }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<FBBReconcileInvRPTBatchModel_MSG> cur { get; set; }
        public List<FBBReconcileInvRPTBatchModel_Cur2> cur2 { get; set; }
    }
    public class FBBReconcileInvRPTBatchModel_MSG
    {
        public string ord_no { get; set; }
    }
    public class FBBReconcileInvRPTBatchModel_Cur2
    {
        public string ACCESS_NUMBER { get; set; }
    }
}
