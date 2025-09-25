using WBBEntity.Minions;

namespace WBBContract.Minions
{
    public class MinionGetExternalSoapServiceQuery : IQuery<MinionGetExternalSoapServiceQueryModel>
    {
        public string UrlEnpoint { get; set; }
        public string SoapHeader { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public string Charset { get; set; }
        public string RequestData { get; set; }
        public string Server { get; set; }
    }
}
