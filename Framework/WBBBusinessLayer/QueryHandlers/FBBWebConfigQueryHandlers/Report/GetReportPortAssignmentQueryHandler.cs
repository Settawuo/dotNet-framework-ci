using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.ReportPortAssignment;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.Report;

//From GetRptAssignment QueryHandler - WBBBusinessLayer Functions
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.Report
{
    public class GetReportPortAssignmentQueryHandler : IQueryHandler<SelectPortAssignmentQuery, List<ReportPortAssignmentModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;

        public GetReportPortAssignmentQueryHandler(ILogger logger
            , IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA
            , IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }

        public List<ReportPortAssignmentModel> Handle(SelectPortAssignmentQuery query)
        {
            List<ReportPortAssignmentModel> result;

            if (query.FlagResult == "ProjectName") //Choose ProjectName DropDownList
            {
                var list1 = (from node in _FBB_COVERAGEAREA.Get()
                             where node.ACTIVEFLAG == "Y"
                             orderby node.NODENAME_TH
                             select new ReportPortAssignmentModel
                             {
                                 NODENAME_TH = node.NODENAME_TH

                             }).Distinct();

                result = list1.ToList();
                return result;

            }

            else if (query.FlagResult == "DSLAMID") //Choose DSLAMID DropDownList
            {
                var list2 = (from dslam in _FBB_DSLAM_INFO.Get()
                             where dslam.ACTIVEFLAG == "Y" && dslam.NODEID == query.NodeID
                                   && dslam.CVRID == query.CVRID && dslam.NODEID != null
                             orderby dslam.NODEID
                             select new ReportPortAssignmentModel
                             {
                                 V_NodeID = dslam.NODEID
                                 //V_CVRID = dslam.CVRID
                             });

                result = list2.ToList();
                return result;

            }

            return null;

        }

    }
}
