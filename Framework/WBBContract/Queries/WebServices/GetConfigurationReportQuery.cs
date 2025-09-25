using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfigurationReportQuery : IQuery<List<ConfigurationReportModel>>
    {
        public string ReportName { get; set; }
        public string Scheduler { get; set; }
        public string SortBy { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnName { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
        public int PageNo { get; set; }
        public int RecordsPerPage { get; set; }
    }
}
