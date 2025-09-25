using System.Collections.Generic;
using WBBContract.QueryModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class GetPackageDetailByNonMobileQuery : IQuery<GetPackageDetailByNonMobileQueryModel>
    {
        public string P_FIBRENET_ID { get; set; }
        public List<PackageFbbor051> P_FBBOR051_PACKAGE_ARRAY { get; set; }
    }

    public class PackageFbbor051
    {
        public string fibrenetId { get; set; }
        public string productCD { get; set; }
        public string productClass { get; set; }
        public string productType { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}
