using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class QueryOrderDetailResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<OrderCustomer> ReturnOrderCustomer { get; set; }
        public List<OrderPackage> ReturnOrderPackage { get; set; }
        public List<OrderContact> ReturnOrderContact { get; set; }
        public List<OrderBillMedia> ReturnOrderBillMedia { get; set; }
        public List<BillingAddressItem> ReturnBillingAddress { get; set; }
        public List<InstallAddressItem> ReturnInstallAddress { get; set; }
        public List<OrderDocument> ReturnOrderDocument { get; set; }
        public List<ForOfficer> ReturnForOfficer { get; set; }
    }

    public class OrderCustomer
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

    public class OrderPackage
    {
        public string OrderNo { get; set; }
        public string SFFPromotionCode { get; set; }
        public string SFFWordInStatementTha { get; set; }
        public string SFFWordInStatementEng { get; set; }
        public string PackageType { get; set; }
        public string PackageTypeDesc { get; set; }
    }

    public class OrderContact
    {
        public string OrderNo { get; set; }
        public string AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string ContactNo { get; set; }
        public string WaitingInstallDate { get; set; }
        public string WaitingTimeSlot { get; set; }
    }

    public class OrderBillMedia
    {
        public string OrderNo { get; set; }
        public string BillMedia { get; set; }
        public string MobileNo { get; set; }
    }
    public class BillingAddressItem
    {
        public string OrderNo { get; set; }
        public string BillingAddress { get; set; }
    }

    public class InstallAddressItem
    {
        public string OrderNo { get; set; }
        public string InstallAddress { get; set; }
    }

    public class OrderDocument
    {
        public string OrderNo { get; set; }
        public string FileName { get; set; }
    }

    public class ForOfficer
    {
        public string OrderNo { get; set; }
        public string LocationName { get; set; }
        public string ASCCode { get; set; }
        public string EmployeeId { get; set; }
        public string CSNote { get; set; }
    }
}
