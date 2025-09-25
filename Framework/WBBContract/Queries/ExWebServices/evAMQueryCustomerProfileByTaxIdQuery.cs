using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evAMQueryCustomerProfileByTaxIdQuery : IQuery<evAMQueryCustomerProfileByTaxIdModel>
    {
        public string TAX_ID { get; set; }
    }
}
