using WBBEntity.Models;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetRegisterPendingDeductionModel
    {
        public FBB_REGISTER_PENDING_DEDUCTION Data { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }
}