using System.Collections.Generic;

namespace WBBContract.Queries.ExWebServices.ATN
{
    public class CustomerProfileFreeUnitItem
    {
        public string freeUnitId { get; set; }
        public string freeUnitName { get; set; }
        public string measureUnitName { get; set; }
        public List<CustomerProfileFreeUnitItemDetail> freeUnitItemDetailList { get; set; }
    }

    public class CustomerProfileFreeUnitItemDetail
    {
        public string remainingAmount { get; set; }
        public string initialAmount { get; set; }
        public string freeUnitEffectiveTime { get; set; }
        public string freeUnitExpireTime { get; set; }
    }
}
