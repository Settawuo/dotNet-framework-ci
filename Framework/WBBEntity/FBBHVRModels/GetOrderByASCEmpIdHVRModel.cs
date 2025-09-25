using System.Collections.Generic;

namespace WBBEntity.FBBHVRModels
{
    public class GetOrderByASCEmpIdHVRModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string TotalKeyIn { get; set; }
        public string TotalComplete { get; set; }
        public string TotalCancel { get; set; }
        public List<ReturnOrderDetailDataHVR> ReturnOrderDetail { get; set; }
    }

    public class ReturnOrderDetailDataHVR
    {
        public string OrderNo { get; set; }
        public string RegisterDate { get; set; }
        public string CustomerName { get; set; }
        public string PrivilegeNo { get; set; }
        public string MainPackage { get; set; }
        public string NotifyDetail { get; set; }
        public string NoteDetail { get; set; }
        public string OrderExpireDate { get; set; }
        public string AppointmentDate { get; set; }
        public string CancelDate { get; set; }
        public string RegisterChannel { get; set; }
        public string OrderStatus { get; set; }
    }
}
