using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class FBSSInvSendTerminateQuery : IQuery<FBSSInvSendTerminateReturn>
    {
        public string p_term_date { get; set; }
    }
}
