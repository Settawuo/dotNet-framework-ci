using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Commands.ExWebServices.FbbCpGw
{
    public class AisWiFiRegCommand : CpGateWayCommandBase
    {
        public string InternetNo { get; set; }

        public string IDCardNo { get; set; }

        public string OrderNo { get; set; }

        public string ServiceCode { get; set; }

        public string PromotionCode { get; set; }

        public evOMServiceCheckChangePromotionModel CheckChangePromotionModel { get; set; }
    }
}
