using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetExportPaymentQuery : IQuery<List<ExportPaymentModel>>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string InternetNo { get; set; }
        public string SortBy { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnName { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }
    }
}

