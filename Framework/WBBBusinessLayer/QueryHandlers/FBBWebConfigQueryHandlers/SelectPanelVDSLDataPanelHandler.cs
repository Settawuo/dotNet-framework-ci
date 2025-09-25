using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class SelectPanelVDSLDataPanelHandler : IQueryHandler<SelectMainVDLSPanelQuery, List<Vdsl_fbb_PanelModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGEAREA> _FBB_COVERAGEAREA;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGE_ZIPCODE> _FBB_COVERAGE_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RELATION> _FBB_COVERAGEAREA_RELATION;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _FBB_DSLAM_INFO;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;



        public SelectPanelVDSLDataPanelHandler(ILogger logger, IEntityRepository<FBB_COVERAGEAREA> FBB_COVERAGEAREA,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
             IEntityRepository<FBB_COVERAGE_ZIPCODE> FBB_COVERAGE_ZIPCODE, IEntityRepository<FBB_COVERAGEAREA_RELATION> FBB_COVERAGEAREA_RELATION,
            IEntityRepository<FBB_DSLAM_INFO> FBB_DSLAM_INFO,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _FBB_COVERAGEAREA = FBB_COVERAGEAREA;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_COVERAGE_ZIPCODE = FBB_COVERAGE_ZIPCODE;
            _FBB_COVERAGEAREA_RELATION = FBB_COVERAGEAREA_RELATION;
            _FBB_DSLAM_INFO = FBB_DSLAM_INFO;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }



        public List<Vdsl_fbb_PanelModel> Handle(SelectMainVDLSPanelQuery query)
        {
            List<ProvincModel_VDSL> resultprovince;
            List<TumbonModel_VDSL> resulttumbon;
            List<Vdsl_fbb_PanelModel> finalresult = new List<Vdsl_fbb_PanelModel>();
            List<NodeNameTH_VDSL> resultNodeName;

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
                                 zipcode_rowid = zc.ZIPCODE_ROWID

                             }).Distinct();




            if (query.region != "" && query.aumphur == "" && query.province == "" && query.tumbon == "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.RegionCode == query.region select z2.zipcode_rowid).Distinct().ToList();

            }
            else if (query.region == "" && query.aumphur != "" && query.province == "" && query.tumbon == "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.Amphur == query.aumphur select z2.zipcode_rowid).Distinct().ToList();
            }
            else if (query.region == "" && query.aumphur == "" && query.province != "" && query.tumbon == "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.Province == query.province select z2.zipcode_rowid).Distinct().ToList();
            }

            else if (query.region == "" && query.aumphur == "" && query.province == "" && query.tumbon != "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.Tumbon == query.tumbon select z2.zipcode_rowid).Distinct().ToList();
            }
            else if (query.region != "" && query.aumphur != "" && query.province == "" && query.tumbon == "")
            {
                Zipcode_idListData = (from z2 in ZipcodeID where z2.RegionCode == query.region && z2.Amphur == query.aumphur select z2.zipcode_rowid).Distinct().ToList();
            }
            else if (query.region == "" && query.aumphur == "" && query.province != "" && query.tumbon != "")
            {

                Zipcode_idListData = (from z2 in ZipcodeID where z2.Province == query.province && z2.Tumbon == query.tumbon select z2.zipcode_rowid).Distinct().ToList();

            }

            else if (query.region != "" && query.aumphur != "" && query.province != "" && query.tumbon != "")
            {

                Zipcode_idListData = (from z2 in ZipcodeID
                                      where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                      && z2.Province == query.province && z2.Tumbon == query.tumbon
                                      select z2.zipcode_rowid).Distinct().ToList();
            }

            else if (query.region != "" && query.aumphur != "" && query.province != "" && query.tumbon == "")
            {

                Zipcode_idListData = (from z2 in ZipcodeID
                                      where z2.RegionCode == query.region && z2.Amphur == query.aumphur
                                          && z2.Province == query.province
                                      select z2.zipcode_rowid).Distinct().ToList();
            }

            else if (query.region == "" && query.aumphur == "" && query.province == "" && query.tumbon == "")
            {

                Zipcode_idListData = (from z2 in ZipcodeID

                                      select z2.zipcode_rowid).Distinct().ToList();
            }

            else if (query.region != "" && query.aumphur == "" && query.province != "" && query.tumbon == "")
            {

                Zipcode_idListData = (from z2 in ZipcodeID
                                      where z2.RegionCode == query.region
                                         && z2.Province == query.province
                                      select z2.zipcode_rowid).Distinct().ToList();
            }








            if (query.region == "" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {


                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                     && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) &&
                                                  zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province

                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                              && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                     && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            //resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                            //               .Select(group => group.First()).ToList();


                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }


                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }
            else if (query.region != "" && query.region != "not" && query.region != "BKK" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {
                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && !zc.REGION_CODE.Contains("BKK")

                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                             && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                     && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON
                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();


                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;

            }
            else if (query.region != "" && query.province != "" && query.province != "notBKK" && query.aumphur == "" && query.tumbon == "")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == query.region
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                           && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                     && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }
                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;

            }

            else if (query.region == "BKK" && query.province == "" && query.province != "not" && query.aumphur == "" && query.tumbon == "")
            {


                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                     && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == query.region

                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                     && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                            && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();


                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }
                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon == "")
            {
                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == query.region
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();

                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                            && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                     && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }

                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }
            else if (query.region != "" && query.province != "" && query.aumphur != "" && query.tumbon != "")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == query.region
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                       && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                                   && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                            && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();



                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }

                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }
            else if (query.region != "BKK" && query.province != "" && query.aumphur == "" && query.tumbon == "not")
            {


                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                                   && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                            && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();


                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }
                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }

            else if (query.region == "BKK" && query.province == "กรุงเทพ" && query.aumphur == "" && query.tumbon == "not")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && zc.PROVINCE == "กรุงเทพ"
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {

                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                       && zc.REGION_CODE == regionin.region && zc.PROVINCE == "กรุงเทพ"
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                      && zc.PROVINCE == "กรุงเทพ"
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();

                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                    .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                          && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                          && zc.AMPHUR == aumphurin.aumphur && zc.PROVINCE == "กรุงเทพ"
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();




                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                && zc.AMPHUR == aumphurin.aumphur && zc.PROVINCE == "กรุงเทพ"
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }

                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }

            else if (query.region == "BKK" && query.province == "not" && query.aumphur == "" && query.tumbon == "not")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                   && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == query.region
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                       && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH) && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();



                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                            && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                     && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();




                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                  && Zipcode_idListData.Contains(z.ZIPCODE_ROWID_TH)
                                                           && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }


                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }

            else if (query.region == "BKK" && query.province == "notBKK" && query.aumphur == "" && query.tumbon == "")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                    && !zc.PROVINCE.Contains("กรุงเทพ")
                                    && zc.REGION_CODE == query.region
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                     && !zc.PROVINCE.Contains("กรุงเทพ") && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                 && !zc.PROVINCE.Contains("กรุงเทพ")
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                             && zc.REGION_CODE == regionin.region && !zc.PROVINCE.Contains("กรุงเทพ")
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();
                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                   && zc.REGION_CODE == regionin.region && !zc.PROVINCE.Contains("กรุงเทพ")
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }



                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }
            else if (query.region == "not" && query.province == "" && query.aumphur == "" && query.tumbon == "")
            {

                var resultregion = (from c in _FBB_COVERAGEAREA.Get()
                                    join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                    join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                    join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                    join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                    join lov in _FBB_CFG_LOV.Get() on zc.REGION_CODE equals lov.DISPLAY_VAL
                                    where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                    && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                    && !zc.REGION_CODE.Contains("BKK")
                                    select new Vdsl_fbb_PanelModel
                                    {
                                        regiondisplay = lov.LOV_VAL2,
                                        region = zc.REGION_CODE
                                    }).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                           .Select(group => group.First()).ToList();

                resultregion = resultregion.GroupBy(test => test.region)
                                          .Select(group => group.First()).ToList();
                foreach (var regionin in resultregion)
                {
                    resultprovince = (from c in _FBB_COVERAGEAREA.Get()
                                      join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                      join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                      join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                      join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                      where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                      && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                      && zc.REGION_CODE == regionin.region
                                      group zc by zc.PROVINCE into g
                                      orderby g.Key
                                      select new ProvincModel_VDSL
                                      {
                                          province = g.Key

                                      }).ToList();

                    foreach (var provincein in resultprovince)
                    {
                        var resultaumphurUnion = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                    && zc.REGION_CODE == regionin.region && zc.PROVINCE == provincein.province
                                                  select new AumphurModel_VDSL
                                                  {
                                                      aumphur = zc.AMPHUR,
                                                      createdate = c.ONTARGET_DATE_EX

                                                  }).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                     .Select(group => group.First()).ToList();
                        resultaumphurUnion = resultaumphurUnion.GroupBy(test => test.aumphur)
                                   .Select(group => group.First()).ToList();


                        foreach (var aumphurin in resultaumphurUnion)
                        {
                            resulttumbon = (from c in _FBB_COVERAGEAREA.Get()
                                            join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                            join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                            join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                            join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                            where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                            && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                             && zc.REGION_CODE == regionin.region
                                            && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                            select new TumbonModel_VDSL
                                            {
                                                tumbon = zc.TUMBON,
                                                Node_Name = c.NODENAME_TH

                                            }).Distinct().ToList();


                            resulttumbon = resulttumbon.GroupBy(test => test.tumbon)
                                           .Select(group => group.First()).ToList();




                            foreach (var nodenamethdata in resulttumbon)
                            {

                                resultNodeName = (from c in _FBB_COVERAGEAREA.Get()
                                                  join d in _FBB_DSLAM_INFO.Get() on c.CVRID equals d.CVRID
                                                  join r in _FBB_COVERAGEAREA_RELATION.Get() on c.CVRID equals r.CVRID
                                                  join z in _FBB_COVERAGE_ZIPCODE.Get() on c.CVRID equals z.CVRID
                                                  join zc in _FBB_ZIPCODE.Get() on z.ZIPCODE_ROWID_TH equals zc.ZIPCODE_ROWID
                                                  where c.NODESTATUS == "ON_SITE" && c.COMPLETE_FLAG == "Y" && d.ACTIVEFLAG == "Y"
                                                  && r.ACTIVEFLAG == "Y" && zc.LANG_FLAG == "N" && !zc.AMPHUR.Contains("ปณ")
                                                   && zc.REGION_CODE == regionin.region
                                                  && zc.PROVINCE == provincein.province && zc.AMPHUR == aumphurin.aumphur
                                                  && zc.TUMBON == nodenamethdata.tumbon
                                                  select new NodeNameTH_VDSL
                                                  {
                                                      Node_Name = c.NODENAME_TH
                                                  }).Distinct().ToList();
                                nodenamethdata.NodeNamelist = resultNodeName;


                            }
                            aumphurin.Tumbonlist = resulttumbon;
                        }
                        provincein.Aumphurlist = resultaumphurUnion;

                    }
                    regionin.Provincelist = resultprovince;
                    finalresult.Add(regionin);
                }
                return finalresult;
            }

            return new List<Vdsl_fbb_PanelModel>();
        }
    }
}
