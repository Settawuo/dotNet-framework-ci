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
    public class GetNodeNameTHHandler : IQueryHandler<GetNodeNameTHQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;

        public GetNodeNameTHHandler(ILogger logger
            , IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA
            , IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
        }

        public List<DropdownModel> Handle(GetNodeNameTHQuery query)
        {
            //select nodename_th from fbb_coveragearea where activeflag = 'Y' group by nodename_th order by nodename_th
            return (from r in _FBB_COVERAGEAREA.Get()
                    where r.ACTIVEFLAG == "Y"
                    group r by r.NODENAME_TH into g
                    orderby g.Key
                    select new DropdownModel
                    {
                        Text = g.Key,
                        Value = g.Key
                    }).ToList();
        }

    }
}
