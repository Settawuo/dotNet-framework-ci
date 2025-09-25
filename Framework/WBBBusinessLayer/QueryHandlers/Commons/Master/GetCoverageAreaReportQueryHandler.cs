using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCoverageAreaReportQueryHandler : IQueryHandler<GetCoverageAreaReportQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;

        public GetCoverageAreaReportQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverageArea)
        {
            _logger = logger;
            _coverageArea = coverageArea;
        }

        public List<string> Handle(GetCoverageAreaReportQuery query)
        {
            var result = (from covArea in _coverageArea.Get()
                          select covArea.NODENAME_TH).Distinct().ToList();
            return result;
        }
    }
}
