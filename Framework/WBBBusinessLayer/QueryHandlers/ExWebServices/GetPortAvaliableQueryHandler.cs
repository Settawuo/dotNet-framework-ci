using System;
using System.Diagnostics;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetPortAvaliableQueryHandler : IQueryHandler<GetPortAvaliableQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _covService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _covRelService;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfoService;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfoService;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardModelService;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfoService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _covAreaBuildingService;
        protected Stopwatch timer;

        public GetPortAvaliableQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> covService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> covRelService,
            IEntityRepository<FBB_DSLAM_INFO> dslamInfoService,
            IEntityRepository<FBB_CARD_INFO> cardInfoService,
            IEntityRepository<FBB_CARDMODEL> cardModelService,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> covAreaBuildingService)
        {
            _logger = logger;
            _covService = covService;
            _covRelService = covRelService;
            _dslamInfoService = dslamInfoService;
            _cardInfoService = cardInfoService;
            _cardModelService = cardModelService;
            _portInfoService = portInfoService;
            _covAreaBuildingService = covAreaBuildingService;
        }

        private void StartWatch()
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            _logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public SBNCheckCoverageResponse Handle(GetPortAvaliableQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            var avaliable = false;

            try
            {
                StartWatch();
                var portAvaQuery = new PortAvaliableQuery
                {
                    FlagOnlineNo = query.FLAGONLINENUMBER.ToYesNoFlgBoolean(),
                    Cvrid = query.CVRID,
                    CovService = _covService,
                    CovRelService = _covRelService,
                    DslamInfoService = _dslamInfoService,
                    CardInfoService = _cardInfoService,
                    CardModelService = _cardModelService,
                    PortInfoService = _portInfoService,
                    Tower = query.TOWER,
                    CovAreaBuildingService = _covAreaBuildingService,
                    Logger = _logger,
                    FlagFromWorkFlow = false,
                };

                avaliable = ExWebServiceHelper.PortAvaliable(portAvaQuery);
                StopWatch("PortAvaliable");

                response.SBNCheckCoverageData.AVALIABLE = avaliable.ToYesNoFlgString();
                response.RETURN_CODE = 0;
                response.RETURN_DESC = "";
            }
            catch (Exception ex)
            {
                response.RETURN_CODE = -1;
                response.RETURN_DESC = ex.GetErrorMessage();
            }

            return response;
        }
    }
}
