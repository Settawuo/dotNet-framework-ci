using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class CheckPremiumFlagQuery : IQuery<CheckPremiumFlagResponse>
    {
        public string TransactionID { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string RecurringCharge { get; set; }
        public string LocationCode { get; set; }
        public string Language { get; set; }
        public string AccessMode { get; set; }
        public string PartnerSubtype { get; set; }
        public string MemoFlag { get; set; }
    }

    public class CheckTimeSlotbySubTypeQuery : IQuery<CheckTimeSlotbySubTypeResponse>
    {
        public string TransactionID { get; set; }
        public string PartnerSubtype { get; set; }
        public string AccessMode { get; set; }
    }

    public class CheckFMCPackageQuery : IQuery<CheckFMCPackageResponse>
    {
        public string TransactionID { get; set; }
        public string MobileNo { get; set; }
        public string SFFPromotionCode { get; set; }
    }
}
