using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class SearchInterfaceLogQuery : IQuery<SearchInterfaceLogData>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string TransactionId { get; set; }
        public string MethodName { get; set; }
        public string IdCardNo { get; set; }
        public string SortColumn { get; set; }
        public string OrderBy { get; set; }
        public int PageNo { get; set; }
        public int RecordsPerPage { get; set; }
    }

    public class GetInterfaceLogMethodNameQuery : IQuery<List<string>>
    {

    }
}
