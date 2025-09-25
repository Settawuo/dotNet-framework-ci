namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using System.Linq;
    using WBBBusinessLayer.QueryHandlers.WebServices;
    using WBBContract;
    using WBBContract.Queries.ExWebServices;
    using WBBContract.Queries.ExWebServices.FbbCpGw;
    using WBBContract.Queries.WebServices;
    using WBBData.Repository;
    using WBBEntity.Extensions;
    using WBBEntity.Models;

    public class GetCoverageResultQueryHandler : IQueryHandler<GetCoverageResultQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRel;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _covRegion;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipcode;

        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfoService;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfoService;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardModelService;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfoService;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _covAreaBuildingService;

        private readonly IEntityRepository<FBB_APCOVERAGE> _apCovService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetCoverageResultQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> coverageArea,
            IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRel,
            IEntityRepository<FBB_COVERAGE_REGION> covRegion,
            IEntityRepository<FBB_ZIPCODE> zipcode,
            IEntityRepository<FBB_DSLAM_INFO> dslamInfoService,
            IEntityRepository<FBB_CARD_INFO> cardInfoService,
            IEntityRepository<FBB_CARDMODEL> cardModelService,
            IEntityRepository<FBB_PORT_INFO> portInfoService,
            IEntityRepository<FBB_COVERAGEAREA_BUILDING> covAreaBuildingService,
            IEntityRepository<FBB_APCOVERAGE> apCovService,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _coverageAreaRel = coverageAreaRel;
            _covRegion = covRegion;
            _zipcode = zipcode;

            _dslamInfoService = dslamInfoService;
            _cardInfoService = cardInfoService;
            _cardModelService = cardModelService;
            _portInfoService = portInfoService;
            _covAreaBuildingService = covAreaBuildingService;
            _apCovService = apCovService;
            _lov = lov;
        }

        public List<string> Handle(GetCoverageResultQuery query)
        {
            var result = new List<string>();
            decimal? cvrid = null;
            var lat = "";
            var lng = "";
            var ownerProduct = "";
            try
            {
                var zipcodeRowId = (from z in _zipcode.Get()
                                    where z.PROVINCE == query.Province
                                    && z.AMPHUR == query.District
                                    && z.TUMBON == query.SubDistrict
                                    select z.ZIPCODE_ROWID).FirstOrDefault();

                if (query.BuildingType == "CONDOMINIUM")
                {
                    #region CONDOMINUIM
                    var coverageAreaList = (from a in _coverageArea.Get()
                                            join r in _coverageAreaRel.Get() on a.CVRID equals r.CVRID
                                            where a.NODESTATUS == "ON_SITE"
                                            && a.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y"
                                            select new { a, r });

                    if (query.Language.ToCultureCode().IsThaiCulture())
                    {
                        var coverage = (from t in coverageAreaList
                                        where (t.a.NODENAME_TH == query.BuildingName)
                                           && (string.IsNullOrEmpty(query.Tower) || t.r.TOWERNAME_TH == query.Tower)
                                        select new { t.a.CVRID, t.r.LATITUDE, t.r.LONGITUDE })
                                            .FirstOrDefault();

                        if (null != coverage)
                        {
                            cvrid = coverage.CVRID;
                            lat = coverage.LATITUDE;
                            lng = coverage.LONGITUDE;
                        }

                    }
                    else
                    {
                        var coverage = (from t in coverageAreaList
                                        where (t.a.NODENAME_EN == query.BuildingName)
                                                      && (string.IsNullOrEmpty(query.Tower) || t.r.TOWERNAME_EN == query.Tower)
                                        select new { t.a.CVRID, t.r.LATITUDE, t.r.LONGITUDE }).FirstOrDefault();

                        if (null != coverage)
                        {
                            cvrid = coverage.CVRID;
                            lat = coverage.LATITUDE;
                            lng = coverage.LONGITUDE;
                        }
                    }

                    #endregion
                }

                ownerProduct = FindOwnerProduct(query, cvrid);

                result.Add(ownerProduct);
                result.Add(cvrid.GetValueOrDefault().ToSafeString());
                result.Add(zipcodeRowId);
                result.Add(lat);
                result.Add(lng);

                return result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private string FindOwnerProduct(GetCoverageResultQuery query, decimal? cvrid)
        {
            var listOfOwner = new List<string>();

            if (cvrid.HasValue)
            {
                var avaliable = false;

                // check port available
                var portAvaQuery = new PortAvaliableQuery
                {
                    FlagOnlineNo = query.OnlineNumberFlag.ToYesNoFlgBoolean(),
                    Cvrid = cvrid.GetValueOrDefault(),
                    CovService = _coverageArea,
                    CovRelService = _coverageAreaRel,
                    DslamInfoService = _dslamInfoService,
                    CardInfoService = _cardInfoService,
                    CardModelService = _cardModelService,
                    PortInfoService = _portInfoService,
                    Tower = query.Tower,
                    CovAreaBuildingService = _covAreaBuildingService,
                    Logger = _logger,
                    FlagFromWorkFlow = false,
                };

                avaliable = ExWebServiceHelper.PortAvaliable(portAvaQuery);

                // มี port ว่าง
                if (avaliable)
                {
                    return "WireBB";
                }
            }

            // หา symphony
            var findOwnerQuery = new GetOwnerByBuildingQuery
            {
                Building = query.BuildingName,
                LanguageFlag = query.Language.ToCultureCode().ToSafeString(),
            };

            var ownerOfBuilding = GetOwnerByBuildingHelper
                .GetOwnerByBuilding(_logger, _covRegion, findOwnerQuery);

            // เจอ symphony,NSN,SIMAT
            if (!string.IsNullOrEmpty(ownerOfBuilding.OWNER_PRODUCT))
            {
                listOfOwner.Add(ownerOfBuilding.OWNER_PRODUCT);
                return string.Join(",", listOfOwner);
            }
            // ไม่เจอ symphony,NSN,SIMAT
            else
            {
                var findCoverageRegionQuery = new GetCoverageRegionQuery
                {
                    Province = query.Province,
                    Aumphur = query.District,
                    Tambon = query.SubDistrict,
                    ServiceType = "OTHER",
                };

                var coverageRegionResult = GetCoverageRegionHelper
                    .GetCoverageRegion(_logger, _covRegion, _zipcode, findCoverageRegionQuery);

                if (coverageRegionResult.SBNCheckCoverageData.AVALIABLE.ToYesNoFlgBoolean())
                {
                    listOfOwner.Add(coverageRegionResult.SBNCheckCoverageData.OWNER_PRODUCT);
                }
            }

            var awcQuery = new GetAirnetWirelessCoverageQuery
            {
                LAT = query.Lat,
                LNG = query.Long,
                COVERAGETYPE = query.BuildingType,
                FLOOR = query.Floor.ToSafeDecimal(),
            };

            var awcCoverage = GetAirnetWirelessCoverageHelper
                    .GetAirnetWirelessCoverage(_logger, _apCovService, _coverageAreaRel,
                    _lov, awcQuery);

            if (awcCoverage.SBNCheckCoverageData.AVALIABLE.ToYesNoFlgBoolean())
            {
                listOfOwner.Add("SWiFi");
            }

            return string.Join(",", listOfOwner);
        }
    }
}