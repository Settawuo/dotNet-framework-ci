using AIRNETEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class SearchAirnetInterfaceLogQuery : IQuery<SearchAirnetInterfaceLogData>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string OrderNo { get; set; }
        public string SortColumn { get; set; }
        public string OrderBy { get; set; }
        public int PageNo { get; set; }
        public int RecordsPerPage { get; set; }
    }
}
