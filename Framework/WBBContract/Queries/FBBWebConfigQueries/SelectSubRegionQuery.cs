namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectSubRegionQuery : IQuery<string>
    {
        public string rowid { get; set; }
        public int currentculture { get; set; }
    }
}
