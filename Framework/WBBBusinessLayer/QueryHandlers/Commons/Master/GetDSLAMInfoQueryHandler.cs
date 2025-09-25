using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetDSLAMInfoQueryHandler : IQueryHandler<GetDSLAMInfoQuery, List<FBB_DSLAM_INFO>>
    {
        //select d.* from FBB_COVERAGEAREA c,Fbb_Coveragearea_Relation r ,FBB_DSLAM_INFO d
        //where c.nodename_th='ลุมพินีพาร์ค ริเวอร์ไซด์-พระราม3'
        //and c.cvrid = r.cvrid and r.dslamid = d.dslamid

        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverage;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;

        public GetDSLAMInfoQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverage,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
            IEntityRepository<FBB_DSLAM_INFO> dslamInfo)
        {
            _logger = logger;
            _coverage = coverage;
            _coverageAreaRelation = coverageAreaRelation;
            _dslamInfo = dslamInfo;
        }

        public List<FBB_DSLAM_INFO> Handle(GetDSLAMInfoQuery query)
        {
            var result = (from coverageArea in _coverage.Get()
                          join coverageAreaRelation in _coverageAreaRelation.Get() on coverageArea.CVRID equals coverageAreaRelation.CVRID
                          join dslamInfo in _dslamInfo.Get() on coverageAreaRelation.DSLAMID equals dslamInfo.DSLAMID
                          where coverageArea.NODENAME_TH == query.NodeName
                          select dslamInfo).ToList();

            return result;
        }
    }
}
