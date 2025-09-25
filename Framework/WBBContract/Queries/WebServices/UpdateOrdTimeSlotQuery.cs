using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class UpdateOrdTimeSlotQuery : IQuery<UpdateOrdTimeSlotModel>
    {
        public string P_ORDER_NO { get; set; }
        public string P_INSTALL_DATE { get; set; }
        public string P_TIME_SLOT { get; set; }
        public string P_RESERVED_ID { get; set; }
        public string P_USER { get; set; }
        public string ID_CARD_NO { get; set; }
    }
}
