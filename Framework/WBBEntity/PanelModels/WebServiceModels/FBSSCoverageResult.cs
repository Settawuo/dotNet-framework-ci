using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBSSCoverageResult
    {
        public string Coverage { get; set; }
        public string AddressId { get; set; }
        public List<FBSSAccessModeInfo> AccessModeList { get; set; }
        public FBSSAccessModeInfo PlanningSite { get; set; }
        public string IsPartner { get; set; }
        public string PartnerName { get; set; }
        public decimal InterfaceLogId { get; set; }
    }
}
