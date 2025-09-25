using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ZTEResReservedQuery : IQuery<ZTEResReservedModel>
    {
        public string PRODUCT { get; set; }
        public SPLITTER_INFO[] LISTOFSPLITTER { get; set; }
        public DSLAM_INFO[] LISTOFDSLAM { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string PHONE_FLAG { get; set; }
        public string ADDRESS_ID { get; set; }
    }
}
