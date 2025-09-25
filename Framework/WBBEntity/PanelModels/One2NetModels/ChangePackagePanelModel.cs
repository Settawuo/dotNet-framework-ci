using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.One2NetModels.InWebServices;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class ChangePackagePanelModel
    {
        public string ReturnCode { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        //public string CurrentPackageNameTha { get; set; }
        //public string CurrentPackageNameEng { get; set; }
        //public string CurrentPromotionBillTha { get; set; }
        //public string CurrentPromotionBillEng { get; set; }
        //public decimal RecurringCharge { get; set; }

        public string Mobile_No { get; set; }
        public DateTime NextCycleStartDate { get; set; }
        public string DORMITARY_NAME { get; set; }
        public string DORMITARY_NO { get; set; }
        public string PackageNameThai { get; set; }
        public int CurrentCulture { get; set; }
        public long Amount { get; set; }
        // public decimal recurringCharge { get; set; }

        public List<PackageByServiceModel> Package { get; set; }
        public List<PackageByServiceModel> Packlist_DISPLAY { get; set; }
        public List<PackageByServiceModel> Package_USER { get; set; }
    }
}
