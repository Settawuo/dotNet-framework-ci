namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetMaxDLQuery : IQuery<decimal>
    {
        public decimal CVRID { get; set; }
        public decimal CarID { get; set; }
        public decimal Dsalam { get; set; }
        public string Node_ID { get; set; }

    }
}
