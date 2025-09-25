namespace WBBContract.Queries.WebServices
{
    public class CheckPopupFMCQuery : IQuery<string>
    {
        public string p_sff_promotion_code { get; set; }
        public string p_existing_mobile_flag { get; set; }
    }
}
