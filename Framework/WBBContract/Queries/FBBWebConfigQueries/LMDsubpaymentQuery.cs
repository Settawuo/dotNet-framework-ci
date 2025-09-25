namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class LMDsubpaymentQuery : IQuery<LMDsubpaymentReturn>
    {
    }
    public class LMDsubpaymentReturn
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

}
