namespace WBBContract.Queries.Commons.Masters
{
    public class GetSendMailQuery : IQuery<string>
    {
        public int CurrentCulture { get; set; }
        public string SendTo { get; set; }
        public string FilePath { get; set; }
    }
}
