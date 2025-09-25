using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBSS
{
    public class CheckInOrderQuery : IQuery<CheckInOrderModel>
    {
        public string queryString { get; set; }
    }

}
