using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class TerminatePanelModel
    {
        //public string status { get; set; }

        public string mobileNo { get; set; }
        public string PackageNameThai { get; set; }
        public string PackageNameEng { get; set; }
        public string SFFPromotionBillThai { get; set; }
        public string SFFPromotionBillEng { get; set; }
        public int CurrentCulture { get; set; }
        public List<WBBEntity.PanelModels.DropdownModel> OrderReason { get; set; }
    }
}
