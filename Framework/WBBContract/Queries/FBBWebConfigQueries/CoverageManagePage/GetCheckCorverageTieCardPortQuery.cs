namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCheckCorverageTieCardPortQuery : IQuery<bool>
    {
        public bool Corverage_Tie { get; set; }
        public decimal CVRID { get; set; }

    }
}
