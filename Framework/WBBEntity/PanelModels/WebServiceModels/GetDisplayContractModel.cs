using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetDisplayContractModel
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<DisplayContractData> DisplayContractDatas { get; set; }
    }

    public class DisplayContractData
    {
        public string FIBRENETID { get; set; }
        public string CONTRACT_NO { get; set; }
        public string GROUP_NAME { get; set; }
        public string PACKAGE_DISPLAY { get; set; }
        public string DISPLAY_SEQ { get; set; }
        public string CONTRACT_DISPLAY_TH_1 { get; set; }
        public string CONTRACT_DISPLAY_TH_2 { get; set; }
        public string CONTRACT_DISPLAY_EN_1 { get; set; }
        public string CONTRACT_DISPLAY_EN_2 { get; set; }
    }
}
