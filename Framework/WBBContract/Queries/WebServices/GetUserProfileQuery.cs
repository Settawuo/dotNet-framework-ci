using System.Collections.Generic;
using WBBContract.QueryModels.WebServices;
namespace WBBContract.Queries.WebServices
{
    public class GetUserProfileQuery : IQuery<GetUserProfileQueryModel>
    {
        public string authToken { get; set; }
        public string userName { get; set; }
        public string locationCode { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FullUrl { get; set; }
    }
}
