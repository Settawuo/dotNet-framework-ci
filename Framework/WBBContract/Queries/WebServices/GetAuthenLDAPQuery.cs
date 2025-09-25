namespace WBBContract.Queries.WebServices
{
    public class GetAuthenLDAPQuery : IQuery<bool>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ProjectCode { get; set; }
    }
}
