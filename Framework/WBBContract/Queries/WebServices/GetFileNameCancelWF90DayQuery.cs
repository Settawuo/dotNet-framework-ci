using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetFileNameCancelWF90DayQuery : IQuery<AutoMoveFileBatchModel>
    {
        public int P_DAY { get; set; }
    }
}
