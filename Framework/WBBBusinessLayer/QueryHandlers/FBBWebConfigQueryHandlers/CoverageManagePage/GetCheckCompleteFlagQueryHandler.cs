using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetCheckCompleteFlagQueryHandler : IQueryHandler<GetCheckCompleteFlagQuery, bool>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;

        public GetCheckCompleteFlagQueryHandler(ILogger logger,
                                                IEntityRepository<FBB_COVERAGEAREA> coverageArea,
                                                IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
                                                IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                IEntityRepository<FBB_PORT_INFO> portInfo)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _coverageAreaRelation = coverageAreaRelation;
            _dslamInfo = dslamInfo;
            _cardInfo = cardInfo;
            _portInfo = portInfo;
        }

        public bool Handle(GetCheckCompleteFlagQuery query)
        {
            bool result = false;
            try
            {
                #region sql
                //select count(p.portid)
                //from FBB_COVERAGEAREA C, FBB_COVERAGEAREA_RELATION CR, FBB_DSLAM_INFO D, 
                //Fbb_Card_Info CI,FBB_PORT_INFO p 
                //where c.cvrid = cr.cvrid and cr.dslamid = d.dslamid
                //      and ci.dslamid = d.dslamid
                //and  ci.cardid = p.cardid 
                //and  c.activeflag = 'Y' and cr.activeflag = 'Y'
                //and d.activeflag = 'Y' and CI.ACTIVEFLAG = 'Y' 
                //and p.activeflag = 'Y'
                //and c.cvrid = @CVRID
                #endregion

                var checkPort = from c in _coverageArea.Get()
                                join cr in _coverageAreaRelation.Get() on c.CVRID equals cr.CVRID
                                join d in _dslamInfo.Get() on cr.DSLAMID equals d.DSLAMID
                                join ci in _cardInfo.Get() on d.DSLAMID equals ci.DSLAMID
                                join p in _portInfo.Get() on ci.CARDID equals p.CARDID
                                where c.ACTIVEFLAG == "Y" && cr.ACTIVEFLAG == "Y"
                                && d.ACTIVEFLAG == "Y" && ci.ACTIVEFLAG == "Y"
                                && p.ACTIVEFLAG == "Y" && c.CVRID == query.CVRId
                                select c;

                if (checkPort.Any())
                    result = checkPort.Count() > 0 ? true : false;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }

            return result;
        }
    }
}
