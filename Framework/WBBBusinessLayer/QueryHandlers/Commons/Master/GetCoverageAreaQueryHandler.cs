using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCoverageAreaQueryHandler : IQueryHandler<GetCoverageAreaQuery, List<CoverageValueModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _FBB_COVERAGE_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public GetCoverageAreaQueryHandler(ILogger logger,
            IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA,
            IEntityRepository<FBB_COVERAGE_ZIPCODE> FBB_COVERAGE_ZIPCODE,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _FBB_COVERAGE_ZIPCODE = FBB_COVERAGE_ZIPCODE;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public List<CoverageValueModel> Handle(GetCoverageAreaQuery query)
        {
            return CoverageAreaHelper.GetCoverageArea(_FBB_COVERAGEAREA, _FBB_COVERAGE_ZIPCODE,
                _FBB_COVERAGE_REGION, _FBB_ZIPCODE, query);

        }
    }

    public static class CoverageAreaHelper
    {
        public static List<CoverageValueModel> GetCoverageArea(
            IEntityRepository<FBB_COVERAGEAREA> fbb_coveragearea,
            IEntityRepository<FBB_COVERAGE_ZIPCODE> fbb_coverage_zipcode,
            IEntityRepository<FBB_COVERAGE_REGION> fbb_coverage_region,
            IEntityRepository<FBB_ZIPCODE> fbb_zipcode,
            GetCoverageAreaQuery query)
        {
            var isThai = query.CurrentCulture.IsThaiCulture();
            IEnumerable<FBB_COVERAGEAREA> temp1 = null;
            IEnumerable<CoverageValueModel> dataSymphony = null;

            if (isThai)
            {
                temp1 = (from a in fbb_coveragearea.Get()
                         join cz in fbb_coverage_zipcode.Get() on a.CVRID equals cz.CVRID
                         join z in fbb_zipcode.Get() on cz.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                         where z.LANG_FLAG == "N" && a.ACTIVEFLAG == "Y" && z.ZIPCODE_ROWID == query.ZipCodeId
                         && a.NODESTATUS == "ON_SITE" && a.NODETYPE == "CONDOMINIUM"
                         select a).DistinctBy(d => d.NODENAME_TH);

                dataSymphony = (from r in fbb_coverage_region.Get()
                                where r.ZIPCODE_ROWID_TH == query.ZipCodeId && r.SERVICE_TYPE == "BUILDING" && r.ACTIVEFLAG == "Y"
                                select new CoverageValueModel
                                {
                                    CvrId = -9,
                                    NodeNameEn = r.TOWER_EN,
                                    NodeNameTh = r.TOWER_TH,
                                    NodeStatus = "",
                                    NodeType = "",
                                    SiteCode = "",
                                    Moo = "",
                                    Soi_Th = "",
                                    Road_Th = "",
                                    Soi_En = "",
                                    Road_En = "",
                                    Zipcode = "",
                                    ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                    ONTARGET_DATE_IN = r.ONTARGET_DATE_IN
                                }).Distinct();
            }
            else
            {
                temp1 = (from a in fbb_coveragearea.Get()
                         join cz in fbb_coverage_zipcode.Get() on a.CVRID equals cz.CVRID
                         join z in fbb_zipcode.Get() on cz.ZIPCODE_ROWID_EN equals z.ZIPCODE_ROWID //for eng
                         where z.LANG_FLAG == "Y" && a.ACTIVEFLAG == "Y" && z.ZIPCODE_ROWID == query.ZipCodeId
                         && a.NODESTATUS == "ON_SITE" && a.NODETYPE == "CONDOMINIUM"
                         select a).DistinctBy(d => d.NODENAME_EN);

                dataSymphony = (from r in fbb_coverage_region.Get()
                                where r.ZIPCODE_ROWID_EN == query.ZipCodeId && r.SERVICE_TYPE == "BUILDING" && r.ACTIVEFLAG == "Y"
                                select new CoverageValueModel
                                {
                                    CvrId = -9,
                                    NodeNameEn = r.TOWER_EN,
                                    NodeNameTh = r.TOWER_TH,
                                    NodeStatus = "",
                                    NodeType = "",
                                    SiteCode = "",
                                    Moo = "",
                                    Soi_Th = "",
                                    Road_Th = "",
                                    Soi_En = "",
                                    Road_En = "",
                                    Zipcode = "",
                                    ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                    ONTARGET_DATE_IN = r.ONTARGET_DATE_IN
                                }).Distinct();
            }

            if (string.IsNullOrEmpty(query.SSO))
            {
                temp1 = temp1.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
                dataSymphony = dataSymphony.Where(a => a.ONTARGET_DATE_EX <= DateTime.Now.Date);
            }
            else
            {
                temp1 = temp1.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);
                dataSymphony = dataSymphony.Where(a => a.ONTARGET_DATE_IN <= DateTime.Now.Date);
            }

            var data1 = temp1.Select(c => new CoverageValueModel
            {
                CvrId = c.CVRID,
                NodeNameEn = c.NODENAME_EN,
                NodeNameTh = c.NODENAME_TH,
                NodeStatus = c.NODESTATUS,
                NodeType = c.NODETYPE,
                SiteCode = c.LOCATIONCODE,
                Moo = c.MOO.ToSafeString(),
                Soi_Th = c.SOI_TH,
                Road_Th = c.ROAD_TH,
                Soi_En = c.SOI_EN,
                Road_En = c.ROAD_EN,
                Zipcode = c.ZIPCODE,
            });

            var result = data1.Union(dataSymphony).ToList();

            //if (isThai)
            //    result.OrderBy(o => o.NodeNameTh);
            //else
            //    result.OrderBy(o => o.NodeNameEn);

            return result;
        }
    }
}
