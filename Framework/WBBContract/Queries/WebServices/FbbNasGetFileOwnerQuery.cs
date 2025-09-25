namespace WBBContract.Queries.WebServices
{
    public class FbbNasGetFileOwnerQuery : IQuery<string>
    {
        public string file_name { get; set; }
    }
}
