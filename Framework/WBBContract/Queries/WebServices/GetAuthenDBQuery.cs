namespace WBBContract.Queries.WebServices
{
    public class GetAuthenDBQuery : IQuery<string>
    {
        public string Template { get; set; }
        public string Database { get; set; }
        public string ProjectCode { get; set; }
    }
}