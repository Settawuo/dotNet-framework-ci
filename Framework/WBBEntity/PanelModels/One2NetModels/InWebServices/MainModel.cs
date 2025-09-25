using System;

namespace WBBEntity.PanelModels.One2NetModels.InWebServices
{
    public class MainModel
    {
        public string ContactLastName { get; set; }
        public string ProductCD { get; set; }
        public bool HasMainPro { get; set; }
        public bool IsUserActive { get; set; }
        public string PackageStatus { get; set; }
        public string PackageNameThai { get; set; }
        public string SFFPromotionBillThai { get; set; }
        public string PackageNameEng { get; set; }
        public string SFFPromotionBillEng { get; set; }
        public string RecurringChange { get; set; }
        public long Amount { get; set; }
        public int TotalAmount { get; set; }
        public string PackageCode { get; set; }
        public DateTime NextCycleStartDate { get; set; }
        public int TotalDays { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }

        public string MobileNo { get; set; }
        public string CardNo { get; set; }
        public string CardType { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class MainRegisterModel
    {
        public string NonmobileNo { get; set; }
        public string Pincode { get; set; }
        public string ProjectName { get; set; }
        public string result { get; set; }

    }
}
