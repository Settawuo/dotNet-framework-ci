namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDeleteMinQuery : IQuery<decimal>
    {
        public string Lot { get; set; }
        public string RegionCode { get; set; }
    }
}
