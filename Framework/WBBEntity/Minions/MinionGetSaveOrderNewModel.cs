using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.Minions
{
    public class MinionGetSaveOrderNewModel
    {
        public SaveOrderResp MinionSaveOrderResponse { get; set; }
        public SaveOrderResp MinionSaveOrderResponseVoIp { get; set; }
        public string ReturnIaNo { get; set; }
        public string ReturnIaNoVoIp { get; set; }
    }
}