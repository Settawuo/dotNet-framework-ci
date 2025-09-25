using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.Report
{
    public class UpdateInterfaceQuery : IQuery<LogInterfaceReportResponse>
    {
        public List<String> INTERFACE_ID { get; set; }
    }
}
