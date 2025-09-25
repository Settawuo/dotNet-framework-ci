using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.Report
{
    public class GetExportLogInterfaceQuery : IQuery<List<LogInterfaceModel>>
    {
        public string IN_TRANSACTION_ID { get; set; }
        public string METHOD_NAME { get; set; }
        public string IN_ID_CARD_NO { get; set; }
        public string SERVICE_NAME { get; set; }
        public DateTime? CREATE_DATE_FROM { get; set; }
        public DateTime? CREATE_DATE_TO { get; set; }

        public string SortBy { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnName { get; set; }

        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}
