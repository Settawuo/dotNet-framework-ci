using WBBEntity.PanelModels.ExWebApiModel;

namespace WBBContract.Queries.ExWebApi
{
    public class RetrieveAddressQuery : IQuery<RetrieveAddressQueryModel>
    {
        public string postal_code { set; get; }
        public string address_id { set; get; }
    }
}
