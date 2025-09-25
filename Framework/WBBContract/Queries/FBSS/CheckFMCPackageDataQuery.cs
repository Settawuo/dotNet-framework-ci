using WBBEntity.FBSSModels;

namespace WBBContract.Queries.FBSS
{
    public class CheckFMCPackageDataQuery : IQuery<CheckFMCPackageModel>
    {
        public string p_mobile_price_excl_vat { get; set; }
        public string p_project_name { get; set; }
        public string p_sff_promotion_code { get; set; }
        public string TransactionID { get; set; }
    }
}
