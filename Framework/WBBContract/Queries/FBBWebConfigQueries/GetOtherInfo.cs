namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetOtherInfo : IQuery<string>
    {
        public string mobileno { get; set; }
        public string selectvalue { get; set; }
        public string caid { get; set; }
        public string baid { get; set; }
        // Update 18.11
        public string servicemobileno { get; set; }
    }
}
