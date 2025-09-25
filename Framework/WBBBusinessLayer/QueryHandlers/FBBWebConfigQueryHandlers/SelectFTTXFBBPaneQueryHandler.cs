using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class SelectFTTXFBBPaneQueryHandler : IQueryHandler<SelectFTTXFBBQuery, List<Fttx_Fbb_PanelModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_APCOVERAGE> _FBB_APCOVERAGE;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public SelectFTTXFBBPaneQueryHandler(ILogger logger, IEntityRepository<FBB_APCOVERAGE> FBB_APCOVERAGE, IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_CFG_LOV = FBB_CFG_LOV;
            _FBB_APCOVERAGE = FBB_APCOVERAGE;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
            _FBB_ZIPCODE = FBB_ZIPCODE;
        }




        public List<Fttx_Fbb_PanelModel> Handle(SelectFTTXFBBQuery query)
        {
            var resultprovince = new List<ProvincModel_Fttx>();
            var resultprovinceU = new List<ProvincModel_Fttx>();
            var resultaumphur = new List<AumphurModel_Fttx>();
            var resulttumbon = new List<TumbonModel_Fttx>();
            var resultTower = new List<Towerth_Fttx>();
            var finalresult = new List<Fttx_Fbb_PanelModel>();

            var GROUP_AMPHURListData = new List<string>();
            var GROUP_AMPHUR = (from z in _FBB_ZIPCODE.Get()

                                join r in _FBB_COVERAGE_REGION.Get() on z.GROUP_AMPHUR equals r.ZIPCODE_ROWID_TH
                                where z.LANG_FLAG == "N" && r.ACTIVEFLAG == "Y"
                                 && r.COVERAGE_STATUS == "ON_SITE"
                                 && !z.AMPHUR.Contains("ปณ")

                                select new
                                {
                                    RegionCode = z.REGION_CODE,
                                    Province = z.PROVINCE,
                                    Amphur = z.AMPHUR,
                                    GroupAmphur = z.GROUP_AMPHUR,
                                    Tumbon = z.TUMBON,
                                    zipcode_rowid_th = z.ZIPCODE_ROWID

                                }).Distinct();





            var groupIdListZiploIDListData = new List<string>();
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
                                          zipcode_rowid_th = z.ZIPCODE_ROWID,
                                          Tower = r.TOWER_TH

                                      }).Distinct();


            if (query.region != "" && query.aumphur == "" && query.province == "" && query.tumbon == "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID where z2.RegionCode == query.region select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR where z2.RegionCode == query.region select z2.GroupAmphur).Distinct().ToList();
            }
            else if (query.region == "" && query.aumphur != "" && query.province == "" && query.tumbon == "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID where z2.Amphur == query.aumphur select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR where z2.Amphur == query.aumphur select z2.GroupAmphur).Distinct().ToList();
            }
            else if (query.region == "" && query.aumphur == "" && query.province != "" && query.tumbon == "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID where z2.RegionCode == query.region && z2.Amphur == query.aumphur select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in groupIdListZiploID where z2.RegionCode == query.region && z2.Amphur == query.aumphur select z2.GroupAmphur).Distinct().ToList();
            }

            else if (query.region == "" && query.aumphur == "" && query.province == "" && query.tumbon != "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID
                                              where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                                  && z2.Province == query.province
                                              select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR
                                        where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                            && z2.Province == query.province
                                        select z2.GroupAmphur).Distinct().ToList();
            }
            else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID
                                              where z2.RegionCode == query.region && z2.Province == query.province
                                              select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR
                                        where z2.RegionCode == query.region && z2.Province == query.province
                                        select z2.GroupAmphur).Distinct().ToList();
            }
            else if (query.region == "" && query.aumphur == "" && query.province != "" && query.tumbon != "")
            {

                groupIdListZiploIDListData = (from z2 in groupIdListZiploID
                                              where z2.Province == query.province && z2.Tumbon == query.tumbon
                                              select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR
                                        where z2.Province == query.province && z2.Tumbon == query.tumbon
                                        select z2.GroupAmphur).Distinct().ToList();

            }
            else if (query.region == "" && query.aumphur == "" && query.province == "" && query.tumbon == "")
            {
                groupIdListZiploIDListData = (from z2 in groupIdListZiploID select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR select z2.GroupAmphur).Distinct().ToList();
            }
            else if (query.region != "" && query.aumphur != "" && query.province != "" && query.tumbon != "")
            {

                groupIdListZiploIDListData = (from z2 in groupIdListZiploID
                                              where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                                  && z2.Province == query.province && z2.Tumbon == query.tumbon
                                              select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR
                                        where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                            && z2.Province == query.province && z2.Tumbon == query.tumbon
                                        select z2.GroupAmphur).Distinct().ToList();
            }

            else if (query.region != "" && query.aumphur != "" && query.province != "" && query.tumbon == "")
            {

                groupIdListZiploIDListData = (from z2 in groupIdListZiploID
                                              where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                                  && z2.Province == query.province
                                              select z2.zipcode_rowid_th).Distinct().ToList();
                GROUP_AMPHURListData = (from z2 in GROUP_AMPHUR
                                        where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                            && z2.Province == query.province
                                        select z2.GroupAmphur).Distinct().ToList();
            }



            if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                       groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                        && r.ACTIVEFLAG == "Y" && r.COVERAGE_STATUS == "ON_SITE"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                     GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                      && r.ACTIVEFLAG == "Y" && r.COVERAGE_STATUS == "ON_SITE"
                                        && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                 r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key
                                          }).ToList().Union(resultaumphuRegion_U).Distinct();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                          r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)

                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();



                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                      && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && !z.AMPHUR.Contains("ปณ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);




                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();
                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                    !z.AMPHUR.Contains("ปณ") &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON
                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                     r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);

                            resulttumbon_UU = resulttumbon_UU.GroupBy(s => new { s.tumbon, s.Tower_TH }).Select(g => g.First()).ToList();

                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                        !z.AMPHUR.Contains("ปณ") &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;


            }
            ///Region not null
            else if (query.region != "" && query.region != "not" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                       groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                        && r.ACTIVEFLAG == "Y"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                      && r.ACTIVEFLAG == "Y" && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                              r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                          .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX
                                               }).Distinct().ToList();

                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                      && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                                     && z.PROVINCE == provincein.province
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                          && !z.AMPHUR.Contains("ปณ")
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                   (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON
                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            resulttumbon_UU = resulttumbon_UU.GroupBy(s => new { s.tumbon, s.Tower_TH }).Select(g => g.First()).ToList();


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                        !z.AMPHUR.Contains("ปณ") &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }


                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;

            }
            ///province not null and Region 
            else if (query.region != "" && query.region != "not" && query.region != "ROC" && query.province != "notMuti" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                        z.LANG_FLAG == "N"
                                        && r.ACTIVEFLAG == "Y"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                      && r.ACTIVEFLAG == "Y" && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && z.REGION_CODE == regionin.region
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE" && !z.AMPHUR.Contains("ปณ")
                                           && z.REGION_CODE == regionin.region
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                         (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX
                                               }).Distinct().ToList();

                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && !z.AMPHUR.Contains("ปณ")
                                                && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX


                                                    }).Distinct().ToList().Union(resultaumphur_U);


                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();

                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && !z.AMPHUR.Contains("ปณ")
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                  r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                        !z.AMPHUR.Contains("ปณ") &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;

            }


            else if (query.region != "" && query.region == "ROC" && query.province != "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                       groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                        && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && z.LANG_FLAG == "N"
                                      && r.ACTIVEFLAG == "Y" && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                           r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && z.REGION_CODE == regionin.region
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX
                                               }).Distinct().ToList();


                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR && !z.AMPHUR.Contains("ปณ") &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                      && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && !z.AMPHUR.Contains("ปณ")
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                   (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);




                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                        !z.AMPHUR.Contains("ปณ") &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;

            }

            ///province   region  amphur  not null   
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                       groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                        && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                   GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                      && r.ACTIVEFLAG == "Y" && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                            r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && z.REGION_CODE == regionin.region
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && !z.AMPHUR.Contains("ปณ")
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                         (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX
                                               }).Distinct().ToList();


                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              (r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR || r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID) &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                   && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                                     && z.PROVINCE == provincein.province
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX
                                                    }).Distinct().ToList().Union(resultaumphur_U);

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                 r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE" && !z.AMPHUR.Contains("ปณ")
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON


                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);





                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                        !z.AMPHUR.Contains("ปณ") &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)

                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                         r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;
            }

            else if (query.region == "BKK" && query.region != "not" && query.province == "กรุงเทพ" && query.aumphur == "" && query.tumbon == "not")
            {

                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                        r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                        && z.REGION_CODE == "BKK" && z.PROVINCE == "กรุงเทพ" && r.COVERAGE_STATUS == "ON_SITE"

                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList();

                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                      r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                      && z.REGION_CODE == "BKK" && z.PROVINCE == "กรุงเทพ" && r.COVERAGE_STATUS == "ON_SITE"
                                      && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU);

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                               r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID
                                                && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && z.PROVINCE == "กรุงเทพ"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR && r.COVERAGE_STATUS == "ON_SITE"
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && z.PROVINCE == "กรุงเทพ"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();

                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                        r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                        r.COVERAGE_STATUS == "ON_SITE" && r.ACTIVEFLAG == "Y"
                                        && z.LANG_FLAG == "N" && z.PROVINCE == "กรุงเทพ"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();


                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                              r.COVERAGE_STATUS == "ON_SITE" && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                                     && z.PROVINCE == provincein.province && z.PROVINCE == "กรุงเทพ"
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX


                                                    }).Distinct().ToList().Union(resultaumphur_U);

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();

                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                  where
                                                 r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && z.PROVINCE == "กรุงเทพ"
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                   where
                                                   r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                    r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                  && r.COVERAGE_STATUS == "ON_SITE" && !z.AMPHUR.Contains("ปณ")
                                                       && z.REGION_CODE == regionin.region && z.PROVINCE == "กรุงเทพ"
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            resulttumbon_UU = resulttumbon_UU.GroupBy(s => new { s.tumbon, s.Tower_TH }).Select(g => g.First()).ToList();


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                       r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && r.COVERAGE_STATUS == "ON_SITE" && !z.AMPHUR.Contains("ปณ")
                                                          && z.REGION_CODE == regionin.region && z.PROVINCE == "กรุงเทพ"
                                                    && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur

                                             && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                      r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                     r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                      && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                               && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && z.PROVINCE == "กรุงเทพ"
                                                      && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;
            }
            else if (query.region == "BKK" && query.province == "notMuti" && query.aumphur == "" && query.tumbon == "")
            {

                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                        r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                        && z.REGION_CODE == query.region && z.PROVINCE.Contains("กรุงเทพ") && r.COVERAGE_STATUS == "ON_SITE"

                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList();

                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                      r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                       && z.PROVINCE.Contains("กรุงเทพ") && r.COVERAGE_STATUS == "ON_SITE"
                                          && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU);

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR
                                                && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE" && !z.PROVINCE.Contains("กรุงเทพ")
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                           r.COVERAGE_STATUS == "ON_SITE" && !z.PROVINCE.Contains("กรุงเทพ")
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                           .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();

                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                        r.COVERAGE_STATUS == "ON_SITE" && r.ACTIVEFLAG == "Y"
                                        && z.LANG_FLAG == "N" && !z.PROVINCE.Contains("กรุงเทพ")
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();


                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                              r.COVERAGE_STATUS == "ON_SITE" && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                                     && z.PROVINCE == provincein.province && !z.PROVINCE.Contains("กรุงเทพ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);


                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();

                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                  where
                                            r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.PROVINCE.Contains("กรุงเทพ")
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON,
                                                      Tower_TH = r.TOWER_TH

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                   where
                                                   r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                    r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                  && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region && !z.PROVINCE.Contains("กรุงเทพ")
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.AMPHUR.Contains("ปณ")


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON,
                                                       Tower_TH = r.TOWER_TH

                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                       r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && r.COVERAGE_STATUS == "ON_SITE"
                                                          && z.REGION_CODE == regionin.region && !z.PROVINCE.Contains("กรุงเทพ")
                                                    && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.AMPHUR.Contains("ปณ")
                                                     && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                 r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                     r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                      && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                               && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.PROVINCE.Contains("กรุงเทพ")
                                                           && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }



                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;
            }
            ///select Dropdown 3 drowrn
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                       groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                        && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                    GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                      && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                 r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                               && z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                            GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                          .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                        r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                   groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                           && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();

                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                    && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province
                                                      && !z.AMPHUR.Contains("ปณ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);





                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();

                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.AMPHUR.Contains("ปณ")
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).OrderByDescending(a => a.createdate).ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).OrderByDescending(a => a.createdate).ToList().Union(resulttumbon_U);



                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur && !z.AMPHUR.Contains("ปณ")
                                                     && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                    r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                       && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                           && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;


            }
            else if (query.region != "" && query.province != "" && query.aumphur == "" && query.tumbon == "not")
            {

                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                     GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && !z.AMPHUR.Contains("ปณ")
                                      && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                               r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH) && z.REGION_CODE == regionin.region
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                      r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                           GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH) && z.REGION_CODE == regionin.region
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && z.REGION_CODE == regionin.region
                                                 && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                        .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();

                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                           groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();

                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                    && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && !z.AMPHUR.Contains("ปณ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                 GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH) &&
                                                    r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);





                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                     GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                              && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                     && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                    r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH) &&
                                                        r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                           && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;

            }
            else if (query.region != "" && query.province == "not" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                     r.ACTIVEFLAG == "Y" && z.REGION_CODE == query.region && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                 r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                             && z.REGION_CODE == regionin.region
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key

                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                          r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR && GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                          && z.REGION_CODE == regionin.region
                                           && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key



                                          }).ToList().Union(resultaumphuRegion_U).Distinct();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                        .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                  r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                 groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH)
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {

                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX

                                               }).Distinct().ToList();

                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                              r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                  && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province
                                                        && !z.AMPHUR.Contains("ปณ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX

                                                    }).Distinct().ToList().Union(resultaumphur_U);


                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                      .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                  r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                  GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                 && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where
                                                   r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                  groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH) &&
                                                    r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON


                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            resulttumbon_UU = resulttumbon_UU.GroupBy(test => test.tumbon)
                                              .Select(group => group.First()).ToList();


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                      r.ZIPCODE_ROWID_TH == z.GROUP_AMPHUR &&
                                                      GROUP_AMPHURListData.Contains(r.ZIPCODE_ROWID_TH)
                                                     && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                              && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                     && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where
                                                       r.ZIPCODE_ROWID_TH == z.ZIPCODE_ROWID &&
                                                      groupIdListZiploIDListData.Contains(r.ZIPCODE_ROWID_TH) &&
                                                        r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == provincein.aumphur
                                                           && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }

                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;

            }
            else if (query.region == "not" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregionUU = (from r in _FBB_COVERAGE_REGION.Get()
                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                      join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                      where
                                        !z.REGION_CODE.Contains("BKK") && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                      select new Fttx_Fbb_PanelModel
                                      {
                                          regiondisplay = lov.LOV_VAL2,
                                          region = z.REGION_CODE
                                      }).ToList().Distinct();


                var resultregion = (from r in _FBB_COVERAGE_REGION.Get()
                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                    join lov in _FBB_CFG_LOV.Get() on z.REGION_CODE equals lov.DISPLAY_VAL
                                    where
                                        !z.REGION_CODE.Contains("BKK") && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                        && !z.AMPHUR.Contains("ปณ")
                                    select new Fttx_Fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = z.REGION_CODE
                                    }).ToList().Union(resultregionUU).Distinct();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();





                foreach (var regionin in resultregion)
                {


                    var resultaumphuRegion_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                where
                                                   z.REGION_CODE == regionin.region
                                                 && r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                group z by z.PROVINCE into g
                                                orderby g.Key
                                                select new ProvincModel_Fttx
                                                {
                                                    province = g.Key
                                                }).ToList();
                    var resultregionss = (from r in _FBB_COVERAGE_REGION.Get()
                                          join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                          where
                                        r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                            && r.COVERAGE_STATUS == "ON_SITE"
                                           && z.REGION_CODE == regionin.region && !z.AMPHUR.Contains("ปณ")
                                          group z by z.PROVINCE into g
                                          orderby g.Key
                                          select new ProvincModel_Fttx
                                          {
                                              province = g.Key
                                          }).ToList().Union(resultaumphuRegion_U).Distinct();


                    resultregionss = resultregionss.GroupBy(test => test.province)
                                          .Select(group => group.First()).ToList();

                    resultregionss = resultregionss.GroupBy(test => test.province)
                                              .Select(group => group.First()).ToList();


                    foreach (var provincein in resultregionss)
                    {

                        var resultaumphur_U = (from r in _FBB_COVERAGE_REGION.Get()
                                               join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                               where
                                            r.COVERAGE_STATUS == "ON_SITE"
                                                && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                && z.REGION_CODE == regionin.region
                                          && z.PROVINCE == provincein.province
                                               select new AumphurModel_Fttx
                                               {
                                                   aumphur = z.AMPHUR,
                                                   createdate = r.ONTARGET_DATE_EX


                                               }).Distinct().ToList();


                        var resultaumphurOfUnion = (from r in _FBB_COVERAGE_REGION.Get()
                                                    join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                    where
                                            r.COVERAGE_STATUS == "ON_SITE"
                                                   && r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province
                                                     && !z.AMPHUR.Contains("ปณ")
                                                    select new AumphurModel_Fttx
                                                    {
                                                        aumphur = z.AMPHUR,
                                                        createdate = r.ONTARGET_DATE_EX
                                                    }).Distinct().ToList().Union(resultaumphur_U);
                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                             .Select(group => group.First()).ToList();

                        resultaumphurOfUnion = resultaumphurOfUnion.GroupBy(test => test.aumphur)
                                                  .Select(group => group.First()).ToList();




                        foreach (var aumphurin in resultaumphurOfUnion)
                        {
                            var resulttumbon_U = (from r in _FBB_COVERAGE_REGION.Get()
                                                  join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                  where
                                                   r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                 && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                          && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                          && !z.AMPHUR.Contains("ปณ")
                                                  select new TumbonModel_Fttx
                                                  {
                                                      tumbon = z.TUMBON

                                                  }).Distinct().ToList();

                            var resulttumbon_UU = (from r in _FBB_COVERAGE_REGION.Get()
                                                   join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                   where r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"

                                                       && z.REGION_CODE == regionin.region
                                                 && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur


                                                   select new TumbonModel_Fttx
                                                   {
                                                       tumbon = z.TUMBON

                                                   }).Distinct().ToList().Union(resulttumbon_U);


                            foreach (var Towerdata in resulttumbon_UU)
                            {

                                var resultTowerUUA = (from r in _FBB_COVERAGE_REGION.Get()
                                                      join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.GROUP_AMPHUR
                                                      where
                                                       r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N"
                                                     && z.REGION_CODE == regionin.region && r.COVERAGE_STATUS == "ON_SITE"
                                              && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                              && !z.AMPHUR.Contains("ปณ")
                                                     && z.TUMBON == Towerdata.tumbon
                                                      select new Towerth_Fttx
                                                      {
                                                          Tower_TH = r.TOWER_TH
                                                      }).Distinct().ToList();

                                var resultTowerloop = (from r in _FBB_COVERAGE_REGION.Get()
                                                       join z in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals z.ZIPCODE_ROWID
                                                       where r.ACTIVEFLAG == "Y" && z.LANG_FLAG == "N" && r.COVERAGE_STATUS == "ON_SITE"

                                                           && z.REGION_CODE == regionin.region
                                                     && z.PROVINCE == provincein.province && z.AMPHUR == aumphurin.aumphur
                                                           && z.TUMBON == Towerdata.tumbon

                                                       select new Towerth_Fttx
                                                       {
                                                           Tower_TH = r.TOWER_TH

                                                       }).Distinct().ToList().Union(resultTowerUUA);

                                Towerdata.Towerlist = resultTowerloop.ToList();
                            }


                            aumphurin.Tumbonlist = resulttumbon_UU.ToList();
                        }
                        provincein.Aumphurlist = resultaumphurOfUnion.ToList();

                    }
                    regionin.Provincelist = resultregionss.ToList();
                    finalresult.Add(regionin);

                }
                return finalresult;


            }

            return new List<Fttx_Fbb_PanelModel>();
        }
    }
}
