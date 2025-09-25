using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ZTEResQueryQuery : IQuery<ZTEResQueryModel>
    {
        public string PRODUCT { get; set; }
        public SPLITTER_INFO[] LISTOFSPLITTER { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string PHONE_FLAGE { get; set; }
        public DSLAM_INFO[] LISTOFDSLAM { get; set; }
        public string ADDRESS_ID { get; set; }
        public string FullUrl { get; set; }
    }
}
