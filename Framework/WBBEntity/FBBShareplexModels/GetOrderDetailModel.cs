using System.Collections.Generic;

namespace WBBEntity.FBBShareplexModels
{
    public class GetOrderDetailModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<OrderCustomerData> ReturnOrderCustomer { get; set; }
        public List<OrderPackageData> ReturnOrderPackage { get; set; }
        public List<OrderContactData> ReturnOrderContact { get; set; }
        public List<OrderBillMediaData> ReturnOrderBillMedia { get; set; }
        public List<BillingAddressData> ReturnBillingAddress { get; set; }
        public List<InstallAddressData> ReturnInstallAddress { get; set; }
        public List<OrderDocumentData> ReturnOrderDocument { get; set; }
        public List<ForOfficerData> ReturnForOfficer { get; set; }

    }

    public class OrderCustomerData
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

    public class OrderPackageData
    {
        public string OrderNo { get; set; }
        public string SFFPromotionCode { get; set; }
        public string SFFWordInStatementTha { get; set; }
        public string SFFWordInStatementEng { get; set; }
        public string PackageType { get; set; }
        public string PackageTypeDesc { get; set; }
    }

    public class OrderContactData
    {
        public string OrderNo { get; set; }
        public string AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string ContactNo { get; set; }
        public string WaitingInstallDate { get; set; }
        public string WaitingTimeSlot { get; set; }
    }

    public class OrderBillMediaData
    {
        public string OrderNo { get; set; }
        public string BillMedia { get; set; }
        public string MobileNo { get; set; }
    }

    public class BillingAddressData
    {
        public string OrderNo { get; set; }
        public string BillingAddress { get; set; }
    }

    public class InstallAddressData
    {
        public string OrderNo { get; set; }
        public string InstallAddress { get; set; }
    }

    public class OrderDocumentData
    {
        public string OrderNo { get; set; }
        public string FileName { get; set; }
    }

    public class ForOfficerData
    {
        public string OrderNo { get; set; }
        public string LocationName { get; set; }
        public string ASCCode { get; set; }
        public string EmployeeId { get; set; }
        public string CSNote { get; set; }
    }
}
