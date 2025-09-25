using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetRequestFormReportQuery : IQuery<RequestFormReturn>// IQuery<List<ReportRequestFormListDetail>>
    {
        public string P_DATE_FROM { get; set; }
        public string P_DATE_TO { get; set; }

        public string P_REGION_CODE { get; set; }
        public string P_PROVINCE { get; set; }
        public string P_PROCESS_STATUS { get; set; }

        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }


    }

}