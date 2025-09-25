using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class FBBTransactionWorkflowQuery : IQuery<FBBTransactionWorkflowModel>
    {
        public string DomainHost { get; set; }
        public string UserHost { get; set; }
        public string PassHost { get; set; }
        public string TargetArchivePath { get; set; }
        public string TargetDomainPath { get; set; }

    }
}
