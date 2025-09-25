using System;
using System.Collections.Generic;
using System.Linq;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Queries.FBSS;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBSS
{
    public class GetbuildingChangeHandler : IQueryHandler<GetbuildingChangeQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_LISTBV_BY_LOCATION> _FBB_LISTBV_BY_LOCATION;
        private readonly IEntityRepository<FBB_LISTBV_LOCATION_GROUP> _FBB_LISTBV_LOCATION_GROUP;

        public GetbuildingChangeHandler(ILogger logger, IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV
            , IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE
            , IEntityRepository<FBB_LISTBV_BY_LOCATION> FBB_LISTBV_BY_LOCATION
            , IEntityRepository<FBB_LISTBV_LOCATION_GROUP> FBB_LISTBV_LOCATION_GROUP)
        {
            _logger = logger;
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_LISTBV_BY_LOCATION = FBB_LISTBV_BY_LOCATION;
            _FBB_LISTBV_LOCATION_GROUP = FBB_LISTBV_LOCATION_GROUP;
        }

        public List<DropdownModel> Handle(GetbuildingChangeQuery query)
        {
            List<DropdownModel> result = new List<DropdownModel>();
            string type = query.Typeaddress.ToSafeString();
            string accessMode = query.AccessMode.ToSafeString();
            string location_code = query.LOC_CODE.ToSafeString();

            try
            {
                if (query.ReloadCache.ToSafeString() == "Y")
                {
                    FBBCache.Clear("BV_ZIPCODE_TABLE");
                    FBBCache.Clear("BV_LISTBV_LOCATION_GROUP_TABLE");
                    FBBCache.Clear("BV_LISTBV_BY_LOCATION_TABLE");
                    FBBCache.Clear("BV_FBSS_LISTBV_TABLE");
                }

                var zipcodeCache = FBBCache.Get<List<FBB_ZIPCODE>>("BV_ZIPCODE_TABLE");
                var locationGroupCache = FBBCache.Get<List<FBB_LISTBV_LOCATION_GROUP>>("BV_LISTBV_LOCATION_GROUP_TABLE");
                var locationCache = FBBCache.Get<List<FBB_LISTBV_BY_LOCATION>>("BV_LISTBV_BY_LOCATION_TABLE");
                var fbssListBVCache = FBBCache.Get<List<FBB_FBSS_LISTBV>>("BV_FBSS_LISTBV_TABLE");

                if (null == zipcodeCache)
                {
                    FBBCache.Add(_FBB_ZIPCODE.Get().ToList(), "BV_ZIPCODE_TABLE");
                    zipcodeCache = FBBCache.Get<List<FBB_ZIPCODE>>("BV_ZIPCODE_TABLE");
                }

                if (null == locationGroupCache)
                {
                    FBBCache.Add(_FBB_LISTBV_LOCATION_GROUP.Get().ToList(), "BV_LISTBV_LOCATION_GROUP_TABLE");
                    locationGroupCache = FBBCache.Get<List<FBB_LISTBV_LOCATION_GROUP>>("BV_LISTBV_LOCATION_GROUP_TABLE");
                }

                if (null == locationCache)
                {
                    FBBCache.Add(_FBB_LISTBV_BY_LOCATION.Get().ToList(), "BV_LISTBV_BY_LOCATION_TABLE");
                    locationCache = FBBCache.Get<List<FBB_LISTBV_BY_LOCATION>>("BV_LISTBV_BY_LOCATION_TABLE");
                }

                if (null == fbssListBVCache)
                {
                    FBBCache.Add(_FBB_FBSS_LISTBV.Get().ToList(), "BV_FBSS_LISTBV_TABLE");
                    fbssListBVCache = FBBCache.Get<List<FBB_FBSS_LISTBV>>("BV_FBSS_LISTBV_TABLE");
                }

                if (type == "V")
                {
                    var moobanAddressIdList = (from t in fbssListBVCache
                                               where t.BUILDING_NAME.StartsWith("หมู่บ้าน") && t.ADDRESS_TYPE == "V"
                                               select t.ADDRESS_ID);

                    result = (from z in zipcodeCache
                              join l in
                                  (from t in fbssListBVCache
                                   where moobanAddressIdList.Contains(t.ADDRESS_ID)
                                   select t) on z.TUMBON equals l.SUB_DISTRICT
                              where z.ZIPCODE == l.POSTAL_CODE
                                && l.ACTIVE_FLAG == "Y"
                                && l.ADDRESS_TYPE == type
                              select new DropdownModel
                              {
                                  Text = l.BUILDING_NAME,
                                  Value = @"{ " +
                                      @"""zipcode_rowid"": """ + z.ZIPCODE_ROWID + @"""," +
                                      @"""address_id"": """ + l.ADDRESS_ID + @"""," +
                                      @"""lang"": """ + l.LANGUAGE + @"""," +
                                      @"""building_name"": """ + l.BUILDING_NAME + @"""," +
                                      @"""building_name_for_srh"": """ +
                                          l.BUILDING_NAME + " " +
                                          z.TUMBON + " " +
                                          z.AMPHUR + " " +
                                          z.PROVINCE +
                                      @"""," +
                                      @"""province"": """ + z.PROVINCE + @"""," +
                                      @"""amphur"": """ + z.AMPHUR + @"""," +
                                      @"""tumbon"": """ + z.TUMBON + @"""," +
                                      @"""zipcode"": """ + z.ZIPCODE + @"""," +
                                      @"""sitecode"": """ + l.SITE_CODE + @"""," +
                                      @"""latitude"": """ + l.LATITUDE + @"""," +
                                      @"""longtitude"": """ + l.LONGTITUDE + @"""," +
                                      @"""partner"": """ + l.PARTNER + @"""" +
                                   @" }",
                                  Value4 = l.BUILDING_NAME + z.TUMBON + z.ZIPCODE,
                              })
                          .ToList();
                }
                else
                {
                    #region For Millenium Condo
                    if (!string.IsNullOrEmpty(query.LOC_CODE))
                    {
                        var specificResult = locationGroupCache.Where(x => x.LOCATION_CODE == query.LOC_CODE && x.ACTIVE_FLAG == "Y").Select(x => x.SPECIFIC_FLAG);
                        if (specificResult.Any())
                        {
                            string specificFlag = specificResult.FirstOrDefault();
                            if (specificFlag == "Y")
                            {
                                result = zipcodeCache
                                    .Join(fbssListBVCache, z => new { SUB_DISTRICT = z.TUMBON, POSTAL_CODE = z.ZIPCODE }, l => new { SUB_DISTRICT = l.SUB_DISTRICT, POSTAL_CODE = l.POSTAL_CODE }, (z, l) => new { z, l })
                                    .Join(locationCache, zl => zl.l.ADDRESS_ID, b => b.ADDRESS_ID, (zl, b) => new { zl, b })
                                    .Join(locationGroupCache, zlb => zlb.b.LOCATION_CODE, g => g.LOCATION_CODE, (zlb, g) => new { zlb, g })
                                    .Where(x => (x.zlb.zl.l.ACTIVE_FLAG == "Y" || x.zlb.zl.l.ACTIVE_FLAG == "S")
                                        && x.zlb.b.ACTIVE_FLAG == "Y"
                                        && x.g.ACTIVE_FLAG == "Y"
                                        && x.zlb.zl.l.ADDRESS_TYPE == type
                                        && x.g.LOCATION_CODE == location_code)
                                    .Select(x => new DropdownModel
                                    {
                                        Text = x.zlb.zl.l.BUILDING_NAME,
                                        Value = @"{ " +
                                                  @"""zipcode_rowid"": """ + x.zlb.zl.z.ZIPCODE_ROWID + @"""," +
                                                  @"""address_id"": """ + x.zlb.zl.l.ADDRESS_ID + @"""," +
                                                  @"""lang"": """ + x.zlb.zl.l.LANGUAGE + @"""," +
                                                  @"""building_name"": """ + x.zlb.zl.l.BUILDING_NAME + @"""," +
                                                  @"""building_name_for_srh"": """ +
                                                      x.zlb.zl.l.BUILDING_NAME + " " +
                                                      x.zlb.zl.z.TUMBON + " " +
                                                      x.zlb.zl.z.AMPHUR + " " +
                                                      x.zlb.zl.z.PROVINCE +
                                                  @"""," +
                                                  @"""province"": """ + x.zlb.zl.z.PROVINCE + @"""," +
                                                  @"""amphur"": """ + x.zlb.zl.z.AMPHUR + @"""," +
                                                  @"""tumbon"": """ + x.zlb.zl.z.TUMBON + @"""," +
                                                  @"""zipcode"": """ + x.zlb.zl.z.ZIPCODE + @"""," +
                                                  @"""sitecode"": """ + x.zlb.zl.l.SITE_CODE + @"""," +
                                                  @"""latitude"": """ + x.zlb.zl.l.LATITUDE + @"""," +
                                                  @"""longtitude"": """ + x.zlb.zl.l.LONGTITUDE + @"""," +
                                                  @"""AccessMode"": """ + x.zlb.zl.l.ACCESS_MODE + @"""," +
                                                  @"""partner"": """ + x.zlb.zl.l.PARTNER + @"""" +
                                               @" }",
                                        Value4 = x.zlb.zl.l.BUILDING_NAME + x.zlb.zl.z.TUMBON + x.zlb.zl.z.ZIPCODE,
                                    }).ToList();
                            }
                            else
                            {
                                var result1 = zipcodeCache
                                    .Join(fbssListBVCache, z => new { SUB_DISTRICT = z.TUMBON, POSTAL_CODE = z.ZIPCODE }, l => new { SUB_DISTRICT = l.SUB_DISTRICT, POSTAL_CODE = l.POSTAL_CODE }, (z, l) => new { z, l })
                                    .Where(x => x.l.ACTIVE_FLAG == "Y"
                                        && x.l.ADDRESS_TYPE == type)
                                    .Select(x => new DropdownModel
                                    {
                                        Text = x.l.BUILDING_NAME,
                                        Value = @"{ " +
                                                  @"""zipcode_rowid"": """ + x.z.ZIPCODE_ROWID + @"""," +
                                                  @"""address_id"": """ + x.l.ADDRESS_ID + @"""," +
                                                  @"""lang"": """ + x.l.LANGUAGE + @"""," +
                                                  @"""building_name"": """ + x.l.BUILDING_NAME + @"""," +
                                                  @"""building_name_for_srh"": """ +
                                                      x.l.BUILDING_NAME + " " +
                                                      x.z.TUMBON + " " +
                                                      x.z.AMPHUR + " " +
                                                      x.z.PROVINCE +
                                                  @"""," +
                                                  @"""province"": """ + x.z.PROVINCE + @"""," +
                                                  @"""amphur"": """ + x.z.AMPHUR + @"""," +
                                                  @"""tumbon"": """ + x.z.TUMBON + @"""," +
                                                  @"""zipcode"": """ + x.z.ZIPCODE + @"""," +
                                                  @"""sitecode"": """ + x.l.SITE_CODE + @"""," +
                                                  @"""latitude"": """ + x.l.LATITUDE + @"""," +
                                                  @"""longtitude"": """ + x.l.LONGTITUDE + @"""," +
                                                  @"""AccessMode"": """ + x.l.ACCESS_MODE + @"""," +
                                                  @"""partner"": """ + x.l.PARTNER + @"""" +
                                               @" }",
                                        Value4 = x.l.BUILDING_NAME + x.z.TUMBON + x.z.ZIPCODE,
                                    }).ToList();

                                var result2 = zipcodeCache
                                    .Join(fbssListBVCache, z => new { SUB_DISTRICT = z.TUMBON, POSTAL_CODE = z.ZIPCODE }, l => new { SUB_DISTRICT = l.SUB_DISTRICT, POSTAL_CODE = l.POSTAL_CODE }, (z, l) => new { z, l })
                                    .Join(locationCache, zl => zl.l.ADDRESS_ID, b => b.ADDRESS_ID, (zl, b) => new { zl, b })
                                    .Join(locationGroupCache, zlb => zlb.b.LOCATION_CODE, g => g.LOCATION_CODE, (zlb, g) => new { zlb, g })
                                    .Where(x => (x.zlb.zl.l.ACTIVE_FLAG == "Y" || x.zlb.zl.l.ACTIVE_FLAG == "S")
                                        && x.zlb.b.ACTIVE_FLAG == "Y"
                                        && x.g.ACTIVE_FLAG == "Y"
                                        && x.zlb.zl.l.ADDRESS_TYPE == type
                                        && x.g.LOCATION_CODE == location_code)
                                    .Select(x => new DropdownModel
                                    {
                                        Text = x.zlb.zl.l.BUILDING_NAME,
                                        Value = @"{ " +
                                                  @"""zipcode_rowid"": """ + x.zlb.zl.z.ZIPCODE_ROWID + @"""," +
                                                  @"""address_id"": """ + x.zlb.zl.l.ADDRESS_ID + @"""," +
                                                  @"""lang"": """ + x.zlb.zl.l.LANGUAGE + @"""," +
                                                  @"""building_name"": """ + x.zlb.zl.l.BUILDING_NAME + @"""," +
                                                  @"""building_name_for_srh"": """ +
                                                      x.zlb.zl.l.BUILDING_NAME + " " +
                                                      x.zlb.zl.z.TUMBON + " " +
                                                      x.zlb.zl.z.AMPHUR + " " +
                                                      x.zlb.zl.z.PROVINCE +
                                                  @"""," +
                                                  @"""province"": """ + x.zlb.zl.z.PROVINCE + @"""," +
                                                  @"""amphur"": """ + x.zlb.zl.z.AMPHUR + @"""," +
                                                  @"""tumbon"": """ + x.zlb.zl.z.TUMBON + @"""," +
                                                  @"""zipcode"": """ + x.zlb.zl.z.ZIPCODE + @"""," +
                                                  @"""sitecode"": """ + x.zlb.zl.l.SITE_CODE + @"""," +
                                                  @"""latitude"": """ + x.zlb.zl.l.LATITUDE + @"""," +
                                                  @"""longtitude"": """ + x.zlb.zl.l.LONGTITUDE + @"""," +
                                                  @"""AccessMode"": """ + x.zlb.zl.l.ACCESS_MODE + @"""," +
                                                  @"""partner"": """ + x.zlb.zl.l.PARTNER + @"""" +
                                               @" }",
                                        Value4 = x.zlb.zl.l.BUILDING_NAME + x.zlb.zl.z.TUMBON + x.zlb.zl.z.ZIPCODE,
                                    }).ToList();

                                result = result1.Union(result2).ToList();
                            }
                        }
                        else
                        {
                            var result1 = zipcodeCache
                                    .Join(fbssListBVCache, z => new { SUB_DISTRICT = z.TUMBON, POSTAL_CODE = z.ZIPCODE }, l => new { SUB_DISTRICT = l.SUB_DISTRICT, POSTAL_CODE = l.POSTAL_CODE }, (z, l) => new { z, l })
                                    .Where(x => x.l.ACTIVE_FLAG == "Y"
                                        && x.l.ADDRESS_TYPE == type)
                                    .Select(x => new DropdownModel
                                    {
                                        Text = x.l.BUILDING_NAME,
                                        Value = @"{ " +
                                                  @"""zipcode_rowid"": """ + x.z.ZIPCODE_ROWID + @"""," +
                                                  @"""address_id"": """ + x.l.ADDRESS_ID + @"""," +
                                                  @"""lang"": """ + x.l.LANGUAGE + @"""," +
                                                  @"""building_name"": """ + x.l.BUILDING_NAME + @"""," +
                                                  @"""building_name_for_srh"": """ +
                                                      x.l.BUILDING_NAME + " " +
                                                      x.z.TUMBON + " " +
                                                      x.z.AMPHUR + " " +
                                                      x.z.PROVINCE +
                                                  @"""," +
                                                  @"""province"": """ + x.z.PROVINCE + @"""," +
                                                  @"""amphur"": """ + x.z.AMPHUR + @"""," +
                                                  @"""tumbon"": """ + x.z.TUMBON + @"""," +
                                                  @"""zipcode"": """ + x.z.ZIPCODE + @"""," +
                                                  @"""sitecode"": """ + x.l.SITE_CODE + @"""," +
                                                  @"""latitude"": """ + x.l.LATITUDE + @"""," +
                                                  @"""longtitude"": """ + x.l.LONGTITUDE + @"""," +
                                                  @"""AccessMode"": """ + x.l.ACCESS_MODE + @"""," +
                                                  @"""partner"": """ + x.l.PARTNER + @"""" +
                                               @" }",
                                        Value4 = x.l.BUILDING_NAME + x.z.TUMBON + x.z.ZIPCODE,
                                    }).ToList();

                            var result2 = zipcodeCache
                                .Join(fbssListBVCache, z => new { SUB_DISTRICT = z.TUMBON, POSTAL_CODE = z.ZIPCODE }, l => new { SUB_DISTRICT = l.SUB_DISTRICT, POSTAL_CODE = l.POSTAL_CODE }, (z, l) => new { z, l })
                                .Join(locationCache, zl => zl.l.ADDRESS_ID, b => b.ADDRESS_ID, (zl, b) => new { zl, b })
                                .Join(locationGroupCache, zlb => zlb.b.LOCATION_CODE, g => g.LOCATION_CODE, (zlb, g) => new { zlb, g })
                                .Where(x => x.zlb.zl.l.ACTIVE_FLAG == "Y"
                                    && x.zlb.b.ACTIVE_FLAG == "Y"
                                    && x.g.ACTIVE_FLAG == "Y"
                                    && x.zlb.zl.l.ADDRESS_TYPE == type
                                    && x.g.LOCATION_CODE == location_code)
                                .Select(x => new DropdownModel
                                {
                                    Text = x.zlb.zl.l.BUILDING_NAME,
                                    Value = @"{ " +
                                              @"""zipcode_rowid"": """ + x.zlb.zl.z.ZIPCODE_ROWID + @"""," +
                                              @"""address_id"": """ + x.zlb.zl.l.ADDRESS_ID + @"""," +
                                              @"""lang"": """ + x.zlb.zl.l.LANGUAGE + @"""," +
                                              @"""building_name"": """ + x.zlb.zl.l.BUILDING_NAME + @"""," +
                                              @"""building_name_for_srh"": """ +
                                                  x.zlb.zl.l.BUILDING_NAME + " " +
                                                  x.zlb.zl.z.TUMBON + " " +
                                                  x.zlb.zl.z.AMPHUR + " " +
                                                  x.zlb.zl.z.PROVINCE +
                                              @"""," +
                                              @"""province"": """ + x.zlb.zl.z.PROVINCE + @"""," +
                                              @"""amphur"": """ + x.zlb.zl.z.AMPHUR + @"""," +
                                              @"""tumbon"": """ + x.zlb.zl.z.TUMBON + @"""," +
                                              @"""zipcode"": """ + x.zlb.zl.z.ZIPCODE + @"""," +
                                              @"""sitecode"": """ + x.zlb.zl.l.SITE_CODE + @"""," +
                                              @"""latitude"": """ + x.zlb.zl.l.LATITUDE + @"""," +
                                              @"""longtitude"": """ + x.zlb.zl.l.LONGTITUDE + @"""," +
                                              @"""AccessMode"": """ + x.zlb.zl.l.ACCESS_MODE + @"""," +
                                              @"""partner"": """ + x.zlb.zl.l.PARTNER + @"""" +

                                           @" }",
                                    Value4 = x.zlb.zl.l.BUILDING_NAME + x.zlb.zl.z.TUMBON + x.zlb.zl.z.ZIPCODE,
                                }).ToList();

                            result = result1.Union(result2).ToList();
                        }
                    }
                    else
                    {

                        var addressIdData = (from t in fbssListBVCache
                                             where t.ADDRESS_TYPE == type
                                             select t);

                        List<string> addressIdList = new List<string>();
                        if (accessMode != "")
                        {
                            addressIdList = addressIdData.Where(r => r.ACCESS_MODE == accessMode).Select(r => r.ADDRESS_ID).ToList();
                        }
                        else
                        {
                            addressIdList = addressIdData.Select(r => r.ADDRESS_ID).ToList();
                        }

                        result = (from z in zipcodeCache
                                  join l in fbssListBVCache on z.TUMBON equals l.SUB_DISTRICT
                                  where z.ZIPCODE == l.POSTAL_CODE
                                    && l.ACTIVE_FLAG == "Y"
                                    && l.ADDRESS_TYPE == type
                                    && addressIdList.Contains(l.ADDRESS_ID)
                                  //&& l.PARTNER == "3BB"
                                  select new DropdownModel
                                  {
                                      Text = l.BUILDING_NAME,
                                      Value = @"{ " +
                                          @"""zipcode_rowid"": """ + z.ZIPCODE_ROWID + @"""," +
                                          @"""address_id"": """ + l.ADDRESS_ID + @"""," +
                                          @"""lang"": """ + l.LANGUAGE + @"""," +
                                          @"""building_name"": """ + l.BUILDING_NAME + @"""," +
                                          @"""building_name_for_srh"": """ +
                                              l.BUILDING_NAME + " " +
                                              z.TUMBON + " " +
                                              z.AMPHUR + " " +
                                              z.PROVINCE +
                                          @"""," +
                                          @"""province"": """ + z.PROVINCE + @"""," +
                                          @"""amphur"": """ + z.AMPHUR + @"""," +
                                          @"""tumbon"": """ + z.TUMBON + @"""," +
                                          @"""zipcode"": """ + z.ZIPCODE + @"""," +
                                          @"""sitecode"": """ + l.SITE_CODE + @"""," +
                                          @"""latitude"": """ + l.LATITUDE + @"""," +
                                          @"""longtitude"": """ + l.LONGTITUDE + @"""," +
                                          @"""AccessMode"": """ + l.ACCESS_MODE + @"""," +
                                          @"""partner"": """ + l.PARTNER + @"""" +
                                       @" }",
                                      Value4 = l.BUILDING_NAME + z.TUMBON + z.ZIPCODE,
                                  })
                              .ToList();
                    }
                    #endregion
                }

            }
            catch (System.Exception ex)
            {
                _logger.Error("Error Call GetbuildingChangeHandler : " + "[Error Message : " + ex.Message + "][Error StackTrace : " + ex.StackTrace + "][Error InnerException : " + ex.InnerException + "]");
            }

            return result;
        }
    }

    public class GetbuildingAllHandler : IQueryHandler<GetbuildingAllQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<FBB_LISTBV_BY_LOCATION> _FBB_LISTBV_BY_LOCATION;
        private readonly IEntityRepository<FBB_LISTBV_LOCATION_GROUP> _FBB_LISTBV_LOCATION_GROUP;

        public GetbuildingAllHandler(ILogger logger, IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV
            , IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE
            , IEntityRepository<FBB_LISTBV_BY_LOCATION> FBB_LISTBV_BY_LOCATION
            , IEntityRepository<FBB_LISTBV_LOCATION_GROUP> FBB_LISTBV_LOCATION_GROUP)
        {
            _logger = logger;
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _FBB_LISTBV_BY_LOCATION = FBB_LISTBV_BY_LOCATION;
            _FBB_LISTBV_LOCATION_GROUP = FBB_LISTBV_LOCATION_GROUP;
        }

        public List<DropdownModel> Handle(GetbuildingAllQuery query)
        {
            List<DropdownModel> result = new List<DropdownModel>();
            var type = query.Typeaddress.ToSafeString();
            var zipcodeCache = FBBCache.Get<List<FBB_ZIPCODE>>("BV_ZIPCODE_TABLE");
            var fbbFBSSListBV = (from l in _FBB_FBSS_LISTBV.Get() where l.ADDRESS_TYPE == type select l).ToList();

            if (null == zipcodeCache)
            {
                FBBCache.Add(_FBB_ZIPCODE.Get().ToList(), "BV_ZIPCODE_TABLE");
                zipcodeCache = FBBCache.Get<List<FBB_ZIPCODE>>("BV_ZIPCODE_TABLE");
            }

            if (null != fbbFBSSListBV && fbbFBSSListBV.Count > 0)
            {

                result = (from z in zipcodeCache
                          join l in fbbFBSSListBV on z.TUMBON equals l.SUB_DISTRICT
                          where z.ZIPCODE == l.POSTAL_CODE
                          select new DropdownModel
                          {
                              Text = l.BUILDING_NAME,
                              Value = @"{ " +
                                  @"""zipcode_rowid"": """ + z.ZIPCODE_ROWID + @"""," +
                                  @"""address_id"": """ + l.ADDRESS_ID + @"""," +
                                  @"""lang"": """ + l.LANGUAGE + @"""," +
                                  @"""building_name"": """ + l.BUILDING_NAME + @"""," +
                                  @"""building_name_for_srh"": """ +
                                      l.BUILDING_NAME + " " +
                                      z.TUMBON + " " +
                                      z.AMPHUR + " " +
                                      z.PROVINCE +
                                  @"""," +
                                  @"""province"": """ + z.PROVINCE + @"""," +
                                  @"""amphur"": """ + z.AMPHUR + @"""," +
                                  @"""tumbon"": """ + z.TUMBON + @"""," +
                                  @"""zipcode"": """ + z.ZIPCODE + @"""," +
                                  @"""latitude"": """ + l.LATITUDE + @"""," +
                                  @"""longtitude"": """ + l.LONGTITUDE + @"""," +
                                  @"""sitecode"": """ + l.SITE_CODE + @"""," +
                                  @"""partner"": """ + l.PARTNER + @"""" +
                               @" }",
                              Value4 = l.BUILDING_NAME + z.TUMBON + z.ZIPCODE,
                          })
                      .ToList();
            }

            return result;
        }
    }
}