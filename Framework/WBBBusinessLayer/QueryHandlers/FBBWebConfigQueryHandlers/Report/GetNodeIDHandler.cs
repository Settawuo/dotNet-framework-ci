using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.ReportPortAssignment;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

//From GetRptAssignment QueryHandler - WBBBusinessLayer Functions
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.Report
{
    public class GetNodeIDHandler : IQueryHandler<GetNodeIDQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;

        public GetNodeIDHandler(ILogger logger
            , IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA
            , IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }

        public List<DropdownModel> Handle(GetNodeIDQuery query)
        {
            if (query.NodeNameTH == "")
            {
                var a = (from r in _FBB_DSLAM_INFO.Get()
                         where r.NODEID != null && r.ACTIVEFLAG == "Y"
                         orderby r.NODEID
                         select new DropdownModel
                         {
                             Text = r.NODEID,
                             Value = r.NODEID
                         }).ToList();

                return a;
            }
            else
            {
                var cvrId = from r in _FBB_COVERAGEAREA.Get()
                            where r.NODENAME_TH == query.NodeNameTH && r.ACTIVEFLAG == "Y"
                            select r.CVRID;

                var temp = new List<decimal?>();
                foreach (var a in cvrId)
                {
                    temp.Add(a);
                }

                return (from r in _FBB_DSLAM_INFO.Get()
                        where r.NODEID != null && r.ACTIVEFLAG == "Y" && temp.Contains(r.CVRID)
                        orderby r.NODEID
                        select new DropdownModel
                        {
                            Text = r.NODEID,
                            ID = r.DSLAMID
                        }).ToList();
            }
        }

    }
}
