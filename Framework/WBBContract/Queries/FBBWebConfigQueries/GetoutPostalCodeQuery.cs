namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetoutPostalCodeQuery : IQuery<string>
    {
        public string outPostalCode { get; set; }
        public string outTumbol { get; set; }
        public string outAmphur { get; set; }
        public string outProvince { get; set; }
        public string FagDataQuery { get; set; }

    }
}
