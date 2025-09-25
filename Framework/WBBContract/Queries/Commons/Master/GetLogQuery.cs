namespace WBBContract.Queries.Commons.Master
{
    public class GetLogQuery : IQuery<string>
    {
        public string FilePath { get; set; }
        public string Date { get; set; }
        public string LogAppendedNo { get; set; }
    }
}
