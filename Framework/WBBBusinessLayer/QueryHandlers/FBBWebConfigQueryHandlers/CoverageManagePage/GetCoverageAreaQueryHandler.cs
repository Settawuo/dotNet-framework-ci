using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.CoverageManagePage
{
    public class GetCoverageAreaQueryHandler : IQueryHandler<GetCoverageAreaQuery, CoverageSitePanelModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _coverageArea;
        private readonly IEntityRepository<FBB_ZIPCODE> _zipCode;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _coverageZipCode;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IEntityRepository<FBB_PORT_INFO> _portInfo;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _coverageAreaRelation;
        private readonly IEntityRepository<FBB_CARD_INFO> _cardInfo;
        private readonly IEntityRepository<FBB_DSLAMMODEL> _dslamModel;
        private readonly IEntityRepository<FBB_CARDMODEL> _cardModel;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public GetCoverageAreaQueryHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> coverageArea,
                                                           IEntityRepository<FBB_ZIPCODE> zipCode,
                                                           IEntityRepository<FBB_COVERAGE_ZIPCODE> coverageZipCode,
                                                           IEntityRepository<FBB_DSLAM_INFO> dslamInfo,
                                                           IEntityRepository<FBB_PORT_INFO> portInfo,
                                                           IEntityRepository<FBB_COVERAGEAREA_RELATION> coverageAreaRelation,
                                                           IEntityRepository<FBB_CARD_INFO> cardInfo,
                                                           IEntityRepository<FBB_DSLAMMODEL> dslamModel,
                                                           IEntityRepository<FBB_CARDMODEL> cardModel,
                                                           IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _coverageArea = coverageArea;
            _zipCode = zipCode;
            _coverageZipCode = coverageZipCode;
            _dslamInfo = dslamInfo;
            _portInfo = portInfo;
            _coverageAreaRelation = coverageAreaRelation;
            _cardInfo = cardInfo;
            _dslamModel = dslamModel;
            _cardModel = cardModel;
            _cfgLov = cfgLov;
        }
        public CoverageSitePanelModel Handle(GetCoverageAreaQuery query)
        {
            var listCoverageSite = new List<CoverageAreaPanel>();
            var coverageSitePanelModel = new CoverageSitePanelModel();
            var listCVRId = new List<decimal>();
            try
            {
                #region sql
                //c.ipran_code,c.locationcode,c.nodestatus,c.ontarget_date,c.nodetype,c.nodename_th,c.nodename_en,c.contact_number,
                //c.moo,c.soi_th,c.road_th,c.zipcode,z.tumbon,z.amphur,z.province 
                //from FBB_COVERAGEAREA c,Fbb_Zipcode z,FBB_COVERAGE_ZIPCODE cz
                //where c.cvrid = cz.cvrid and cz.zipcode_rowid_th = z.zipcode_rowid 
                //and z.amphur Not like '%(ปณ.)' and c.activeflag='Y'
                //group by c.ipran_code,c.locationcode,c.nodestatus,c.ontarget_date,c.nodetype,c.nodename_th,c.nodename_en,c.contact_number,
                //c.moo,c.soi_th,c.road_th,c.zipcode,z.tumbon,z.amphur,z.province;

                #endregion
                var data = from c in _coverageArea.Get()
                           join cz in _coverageZipCode.Get() on c.CVRID equals cz.CVRID
                           join z in _zipCode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                           where c.ACTIVEFLAG == "Y" && !z.AMPHUR.EndsWith("(ปณ.)")
                           group new { c, z } by new
                           {
                               c.IPRAN_CODE,
                               c.LOCATIONCODE,
                               c.NODESTATUS,
                               c.ONTARGET_DATE_IN,

                               c.NODETYPE,
                               c.NODENAME_TH,
                               c.NODENAME_EN,
                               c.CONTACT_NUMBER,

                               c.MOO,
                               c.SOI_TH,
                               c.ROAD_TH,
                               c.ZIPCODE,

                               z.TUMBON,
                               z.AMPHUR,
                               z.PROVINCE,
                               c.REGION_CODE,

                               c.CVRID,
                               c.CONTACT_ID,
                               c.FAX_NUMBER,
                               c.BUILDINGCODE,
                               c.LATITUDE,
                               c.LONGITUDE,
                               c.COMPLETE_FLAG,
                               c.TIE_FLAG,
                               c.ONTARGET_DATE_EX
                           } into grp
                           select grp;

                if (data.Any())
                {

                    if (!string.IsNullOrEmpty(query.Province))
                        data = data.Where(a => a.Key.PROVINCE.Contains(query.Province));

                    if (!string.IsNullOrEmpty(query.Amphur))
                        data = data.Where(a => a.Key.AMPHUR.Contains(query.Amphur));

                    if (!string.IsNullOrEmpty(query.RegionCode))
                        data = data.Where(a => a.Key.REGION_CODE.Contains(query.RegionCode));

                    if (!string.IsNullOrEmpty(query.NodeName))
                        data = data.Where(a => a.Key.NODENAME_TH.ToUpper().Contains(query.NodeName.ToUpper()) || a.Key.NODENAME_EN.ToUpper().Contains(query.NodeName.ToUpper()));

                    if (!string.IsNullOrEmpty(query.IpRanCode))
                        data = data.Where(a => a.Key.IPRAN_CODE.Contains(query.IpRanCode));

                    if (!string.IsNullOrEmpty(query.LocationCode))
                        data = data.Where(a => a.Key.LOCATIONCODE.Contains(query.LocationCode));

                    if (!string.IsNullOrEmpty(query.NodeStaus))
                        data = data.Where(a => a.Key.NODESTATUS.Contains(query.NodeStaus));

                    if (!string.IsNullOrEmpty(query.Port) && data.Count() > 0)
                    {
                        #region Calculate port
                        foreach (var item in data)
                        {
                            var flagData = "Y";

                            if (query.Port.Substring(0, 2).ToUpper() == "DV")
                            {
                                flagData = "N";
                                query.Port = query.Port.Substring(0, 2);
                            }
                            else
                            {
                                flagData = "Y";
                                query.Port = query.Port.Substring(0, 1);
                            }

                            #region sql
                            //select c.cvrid,count(p.portid)
                            //from FBB_COVERAGEAREA c,FBB_COVERAGEAREA_relation cr,FBB_DSLAM_INFO di,FBB_Port_info p
                            //,FBB_DSLAMMODEL d,FBB_CARD_INFO ci,FBB_CARDMODEL cm
                            //where c.cvrid = cr.cvrid 
                            //and cr.dslamid = di.dslamid
                            //and di.dslammodelid = d.dslammodelid 
                            //and ci.dslamid = di.dslamid 
                            //and ci.cardmodelid = cm.cardmodelid
                            //and p.cardid = ci.cardid
                            //and p.portstatusid = 3
                            //group by c.cvrid;

                            //--Ava Port
                            //select c.cvrid,count(p.portid)
                            // from FBB_COVERAGEAREA c,FBB_DSLAM_INFO di,FBB_Port_info p
                            // ,FBB_CARD_INFO ci
                            // where 
                            // c.cvrid = di.cvrid 
                            // and ci.dslamid = di.dslamid 
                            // and p.cardid = ci.cardid 

                            // and c.nodestatus = 'ON_SITE'
                            // and c.activeflag = 'Y'
                            // and di.activeflag = 'Y'
                            // and ci.activeflag = 'Y'
                            // and p.activeflag = 'Y'
                            // and p.portstatusid = 1   
                            // and c.cvrid = 183                
                            // group by c.cvrid; 

                            #endregion
                            #region linq
                            var listX = from c in _coverageArea.Get()
                                        join di in _dslamInfo.Get() on c.CVRID equals di.CVRID
                                        join ci in _cardInfo.Get() on di.DSLAMID equals ci.DSLAMID
                                        join p in _portInfo.Get() on ci.CARDID equals p.CARDID
                                        join cm in _cardModel.Get() on ci.CARDMODELID equals cm.CARDMODELID
                                        where c.NODESTATUS == "ON_SITE" && c.ACTIVEFLAG == "Y"
                                        && di.ACTIVEFLAG == "Y" && ci.ACTIVEFLAG == "Y"
                                        && p.ACTIVEFLAG == "Y" && p.PORTSTATUSID == 1
                                        && cm.DATAONLY_FLAG == flagData
                                        && c.CVRID == item.Key.CVRID
                                        group c by new
                                        {
                                            c.CVRID
                                        } into grp
                                        select new PortUtillzationPanel()
                                        {
                                            GroupData = grp.Key.CVRID,
                                            CountData = grp.Count()
                                        };
                            #endregion
                            #region sql
                            //select c.cvrid,count(p.portid)
                            //from FBB_COVERAGEAREA c,FBB_COVERAGEAREA_relation cr,FBB_DSLAM_INFO di,FBB_Port_info p
                            //,FBB_DSLAMMODEL d,FBB_CARD_INFO ci,FBB_CARDMODEL cm
                            //where   c.cvrid = cr.cvrid and cr.dslamid = di.dslamid
                            //and di.dslammodelid = d.dslammodelid 
                            //and ci.dslamid = di.dslamid 
                            //and ci.cardmodelid = cm.cardmodelid
                            //and p.cardid = ci.cardid
                            //and p.portstatusid != 3
                            //group by c.cvrid;

                            // --total port
                            //select c.cvrid,count(p.portid)
                            // from FBB_COVERAGEAREA c,FBB_DSLAM_INFO di,FBB_Port_info p
                            // ,FBB_CARD_INFO ci
                            // where 
                            // c.cvrid = di.cvrid 
                            // and ci.dslamid = di.dslamid 
                            // and p.cardid = ci.cardid 
                            // and c.nodestatus = 'ON_SITE'
                            // and c.activeflag = 'Y'
                            // and di.activeflag = 'Y'
                            //and ci.activeflag = 'Y'
                            // and p.activeflag = 'Y'                             
                            // and c.cvrid = 183                
                            // group by c.cvrid;
                            #endregion
                            #region linq
                            var listY = from c in _coverageArea.Get()
                                        join di in _dslamInfo.Get() on c.CVRID equals di.CVRID
                                        join ci in _cardInfo.Get() on di.DSLAMID equals ci.DSLAMID
                                        join p in _portInfo.Get() on ci.CARDID equals p.CARDID
                                        join cm in _cardModel.Get() on ci.CARDMODELID equals cm.CARDMODELID
                                        where c.NODESTATUS == "ON_SITE" && c.ACTIVEFLAG == "Y"
                                        && di.ACTIVEFLAG == "Y" && ci.ACTIVEFLAG == "Y"
                                        && p.ACTIVEFLAG == "Y"
                                        && cm.DATAONLY_FLAG == flagData
                                        && c.CVRID == item.Key.CVRID
                                        group c by new
                                        {
                                            c.CVRID
                                        } into grp
                                        select new PortUtillzationPanel()
                                        {
                                            GroupData = grp.Key.CVRID,
                                            CountData = grp.Count()
                                        };
                            #endregion

                            #region Put Data to List
                            if (listX.Any() && listY.Any())
                            {
                                var x = listX.FirstOrDefault();
                                var y = listY.FirstOrDefault();
                                decimal calculate = 0;

                                if (y.CountData != 0)
                                    calculate = (x.CountData / y.CountData) * 100;

                                if (calculate <= Convert.ToDecimal(query.Port))
                                {
                                    var coverage = new CoverageAreaPanel();
                                    coverage.NodeType = item.Key.NODETYPE.ToSafeString();
                                    coverage.NodeNameTH = item.Key.NODENAME_TH.ToSafeString();
                                    coverage.NodeNameEN = item.Key.NODENAME_EN.ToSafeString();
                                    coverage.ContactNumber = item.Key.CONTACT_NUMBER.ToSafeString();
                                    coverage.FaxNumber = item.Key.FAX_NUMBER.ToSafeString();
                                    coverage.ProvinceTH = item.Key.PROVINCE.ToSafeString();
                                    coverage.AmphurTH = item.Key.AMPHUR.ToSafeString();
                                    coverage.TumbonTH = item.Key.TUMBON.ToSafeString();
                                    coverage.MooTH = item.Key.MOO.ToSafeDecimal();
                                    coverage.SoiTH = item.Key.SOI_TH.ToSafeString();
                                    coverage.RoadTH = item.Key.ROAD_TH.ToSafeString();
                                    coverage.ZipCodeTH = item.Key.ZIPCODE.ToSafeString();
                                    coverage.ProvinceEN = item.Key.PROVINCE.ToSafeString();
                                    coverage.AmphurEN = item.Key.AMPHUR.ToSafeString();
                                    coverage.TumbonEN = item.Key.TUMBON.ToSafeString();
                                    coverage.MooEN = item.Key.MOO.ToSafeDecimal();
                                    coverage.SoiEN = item.Key.SOI_TH.ToSafeString();
                                    coverage.RoadEN = item.Key.ROAD_TH.ToSafeString();
                                    coverage.ZipCodeEN = item.Key.ZIPCODE.ToSafeString();
                                    coverage.IpRanSiteCode = item.Key.IPRAN_CODE.ToSafeString();
                                    coverage.CondoCode = item.Key.LOCATIONCODE.ToSafeString();
                                    coverage.Lat = item.Key.LATITUDE.ToSafeString();
                                    coverage.Long = item.Key.LONGITUDE.ToSafeString();
                                    coverage.BuildingCode = item.Key.BUILDINGCODE.ToSafeString();
                                    coverage.OnTargetDateIn = item.Key.ONTARGET_DATE_IN;
                                    coverage.OnTargetDateEx = item.Key.ONTARGET_DATE_EX;
                                    coverage.Status = item.Key.NODESTATUS.ToSafeString();
                                    coverage.ConfigComplete = item.Key.COMPLETE_FLAG == "Y" ? true : false;
                                    coverage.TieFlag = item.Key.TIE_FLAG == "Y" ? true : false;
                                    coverage.CVRId = item.Key.CVRID;
                                    coverage.ContactId = item.Key.CONTACT_ID.ToSafeDecimal();
                                    coverage.RegionCode = item.Key.REGION_CODE.ToSafeString();
                                    listCoverageSite.Add(coverage);
                                    listCVRId.Add(item.Key.CVRID);
                                }
                            }
                            #endregion
                        }

                        #endregion
                    }
                }

                if (data.Count() > 0 && string.IsNullOrEmpty(query.Port))
                {
                    var list = from grp in data.ToList()
                               select new CoverageAreaPanel()
                               {
                                   NodeType = grp.Key.NODETYPE.ToSafeString(),
                                   NodeNameTH = grp.Key.NODENAME_TH.ToSafeString(),
                                   NodeNameEN = grp.Key.NODENAME_EN.ToSafeString(),
                                   ContactNumber = grp.Key.CONTACT_NUMBER.ToSafeString(),
                                   FaxNumber = grp.Key.FAX_NUMBER.ToSafeString(),
                                   ProvinceTH = grp.Key.PROVINCE.ToSafeString(),
                                   AmphurTH = grp.Key.AMPHUR.ToSafeString(),
                                   TumbonTH = grp.Key.TUMBON.ToSafeString(),
                                   MooTH = grp.Key.MOO.ToSafeDecimal(),
                                   SoiTH = grp.Key.SOI_TH.ToSafeString(),
                                   RoadTH = grp.Key.ROAD_TH.ToSafeString(),
                                   ZipCodeTH = grp.Key.ZIPCODE.ToSafeString(),
                                   ProvinceEN = grp.Key.PROVINCE.ToSafeString(),
                                   AmphurEN = grp.Key.AMPHUR.ToSafeString(),
                                   TumbonEN = grp.Key.TUMBON.ToSafeString(),
                                   MooEN = grp.Key.MOO.ToSafeDecimal(),
                                   SoiEN = grp.Key.SOI_TH.ToSafeString(),
                                   RoadEN = grp.Key.ROAD_TH.ToSafeString(),
                                   ZipCodeEN = grp.Key.ZIPCODE.ToSafeString(),
                                   IpRanSiteCode = grp.Key.IPRAN_CODE.ToSafeString(),
                                   CondoCode = grp.Key.LOCATIONCODE.ToSafeString(),
                                   Lat = grp.Key.LATITUDE.ToSafeString(),
                                   Long = grp.Key.LONGITUDE.ToSafeString(),
                                   BuildingCode = grp.Key.BUILDINGCODE.ToSafeString(),
                                   OnTargetDateIn = grp.Key.ONTARGET_DATE_IN,
                                   OnTargetDateEx = grp.Key.ONTARGET_DATE_EX,
                                   Status = (from c in _cfgLov.Get() where c.LOV_NAME == grp.Key.NODESTATUS && c.ACTIVEFLAG == "Y" && c.LOV_TYPE == "COVERAGESTATUS" select c.DISPLAY_VAL).FirstOrDefault(),
                                   ConfigComplete = grp.Key.COMPLETE_FLAG == "Y" ? true : false,
                                   TieFlag = grp.Key.TIE_FLAG == "Y" ? true : false,
                                   CVRId = grp.Key.CVRID,
                                   ContactId = grp.Key.CONTACT_ID.ToSafeDecimal(),
                                   RegionCode = (from c in _cfgLov.Get() where c.LOV_NAME == grp.Key.REGION_CODE && c.ACTIVEFLAG == "Y" && c.LOV_TYPE == "REGION_CODE" select c.DISPLAY_VAL).FirstOrDefault()
                               };

                    listCoverageSite = list.ToList();

                    foreach (var item in listCoverageSite)
                    {
                        listCVRId.Add(item.CVRId);
                    }

                }

                #region sql
                //select count(*) from FBB_COVERAGEAREA c,Fbb_Zipcode z,FBB_COVERAGE_ZIPCODE cz
                //where c.cvrid = cz.cvrid and cz.zipcode_rowid_th = z.zipcode_rowid 
                //and z.amphur Not like '%(ปณ.)' and c.activeflag='Y'
                //group by c.ipran_code,c.locationcode,c.nodestatus,c.ontarget_date,c.nodetype,c.nodename_th,c.nodename_en,c.contact_number,
                //c.moo,c.soi_th,c.road_th,c.zipcode,z.tumbon,z.amphur,z.province;
                #endregion

                var totalSite = from c in _coverageArea.Get()
                                join cz in _coverageZipCode.Get() on c.CVRID equals cz.CVRID
                                join z in _zipCode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                where !z.AMPHUR.EndsWith("(ปณ.)") && c.ACTIVEFLAG == "Y"
                                group new { c, z } by new
                                {
                                    c.IPRAN_CODE,
                                    c.LOCATIONCODE,
                                    c.NODESTATUS,
                                    c.ONTARGET_DATE_IN,
                                    c.ONTARGET_DATE_EX,
                                    c.NODETYPE,
                                    c.NODENAME_TH,
                                    c.NODENAME_EN,
                                    c.CONTACT_NUMBER,
                                    c.MOO,
                                    c.SOI_TH,
                                    c.ROAD_TH,
                                    c.ZIPCODE,
                                    z.TUMBON,
                                    z.AMPHUR,
                                    z.PROVINCE
                                } into grp
                                select new
                                {
                                    count = grp.Count()
                                };

                #region sql
                //select count(*) from FBB_COVERAGEAREA c,Fbb_Zipcode z,FBB_COVERAGE_ZIPCODE cz
                //where c.cvrid = cz.cvrid and cz.zipcode_rowid_th = z.zipcode_rowid 
                //and z.amphur Not like '%(ปณ.)' and c.activeflag='Y'
                #endregion

                //var totalCoverage = from c in _coverageArea.Get()
                //                    join cz in _coverageZipCode.Get() on c.CVRID equals cz.CVRID
                //                    join z in _zipCode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                //                    where !z.AMPHUR.EndsWith("(ปณ.)") && c.ACTIVEFLAG == "Y"
                //                    select new { c,cz,z };

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
                //and c.cvrid in (@CVRID_List)
                //group by p.portstatusid,cf.display_val
                #endregion

                var totalCoverageSite = from c in _coverageArea.Get().ToList()
                                        join di in _dslamInfo.Get().ToList() on c.CVRID equals di.CVRID
                                        join ci in _cardInfo.Get().ToList() on di.DSLAMID equals ci.DSLAMID
                                        join cm in _cardModel.Get().ToList() on ci.CARDMODELID equals cm.CARDMODELID
                                        join p in _portInfo.Get().ToList() on ci.CARDID equals p.CARDID
                                        join cf in _cfgLov.Get().ToList() on p.PORTSTATUSID.ToString() equals cf.LOV_VAL1
                                        where c.ACTIVEFLAG == "Y" && di.ACTIVEFLAG == "Y"
                                        && ci.ACTIVEFLAG == "Y" && p.ACTIVEFLAG == "Y"
                                        && cf.LOV_TYPE == "PORTSTATUS" && listCVRId.Contains(c.CVRID)
                                        group new { p, cf } by new { p.PORTSTATUSID, cf.DISPLAY_VAL } into grp
                                        select new
                                        {
                                            COUNTDATA = grp.Count(),
                                            PORTSTATUSID = grp.Key.PORTSTATUSID,
                                            DISPLAY_VAL = grp.Key.DISPLAY_VAL
                                        };

                int sumTotalPort = 0;

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

                coverageSitePanelModel.TotalSite = totalSite.Any() ? totalSite.FirstOrDefault().count.ToSafeString() : "0";
                //coverageSitePanelModel.TotalCoverage = totalCoverage.Any() ? totalCoverage.Count() : 0;
                coverageSitePanelModel.TotalCoverage = listCoverageSite.Count();
                coverageSitePanelModel.TotalPort = sumTotalPort;
                coverageSitePanelModel.CoverageAreaPanel = listCoverageSite;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetErrorMessage());
            }
            return coverageSitePanelModel;
        }
    }
}
