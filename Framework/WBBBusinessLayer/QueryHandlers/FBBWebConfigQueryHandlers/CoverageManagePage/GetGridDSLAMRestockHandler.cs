using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetGridDSLAMRestockHandler : IQueryHandler<GetGridDSLAMRestockQuery, List<GridDSLAMRestockModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetGridDSLAMRestockHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                           IEntityRepository<FBB_PORT_INFO> portInfo,
                                                            IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
                                                            IEntityRepository<FBB_COVERAGEAREA> coverageArea,
            IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _dslamInfo = dslamInfo;
            _portInfo = portInfo;
            _cardInfo = cardInfo;
            _coverageAreaRelation = coverageAreaRelation;
            _coverageArea = coverageArea;
            _cfgLov = cfgLov;
        }

        public List<GridDSLAMRestockModel> Handle(GetGridDSLAMRestockQuery query)
        {
            var dslamId = (from r in _coverageAreaRelation.Get()
                           where r.ACTIVEFLAG == "Y"
                           select r.DSLAMID);

            var rc = (from r in _coverageArea.Get()
                      where r.CVRID == query.CVRID
                      select r.REGION_CODE);

            var regionCode = "";

            if (rc.Any())
                regionCode = rc.FirstOrDefault();

            var data = from a in _dslamInfo.Get()
                       join cf in _cfgLov.Get() on a.REGION_CODE equals cf.LOV_NAME
                       where !dslamId.Contains(a.DSLAMID) && a.ACTIVEFLAG == "Y" && a.CVRID == query.CVRID
                       && a.NODEID != null && cf.LOV_TYPE == "REGION_CODE" && cf.LOV_NAME == regionCode
                       select new GridDSLAMRestockModel
                       {
                           DSLAMID = a.DSLAMID,
                           NodeID = a.NODEID,
                           DSALMNumber = a.DSLAMNUMBER,
                           RegionCode = a.REGION_CODE,
                           LotNumber = a.LOT_NUMBER
                       };

            return data.ToList();
            // select a. dslamid ,a.nodeid,a.dslamnumber,a.region_code,a.lot_number from fbb_dslam_info a,FBB_CFG_LOV cf 
            //where a.dslamid not in (select b.dslamid from fbb_coveragearea_relation b where b.activeflag='Y')
            //and a.activeflag= 'Y'
            //and a.cvrid = @CVRID
            //and a.region_code = cf.lov_name
            //and a.nodeid != NULL
            //and cf.lov_type = 'REGION_CODE' and cf.lov_name='@REGION_CODE';

        }
    }
}
