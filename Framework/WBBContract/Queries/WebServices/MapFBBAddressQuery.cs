using WBBContract.QueryModels;

namespace WBBContract.Queries.WebServices
{
    public class MapFBBAddressQuery : IQuery<MapFBBAddressModel>
    {
        public string FullUrl { get; set; }
        public string transaction_id { get; set; }
        public string p_province { get; set; }
        public string p_amphur { get; set; }
        public string p_tumbon { get; set; }
        public string p_zipcode { get; set; }
    }
}
