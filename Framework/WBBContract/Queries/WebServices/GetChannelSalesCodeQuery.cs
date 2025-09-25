using System;
using WBBContract.QueryModels.WebServices;

namespace WBBContract.Queries.WebServices
{
    public class GetChannelSalesCodeQuery : IQuery<GetChannelSalesCodeQueryModel>
    {
        public string channelSalesCode { get; set; }
        public string userType { get; set; }
    }
}
