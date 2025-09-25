using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class CustomerProfileInfoQuery : IQuery<CustomerProfileInfoModel>
    {
        public string InternetNo { get; set; }
    }
}
