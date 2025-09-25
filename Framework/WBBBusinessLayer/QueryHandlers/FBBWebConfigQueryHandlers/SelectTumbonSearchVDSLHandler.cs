using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectTumbonSearchVDSLHandler : IQueryHandler<SelectTumbonVDSLQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _FBB_COVERAGE_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;


        public SelectTumbonSearchVDSLHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
             IEntityRepository<FBB_COVERAGE_ZIPCODE> FBB_COVERAGE_ZIPCODE, IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_COVERAGE_ZIPCODE = FBB_COVERAGE_ZIPCODE;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
        }



        public List<DropdownModel> Handle(SelectTumbonVDSLQuery query)
        {


            var Zipcode_idListData = new List<string>();
            var ZipcodeID = (from c in _FBB_COVERAGEAREA.Get()
                             join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                             join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                             join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                             join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                             where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                             && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")

                             select new
                             {
                                 RegionCode = zc.REGION_CODE,
                                 Province = zc.PROVINCE,
                                 Amphur = zc.AMPHUR,
                                 ZipcodeID = c.ZIPCODE,
                                 Tumbon = zc.TUMBON,
                                 zipcode_rowid_th = z.ZIPCODE_ROWID_TH

                             }).Distinct();



            if (query.REGION_CODE != "" && query.PROVINCE != "" && query.AUMPHUR != "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.RegionCode == query.REGION_CODE && z2.Province == query.PROVINCE && z2.Amphur == query.AUMPHUR select z2.ZipcodeID).Distinct().ToList();

            }


            var result = (from c in _FBB_COVERAGEAREA.Get()
                          join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                          join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                          join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                          join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                          where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                          && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                          && Zipcode_idListData.Contains(c.ZIPCODE)
                          group zc by zc.TUMBON into g
                          orderby g.Key
                          select new DropdownModel
                          {
                              Text = g.Key,
                              Value = g.Key
                          }).ToList();
            return result;
        }
    }
}
