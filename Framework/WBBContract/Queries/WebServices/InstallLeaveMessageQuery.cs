using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class InstallLeaveMessageQuery : IQuery<InstallLeaveMessageModel>
    {
        public string p_result_id { get; set; }
        // onservice special
        public string p_status { get; set; }
    }
}
