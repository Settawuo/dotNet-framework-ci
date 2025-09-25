using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class TopupPanelModel
    {
        public int Language { get; set; }
        public string MobileNo { get; set; }
        public List<DropdownModel> TopupAmount { get; set; }
        public List<DropdownModel> VisaType { get; set; }
    }
}
