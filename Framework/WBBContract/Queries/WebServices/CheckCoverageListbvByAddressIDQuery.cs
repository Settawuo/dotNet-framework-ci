using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckCoverageListbvByAddressIDQuery : IQuery<List<CheckCoverageListbvByAddressIDDataModel>>
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string buildingAddressID { get; set; }
        public string transactionId { get; set; }
    }
}
