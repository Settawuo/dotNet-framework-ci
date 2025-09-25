namespace WBBEntity.PanelModels.WebServiceModels
{
    //R22.08 Order Deadlock Change Service not allow

    public class CreateCaseGenericFlowModel
    {
        public string CaseID { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CreateCaseGenericFlowConfigUrlModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string BodyStr { get; set; }
    }

    public class CreateCaseGenericFlowConfigBody
    {
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

    public class CreateCaseGenericFlowConfigCapturingList
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
    }
}
