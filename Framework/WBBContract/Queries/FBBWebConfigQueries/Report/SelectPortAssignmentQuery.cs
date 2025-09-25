using System.Collections.Generic;
using WBBEntity.PanelModels.Report;

//From Config Query Report - WBBContract
namespace WBBContract.Queries.FBBWebConfigQueries.ReportPortAssignment
{
    public class SelectPortAssignmentQuery : IQuery<List<ReportPortAssignmentModel>>
    {
        public decimal CVRID { get; set; }
        public string NODENAME_TH { get; set; }
        public string ACTIVEFLAG { get; set; }

        public string FlagResult { get; set; }
        public string NodeID { get; set; }
    }
}
