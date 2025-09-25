using System.Collections.Generic;

namespace WBBEntity.FBBHVRModels
{
    public class GetOrderDetailHVRModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<OrderCustomerDataHVR> ReturnOrderCustomer { get; set; }
        public List<OrderPackageDataHVR> ReturnOrderPackage { get; set; }
        public List<OrderContactDataHVR> ReturnOrderContact { get; set; }
        public List<OrderBillMediaDataHVR> ReturnOrderBillMedia { get; set; }
        public List<BillingAddressDataHVR> ReturnBillingAddress { get; set; }
        public List<InstallAddressDataHVR> ReturnInstallAddress { get; set; }
        public List<OrderDocumentDataHVR> ReturnOrderDocument { get; set; }
        public List<ForOfficerDataHVR> ReturnForOfficer { get; set; }

    }

    public class OrderCustomerDataHVR
    {
        public string OrderNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string RegisterDate { get; set; }
        public string OrderStatus { get; set; }
        public string PrivilegeNo { get; set; }
        public string NonMobileNo { get; set; }
        public string RegisterChannel { get; set; }
        public string NotifyDetail { get; set; }
        public string NoteDetail { get; set; }
        public string OrderExpireDate { get; set; }
        public string CancelDate { get; set; }
    }

    public class OrderPackageDataHVR
    {
        public string OrderNo { get; set; }
        public string SFFPromotionCode { get; set; }
        public string SFFWordInStatementTha { get; set; }
        public string SFFWordInStatementEng { get; set; }
        public string PackageType { get; set; }
        public string PackageTypeDesc { get; set; }
    }

    public class OrderContactDataHVR
    {
        public string OrderNo { get; set; }
        public string AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string ContactNo { get; set; }
        public string WaitingInstallDate { get; set; }
        public string WaitingTimeSlot { get; set; }
    }

    public class OrderBillMediaDataHVR
    {
        public string OrderNo { get; set; }
        public string BillMedia { get; set; }
        public string MobileNo { get; set; }
    }

    public class BillingAddressDataHVR
    {
        public string OrderNo { get; set; }
        public string BillingAddress { get; set; }
    }

    public class InstallAddressDataHVR
    {
        public string OrderNo { get; set; }
        public string InstallAddress { get; set; }
    }

    public class OrderDocumentDataHVR
    {
        public string OrderNo { get; set; }
        public string FileName { get; set; }
    }

    public class ForOfficerDataHVR
    {
        public string OrderNo { get; set; }
        public string LocationName { get; set; }
        public string ASCCode { get; set; }
        public string EmployeeId { get; set; }
        public string CSNote { get; set; }
    }
}
