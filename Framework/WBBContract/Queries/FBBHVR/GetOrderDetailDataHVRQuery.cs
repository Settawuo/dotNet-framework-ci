using System.Collections.Generic;
using WBBContract.Queries.ExWebServices;
using WBBEntity.FBBHVRModels;
using WBBEntity.FBBShareplexModels;

namespace WBBContract.Queries.FBBHVR
{
    public class GetOrderDetailDataHVRQuery : IQuery<GetOrderDetailModel>
    {
        public string TransactionID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public List<OrderData> ListOrder { get; set; }
        public List<CardNoData> ListCardNo { get; set; }
        public List<NonMobileNoData> ListNonMobileNo { get; set; }
        public List<ContactMobileNoData> ListContactMobileNo { get; set; }
    }
}
