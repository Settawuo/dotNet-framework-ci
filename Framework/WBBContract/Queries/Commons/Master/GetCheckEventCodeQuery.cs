using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCheckEventCodeQuery : IQuery<EventCodeModel>
    {
        public string Event_Code { get; set; }
    }
}
