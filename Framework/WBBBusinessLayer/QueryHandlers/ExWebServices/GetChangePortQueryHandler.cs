using System;
using System.Diagnostics;
using System.Linq;
using WBBContract;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetChangePortQueryHandler : IQueryHandler<GetChangePortQuery, SBNCheckCoverageResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _covService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _covRelService;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfoService;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfoService;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardModelService;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfoService;
        private readonly IEntityRepository<FBB_PORT_NOTE> _portNoteService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _covAreaBuildingService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        protected Stopwatch timer;

        public GetChangePortQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_COVERAGEAREA> covService,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> covRelService,
            IEntityRepository<FBB_DSLAM_INFO> dslamInfoService,
            IEntityRepository<FBB_CARD_INFO> cardInfoService,
            IEntityRepository<FBB_CARDMODEL> cardModelService,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_PORT_NOTE> portNoteService,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> covAreaBuildingService,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _covService = covService;
            _covRelService = covRelService;
            _dslamInfoService = dslamInfoService;
            _cardInfoService = cardInfoService;
            _cardModelService = cardModelService;
            _portInfoService = portInfoService;
            _portNoteService = portNoteService;
            _uow = uow;
            _covAreaBuildingService = covAreaBuildingService;
            _lov = lov;
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

        public SBNCheckCoverageResponse Handle(GetChangePortQuery query)
        {
            var response = new SBNCheckCoverageResponse();
            var available = false;

            try
            {
                var changePortCfg = (from l in _lov.Get()
                                     where l.LOV_TYPE == "CHANGEPORT_REASON"
                                     && l.LOV_NAME == query.CHANGEPORT_REASON
                                     && l.ACTIVEFLAG == "Y"
                                     select new
                                     {
                                         ReasonCode = l.LOV_NAME,
                                         OldPortStatus = l.LOV_VAL1,
                                         NewPortStatus = l.LOV_VAL2,
                                     }).FirstOrDefault();

                if (null == changePortCfg)
                {
                    response.RETURN_CODE = -1;
                    response.RETURN_DESC = "Cannot Find Change Port Configuration.";

                    return response;
                }

                if (WBBExtensions.ParseEnum<PortStatus>(changePortCfg.NewPortStatus) == PortStatus.None
                    || WBBExtensions.ParseEnum<PortStatus>(changePortCfg.OldPortStatus) == PortStatus.None)
                {
                    response.RETURN_CODE = -1;
                    response.RETURN_DESC = "Cannot Find Change Port Configuration.";

                    return response;
                }

                // เมื่อต้องการ port ใหม่ ต้องระบุ new port status
                if (WBBExtensions.ParseEnum<PortStatus>(changePortCfg.NewPortStatus) != PortStatus.None)
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
                        FlagFromWorkFlow = true,
                    };

                    available = ExWebServiceHelper.PortAvaliable(portAvaQuery);

                    response.SBNCheckCoverageData.AVALIABLE = available.ToYesNoFlgString();

                    StopWatch("PortAvaliable");

                    if (available)
                    {
                        StartWatch();

                        var portNoQuery = new PortNumberQuery
                        {
                            FlagOnlineNo = query.FLAGONLINENUMBER.ToYesNoFlgBoolean(),
                            Technology = query.NETWORKTECHNOLOGY,
                            Cvrid = query.CVRID,
                            CovService = _covService,
                            CovRelService = _covRelService,
                            DslamInfoService = _dslamInfoService,
                            CardInfoService = _cardInfoService,
                            CardModelService = _cardModelService,
                            PortInfoService = _portInfoService,
                            Tower = query.TOWER,
                            CovAreaBuildingService = _covAreaBuildingService,

                            IsDescending = true,
                        };

                        var port = ExWebServiceHelper.GetPortNo(portNoQuery);
                        StopWatch("GetPortNo");

                        if (null == port)
                        {
                            response.SBNCheckCoverageData.AVALIABLE = "N";
                        }
                        else
                        {
                            response.SBNCheckCoverageData.NODEID = port.NODEId;
                            response.SBNCheckCoverageData.MAXIMUMPORT = port.MaxPort;

                            StartWatch();
                            ExWebServiceHelper.LogBeforeChangePort(port,
                                WBBExtensions.ParseEnum<PortStatus>(changePortCfg.NewPortStatus),
                                query.REFF_USER,
                                query.REFF_KEY,
                                _uow,
                                _portInfoService,
                                _portNoteService);
                            StopWatch("LogAfterChangePort");

                            response.SBNCheckCoverageData.NEWPORTID = (int)port.PortId;
                            response.SBNCheckCoverageData.NEWPORTDESC = port.PortDescription;
                            response.SBNCheckCoverageData.DATAPORTDESC =
                                string.Format("D{0}/{1}", port.CardNumber, port.PortNumber);

                            if (!port.DataOnlyFlag.ToYesNoFlgBoolean())
                                response.SBNCheckCoverageData.VOICEPORTDESC =
                                    string.Format("V{0}/{1}", port.CardNumber, port.PortNumber);
                        }
                    }
                }

                // new port status == none เพื่อ update port เก่าเท่านั้น
                // หรือ port available และ ต้องระบุ old port statusเพื่อ update port เก่า
                if (WBBExtensions.ParseEnum<PortStatus>(changePortCfg.NewPortStatus) == PortStatus.None
                    || (available && WBBExtensions.ParseEnum<PortStatus>(changePortCfg.OldPortStatus)
                                        != PortStatus.None))
                {
                    StartWatch();
                    ExWebServiceHelper.LogAfterChangePort(query.CURRENTPORTID,
                        WBBExtensions.ParseEnum<PortStatus>(changePortCfg.OldPortStatus),
                        query.REFF_USER,
                        query.REFF_KEY,
                        _uow,
                        _portInfoService,
                        _portNoteService);
                    StopWatch("LogAfterChangePort");
                }

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
