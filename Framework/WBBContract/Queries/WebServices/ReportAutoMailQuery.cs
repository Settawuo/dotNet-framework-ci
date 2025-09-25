using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ReportAutoMailQuery : IQuery<ReportAutoMailModel>
    {
        public string ReportId { get; set; }
        public string CreateBy { get; set; }

        public string PathTempFile { get; set; }
        public string DomainTempFile { get; set; }
        public string UserTempFile { get; set; }
        public string PassTempFile { get; set; }
    }
}
