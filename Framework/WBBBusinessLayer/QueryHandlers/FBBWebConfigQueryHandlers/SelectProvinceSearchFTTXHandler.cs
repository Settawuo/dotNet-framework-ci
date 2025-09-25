using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class SelectProvinceSearchFTTXHandler : IQueryHandler<SelectProvinceFttxQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectProvinceSearchFTTXHandler(ILogger logger, IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }


        public List<DropdownModel> Handle(SelectProvinceFttxQuery query)
        {


            var GROUP_AMPHURListData = new List<string>();
            var GROUP_AMPHUR = (from z in _FBB_ZIPCODE.Get()

                                join r in _FBB_COVERAGE_REGION.Get() on z.GROUP_AMPHUR equals r.ZIPCODE_ROWID_TH
                                where z.LANG_FLAG == "N" && r.ACTIVEFLAG == "Y" && r.COVERAGE_STATUS == "ON_SITE"

                                select new
                                {
                                    RegionCode = z.REGION_CODE,
                                    Province = z.PROVINCE,
                                    Amphur = z.AMPHUR,
                                    GroupAmphur = r.GROUP_AMPHUR,
                                    Tumbon = z.TUMBON,
                                    zipcode_rowid_th = r.ZIPCODE_ROWID_TH

                                }).Distinct();

            var groupIdListZiploID = (from z in _FBB_ZIPCODE.Get()

                                      join r in _FBB_COVERAGE_REGION.Get() on z.ZIPCODE_ROWID equals r.ZIPCODE_ROWID_TH
                                      where z.LANG_FLAG == "N" && r.ACTIVEFLAG == "Y"
                                       && r.COVERAGE_STATUS == "ON_SITE"
                                      select new
                                      {
                                          RegionCode = z.REGION_CODE,
                                          Province = z.PROVINCE,
                                          Amphur = z.AMPHUR,
                                          GroupAmphur = r.GROUP_AMPHUR,
                                          Tumbon = z.TUMBON,
                                          zipcode_rowid_th = r.ZIPCODE_ROWID_TH

                                      }).Distinct().Union(GROUP_AMPHUR);


            if (query.REGION_CODE != "")
            {

                GROUP_AMPHURListData = (from z2 in groupIdListZiploID where z2.RegionCode == query.REGION_CODE select z2.zipcode_rowid_th).Distinct().ToList();


            }


            var resultUU = (from r in _FBB_COVERAGE_REGION.Get()
                            join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                            where
                             GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) &&
                            r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                            && r.COVERAGE_STATUS == "ON_SITE"
                            group z by z.PROVINCE into g
                            orderby g.Key
                            select new DropdownModel
                            {
                                Text = g.Key,
                                Value = g.Key
                            }).ToList();

            var result = (from r in _FBB_COVERAGE_REGION.Get()
                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                          where
                           GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) &&
                          r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                          && r.COVERAGE_STATUS == "ON_SITE"
                          group z by z.PROVINCE into g
                          orderby g.Key
                          select new DropdownModel
                          {
                              Text = g.Key,
                              Value = g.Key
                          }).ToList().Union(resultUU).Distinct();


            result = result.GroupBy(test => test.Text)
                                          .Select(group => group.First()).ToList();

            result = result.GroupBy(test => test.Text)
                                      .Select(group => group.First()).ToList();
            return result.ToList();



        }
    }
}
