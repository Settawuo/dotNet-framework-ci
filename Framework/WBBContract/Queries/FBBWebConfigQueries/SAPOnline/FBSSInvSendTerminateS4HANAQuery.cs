using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class FBSSInvSendTerminateS4HANAQuery : IQuery<FBSSInvSendTerminateS4HANAReturn>
    {
        public string p_term_date { get; set; }
    }
}
