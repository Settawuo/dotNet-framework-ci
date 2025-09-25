using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ZTEReleaseTimeslotQuery : IQuery<ZTEReleaseTimeslotModel>
    {
        public string RESERVED_ID { get; set; }
        public string FullUrl { get; set; }
        public string ID_CARD_NO { get; set; }
    }
}
