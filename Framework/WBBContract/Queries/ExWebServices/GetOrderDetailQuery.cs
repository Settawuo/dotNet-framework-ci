using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices
{
    public class GetOrderDetailQuery
    {
        public string TransactionID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public List<OrderData> ListOrder { get; set; }
        public List<CardNoData> ListCardNo { get; set; }
        public List<NonMobileNoData> ListNonMobileNo { get; set; }
        public List<ContactMobileNoData> ListContactMobileNo { get; set; }
    }

    public class OrderData
    {
        public string OrderNo { get; set; }
    }

    public class CardNoData
    {
        public string CardNo { get; set; }
    }

    public class NonMobileNoData
    {
        public string NonMobileNo { get; set; }
    }

    public class ContactMobileNoData
    {
        public string ContactMobileNo { get; set; }
    }
}
