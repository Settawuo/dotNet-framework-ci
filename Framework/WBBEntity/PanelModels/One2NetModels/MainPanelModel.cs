using System;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class MainPanelModel
    {
        public string ContactName { get; set; }
        public string PackageNameThai { get; set; }
        public string SFFPromotionBillThai { get; set; }
        public long Amount { get; set; }
        public int TotalAmount { get; set; }
        public string PackageCode { get; set; }
        public string PackageStatus { get; set; }
        public DateTime NextCycleStartDate { get; set; }
        public string InternetNumber { get; set; }
        public int TotalDays { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }
    }
}
