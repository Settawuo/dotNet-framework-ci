using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCoverageSiteQueryHandler : IQueryHandler<GetCoverageSiteQuery, List<CoverageSitePanelModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;

        public GetCoverageSiteQueryHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> coverageArea)
        {
            _logger = logger;
            _coverageArea = coverageArea;
        }
        public List<CoverageSitePanelModel> Handle(GetCoverageSiteQuery query)
        {            
            var listCoverageSite = new List<CoverageSitePanelModel>();

            var data = (from coverage in _coverageArea.Get()
                       select new CoverageSitePanelModel()
                       {
                           
                       }).ToList();

            return data;                    
        }
    }
}
