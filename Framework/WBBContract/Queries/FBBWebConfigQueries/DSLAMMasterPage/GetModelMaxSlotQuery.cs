namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetModelMaxSlotQuery : IQuery<decimal>
    {
        public string Model { get; set; }
    }
}
