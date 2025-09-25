using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.FTTx;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHanlders.FTTx
{
    public class GetCoverageRegionHandler : IQueryHandler<GetCoverageRegionQuery, List<GridFTTxModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;

        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public GetCoverageRegionHandler(ILogger logger
            , IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;

            _FBB_ZIPCODE = FBB_ZIPCODE;
        }

        public List<GridFTTxModel> Handle(GetCoverageRegionQuery query)
        {

            var Nodatat = "";
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

                var dataEN = (from r in _FBB_COVERAGE_REGION.Get()
                              join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_EN equals zip.ZIPCODE_ROWID
                              where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "Y"
                              && groupIdList.Contains(r.ZIPCODE_ROWID_TH) && !zip.AMPHUR.Contains("PO")
                              select new GridFTTxModel
                              {
                                  AmphurEN = zip.AMPHUR,
                                  ProvinceEN = zip.PROVINCE,
                                  FTTX_ID = r.FTTX_ID

                              }).Distinct();

                var dataTH = (from r in _FBB_COVERAGE_REGION.Get()
                              join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.ZIPCODE_ROWID
                              where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
                              && groupIdList.Contains(r.ZIPCODE_ROWID_TH) && !zip.AMPHUR.Contains("ปณ")
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
                                  CREATED_DATE = r.CREATED_DATE,
                                  UPDATE_DATE = r.UPDATED_DATE,
                                  GroupAmphur = zip.GROUP_AMPHUR,
                                  Zipcode = zip.ZIPCODE,
                                  latitude = r.LATITUDE,
                                  longitude = r.LONGITUDE,
                                  FTTX_ID = r.FTTX_ID,
                                  ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                  ONTARGET_DATE_IN = r.ONTARGET_DATE_IN

                              }).Distinct();

                var datasum = (from en in dataEN.ToList()
                               join th in dataTH.ToList() on en.FTTX_ID equals th.FTTX_ID
                               select new GridFTTxModel
                               {
                                   RegionCode = th.RegionCode,
                                   Amphur = th.Amphur,
                                   Tumbon = th.Tumbon,
                                   Province = th.Province,
                                   Coverage_Status = th.Coverage_Status,
                                   OwnerProduct = th.OwnerProduct,
                                   OwnerType = th.OwnerType,
                                   SERVICE_TYPE = th.SERVICE_TYPE,
                                   tower_th = th.tower_th,
                                   tower_en = th.tower_en,
                                   ONTARGET_DATE_EX = th.ONTARGET_DATE_EX,
                                   ONTARGET_DATE_IN = th.ONTARGET_DATE_IN,
                                   CREATED_DATE = th.CREATED_DATE,
                                   UPDATE_DATE = th.UPDATE_DATE,
                                   GroupAmphur = th.GroupAmphur,
                                   Zipcode = th.Zipcode,
                                   FTTX_ID = th.FTTX_ID,
                                   latitude = th.latitude,
                                   longitude = th.longitude,
                                   //// eng ////
                                   ProvinceEN = en.ProvinceEN,
                                   AmphurEN = en.AmphurEN,
                                   TumbonEN = en.TumbonEN
                               });


                if (query.OwnerProduct != "")
                    datasum = from r in datasum where r.OwnerProduct == query.OwnerProduct select r;

                if (query.OwnerType != "")
                    datasum = from r in datasum where r.OwnerType == query.OwnerType select r;
                if (query.tower_th != "")
                    datasum = from r in datasum where r.tower_th == query.tower_th select r;


                return datasum.ToList();
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

                    Nodatat = "2";
                }


                if (Nodatat == "2")
                {
                    var dataEN = (from r in _FBB_COVERAGE_REGION.Get()
                                  join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_EN equals zip.GROUP_AMPHUR
                                  where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "Y" && r.SERVICE_TYPE == "OTHER"
                                   && !zip.AMPHUR.Contains("PO")
                                  select new GridFTTxModel
                                  {
                                      AmphurEN = zip.AMPHUR,
                                      ProvinceEN = zip.PROVINCE,
                                      FTTX_ID = r.FTTX_ID
                                  }).Distinct();


                    var dataTH = (from r in _FBB_COVERAGE_REGION.Get()
                                  join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.GROUP_AMPHUR
                                  where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N" && r.SERVICE_TYPE == "OTHER"
                                   && !zip.AMPHUR.Contains("ปณ")
                                  select new GridFTTxModel
                                  {
                                      RegionCode = zip.REGION_CODE,
                                      Amphur = zip.AMPHUR,
                                      Province = zip.PROVINCE,
                                      Coverage_Status = r.COVERAGE_STATUS,
                                      OwnerProduct = r.OWNER_PRODUCT,
                                      OwnerType = r.OWNER_TYPE,
                                      SERVICE_TYPE = r.SERVICE_TYPE,
                                      tower_th = r.TOWER_TH,
                                      tower_en = r.TOWER_EN,
                                      ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                      ONTARGET_DATE_IN = r.ONTARGET_DATE_IN,
                                      CREATED_DATE = r.CREATED_DATE,
                                      UPDATE_DATE = r.UPDATED_DATE,
                                      GroupAmphur = zip.GROUP_AMPHUR,
                                      Zipcode = "",
                                      FTTX_ID = r.FTTX_ID
                                  }).Distinct();

                    var datasum = (from en in dataEN.ToList()
                                   join th in dataTH.ToList() on en.FTTX_ID equals th.FTTX_ID
                                   select new GridFTTxModel
                                   {
                                       RegionCode = th.RegionCode,
                                       Amphur = th.Amphur,
                                       Province = th.Province,
                                       Coverage_Status = th.Coverage_Status,
                                       OwnerProduct = th.OwnerProduct,
                                       OwnerType = th.OwnerType,
                                       SERVICE_TYPE = th.SERVICE_TYPE,
                                       tower_th = th.tower_th,
                                       tower_en = th.tower_en,
                                       ONTARGET_DATE_EX = th.ONTARGET_DATE_EX,
                                       ONTARGET_DATE_IN = th.ONTARGET_DATE_IN,
                                       CREATED_DATE = th.CREATED_DATE,
                                       UPDATE_DATE = th.UPDATE_DATE,
                                       GroupAmphur = th.GroupAmphur,
                                       Zipcode = th.Zipcode,
                                       FTTX_ID = th.FTTX_ID,
                                       //// eng ////
                                       ProvinceEN = en.ProvinceEN,
                                       AmphurEN = en.AmphurEN
                                   }).Distinct();


                    var data2EN = (from r in _FBB_COVERAGE_REGION.Get()
                                   join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_EN equals zip.ZIPCODE_ROWID
                                   where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "Y"
                                   //&& !zip.AMPHUR.Contains("PO")
                                   select new GridFTTxModel
                                   {
                                       AmphurEN = zip.AMPHUR,
                                       ProvinceEN = zip.PROVINCE,
                                       TumbonEN = zip.TUMBON,
                                       FTTX_ID = r.FTTX_ID
                                   });

                    var data2TH = (from r in _FBB_COVERAGE_REGION.Get()
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
                                       Coverage_Status = r.COVERAGE_STATUS,
                                       SERVICE_TYPE = r.SERVICE_TYPE,
                                       tower_th = r.TOWER_TH,
                                       tower_en = r.TOWER_EN,
                                       ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                       ONTARGET_DATE_IN = r.ONTARGET_DATE_IN,
                                       CREATED_DATE = r.CREATED_DATE,
                                       UPDATE_DATE = r.UPDATED_DATE,
                                       GroupAmphur = zip.GROUP_AMPHUR,
                                       Zipcode = zip.ZIPCODE,
                                       latitude = r.LATITUDE,
                                       longitude = r.LONGITUDE,
                                       FTTX_ID = r.FTTX_ID
                                   });

                    var data2sum = (from en in data2EN.ToList()
                                    join th in data2TH.ToList() on en.FTTX_ID equals th.FTTX_ID
                                    select new GridFTTxModel
                                    {
                                        RegionCode = th.RegionCode,
                                        Amphur = th.Amphur,
                                        Tumbon = th.Tumbon,
                                        Province = th.Province,
                                        Coverage_Status = th.Coverage_Status,
                                        OwnerProduct = th.OwnerProduct,
                                        OwnerType = th.OwnerType,
                                        SERVICE_TYPE = th.SERVICE_TYPE,
                                        tower_th = th.tower_th,
                                        tower_en = th.tower_en,
                                        ONTARGET_DATE_EX = th.ONTARGET_DATE_EX,
                                        ONTARGET_DATE_IN = th.ONTARGET_DATE_IN,
                                        CREATED_DATE = th.CREATED_DATE,
                                        UPDATE_DATE = th.UPDATE_DATE,
                                        GroupAmphur = th.GroupAmphur,
                                        FTTX_ID = th.FTTX_ID,
                                        Zipcode = th.Zipcode,
                                        latitude = th.latitude,
                                        longitude = th.longitude,
                                        //// eng ////
                                        ProvinceEN = en.ProvinceEN,
                                        AmphurEN = en.AmphurEN,
                                        TumbonEN = en.TumbonEN
                                    });


                    if (query.OwnerType != "")
                    {
                        datasum = from r in datasum where r.OwnerType == query.OwnerType select r;
                        data2sum = from r in data2sum where r.OwnerType == query.OwnerType select r;
                    }
                    if (query.OwnerProduct != "")
                    {
                        datasum = from r in datasum where r.OwnerProduct == query.OwnerProduct select r;
                        data2sum = from r in data2sum where r.OwnerProduct == query.OwnerProduct select r;
                    }
                    if (query.tower_th != "")
                    {
                        datasum = from r in datasum where r.tower_th == query.tower_th select r;
                        data2sum = from r in data2sum where r.tower_th == query.tower_th select r;
                    }

                    var result = datasum.ToList();
                    result = result.GroupBy(s => new { s.Province, s.Amphur, s.Tumbon, s.OwnerProduct, s.OwnerType }).Select(g => g.First()).ToList();
                    result.AddRange(data2sum.ToList());

                    return result;

                }
                else // has parameter except tumbon 
                {

                    var dataEN = (from r in _FBB_COVERAGE_REGION.Get()
                                  join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_EN equals zip.GROUP_AMPHUR
                                  where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "Y" && r.SERVICE_TYPE == "OTHER"
                                   && !zip.AMPHUR.Contains("PO") && groupIdList.Contains(r.GROUP_AMPHUR)
                                  select new GridFTTxModel
                                  {
                                      AmphurEN = zip.AMPHUR,
                                      ProvinceEN = zip.PROVINCE,
                                      FTTX_ID = r.FTTX_ID

                                  }).Distinct();

                    var dataTH = (from r in _FBB_COVERAGE_REGION.Get()
                                  join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_TH equals zip.GROUP_AMPHUR
                                  where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N" && r.SERVICE_TYPE == "OTHER"
                                  && !zip.AMPHUR.Contains("ปณ") && groupIdList.Contains(r.GROUP_AMPHUR)
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
                                      Coverage_Status = r.COVERAGE_STATUS,
                                      CREATED_DATE = r.CREATED_DATE,
                                      UPDATE_DATE = r.UPDATED_DATE,
                                      Zipcode = "",
                                      GroupAmphur = zip.GROUP_AMPHUR,
                                      FTTX_ID = r.FTTX_ID,
                                      ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                      ONTARGET_DATE_IN = r.ONTARGET_DATE_IN

                                  }).Distinct();


                    var datasum = (from en in dataEN.ToList()
                                   join th in dataTH.ToList() on en.FTTX_ID equals th.FTTX_ID
                                   select new GridFTTxModel
                                   {
                                       RegionCode = th.RegionCode,
                                       Amphur = th.Amphur,
                                       Province = th.Province,
                                       Coverage_Status = th.Coverage_Status,
                                       OwnerProduct = th.OwnerProduct,
                                       OwnerType = th.OwnerType,
                                       SERVICE_TYPE = th.SERVICE_TYPE,
                                       tower_th = th.tower_th,
                                       tower_en = th.tower_en,
                                       ONTARGET_DATE_EX = th.ONTARGET_DATE_EX,
                                       ONTARGET_DATE_IN = th.ONTARGET_DATE_IN,
                                       CREATED_DATE = th.CREATED_DATE,
                                       UPDATE_DATE = th.UPDATE_DATE,
                                       GroupAmphur = th.GroupAmphur,
                                       FTTX_ID = th.FTTX_ID,
                                       Zipcode = th.Zipcode,
                                       //// eng ////
                                       ProvinceEN = en.ProvinceEN,
                                       AmphurEN = en.AmphurEN
                                   }).Distinct();

                    var data2EN = (from r in _FBB_COVERAGE_REGION.Get()
                                   join zip in _FBB_ZIPCODE.Get() on r.ZIPCODE_ROWID_EN equals zip.ZIPCODE_ROWID
                                   where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "Y"
                                   && groupIdListzip.Contains(r.ZIPCODE_ROWID_TH)// && !zip.AMPHUR.Contains("PO") && groupIdListzip.Contains(r.ZIPCODE_ROWID_TH)
                                   select new GridFTTxModel
                                   {
                                       AmphurEN = zip.AMPHUR,
                                       ProvinceEN = zip.PROVINCE,
                                       TumbonEN = zip.TUMBON,
                                       FTTX_ID = r.FTTX_ID
                                   });

                    var data2TH = (from r in _FBB_COVERAGE_REGION.Get()
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
                                       CREATED_DATE = r.CREATED_DATE,
                                       UPDATE_DATE = r.UPDATED_DATE,
                                       GroupAmphur = zip.GROUP_AMPHUR,
                                       Zipcode = zip.ZIPCODE,
                                       latitude = r.LATITUDE,
                                       longitude = r.LONGITUDE,
                                       FTTX_ID = r.FTTX_ID,
                                       ONTARGET_DATE_EX = r.ONTARGET_DATE_EX,
                                       ONTARGET_DATE_IN = r.ONTARGET_DATE_IN
                                   });

                    var data2sum = (from en in data2EN.ToList()
                                    join th in data2TH.ToList() on en.FTTX_ID equals th.FTTX_ID
                                    select new GridFTTxModel
                                    {
                                        RegionCode = th.RegionCode,
                                        Amphur = th.Amphur,
                                        Province = th.Province,
                                        Tumbon = th.Tumbon,
                                        Coverage_Status = th.Coverage_Status,
                                        OwnerProduct = th.OwnerProduct,
                                        OwnerType = th.OwnerType,
                                        SERVICE_TYPE = th.SERVICE_TYPE,
                                        tower_th = th.tower_th,
                                        tower_en = th.tower_en,
                                        ONTARGET_DATE_EX = th.ONTARGET_DATE_EX,
                                        ONTARGET_DATE_IN = th.ONTARGET_DATE_IN,
                                        CREATED_DATE = th.CREATED_DATE,
                                        UPDATE_DATE = th.UPDATE_DATE,
                                        GroupAmphur = th.GroupAmphur,
                                        FTTX_ID = th.FTTX_ID,
                                        latitude = th.latitude,
                                        longitude = th.longitude,
                                        Zipcode = th.Zipcode,
                                        //// eng ////
                                        ProvinceEN = en.ProvinceEN,
                                        AmphurEN = en.AmphurEN,
                                        TumbonEN = en.TumbonEN
                                    });


                    if (query.OwnerType != "")
                    {
                        datasum = from r in datasum where r.OwnerType == query.OwnerType select r;
                        data2sum = from r in data2sum where r.OwnerType == query.OwnerType select r;
                    }
                    if (query.OwnerProduct != "")
                    {
                        datasum = from r in datasum where r.OwnerProduct == query.OwnerProduct select r;
                        data2sum = from r in data2sum where r.OwnerProduct == query.OwnerProduct select r;
                    }
                    if (query.tower_th != "")
                    {
                        datasum = from r in datasum where r.tower_th == query.tower_th select r;
                        data2sum = from r in data2sum where r.tower_th == query.tower_th select r;
                    }

                    var result = datasum.ToList();
                    result = result.GroupBy(s => new { s.Province, s.Amphur, s.Tumbon, s.OwnerProduct, s.OwnerType }).Select(g => g.First()).ToList();
                    result.AddRange(data2sum.ToList());
                    return result;

                }
            }
        }
    }
}


//var Nodatat = "";
//var groupIdList = new List<string>();
//var groupIdList2 = (from r in _FBB_ZIPCODE.Get()
//                    where r.LANG_FLAG == "N"
//                    select new
//                    {
//                        RegionCode = r.REGION_CODE,
//                        Province = r.PROVINCE,
//                        Amphur = r.AMPHUR,
//                        GroupAmphur = r.GROUP_AMPHUR


//                    });



//if (query.Region != "" && query.Amphur =="" && query.Province == "")
//{
//    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region select r2.GroupAmphur).Distinct().ToList();

//}
//else if (query.Amphur != "" && query.Province == "" && query.Region == "")
//{
//    groupIdList = (from r2 in groupIdList2 where r2.Amphur == query.Amphur select r2.GroupAmphur).Distinct().ToList();
//}
//else if (query.Province != "" && query.Region == "" && query.Amphur=="")
//{
//    groupIdList = (from r2 in groupIdList2 where r2.Province == query.Province select r2.GroupAmphur).Distinct().ToList();
//}

//else if (query.Region != "" && query.Province != "" && query.Amphur=="")
//{
//    groupIdList = (from r2 in groupIdList2 where r2.RegionCode == query.Region && r2.Province == query.Province select r2.GroupAmphur).Distinct().ToList();
//}
//else if (query.Region != "" && query.Amphur != "" && query.Province=="")
//{
//    groupIdList = (from r2 in groupIdList2
//                   where r2.RegionCode == query.Region &&
//                       r2.Amphur == query.Amphur
//                   select r2.GroupAmphur).Distinct().ToList();

//}
//else if (query.Province != "" && query.Amphur != "" &&query.Region=="")
//{

//    groupIdList = (from r2 in groupIdList2
//                   where r2.Province == query.Province &&
//                       r2.Amphur == query.Amphur
//                   select r2.GroupAmphur).Distinct().ToList();

//}
//else if (query.Province != "" && query.Region != "" && query.Amphur != "")
//{

//    groupIdList = (from r2 in groupIdList2
//                   where r2.RegionCode == query.Region
//                       && r2.Amphur == query.Amphur && r2.Province == query.Province
//                   select r2.GroupAmphur).Distinct().ToList();
//}
//else if (query.Province == "" && query.Region == "" && query.Amphur == "")
//{

//    Nodatat = "2";
//}


//if (Nodatat == "2")
//{
//    var data = (from r in _FBB_COVERAGE_REGION.Get()
//                join zip in _FBB_ZIPCODE.Get() on r.GROUP_AMPHUR equals zip.GROUP_AMPHUR
//                where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
//                 && !zip.AMPHUR.Contains("ปณ")
//                select new GridFTTxModel
//                {

//                    RegionCode = zip.REGION_CODE,
//                    Amphur = zip.AMPHUR,
//                    Province = zip.PROVINCE,
//                    OwnerProduct = r.OWNER_PRODUCT,
//                    OwnerType = r.OWNER_TYPE,
//                    SERVICE_TYPE = r.SERVICE_TYPE,
//                    tower_th = r.TOWER_TH,
//                    tower_en = r.TOWER_EN,
//                    //ONTARGET_DATE = r.ONTARGET_DATE
//                    CREATED_DATE = r.CREATED_DATE,
//                    UPDATE_DATE = r.UPDATED_DATE,
//                    GroupAmphur = zip.GROUP_AMPHUR,
//                    FTTX_ID = r.FTTX_ID
//                }).Distinct();

//    if (query.OwnerType != "")
//        data = from r in data where r.OwnerType == query.OwnerType select r;
//    if (query.OwnerProduct != "")
//        data = from r in data where r.OwnerProduct == query.OwnerProduct select r;
//    if (query.tower_th != "")
//        data = from r in data where r.tower_th == query.tower_th select r;
//    return data.ToList();

//}
//else
//{

//    var data = (from r in _FBB_COVERAGE_REGION.Get()
//                join zip in _FBB_ZIPCODE.Get() on r.GROUP_AMPHUR equals zip.GROUP_AMPHUR
//                where r.ACTIVEFLAG == "Y" && zip.LANG_FLAG == "N"
//                && !zip.AMPHUR.Contains("ปณ") && groupIdList.Contains(r.GROUP_AMPHUR)
//                select new GridFTTxModel
//                {
//                    RegionCode = zip.REGION_CODE,
//                    Amphur = zip.AMPHUR,
//                    Province = zip.PROVINCE,
//                    OwnerProduct = r.OWNER_PRODUCT,
//                    OwnerType = r.OWNER_TYPE,
//                    SERVICE_TYPE = r.SERVICE_TYPE,
//                    tower_th = r.TOWER_TH,
//                    tower_en = r.TOWER_EN,
//                    CREATED_DATE = r.CREATED_DATE,
//                    UPDATE_DATE = r.UPDATED_DATE,
//                    GroupAmphur = zip.GROUP_AMPHUR,
//                    FTTX_ID = r.FTTX_ID
//                    //ONTARGET_DATE = r.ONTARGET_DATE
//                }).Distinct();
//    if (query.OwnerProduct != "")
//        data = from r in data where r.OwnerProduct == query.OwnerProduct select r;
//    if (query.OwnerType != "")
//        data = from r in data where r.OwnerType == query.OwnerType select r;
//    if (query.tower_th != "")
//        data = from r in data where r.tower_th == query.tower_th select r;
//    return data.ToList();

//}