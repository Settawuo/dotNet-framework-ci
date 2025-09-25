using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCustomerTitleQuery : IQuery<List<CustomerTitleModel>>
    {
        public int CurrentCulture { get; set; }
        public string CustomerType { get; set; }
    }
}
