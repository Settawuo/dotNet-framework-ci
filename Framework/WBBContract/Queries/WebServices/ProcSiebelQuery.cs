using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ProcSiebelQuery : IQuery<ProcSiebelModel>
    {
        public string P_INTERNET_ID { get; set; }
        public string P_CONTACT_MOBILE { get; set; }

        public string client_ip { get; set; }
        public string FullUrl { get; set; }
    }
}
