using WBBEntity.PanelModels.WebServiceModels;
namespace WBBContract.Queries.WebServices
{
    public class ChangeCountInstallDateQuery : IQuery<ChangeCountInstallDateQueryModel>
    {
        public string p_id_card { get; set; }
        public string p_order_no { get; set; }
    }
}
