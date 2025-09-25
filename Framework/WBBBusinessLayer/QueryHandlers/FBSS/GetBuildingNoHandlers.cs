using AIRNETEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBSS;
using WBBContract.QueryModels.FBSS;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBSS
{
    public class GetBuildingNoHandlers : IQueryHandler<GetBuildingNoQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;

        public GetBuildingNoHandlers(ILogger logger, IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE)
        {
            _logger = logger;
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _FBB_ZIPCODE = FBB_ZIPCODE;

        }

        public List<DropdownModel> Handle(GetBuildingNoQuery query)
        {
            //var postcode = (from z in _FBB_ZIPCODE.Get()
            //                where z.ZIPCODE_ROWID == query.Postcode
            //                select z.ZIPCODE
            //                ).FirstOrDefault();


            var lang = query.Language.ToSafeString();
            var result = (from r in _FBB_FBSS_LISTBV.Get()
                          where r.BUILDING_NAME == query.Buildname
                          && r.LANGUAGE == lang && (r.ACTIVE_FLAG == "Y" || r.ACTIVE_FLAG == "S")
                          group r by r.BUILDING_NO into g
                          orderby g.Key
                          select new DropdownModel
                          {
                              Text = g.Key,
                              Value = g.Key
                          }).ToList();
            result = result.Where(a => a.Text.ToSafeString() != "").ToList();
            return result;
        }
    }

    public class GetBuildingNoAllHandlers : IQueryHandler<GetBuildingNoAllQuery, List<DropdownModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_FTTR> _AIR_PACKAGE_FTTR;

        public GetBuildingNoAllHandlers(ILogger logger,
            IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IAirNetEntityRepository<AIR_PACKAGE_FTTR> AIR_PACKAGE_FTTR)
        {
            _logger = logger;
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _AIR_PACKAGE_FTTR = AIR_PACKAGE_FTTR;
        }

        public List<DropdownModel> Handle(GetBuildingNoAllQuery query)
        {

            var lang = query.Language.ToSafeString();
            List<DropdownModel> result = new List<DropdownModel>();

            var tempDatas = (from r in _FBB_FBSS_LISTBV.Get()
                             where r.BUILDING_NAME == query.Buildname
                             && r.LANGUAGE == lang && (r.ACTIVE_FLAG == "Y" || r.ACTIVE_FLAG == "N")
                             orderby r.ADDRESS_ID
                             select r
                           ).ToList();
            if (tempDatas.Count > 0)
            {
                foreach (var item in tempDatas)
                {
                    string fttrFlag = "N";
                    string updateDateSTR = "";
                    if (item.UPDATED_DATE != null)
                    {
                        DateTime UpdatedTime = item.UPDATED_DATE ?? DateTime.Now;
                        updateDateSTR = UpdatedTime.ToString("dd/MM/yyyy");
                    }

                    var tempFttrDatas = (from r in _AIR_PACKAGE_FTTR.Get()
                                         where r.ADDRESS_ID == item.ADDRESS_ID
                                         select r
                           ).ToList();
                    if (tempFttrDatas != null && tempFttrDatas.Count > 0)
                    {
                        fttrFlag = "Y";
                    }

                    DropdownModel tempData = new DropdownModel()
                    {
                        Text = item.BUILDING_NO,
                        Value = @"{ " +
                             @"""lang"": """ + item.LANGUAGE + @"""," +
                             @"""address_id"": """ + item.ADDRESS_ID + @"""," +
                             @"""building_name"": """ + item.BUILDING_NAME + @"""," +
                             @"""building_no"": """ + item.BUILDING_NO + @"""," +
                             @"""active_flag"": """ + item.ACTIVE_FLAG + @"""," +
                             @"""fttr_flag"": """ + fttrFlag + @"""," +
                             @"""updated_by"": """ + item.UPDATED_BY + @"""," +
                             @"""updated_date"": """ + updateDateSTR + @"""," +
                             @"""reason"": """ + item.REASON + @"""" +
                          @" }"
                    };
                    result.Add(tempData);
                }
            }
            else
            {
                DropdownModel tempData = new DropdownModel()
                {
                    Text = "0",
                    Value = "0"
                };
                result.Add(tempData);
            }
            result = result.Where(a => a.Text.ToSafeString() != "").ToList();
            return result;
        }
    }

    public class GetPermissionCondoUpdateStatusHandlers : IQueryHandler<GetPermissionCondoUpdateStatusQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_GROUP_PERMISSION> _FBB_GROUP_PERMISSION;
        private readonly IEntityRepository<FBB_USER> _FBB_USER;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetPermissionCondoUpdateStatusHandlers(ILogger logger,
            IEntityRepository<FBB_GROUP_PERMISSION> fbb_GROUP_PERMISSION,
            IEntityRepository<FBB_USER> fbb_USER,
            IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV)
        {
            _logger = logger;
            _FBB_GROUP_PERMISSION = fbb_GROUP_PERMISSION;
            _FBB_USER = fbb_USER;
            _FBB_CFG_LOV = fbb_CFG_LOV;
        }

        public string Handle(GetPermissionCondoUpdateStatusQuery query)
        {
            string result = "N";

            try
            {
                var UserEnOnServiceData = (from r in _FBB_CFG_LOV.Get()
                                           where r.LOV_NAME == "USER_EN_ON_SERVICE_CONDO_GROUP_ID"
                                           select r
                               ).ToList();

                foreach (var item in UserEnOnServiceData)
                {
                    decimal GroupID = ((item.LOV_VAL1 != null && item.LOV_VAL1 != "") ? Convert.ToDecimal(item.LOV_VAL1) : 0);
                    var UserPermission = (from u in _FBB_USER.Get()
                                          join g in _FBB_GROUP_PERMISSION.Get() on u.USER_ID equals g.USER_ID
                                          where u.USER_NAME == query.UserName && g.GROUP_ID == GroupID
                                          select u
                               ).ToList();
                    if (UserPermission.Count > 0)
                    {
                        result = "Y";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }

    public class GetBuildingByBuildingNameAndNoQueryHandlers : IQueryHandler<GetBuildingByBuildingNameAndNoQuery, GetBuildingByBuildingNameAndNoQueryModel>
    {
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _FBB_FBSS_LISTBV;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public GetBuildingByBuildingNameAndNoQueryHandlers(ILogger logger, IEntityRepository<FBB_FBSS_LISTBV> FBB_FBSS_LISTBV, IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE, IEntityRepository<FBB_CFG_LOV> fbb_CFG_LOV)
        {
            _FBB_FBSS_LISTBV = FBB_FBSS_LISTBV;
            _FBB_CFG_LOV = fbb_CFG_LOV;
        }

        public GetBuildingByBuildingNameAndNoQueryModel Handle(GetBuildingByBuildingNameAndNoQuery query)
        {
            GetBuildingByBuildingNameAndNoQueryModel result = new GetBuildingByBuildingNameAndNoQueryModel();

            var results = (from r in _FBB_FBSS_LISTBV.Get()
                          where r.BUILDING_NAME == query.Buildname
                          && r.BUILDING_NO == query.Buildno
                          select r
                          ).ToList();
            if (results != null && results.Count > 0)
            {
                // Check FTTR_FLAG 3BB Exclusive
                if (Check3bbExclusive(results.FirstOrDefault().FTTR_FLAG.ToSafeString()))
                {
                    result.Exclusive_3bb = "Y";
                }

                result.Partner = results.FirstOrDefault().PARTNER.ToSafeString();
                result.Address_id = results.FirstOrDefault().ADDRESS_ID.ToSafeString();
                result.latitude = results.FirstOrDefault().LATITUDE.ToSafeString();
                result.longtitude = results.FirstOrDefault().LONGTITUDE.ToSafeString();
            }
            return result;
        }

        private bool Check3bbExclusive(string fttrFlag)
        {
            bool result = false;
            var msgLov = (from l in _FBB_CFG_LOV.Get() where l.LOV_TYPE == "EXCUSIVE_CONFIG" select l).ToList();

            if (msgLov != null && msgLov.Count() > 0)
            {
                if (!String.IsNullOrEmpty(fttrFlag) && fttrFlag.Length > 4)
                {
                    var subStringLast = msgLov.FirstOrDefault(t => t.LOV_NAME == "EXCUSIVE_SUBSTRING_3BB").LOV_VAL2;
                    int subStringLastInt = 0;
                    Int32.TryParse(subStringLast, out subStringLastInt);
                    var checkExclusive = msgLov.Where(t => t.LOV_NAME == "EXCUSIVE_SUBSTRING_LAST_3BB").Select(t => t.LOV_VAL1).ToList();
                    int lastIndex = fttrFlag.Length - subStringLastInt;
                    var fttrFlagLast = fttrFlag.Substring(lastIndex, subStringLastInt); // 1 ตัวหลัง
                    if (checkExclusive.IndexOf(fttrFlagLast) >= 0)
                    {
                        var subStringFirst = msgLov.FirstOrDefault(t => t.LOV_NAME == "EXCUSIVE_SUBSTRING_3BB").LOV_VAL1;
                        int subStringFirstInt = 0;
                        Int32.TryParse(subStringFirst, out subStringFirstInt);
                        var checkPartner = msgLov.Where(t => t.LOV_NAME == "EXCUSIVE_SUBSTRING_FRIST_3BB").Select(t => t.LOV_VAL1).ToList();
                        var fttrFlagFirst = fttrFlag.Substring(0, subStringFirstInt); // 3 ตัวแรก

                        if (checkPartner.IndexOf(fttrFlagFirst) >= 0)
                            result = true;
                    }
                }
            }
            return result;
        }
    }
}
