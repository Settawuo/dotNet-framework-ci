using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class LoginPanelModel
    {
        public string Option { get; set; }
        public string MobileNo { get; set; }
        public string IdCardNo { get; set; }
        public string IdCardType { get; set; }

        public List<DropdownModel> CardType { get; set; }
    }
}
