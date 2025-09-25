using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetEventDetailQuery : IQuery<EventDetailModel>
    {
        public string DatePeriodTH { get; set; }
        public string DatePeriodEN { get; set; }
        public string EventCode { get; set; }
        public string IDCardNo { get; set; }
        public bool Language { get; set; }
        public string Technology { get; set; }

    }
}
