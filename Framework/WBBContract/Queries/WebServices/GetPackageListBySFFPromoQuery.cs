using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPackageListBySFFPromoQuery : IQuery<List<PackageModel>>
    {

        public string P_SFF_PROMOCODE { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
        public string VAS_SERVICE { get; set; }
        public string TransactionID { get; set; }

        public string FullUrl { get; set; }

    }
}
