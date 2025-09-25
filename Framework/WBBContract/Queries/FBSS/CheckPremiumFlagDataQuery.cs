using WBBEntity.FBSSModels;

namespace WBBContract.Queries.FBSS
{
    public class CheckPremiumFlagDataQuery : IQuery<CheckPremiumFlagModel>
    {
        public string RecurringCharge { get; set; }
        public string LocationCode { get; set; }
        public string AccessMode { get; set; }
        public string PartnerSubtype { get; set; }
        public string MemoFlag { get; set; }
        public string TransactionID { get; set; }
    }
}
