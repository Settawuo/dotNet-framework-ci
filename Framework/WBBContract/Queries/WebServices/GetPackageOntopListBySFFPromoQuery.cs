using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPackageOntopListBySFFPromoQuery : IQuery<List<PackageModel>>
    {
        public string P_SFF_PROMOCODE { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string FullUrl { get; set; }
    }
}
