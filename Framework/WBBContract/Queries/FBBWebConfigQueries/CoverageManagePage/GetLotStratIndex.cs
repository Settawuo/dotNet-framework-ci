namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetLotStratIndex : IQuery<decimal>
    {
        public decimal CVRID { get; set; }
        public bool Coverage_TieFax { get; set; }
    }
}
