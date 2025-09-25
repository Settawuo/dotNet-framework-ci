using System.Collections.Generic;

namespace WBBEntity.PanelModels.One2NetModels
{
    public class ChangeWifiPassPanelModel
    {
        public bool ModeEdit { get; set; }
        public string TransectionID { get; set; }
        public string AccountField { get; set; }
        public string UserIDField { get; set; }
        public string mobileNo { get; set; }
        public List<DropdownModel> SSID { get; set; }
        public List<SSIDInfo> SSIDInfoList { get; set; }
        public string SSIDSelect { get; set; }
        public string ENABLEStatus { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class SSIDInfo
    {
        public string KEY { get; set; }
        public string SSID { get; set; }
        public string CHANNEL { get; set; }
        public string ENABLE { get; set; }
    }
}
