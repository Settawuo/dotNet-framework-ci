using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPackageListBySFFPromoV2Query : IQuery<List<PackageModel>>
    {

        public string P_SFF_PROMOCODE { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; }
        public string P_OWNER_PRODUCT { get; set; }
        public string P_EXISTING_REQ { get; set; }
        public string P_INTERNET_NO { get; set; }
        //R21.5
        public string P_SALE_CHANNEL { get; set; }

        public string TransactionID { get; set; }
        public string FullUrl { get; set; }
    }

    public class GetPackageListBySFFPromoOnlineQuery : IQuery<List<PackageModel>>
    {
        public string SFF_PROMOCODE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string EXISTING_REQ { get; set; }
        public string INTERNET_NO { get; set; }
        //R21.5
        public string SALE_CHANNEL { get; set; }
    }
}
