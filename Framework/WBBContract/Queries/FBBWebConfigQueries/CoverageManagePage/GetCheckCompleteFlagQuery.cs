namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCheckCompleteFlagQuery : IQuery<bool>
    {
        public decimal CVRId { get; set; }
    }
}
