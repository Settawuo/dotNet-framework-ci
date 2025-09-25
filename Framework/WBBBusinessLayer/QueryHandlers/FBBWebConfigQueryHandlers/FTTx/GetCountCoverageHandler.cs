using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.FTTx;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHanlders.FTTx
{
    public class GetCountCoverageHandler : IQueryHandler<GetCountCoverageQuery, CountCoverageModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;

        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public GetCountCoverageHandler(ILogger logger
            , IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
            _FBB_ZIPCODE = FBB_ZIPCODE;

        }

        public CountCoverageModel Handle(GetCountCoverageQuery query)
        {

            var noData = "";
            var groupIdList = new List<string>();
            var groupIdListzip = new List<string>();
            var groupIdList2 = (from r in _FBB_ZIPCODE.Get()
                                where r.LANG_FLAG == "N"
                                //&& !r.AMPHUR.Contains("ปณ")
                                select new
                                {
                                    RegionCode = r.REGION_CODE,
                                    Province = r.PROVINCE,
                                    Amphur = r.AMPHUR,
                                    Tumbon = r.TUMBON,
                                    GroupAmphur = r.GROUP_AMPHUR,
                                    Zipcode_rowid = r.ZIPCODE_ROWID

                                }).Distinct();


            if (query.Tumbon != "")
            {
                if (query.Region != "" && query.Amphur == "" && query.Province == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region && r2.Tumbon == query.Tumbon select r2.Zipcode_rowid).ToList();

                }
                else if (query.Amphur != "" && query.Province == "" && query.Region == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.Amphur == query.Amphur && r2.Tumbon == query.Tumbon select r2.Zipcode_rowid).ToList();
                }
                else if (query.Province != "" && query.Region == "" && query.Amphur == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.Province == query.Province && r2.Tumbon == query.Tumbon select r2.Zipcode_rowid).ToList();
                }

                else if (query.Region != "" && query.Province != "" && query.Amphur == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region && r2.Province == query.Province && r2.Tumbon == query.Tumbon select r2.Zipcode_rowid).ToList();
                }
                else if (query.Region != "" && query.Amphur != "" && query.Province == "")
                {
                    groupIdList = (from r2 in groupIdList2
                                   where r2.RegionCode == query.Region &&
                                       r2.Amphur == query.Amphur
                                       && r2.Tumbon == query.Tumbon
                                   select r2.Zipcode_rowid).ToList();

                }
                else if (query.Province != "" && query.Amphur != "" && query.Region == "")
                {

                    groupIdList = (from r2 in groupIdList2
                                   where r2.Province == query.Province &&
                                       r2.Amphur == query.Amphur
                                       && r2.Tumbon == query.Tumbon
                                   select r2.Zipcode_rowid).ToList();

                }
                else if (query.Province != "" && query.Region != "" && query.Amphur != "")
                {

                    groupIdList = (from r2 in groupIdList2
                                   where r2.RegionCode == query.Region
                                       && r2.Amphur == query.Amphur && r2.Province == query.Province
                                       && r2.Tumbon == query.Tumbon
                                   select r2.Zipcode_rowid).ToList();
                }
                else
                {
                    groupIdList = (from r2 in groupIdList2
                                   where r2.Tumbon == query.Tumbon
                                   select r2.Zipcode_rowid).ToList();
                }

                var data = (from r in _FBB_COVERAGE_REGION.Get()
                            join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.ZIPCODE_ROWID
                            where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                            && groupIdList.Contains(r.ZIPCODE_ROWID_TH) //&& !zip.AMPHUR.Contains("ปณ")
                            select new GridFTTxModel
                            {
                                RegionCode = zip.REGION_CODE,
                                Amphur = zip.AMPHUR,
                                Province = zip.PROVINCE,
                                OwnerProduct = r.OWNER_PRODUCT,
                                OwnerType = r.OWNER_TYPE,
                                SERVICE_TYPE = r.SERVICE_TYPE,
                                tower_th = r.TOWER_TH,
                                tower_en = r.TOWER_EN,
                                //ONTARGET_DATE = r.ONTARGET_DATE
                                CREATED_DATE = r.CREATED_DATE,
                                UPDATE_DATE = r.UPDATED_DATE,
                                GroupAmphur = zip.GROUP_AMPHUR,
                                FTTX_ID = r.FTTX_ID
                            }).Distinct();


                if (query.OwnerProduct != "")
                    data = from r in data where r.OwnerProduct == query.OwnerProduct select r;

                if (query.OwnerType != "")
                    data = from r in data where r.OwnerType == query.OwnerType select r;
                if (query.tower_th != "")
                    data = from r in data where r.tower_th == query.tower_th select r;


                return new CountCoverageModel
                {
                    Total = data.Count(),
                    NSN = (from r in data where r.OwnerProduct == "NSN" select r).Count(),
                    SIMAT = (from r in data where r.OwnerProduct == "SIMAT" select r).Count(),
                    AIS = (from r in data where r.OwnerProduct == "AIS" select r).Count(),
                    SYMPHONY = (from r in data where r.OwnerProduct == "SYMPHONY" select r).Count(),

                };
            }
            else
            {

                if (query.Region != "" && query.Amphur == "" && query.Province == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2 where r2.RegionCode == query.Region select r2.Zipcode_rowid).Distinct().ToList();
                }
                else if (query.Amphur != "" && query.Province == "" && query.Region == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.Amphur == query.Amphur select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2 where r2.Amphur == query.Amphur select r2.Zipcode_rowid).Distinct().ToList();
                }
                else if (query.Province != "" && query.Region == "" && query.Amphur == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.Province == query.Province select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2 where r2.Province == query.Province select r2.Zipcode_rowid).Distinct().ToList();
                }

                else if (query.Region != "" && query.Province != "" && query.Amphur == "")
                {
                    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region && r2.Province == query.Province select r2.GroupAmphur).Distinct
().ToList();
                    groupIdListzip = (from r2 in groupIdList2 where r2.RegionCode == query.Region && r2.Province == query.Province select r2.Zipcode_rowid).Distinct
().ToList();
                }
                else if (query.Region != "" && query.Amphur != "" && query.Province == "")
                {
                    groupIdList = (from r2 in groupIdList2
                                   where r2.RegionCode == query.Region &&
                                       r2.Amphur == query.Amphur
                                   select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2
                                      where r2.RegionCode == query.Region &&
                                          r2.Amphur == query.Amphur
                                      select r2.Zipcode_rowid).Distinct().ToList();

                }
                else if (query.Province != "" && query.Amphur != "" && query.Region == "")
                {

                    groupIdList = (from r2 in groupIdList2
                                   where r2.Province == query.Province &&
                                       r2.Amphur == query.Amphur
                                   select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2
                                      where r2.Province == query.Province &&
                                          r2.Amphur == query.Amphur
                                      select r2.Zipcode_rowid).Distinct().ToList();

                }
                else if (query.Province != "" && query.Region != "" && query.Amphur != "")
                {

                    groupIdList = (from r2 in groupIdList2
                                   where r2.RegionCode == query.Region
                                       && r2.Amphur == query.Amphur && r2.Province == query.Province
                                   select r2.GroupAmphur).Distinct().ToList();
                    groupIdListzip = (from r2 in groupIdList2
                                      where r2.RegionCode == query.Region
                                          && r2.Amphur == query.Amphur && r2.Province == query.Province
                                      select r2.Zipcode_rowid).Distinct().ToList();
                }
                else if (query.Province == "" && query.Region == "" && query.Amphur == "")
                {

                    noData = "2";
                }

                if (noData == "2")
                {

                    //var data = (from r in _FBB_COVERAGE_REGION.Get()
                    //            join zip in _FBB_ZIPCODE.Get() on r.GROUP_AMPHUR equals zip.GROUP_AMPHUR
                    //            where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                    //           && !zip.AMPHUR.Contains("ปณ")

                    //            select new GridFTTxModel
                    //            {
                    //                RegionCode = zip.REGION_CODE,
                    //                Amphur = zip.AMPHUR,
                    //                Province = zip.PROVINCE,
                    //                OwnerProduct = r.OWNER_PRODUCT,
                    //                OwnerType = r.OWNER_TYPE,
                    //                SERVICE_TYPE = r.SERVICE_TYPE,
                    //                tower_th = r.TOWER_TH,
                    //                tower_en = r.TOWER_EN,
                    //                //ONTARGET_DATE = r.ONTARGET_DATE
                    //                CREATED_DATE = r.CREATED_DATE,
                    //                UPDATE_DATE = r.UPDATED_DATE,
                    //                GroupAmphur = zip.GROUP_AMPHUR,
                    //                FTTX_ID = r.FTTX_ID
                    //            }).Distinct();


                    //if (query.OwnerProduct != "")
                    //    data = from r in data where r.OwnerProduct == query.OwnerProduct select r;

                    //if (query.OwnerType != "")
                    //    data = from r in data where r.OwnerType == query.OwnerType select r;
                    //if (query.tower_th != "")
                    //    data = from r in data where r.tower_th == query.tower_th select r;
                    var data = (from r in _FBB_COVERAGE_REGION.Get()
                                join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.GROUP_AMPHUR
                                where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N" && r.SERVICE_TYPE == "OTHER"
                                 && !zip.AMPHUR.Contains("ปณ")
                                select new GridFTTxModel
                                {
                                    RegionCode = zip.REGION_CODE,
                                    Amphur = zip.AMPHUR,
                                    Province = zip.PROVINCE,
                                    //Tumbon = zip.TUMBON,
                                    OwnerProduct = r.OWNER_PRODUCT,
                                    OwnerType = r.OWNER_TYPE,
                                    SERVICE_TYPE = r.SERVICE_TYPE,
                                    tower_th = r.TOWER_TH,
                                    tower_en = r.TOWER_EN,
                                    //ONTARGET_DATE = r.ONTARGET_DATE
                                    CREATED_DATE = r.CREATED_DATE,
                                    UPDATE_DATE = r.UPDATED_DATE,
                                    GroupAmphur = zip.GROUP_AMPHUR,
                                    FTTX_ID = r.FTTX_ID
                                }).Distinct();

                    var data2 = (from r in _FBB_COVERAGE_REGION.Get()
                                 join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.ZIPCODE_ROWID
                                 where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                                 //&& !zip.AMPHUR.Contains("ปณ")
                                 select new GridFTTxModel
                                 {
                                     RegionCode = zip.REGION_CODE,
                                     Amphur = zip.AMPHUR,
                                     Province = zip.PROVINCE,
                                     Tumbon = zip.TUMBON,
                                     OwnerProduct = r.OWNER_PRODUCT,
                                     OwnerType = r.OWNER_TYPE,
                                     SERVICE_TYPE = r.SERVICE_TYPE,
                                     tower_th = r.TOWER_TH,
                                     tower_en = r.TOWER_EN,
                                     CREATED_DATE = r.CREATED_DATE,
                                     UPDATE_DATE = r.UPDATED_DATE,
                                     GroupAmphur = zip.GROUP_AMPHUR,
                                     FTTX_ID = r.FTTX_ID
                                 });


                    if (query.OwnerType != "")
                    {
                        data = from r in data where r.OwnerType == query.OwnerType select r;
                        data2 = from r in data2 where r.OwnerType == query.OwnerType select r;
                    }
                    if (query.OwnerProduct != "")
                    {
                        data = from r in data where r.OwnerProduct == query.OwnerProduct select r;
                        data2 = from r in data2 where r.OwnerProduct == query.OwnerProduct select r;
                    }
                    if (query.tower_th != "")
                    {
                        data = from r in data where r.tower_th == query.tower_th select r;
                        data2 = from r in data2 where r.tower_th == query.tower_th select r;
                    }

                    var result = data.ToList();

                    result.AddRange(data2.ToList());

                    return new CountCoverageModel
                    {
                        Total = result.Count(),
                        NSN = (from r in result where r.OwnerProduct == "NSN" select r).Count(),
                        SIMAT = (from r in result where r.OwnerProduct == "SIMAT" select r).Count(),
                        AIS = (from r in result where r.OwnerProduct == "AIS" select r).Count(),
                        SYMPHONY = (from r in result where r.OwnerProduct == "SYMPHONY" select r).Count(),

                    };

                }
                else
                {

                    //var data = (from r in _FBB_COVERAGE_REGION.Get()
                    //            join zip in _FBB_ZIPCODE.Get() on r.GROUP_AMPHUR equals zip.GROUP_AMPHUR
                    //            where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                    //            && groupIdList.Contains(r.GROUP_AMPHUR) && !zip.AMPHUR.Contains("ปณ")
                    //            select new GridFTTxModel
                    //            {
                    //                RegionCode = zip.REGION_CODE,
                    //                Amphur = zip.AMPHUR,
                    //                Province = zip.PROVINCE,
                    //                OwnerProduct = r.OWNER_PRODUCT,
                    //                OwnerType = r.OWNER_TYPE,
                    //                SERVICE_TYPE = r.SERVICE_TYPE,
                    //                tower_th = r.TOWER_TH,
                    //                tower_en = r.TOWER_EN,
                    //                //ONTARGET_DATE = r.ONTARGET_DATE
                    //                CREATED_DATE = r.CREATED_DATE,
                    //                UPDATE_DATE = r.UPDATED_DATE,
                    //                GroupAmphur = zip.GROUP_AMPHUR,
                    //                FTTX_ID = r.FTTX_ID
                    //            }).Distinct();


                    //if (query.OwnerProduct != "")
                    //    data = from r in data where r.OwnerProduct == query.OwnerProduct select r;

                    //if (query.OwnerType != "")
                    //    data = from r in data where r.OwnerType == query.OwnerType select r;
                    //if (query.tower_th != "")
                    //    data = from r in data where r.tower_th == query.tower_th select r;

                    var data = (from r in _FBB_COVERAGE_REGION.Get()
                                join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.GROUP_AMPHUR
                                where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N" && r.SERVICE_TYPE == "OTHER"
                                && !zip.AMPHUR.Contains("ปณ") && groupIdList.Contains(r.GROUP_AMPHUR)
                                select new GridFTTxModel
                                {
                                    RegionCode = zip.REGION_CODE,
                                    Amphur = zip.AMPHUR,
                                    //Tumbon = zip.TUMBON,
                                    Province = zip.PROVINCE,
                                    OwnerProduct = r.OWNER_PRODUCT,
                                    OwnerType = r.OWNER_TYPE,
                                    SERVICE_TYPE = r.SERVICE_TYPE,
                                    tower_th = r.TOWER_TH,
                                    tower_en = r.TOWER_EN,
                                    Coverage_Status = r.COVERAGE_STATUS,
                                    CREATED_DATE = r.CREATED_DATE,
                                    UPDATE_DATE = r.UPDATED_DATE,
                                    GroupAmphur = zip.GROUP_AMPHUR,
                                    FTTX_ID = r.FTTX_ID
                                    //ONTARGET_DATE = r.ONTARGET_DATE
                                }).Distinct();

                    var data2 = (from r in _FBB_COVERAGE_REGION.Get()
                                 join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.ZIPCODE_ROWID
                                 where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                                 && groupIdListzip.Contains(r.ZIPCODE_ROWID_TH) //&& !zip.AMPHUR.Contains("ปณ")
                                 select new GridFTTxModel
                                 {
                                     RegionCode = zip.REGION_CODE,
                                     Amphur = zip.AMPHUR,
                                     Tumbon = zip.TUMBON,
                                     Province = zip.PROVINCE,
                                     OwnerProduct = r.OWNER_PRODUCT,
                                     OwnerType = r.OWNER_TYPE,
                                     SERVICE_TYPE = r.SERVICE_TYPE,
                                     tower_th = r.TOWER_TH,
                                     tower_en = r.TOWER_EN,
                                     Coverage_Status = r.COVERAGE_STATUS,
                                     //ONTARGET_DATE = r.ONTARGET_DATE
                                     CREATED_DATE = r.CREATED_DATE,
                                     UPDATE_DATE = r.UPDATED_DATE,
                                     GroupAmphur = zip.GROUP_AMPHUR,
                                     FTTX_ID = r.FTTX_ID
                                 });


                    if (query.OwnerType != "")
                    {
                        data = from r in data where r.OwnerType == query.OwnerType select r;
                        data2 = from r in data2 where r.OwnerType == query.OwnerType select r;
                    }
                    if (query.OwnerProduct != "")
                    {
                        data = from r in data where r.OwnerProduct == query.OwnerProduct select r;
                        data2 = from r in data2 where r.OwnerProduct == query.OwnerProduct select r;
                    }
                    if (query.tower_th != "")
                    {
                        data = from r in data where r.tower_th == query.tower_th select r;
                        data2 = from r in data2 where r.tower_th == query.tower_th select r;
                    }

                    var result = data.ToList();

                    result.AddRange(data2.ToList());

                    return new CountCoverageModel
                    {
                        Total = result.Count(),
                        NSN = (from r in result where r.OwnerProduct == "NSN" select r).Count(),
                        SIMAT = (from r in result where r.OwnerProduct == "SIMAT" select r).Count(),
                        AIS = (from r in result where r.OwnerProduct == "AIS" select r).Count(),
                        SYMPHONY = (from r in result where r.OwnerProduct == "SYMPHONY" select r).Count(),

                    };

                }
            }




        }
    }
}
