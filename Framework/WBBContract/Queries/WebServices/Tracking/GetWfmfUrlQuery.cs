using System.Collections.Generic;

namespace WBBContract.Queries.WebServices.Tracking
{
    public class GetWfmfUrlQuery : IQuery<GetWfmfUrlModel>
    {
        public string fbbId { get; set; }
    }

    public class GetWfmfUrlModel
    {
        public string resultCode { get; set; }
        public string developerMessage { get; set; }
        public List<OnsiteTrackingData> resultData { get; set; }
    }
}
