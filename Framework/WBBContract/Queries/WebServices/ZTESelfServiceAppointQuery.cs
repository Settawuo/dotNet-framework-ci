using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class ZTESelfServiceAppointQuery : IQuery<ZTESelfServiceAppointModel>
    {
        public string FIBRENET_ID { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASSIGN_RULE { get; set; }
        public string CHANNEL { get; set; }
        public string INSTALL_STAFF_CODE { get; set; }

        public string FullUrl { get; set; }
        public string ID_CARD_NO { get; set; }
    }
}
