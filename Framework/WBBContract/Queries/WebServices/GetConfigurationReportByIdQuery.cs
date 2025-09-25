using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfigurationReportByIdQuery : IQuery<ConfigurationReportModel>
    {
        public string ReportId { get; set; }
    }
}
