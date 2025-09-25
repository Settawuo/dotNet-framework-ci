using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetFileNameCancelFBSS90DayQuery : IQuery<AutoMoveFileBatchModel>
    {
        public int P_DAY { get; set; }
    }
}
