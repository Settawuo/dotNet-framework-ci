using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CreateCaseGenericFlowQuery : IQuery<CreateCaseGenericFlowModel>
    {
        //R22.08 Order Deadlock Change Service not allow
        public string MobileNo { get; set; }
        public string InteractionType { get; set; }
        public string OwnerName { get; set; }
        public string Status { get; set; }
        public string TopicName { get; set; }
        public string SubTopic { get; set; }
        public string AssignedType { get; set; }
        public string AssignedTo { get; set; }
        public string Comments { get; set; }
        public CreateCaseGenericFlowConfigCapturingList[] CapturingList { get; set; }
    }
}
