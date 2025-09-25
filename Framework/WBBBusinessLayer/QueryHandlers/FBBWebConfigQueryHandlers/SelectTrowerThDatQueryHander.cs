using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class SelectTrowerThDatQueryHander : IQueryHandler<selectDataTowerTh, List<LovModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;

        public SelectTrowerThDatQueryHander(ILogger logger, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION)
        {
            _logger = logger;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
        }


        public List<LovModel> Handle(selectDataTowerTh query)
        {

            if (query.TUMBON != "") // 9 case
            {
                var ziplowlist = new List<string>();
                if (query.REGION_CODE != "" && query.PROVINCE != "" && query.AUMPHUR != "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.PROVINCE == query.PROVINCE && r.REGION_CODE == query.REGION_CODE && r.AMPHUR == query.AUMPHUR
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE == "" && query.AUMPHUR == "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.REGION_CODE == query.REGION_CODE
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE != "" && query.AUMPHUR == "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.PROVINCE == query.PROVINCE
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE == "" && query.AUMPHUR != "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.AMPHUR == query.AUMPHUR
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE != "" && query.AUMPHUR != "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.PROVINCE == query.PROVINCE && r.AMPHUR == query.AUMPHUR
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE != "" && query.AUMPHUR == "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.PROVINCE == query.PROVINCE && r.REGION_CODE == query.REGION_CODE
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE == "" && query.AUMPHUR != "")
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  && r.AMPHUR == query.AUMPHUR && r.REGION_CODE == query.REGION_CODE
                                  select r.ZIPCODE_ROWID).ToList();
                }
                else
                {
                    ziplowlist = (from r in _FBB_ZIPCODE.Get()
                                  where r.LANG_FLAG == "N"
                                  && !r.AMPHUR.Contains("ปณ")
                                  && r.TUMBON == query.TUMBON
                                  select r.ZIPCODE_ROWID).ToList();
                }

                var b = (from r in _FBB_COVERAGE_REGION.Get()
                         where r.ACTIVEFLAG == "Y" && ziplowlist.Contains(r.ZIPCODE_ROWID_TH)

                         //orderby r.TOWER_TH ascending
                         select new LovModel
                         {
                             LOV_NAME = r.TOWER_TH.Trim(),
                             DISPLAY_VAL = r.TOWER_TH.Trim()


                         }).OrderBy(r => r.LOV_NAME).DistinctBy(r => r.LOV_NAME).ToList();
                return b;

            }
            else
            {
                var grouplist = new List<string>();
                if (query.REGION_CODE != "" && query.PROVINCE != "" && query.AUMPHUR != "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.PROVINCE == query.PROVINCE && r.REGION_CODE == query.REGION_CODE && r.AMPHUR == query.AUMPHUR
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE == "" && query.AUMPHUR == "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.REGION_CODE == query.REGION_CODE
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE != "" && query.AUMPHUR == "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.PROVINCE == query.PROVINCE
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE == "" && query.AUMPHUR != "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.AMPHUR == query.AUMPHUR
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE == "" && query.PROVINCE != "" && query.AUMPHUR != "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.PROVINCE == query.PROVINCE && r.AMPHUR == query.AUMPHUR
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE != "" && query.AUMPHUR == "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.PROVINCE == query.PROVINCE && r.REGION_CODE == query.REGION_CODE
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else if (query.REGION_CODE != "" && query.PROVINCE == "" && query.AUMPHUR != "")
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 && r.AMPHUR == query.AUMPHUR && r.REGION_CODE == query.REGION_CODE
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }
                else
                {
                    grouplist = (from r in _FBB_ZIPCODE.Get()
                                 where r.LANG_FLAG == "N"
                                 && !r.AMPHUR.Contains("ปณ")
                                 select r.GROUP_AMPHUR).Distinct().ToList();
                }

                var b = (from r in _FBB_COVERAGE_REGION.Get()
                         where r.ACTIVEFLAG == "Y" && grouplist.Contains(r.GROUP_AMPHUR)
                         select new LovModel
                         {
                             LOV_NAME = r.TOWER_TH.Trim(),
                             DISPLAY_VAL = r.TOWER_TH.Trim()

                         }).OrderBy(r => r.LOV_NAME).DistinctBy(r => r.LOV_NAME).ToList();
                return b;
            }


            //var groupIdList2 = (from r in _FBB_ZIPCODE.Get()
            //                    where r.LANG_FLAG == "N"
            //                     && !r.AMPHUR.Contains("ปณ")
            //                     && r.AMPHUR == query.AUMPHUR
            //                    select r.GROUP_AMPHUR).Distinct().FirstOrDefault();

            //if (groupIdList2!=null)
            //{


            //    return (from r in _FBB_COVERAGE_REGION.Get()
            //            where r.ACTIVEFLAG == "Y" &&  groupIdList2.Contains(r.GROUP_AMPHUR)

            //            //orderby r.TOWER_TH ascending
            //            select new LovModel
            //            {
            //                LOV_NAME = r.TOWER_TH,
            //                DISPLAY_VAL=r.TOWER_TH


            //            }).Distinct().OrderBy(r=>r.LOV_NAME).ToList();
            //}
            //else {

            //    var b = (from r in _FBB_COVERAGE_REGION.Get()
            //            where  r.ACTIVEFLAG == "Y"
            //            //orderby r.TOWER_TH ascending
            //            select new LovModel
            //            {
            //                LOV_NAME = r.TOWER_TH,
            //                DISPLAY_VAL = r.TOWER_TH

            //            }).Distinct().OrderBy(r => r.LOV_NAME).ToList();
            //    return b;
            //}

        }
    }
}
