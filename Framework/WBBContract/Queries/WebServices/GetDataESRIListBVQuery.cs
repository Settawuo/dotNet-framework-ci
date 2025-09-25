using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDataESRIListBVQuery : IQuery<CoverageAreaResultModel>
    {
        public string sitecode { get; set; }
        public string sub_district { get; set; }
        public string postcode { get; set; }
        public string province { get; set; }
        public string language { get; set; }
        public string addressid { get; set; }
    }
}
