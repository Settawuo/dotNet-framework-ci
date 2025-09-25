using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetFileNameCompleted90DayQuery : IQuery<AutoMoveFileBatchModel>
    {
        public int P_DAY { get; set; }
    }
}
