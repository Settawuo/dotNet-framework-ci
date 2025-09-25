using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFixedOM010SendMailFileLogQuery : IQuery<ReturnFBSSSendMailFileLogBatchModel>
    {
    }

    public class GetOM010EMailNotifyQuery : IQuery<ReturnFBSSOM010Notify>
    {
        public int ret_code { get; set; }
        public string msg { get; set; }
    }
}
