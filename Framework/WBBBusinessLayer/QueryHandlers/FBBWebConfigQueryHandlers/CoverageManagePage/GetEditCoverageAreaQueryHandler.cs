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
    public class GetEditCoverageAreaQueryHandler : IQueryHandler<GetEditCoverageAreaQuery, CoverageSitePanelModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipCode;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _coverageZipCode;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardModel;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBB_COVERAGEAREA_BUILDING> _building;

        public GetEditCoverageAreaQueryHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> coverageArea,
                                                           IEntityRepository<FBB_ZIPCODE> zipCode,
                                                           IEntityRepository<FBB_COVERAGE_ZIPCODE> coverageZipCode,
                                                           IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                           IEntityRepository<FBB_PORT_INFO> portInfo,
                                                           IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
                                                           IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                           IEntityRepository<FBB_CARDMODEL> cardModel,
                                                           IEntityRepository<FBB_CFG_LOV> cfgLov,
                                                           IEntityRepository<FBB_COVERAGEAREA_BUILDING> building)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _zipCode = zipCode;
            _coverageZipCode = coverageZipCode;
            _dslamInfo = dslamInfo;
            _portInfo = portInfo;
            _coverageAreaRelation = coverageAreaRelation;
            _cardInfo = cardInfo;
            _cardModel = cardModel;
            _cfgLov = cfgLov;
            _building = building;
        }

        public CoverageSitePanelModel Handle(GetEditCoverageAreaQuery query)
        {
            var coverageSitePanelModel = new CoverageSitePanelModel();
            var listCoverageArea = new List<CoverageAreaPanel>();

            if (query.ContactId != 0)
            {

                int sumTotalPort = 0;
                try
                {
                    var listZipCode = from c in _coverageArea.Get()
                                      where c.CONTACT_ID == query.ContactId
                                      select c.ZIPCODE;

                    var zipcode = listZipCode.Any() ? listZipCode.FirstOrDefault() : "";

                    var coverageArea = from c in _coverageArea.Get()
                                       join cz in _coverageZipCode.Get() on c.CVRID equals cz.CVRID
                                       join z in _zipCode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                       join z2 in _zipCode.Get() on cz.ZIPCODE_ROWID_EN equals z2.ZIPCODE_ROWID
                                       join cf in _cfgLov.Get() on c.NODESTATUS equals cf.LOV_NAME
                                       where !z.AMPHUR.EndsWith("(ปณ.)")
                                       && !z2.AMPHUR.EndsWith("(PO.)")
                                       && z.ZIPCODE == zipcode
                                       && z2.ZIPCODE == zipcode
                                       && c.ACTIVEFLAG == "Y"
                                       && cf.LOV_TYPE == "COVERAGESTATUS"
                                       && c.CONTACT_ID == query.ContactId

                                       select new CoverageAreaPanel()
                                       {
                                           CVRId = c.CVRID,
                                           ContactId = c.CONTACT_ID,
                                           NodeType = c.NODETYPE,
                                           NodeNameTH = c.NODENAME_TH,
                                           NodeNameEN = c.NODENAME_EN,
                                           ContactNumber = c.CONTACT_NUMBER,
                                           FaxNumber = c.FAX_NUMBER,
                                           TieFlag = c.TIE_FLAG == "Y" ? true : false,

                                           ProvinceTH = z.PROVINCE,
                                           AmphurTH = z.AMPHUR,
                                           TumbonTH = z.TUMBON,
                                           MooTH = c.MOO,
                                           SoiTH = c.SOI_TH,
                                           RoadTH = c.ROAD_TH,
                                           ZipCodeTH = c.ZIPCODE,

                                           ProvinceEN = z2.PROVINCE,
                                           AmphurEN = z2.AMPHUR,
                                           TumbonEN = z2.TUMBON,
                                           MooEN = c.MOO,
                                           SoiEN = c.SOI_EN,
                                           RoadEN = c.ROAD_EN,
                                           ZipCodeEN = c.ZIPCODE,

                                           IpRanSiteCode = c.IPRAN_CODE,
                                           CondoCode = c.LOCATIONCODE,
                                           Lat = c.LATITUDE,
                                           Long = c.LONGITUDE,

                                           BuildingCode = c.BUILDINGCODE,
                                           OnTargetDateIn = c.ONTARGET_DATE_IN,
                                           OnTargetDateEx = c.ONTARGET_DATE_EX,
                                           Status = cf.DISPLAY_VAL,
                                           StatusValue = cf.LOV_NAME,
                                           UpdateDate = c.UPDATED_DATE,
                                           ConfigComplete = c.COMPLETE_FLAG == "Y" ? true : false
                                       };

                    if (!query.GetForEdit)
                    {
                        coverageSitePanelModel.CoverageAreaPanel = coverageArea.ToList();
                        return coverageSitePanelModel;
                    }
                    else
                    {

                        #region sql
                        //select count(p.portid),p.portstatusid,cf.display_val
                        //from FBB_COVERAGEAREA c,FBB_COVERAGEAREA_relation cr,FBB_DSLAM_INFO di,FBB_Port_info p
                        //,FBB_CARD_INFO ci,FBB_CARDMODEL cm,FBB_CFG_LOV cf
                        //where   c.cvrid = cr.cvrid and cr.dslamid = di.dslamid
                        //and ci.dslamid = di.dslamid 
                        //and ci.cardmodelid = cm.cardmodelid
                        //and p.cardid = ci.cardid
                        //and c.activeflag = 'Y'
                        //and cr.activeflag = 'Y'
                        //and di.activeflag = 'Y'
                        //and ci.activeflag = 'Y'
                        //and p.activeflag = 'Y'
                        //and cf.lov_val1 = p.portstatusid
                        //and cf.lov_type = 'PORTSTATUS'
                        //and c.contact_id = @contact_id
                        //group by p.portstatusid,cf.display_val
                        #endregion

                        var totalCoverageSite = from c in _coverageArea.Get().ToList()
                                                join di in _dslamInfo.Get().ToList() on c.CVRID equals di.CVRID
                                                join ci in _cardInfo.Get().ToList() on di.DSLAMID equals ci.DSLAMID
                                                join cm in _cardModel.Get().ToList() on ci.CARDMODELID equals cm.CARDMODELID
                                                join p in _portInfo.Get().ToList() on ci.CARDID equals p.CARDID
                                                join cf in _cfgLov.Get().ToList() on p.PORTSTATUSID.ToString() equals cf.LOV_VAL1
                                                where c.ACTIVEFLAG == "Y"
                                                    && di.ACTIVEFLAG == "Y" && ci.ACTIVEFLAG == "Y"
                                                    && p.ACTIVEFLAG == "Y" && cf.LOV_TYPE == "PORTSTATUS"
                                                    && c.CONTACT_ID == query.ContactId
                                                group new { p, cf } by new
                                                {
                                                    p.PORTSTATUSID,
                                                    cf.DISPLAY_VAL
                                                } into grp
                                                select new
                                                {
                                                    PORTSTATUSID = grp.Key.PORTSTATUSID,
                                                    DISPLAY_VAL = grp.Key.DISPLAY_VAL,
                                                    COUNTDATA = grp.Count()
                                                };

                        var buildingList = from b in _building.Get()
                                           where b.CONTACT_ID == query.ContactId
                                           && b.ACTIVE_FLAG == "Y"
                                           select new BuildingPanel()
                                           {
                                               ContactId = b.CONTACT_ID,
                                               Tower = b.BUILDING,
                                               TowerEN = b.BUILDING_EN,
                                               TowerTH = b.BUILDING_TH,
                                               InstallNote = b.INSTALL_NOTE
                                           };

                        if (totalCoverageSite.Any())
                        {
                            foreach (var item in totalCoverageSite.ToList())
                            {
                                switch ((Int32)item.PORTSTATUSID)
                                {
                                    case 1:
                                        { coverageSitePanelModel.Available = item.COUNTDATA; sumTotalPort = sumTotalPort + item.COUNTDATA; }
                                        break;
                                    case 2:
                                        { coverageSitePanelModel.Reserve = item.COUNTDATA; sumTotalPort = sumTotalPort + item.COUNTDATA; }
                                        break;
                                    case 3:
                                        { coverageSitePanelModel.Active = item.COUNTDATA; sumTotalPort = sumTotalPort + item.COUNTDATA; }
                                        break;
                                    case 4:
                                        { coverageSitePanelModel.OutOfService = item.COUNTDATA; sumTotalPort = sumTotalPort + item.COUNTDATA; }
                                        break;
                                    case 5:
                                        { coverageSitePanelModel.PendingTerminate = item.COUNTDATA; sumTotalPort = sumTotalPort + item.COUNTDATA; }
                                        break;
                                }
                            }
                        }

                        if (coverageArea.Any())
                        {
                            listCoverageArea = coverageArea.ToList();
                            coverageSitePanelModel.Coverage = coverageArea.FirstOrDefault();
                            coverageSitePanelModel.TotalSite = listCoverageArea[0].CondoCode;
                            coverageSitePanelModel.TotalCoverage = listCoverageArea.Count();
                            coverageSitePanelModel.CoverageAreaPanel = listCoverageArea;
                        }

                        if (buildingList.Any())
                            coverageSitePanelModel.Building = buildingList.ToList();

                        coverageSitePanelModel.TotalPort = sumTotalPort;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.InnerException);
                }
            }
            else
            {
                coverageSitePanelModel.CoverageAreaPanel = listCoverageArea;
            }

            return coverageSitePanelModel;
        }
    }
}
