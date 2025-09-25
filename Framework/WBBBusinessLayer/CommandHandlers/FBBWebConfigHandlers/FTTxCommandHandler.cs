using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class FTTxCommandHandler : ICommandHandler<FTTxCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_COVERAGE_REGION> _FBB_COVERAGE_REGION;
        private readonly IEntityRepository<FBB_ZIPCODE> _FBB_ZIPCODE;
        private readonly IEntityRepository<string> _executeStored;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public FTTxCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_COVERAGE_REGION> FBB_COVERAGE_REGION,
            IEntityRepository<FBB_ZIPCODE> FBB_ZIPCODE,
            IEntityRepository<string> executeStored,
            IEntityRepository<FBB_HISTORY_LOG> historyLog)
        {
            _logger = logger;
            _uow = uow;
            _FBB_COVERAGE_REGION = FBB_COVERAGE_REGION;
            _FBB_ZIPCODE = FBB_ZIPCODE;
            _executeStored = executeStored;
            _historyLog = historyLog;
        }

        void FTTxCreate(FTTxCommand command)
        {


            var zipCode = "";
            var zipCodeEN = new List<string>();
            var zipCodeTH = new List<string>(); ;

            var zipCodeP = "";
            var zipCodeENP = new List<string>(); ;
            var zipCodeTHP = new List<string>(); ;

            string[] formats = { "dd/MM/yyyy" };

            if (command.Tumbon != "" && command.TumbonEN != "")
            {
                zipCodeTH = (from r in _FBB_ZIPCODE.Get()
                             where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur && r.TUMBON == command.Tumbon && r.ZIPCODE == command.zipcode
                             orderby r.ZIPCODE
                             select r.ZIPCODE_ROWID).ToList();


                zipCodeEN = (from r in _FBB_ZIPCODE.Get()
                             where r.PROVINCE == command.ProvinceEN && r.AMPHUR == command.AmphurEN && r.TUMBON == command.TumbonEN && r.ZIPCODE == command.zipcode
                             orderby r.ZIPCODE
                             select r.ZIPCODE_ROWID).ToList();//.FirstOrDefault();

                zipCode = (from r in _FBB_ZIPCODE.Get()
                           where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur
                           select r.GROUP_AMPHUR).FirstOrDefault();

                ///// ปณ

                zipCodeTHP = (from r in _FBB_ZIPCODE.Get()
                              where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur + "(ปณ.)" && r.TUMBON == command.Tumbon && r.ZIPCODE == command.zipcode
                              orderby r.ZIPCODE
                              select r.ZIPCODE_ROWID).ToList();//.FirstOrDefault();


                zipCodeENP = (from r in _FBB_ZIPCODE.Get()
                              where r.PROVINCE == command.ProvinceEN && r.AMPHUR == command.AmphurEN + "(PO.)" && r.TUMBON == command.TumbonEN && r.ZIPCODE == command.zipcode
                              orderby r.ZIPCODE
                              select r.ZIPCODE_ROWID).ToList();//.FirstOrDefault();

                zipCodeP = (from r in _FBB_ZIPCODE.Get()
                            where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur + "(ปณ.)"
                            select r.GROUP_AMPHUR).FirstOrDefault();
            }
            else
            {
                zipCode = (from r in _FBB_ZIPCODE.Get()
                           where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur
                           select r.GROUP_AMPHUR).FirstOrDefault();

                zipCodeTH.Add(zipCode);
                zipCodeEN.Add(zipCode);
                zipCodeP = zipCode;
            }




            var action = "";
            var description = "";

            #region CrateFTTX
            if (command.Action == "Create")
            {
                var all = (from r in _FBB_ZIPCODE.Get()
                           where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur
                           select r).FirstOrDefault();

                if (command.tower_en != "" && command.tower_th != "")
                {
                    description = "Region Code: " + all.REGION_CODE + ", Province: " + command.Province + ", District: "
                   + command.Amphur + ", Owner Product: "
                   + command.OwnerProduct + ", Owner Type : "
                   + command.OwnerType + " ,Service Type :  "
                   + command.Service_Type + ",Tower Thai: "
                   + command.tower_th + ",Tower English: "
                   + command.tower_en;




                }
                else
                {
                    description = "Region Code: " + all.REGION_CODE + ", Province: " + command.Province + ", District: "
                      + command.Amphur + ", Owner Product: "
                      + command.OwnerProduct + ", Owner Type :   "
                      + command.OwnerType +
                      " ,Service Type : " + command.Service_Type;


                }

                action = ActionHistory.ADD.ToString();

                for (var i = 0; i < zipCodeTH.Count(); i++)
                {
                    var fbbCoverage = new FBB_COVERAGE_REGION
                    {
                        OWNER_PRODUCT = command.OwnerProduct,
                        OWNER_TYPE = command.OwnerType,
                        CREATED_BY = command.Username,
                        CREATED_DATE = DateTime.Now,
                        UPDATED_BY = command.Username,
                        UPDATED_DATE = DateTime.Now,
                        ACTIVEFLAG = "Y",
                        GROUP_AMPHUR = zipCode,
                        TOWER_TH = command.tower_th,
                        TOWER_EN = command.tower_en,
                        SERVICE_TYPE = command.Service_Type,

                        ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                       ? (DateTime?)null
                       : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                        ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                       ? (DateTime?)null
                       : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                        COVERAGE_STATUS = command.status,
                        LATITUDE = command.lat,
                        LONGITUDE = command.lon,
                        ZIPCODE_ROWID_TH = zipCodeTH[i],
                        ZIPCODE_ROWID_EN = zipCodeEN[i]

                    };

                    _FBB_COVERAGE_REGION.Create(fbbCoverage);
                }

                if (zipCodeTHP.Count() != 0)
                {
                    for (var i = 0; i < zipCodeTHP.Count(); i++)
                    {
                        var fbbCoverage2 = new FBB_COVERAGE_REGION
                        {
                            OWNER_PRODUCT = command.OwnerProduct,
                            OWNER_TYPE = command.OwnerType,
                            CREATED_BY = command.Username,
                            CREATED_DATE = DateTime.Now,
                            UPDATED_BY = command.Username,
                            UPDATED_DATE = DateTime.Now,
                            ACTIVEFLAG = "Y",
                            GROUP_AMPHUR = zipCodeP,
                            TOWER_TH = command.tower_th,
                            TOWER_EN = command.tower_en,
                            SERVICE_TYPE = command.Service_Type,

                            ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                           ? (DateTime?)null
                           : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                            ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                           ? (DateTime?)null
                           : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                            COVERAGE_STATUS = command.status,
                            LATITUDE = command.lat,
                            LONGITUDE = command.lon,
                            ZIPCODE_ROWID_TH = zipCodeTHP[i],
                            ZIPCODE_ROWID_EN = zipCodeENP[i]

                        };

                        _FBB_COVERAGE_REGION.Create(fbbCoverage2);
                    }
                }


                var historyLog = new FBB_HISTORY_LOG();
                historyLog.ACTION = action;
                historyLog.APPLICATION = "FBB_CFG005_1";
                historyLog.CREATED_BY = command.Username;
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = description;
                historyLog.REF_KEY = command.Province;
                historyLog.REF_NAME = "PROVINCE";


                if (description != string.Empty)
                {

                    _historyLog.Create(historyLog);

                }

            }

            #endregion

            #region UPDATE FTTX
            else if (command.Action == "Update")
            {
                var fbbCoverageUpdate = (from r in _FBB_COVERAGE_REGION.Get()  //// main
                                         where r.FTTX_ID == command.Fttx_id
                                         select r).FirstOrDefault();

                var findaddress = (from r in _FBB_ZIPCODE.Get()
                                   where r.LANG_FLAG == "N"
                                   && r.ZIPCODE_ROWID == fbbCoverageUpdate.ZIPCODE_ROWID_TH
                                   select r).FirstOrDefault();

                var groupIdList222 = (from r in _FBB_ZIPCODE.Get()
                                      where r.LANG_FLAG == "N"
                                       && !r.AMPHUR.Contains("ปณ")
                                      && r.GROUP_AMPHUR == command.GroupAmphur
                                      select r).Distinct().FirstOrDefault();

                FBB_COVERAGE_REGION dupUpadataFrist;




                if (command.tower_en != "" && command.tower_th != "") // building or village
                {
                    var dateex = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                          ? (DateTime?)null : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                    var datein = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                        ? (DateTime?)null : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                    dupUpadataFrist = (from r in _FBB_COVERAGE_REGION.Get()
                                       where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                        && r.ACTIVEFLAG == "Y" && r.FTTX_ID == command.Fttx_id && (zipCodeTH.Contains(r.ZIPCODE_ROWID_TH) || zipCodeEN.Contains(r.ZIPCODE_ROWID_EN))
                                         && r.SERVICE_TYPE == command.Service_Type && r.LATITUDE == command.lat && r.LONGITUDE == command.lon && r.COVERAGE_STATUS == command.status && r.TOWER_EN == command.tower_en.Trim() && r.TOWER_TH == command.tower_th.Trim() && r.ONTARGET_DATE_EX == dateex
                                            && r.ONTARGET_DATE_IN == datein
                                       select r).FirstOrDefault();

                    if (dupUpadataFrist != null)
                    {
                        command.FlagDup = false;
                    }

                }
                else // other
                {
                    if (command.Tumbon != "" && command.TumbonEN != "")
                    {
                        var dateex = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                            ? (DateTime?)null : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                        var datein = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                            ? (DateTime?)null : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                        dupUpadataFrist = (from r in _FBB_COVERAGE_REGION.Get()
                                           where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                            && r.ACTIVEFLAG == "Y" && r.FTTX_ID == command.Fttx_id && r.COVERAGE_STATUS == command.status && (zipCodeTH.Contains(r.ZIPCODE_ROWID_TH) || zipCodeEN.Contains(r.ZIPCODE_ROWID_EN)) && r.SERVICE_TYPE == command.Service_Type && r.ONTARGET_DATE_EX == dateex
                                            && r.ONTARGET_DATE_IN == datein
                                           select r).FirstOrDefault();

                        if (dupUpadataFrist != null)
                        {
                            command.FlagDup = false;
                        }
                    }
                    else
                    {
                        var dateex = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                          ? (DateTime?)null : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                        var datein = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                            ? (DateTime?)null : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                        dupUpadataFrist = (from r in _FBB_COVERAGE_REGION.Get()
                                           where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                            && r.ACTIVEFLAG == "Y" && r.FTTX_ID == command.Fttx_id && r.COVERAGE_STATUS == command.status && zipCode.Contains(r.ZIPCODE_ROWID_TH)
                                             && r.SERVICE_TYPE == command.Service_Type && r.ONTARGET_DATE_EX == dateex
                                            && r.ONTARGET_DATE_IN == datein
                                           select r).FirstOrDefault();

                        if (dupUpadataFrist != null)
                        {
                            command.FlagDup = false;
                        }
                    }


                }

                #region Flagelse 
                if (command.FlagDup == false)
                {
                    command.FlagDup = false;
                }
                else /// not itself
                {
                    #region for update history
                    var newObject2 = new FTTxHistoryModel
                    {
                        OWNER_PRODUCT = command.OwnerProduct,
                        OWNER_TYPE = command.OwnerType,
                        tower_th = command.tower_th,
                        tower_en = command.tower_en,
                        Service_Type = command.Service_Type,
                        AMPHUR = command.Amphur,
                        PROVINCE = command.Province

                    };

                    var oldObject = new FTTxHistoryModel
                    {
                        AMPHUR = groupIdList222.AMPHUR,
                        PROVINCE = groupIdList222.PROVINCE,
                        OWNER_PRODUCT = fbbCoverageUpdate.OWNER_PRODUCT,
                        OWNER_TYPE = fbbCoverageUpdate.OWNER_TYPE,
                        Service_Type = fbbCoverageUpdate.SERVICE_TYPE,
                        tower_en = fbbCoverageUpdate.TOWER_EN,
                        tower_th = fbbCoverageUpdate.TOWER_TH,

                    };
                    #endregion
                    bool flag = false;
                    //foreach (var a in rowidPO)
                    //{
                    //    var fbbCoverageUpdatePO = (from r in _FBB_COVERAGE_REGION.Get()  //// ปณ or PO
                    //                               where r.ZIPCODE_ROWID_TH == a
                    //                               select r).FirstOrDefault();
                    //    /// not change
                    //    if (findaddress.PROVINCE == command.Province && findaddress.AMPHUR == command.Amphur && findaddress.TUMBON == command.Tumbon)
                    //    {

                    //        if (zipCodeTHP.Count() != 0)
                    //        {
                    //            //fbbCoverageUpdatePO.GROUP_AMPHUR = zipCodeP.ToString();
                    //            fbbCoverageUpdatePO.UPDATED_BY = command.Username;
                    //            fbbCoverageUpdatePO.UPDATED_DATE = DateTime.Now;
                    //            fbbCoverageUpdatePO.OWNER_PRODUCT = command.OwnerProduct;
                    //            fbbCoverageUpdatePO.OWNER_TYPE = command.OwnerType;
                    //            fbbCoverageUpdatePO.SERVICE_TYPE = command.Service_Type;
                    //            fbbCoverageUpdatePO.TOWER_EN = command.tower_en;
                    //            fbbCoverageUpdatePO.TOWER_TH = command.tower_th;
                    //            fbbCoverageUpdatePO.COVERAGE_STATUS = command.status;
                    //            fbbCoverageUpdatePO.LATITUDE = command.lat;
                    //            fbbCoverageUpdatePO.LONGITUDE = command.lon;
                    //            //fbbCoverageUpdatePO.ZIPCODE_ROWID_TH = zipCodeTHP[i];
                    //            //fbbCoverageUpdatePO.ZIPCODE_ROWID_EN = zipCodeENP[i];
                    //            fbbCoverageUpdatePO.COVERAGE_STATUS = command.status;
                    //            fbbCoverageUpdatePO.ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                    //                ? (DateTime?)null
                    //                : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                    //            fbbCoverageUpdatePO.ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                    //            ? (DateTime?)null
                    //            : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                    //            _FBB_COVERAGE_REGION.Update(fbbCoverageUpdatePO);
                    //            flag = true;

                    //        }
                    //    }
                    //    else // delete ปณ or PO
                    //    {
                    //        var xxx = _FBB_COVERAGE_REGION.GetByKey(fbbCoverageUpdatePO.FTTX_ID);
                    //        if (null != xxx)
                    //        {
                    //            _FBB_COVERAGE_REGION.Delete(xxx);
                    //        }
                    //    }

                    //}

                    if (flag != true)// insert new ปณ ที่อัพเดทถ้ามี 
                    {

                        if (zipCodeTHP.Count() != 0)
                        {
                            for (var i = 0; i < zipCodeTHP.Count(); i++)
                            {
                                var fbbCoverage2 = new FBB_COVERAGE_REGION
                                {
                                    OWNER_PRODUCT = command.OwnerProduct,
                                    OWNER_TYPE = command.OwnerType,
                                    CREATED_BY = command.Username,
                                    CREATED_DATE = DateTime.Now,
                                    UPDATED_BY = command.Username,
                                    UPDATED_DATE = DateTime.Now,
                                    ACTIVEFLAG = "Y",
                                    GROUP_AMPHUR = zipCodeP,
                                    TOWER_TH = command.tower_th,
                                    TOWER_EN = command.tower_en,
                                    SERVICE_TYPE = command.Service_Type,

                                    ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                                    ? (DateTime?)null
                                    : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                                    ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                                    ? (DateTime?)null
                                    : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                                    COVERAGE_STATUS = command.status,
                                    LATITUDE = command.lat,
                                    LONGITUDE = command.lon,
                                    ZIPCODE_ROWID_TH = zipCodeTHP[i],
                                    ZIPCODE_ROWID_EN = zipCodeENP[i]

                                };

                                _FBB_COVERAGE_REGION.Create(fbbCoverage2);
                            }
                        }
                    }

                    if (findaddress != null) /// case has ziprowid (has tumbon)
                    {
                        if (findaddress.PROVINCE == command.Province && findaddress.AMPHUR == command.Amphur && findaddress.TUMBON == command.Tumbon && findaddress.ZIPCODE == command.zipcode)
                        {

                            //fbbCoverageUpdate.GROUP_AMPHUR = zipCode.ToString();
                            fbbCoverageUpdate.UPDATED_BY = command.Username;
                            fbbCoverageUpdate.UPDATED_DATE = DateTime.Now;
                            fbbCoverageUpdate.OWNER_PRODUCT = command.OwnerProduct;
                            fbbCoverageUpdate.OWNER_TYPE = command.OwnerType;
                            fbbCoverageUpdate.SERVICE_TYPE = command.Service_Type;
                            fbbCoverageUpdate.TOWER_EN = command.tower_en;
                            fbbCoverageUpdate.TOWER_TH = command.tower_th;
                            fbbCoverageUpdate.COVERAGE_STATUS = command.status;
                            fbbCoverageUpdate.LATITUDE = command.lat;
                            fbbCoverageUpdate.LONGITUDE = command.lon;
                            //fbbCoverageUpdate.ZIPCODE_ROWID_TH = zipCodeTH[i];
                            //fbbCoverageUpdate.ZIPCODE_ROWID_EN = zipCodeEN[i];
                            fbbCoverageUpdate.COVERAGE_STATUS = command.status;
                            fbbCoverageUpdate.ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                                ? (DateTime?)null
                                : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                            fbbCoverageUpdate.ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                            ? (DateTime?)null
                            : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                            _FBB_COVERAGE_REGION.Update(fbbCoverageUpdate);

                        }
                        else
                        {
                            var xxx = _FBB_COVERAGE_REGION.GetByKey(command.Fttx_id);
                            if (null != xxx)
                            {
                                _FBB_COVERAGE_REGION.Delete(xxx);
                            }

                            for (var i = 0; i < zipCodeTH.Count(); i++)
                            {
                                var fbbCoverage = new FBB_COVERAGE_REGION
                                {
                                    OWNER_PRODUCT = command.OwnerProduct,
                                    OWNER_TYPE = command.OwnerType,
                                    CREATED_BY = command.Username,
                                    CREATED_DATE = DateTime.Now,
                                    UPDATED_BY = command.Username,
                                    UPDATED_DATE = DateTime.Now,
                                    ACTIVEFLAG = "Y",
                                    GROUP_AMPHUR = zipCode,
                                    TOWER_TH = command.tower_th,
                                    TOWER_EN = command.tower_en,
                                    SERVICE_TYPE = command.Service_Type,

                                    ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                                   ? (DateTime?)null
                                   : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                                    ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                                   ? (DateTime?)null
                                   : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None),
                                    COVERAGE_STATUS = command.status,
                                    LATITUDE = command.lat,
                                    LONGITUDE = command.lon,
                                    ZIPCODE_ROWID_TH = zipCodeTH[i],
                                    ZIPCODE_ROWID_EN = zipCodeEN[i]

                                };

                                _FBB_COVERAGE_REGION.Create(fbbCoverage);
                            }
                        }
                    }
                    else /// case other has 1 only
                    {

                        fbbCoverageUpdate.GROUP_AMPHUR = zipCode.ToString();
                        fbbCoverageUpdate.UPDATED_BY = command.Username;
                        fbbCoverageUpdate.UPDATED_DATE = DateTime.Now;
                        fbbCoverageUpdate.OWNER_PRODUCT = command.OwnerProduct;
                        fbbCoverageUpdate.OWNER_TYPE = command.OwnerType;
                        fbbCoverageUpdate.SERVICE_TYPE = command.Service_Type;
                        fbbCoverageUpdate.TOWER_EN = command.tower_en;
                        fbbCoverageUpdate.TOWER_TH = command.tower_th;
                        fbbCoverageUpdate.COVERAGE_STATUS = command.status;
                        fbbCoverageUpdate.LATITUDE = command.lat;
                        fbbCoverageUpdate.LONGITUDE = command.lon;
                        fbbCoverageUpdate.ZIPCODE_ROWID_TH = zipCodeTH[0];
                        fbbCoverageUpdate.ZIPCODE_ROWID_EN = zipCodeEN[0];
                        fbbCoverageUpdate.COVERAGE_STATUS = command.status;
                        fbbCoverageUpdate.ONTARGET_DATE_IN = string.IsNullOrEmpty(command.targetdate_in.ToSafeString())
                            ? (DateTime?)null
                            : DateTime.ParseExact(command.targetdate_in.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                        fbbCoverageUpdate.ONTARGET_DATE_EX = string.IsNullOrEmpty(command.tagetdate_ex.ToSafeString())
                        ? (DateTime?)null
                        : DateTime.ParseExact(command.tagetdate_ex.ToSafeString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);

                        _FBB_COVERAGE_REGION.Update(fbbCoverageUpdate);
                    }






                    #region history update


                    if (command.tower_th != "" && command.tower_en != "")
                    {
                        description = " Province :" + oldObject.PROVINCE + " , District  :" + oldObject.AMPHUR +
                            ",  Owner Product:" + oldObject.OWNER_PRODUCT +
                            ", Owner Type:" + oldObject.OWNER_TYPE +
                            ",  Service Type :" + oldObject.Service_Type +
                            ", Tower Thai :" + oldObject.tower_th +
                            " , Tower English :" + oldObject.tower_th +
                                                " => " + WBBExtensions.CompareObjectToString(oldObject, newObject2);
                        action = ActionHistory.UPDATE.ToString();

                    }
                    else
                    {

                        description = " Province :" + oldObject.PROVINCE + " , District  :" + oldObject.AMPHUR +
                                               ",  Owner Product:" + oldObject.OWNER_PRODUCT +
                                               ", Owner Type:" + oldObject.OWNER_TYPE +
                                               ",  Service Type :" + oldObject.Service_Type +
                                           " => " + WBBExtensions.CompareObjectToString(oldObject, newObject2);
                        action = ActionHistory.UPDATE.ToString();

                    }
                    var historyLog = new FBB_HISTORY_LOG();
                    historyLog.ACTION = action;
                    historyLog.APPLICATION = "FBB_CFG005_1";
                    historyLog.CREATED_BY = command.Username;
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = description;
                    historyLog.REF_KEY = command.Province;
                    historyLog.REF_NAME = "PROVINCE";


                    if (description != string.Empty)
                    {

                        _historyLog.Create(historyLog);

                    }
                    #endregion

                    #endregion
                }




            }
            #endregion




        }

        void FTTxDisabled(FTTxCommand command)
        {
            var fbbCoverage = (from r in _FBB_COVERAGE_REGION.Get()
                               where r.FTTX_ID == command.Fttx_id
                               select r).FirstOrDefault();

            var findaddress = (from r in _FBB_ZIPCODE.Get()
                               where r.LANG_FLAG == "N"
                               && r.ZIPCODE_ROWID == fbbCoverage.ZIPCODE_ROWID_TH
                               select r).FirstOrDefault();

            var rowidPO = new List<string>();
            if (findaddress != null)
            {
                rowidPO = (from r in _FBB_ZIPCODE.Get()
                           where r.LANG_FLAG == "N"
                           && r.PROVINCE == findaddress.PROVINCE && r.AMPHUR == findaddress.AMPHUR + "(ปณ.)" && r.TUMBON == findaddress.TUMBON
                           select r.ZIPCODE_ROWID).ToList();
            }

            foreach (var a in rowidPO)
            {
                var fbbCoverageUpdatePO = (from r in _FBB_COVERAGE_REGION.Get()  //// ปณ or PO
                                           where r.ZIPCODE_ROWID_TH == a
                                           select r).FirstOrDefault();

                if (fbbCoverageUpdatePO != null)
                {

                    var xxx = _FBB_COVERAGE_REGION.GetByKey(fbbCoverageUpdatePO.FTTX_ID);
                    if (null != xxx)
                    {
                        _FBB_COVERAGE_REGION.Delete(xxx);
                    }
                }
            }

            fbbCoverage.ACTIVEFLAG = "N";
            fbbCoverage.UPDATED_BY = command.Username;
            fbbCoverage.UPDATED_DATE = DateTime.Now;
            _FBB_COVERAGE_REGION.Update(fbbCoverage);


            #region history
            var action = "";
            var description = "";
            var zipCode = (from r in _FBB_ZIPCODE.Get()
                           where r.PROVINCE == command.Province && r.AMPHUR == command.Amphur
                           select r).FirstOrDefault();

            if (command.tower_en != "" && command.tower_th != "")
            {

                description = "Region Code : " + zipCode.REGION_CODE + ", Province : " + command.Province + ", District: "
                          + command.Amphur + ", Owner Product : "
                          + command.OwnerProduct + ", Owner Type : " + command.OwnerType
                          + " ,Service Type : " + command.Service_Type +
                          ",Tower Thai:  " + command.tower_th + ",Tower English :  " + command.tower_en;


            }
            else
            {
                description = "Region Code : " + zipCode.REGION_CODE + ", Province : " + command.Province + ", District: "
                         + command.Amphur + ", Owner Product : "
                         + command.OwnerProduct + ", Owner Type : " + command.OwnerType
                         + " ,Service Type : " + command.Service_Type;


            }




            action = ActionHistory.DELETE.ToString();
            var historyLog = new FBB_HISTORY_LOG();
            historyLog.ACTION = action;
            historyLog.APPLICATION = "FBB_CFG005_1";
            historyLog.CREATED_BY = command.Username;
            historyLog.CREATED_DATE = DateTime.Now;
            historyLog.DESCRIPTION = description;
            historyLog.REF_KEY = command.Province;
            historyLog.REF_NAME = "PROVINCE";

            if (description != string.Empty)
            {
                _historyLog.Create(historyLog);
            }
            #endregion
        }

        void FTTxDelete(FTTxCommand command)
        {
            var fbbCoverage = from r in _FBB_COVERAGE_REGION.Get()
                              where r.FTTX_ID == command.Fttx_id
                              select r;

            foreach (var a in fbbCoverage)
            {
                var x = _FBB_COVERAGE_REGION.GetByKey(a.FTTX_ID);

                if (x != null)
                    _FBB_COVERAGE_REGION.Delete(x);
            }
        }

        public void Handle(FTTxCommand command)
        {

            var groupIdListMain = (from r in _FBB_ZIPCODE.Get()
                                   where r.LANG_FLAG == "N"
                                    //&& !r.AMPHUR.Contains("ปณ")
                                    && r.AMPHUR == command.Amphur
                                    && r.PROVINCE == command.Province
                                   select r.GROUP_AMPHUR).Distinct().FirstOrDefault();

            var groupIdListMainTH = (from r in _FBB_ZIPCODE.Get()
                                     where r.LANG_FLAG == "N"
                                      && !r.AMPHUR.Contains("ปณ")
                                      && r.AMPHUR == command.Amphur
                                      && r.PROVINCE == command.Province
                                      && r.TUMBON == command.Tumbon
                                     select r.ZIPCODE_ROWID).Distinct().FirstOrDefault();

            var groupIdListMainEN = (from r in _FBB_ZIPCODE.Get()
                                     where r.LANG_FLAG == "Y"
                                      && !r.AMPHUR.Contains("PO")
                                      && r.AMPHUR == command.AmphurEN
                                      && r.PROVINCE == command.ProvinceEN
                                      && r.TUMBON == command.TumbonEN
                                     select r.ZIPCODE_ROWID).Distinct().FirstOrDefault();
            try
            {
                if (command.Action == "Package")
                {
                    //var outp = new List<object>();
                    //var paramOut = outp.ToArray();
                    //var a = _executeStored.ExecuteStoredProc("WBB.S_FBB_COVERAGE_REGION_MASTER", out paramOut, new { });
                }
                else
                {
                    if (command.Action == "Create")
                    {
                        if (command.tower_th != "" && command.tower_en != "")
                        {
                            var dup2 = from r in _FBB_COVERAGE_REGION.Get()
                                       where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                        && r.ACTIVEFLAG == "Y" && r.GROUP_AMPHUR == groupIdListMain
                                         && r.TOWER_EN == command.tower_en && r.TOWER_TH == command.tower_th.Trim() && r.SERVICE_TYPE == command.Service_Type
                                       select r;
                            if (dup2.Any())
                                command.FlagDup = true;

                            else
                                FTTxCreate(command);


                        }
                        else
                        {
                            if (command.Tumbon == "")
                            {
                                var dup = from r in _FBB_COVERAGE_REGION.Get()
                                          where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                           && r.ACTIVEFLAG == "Y" && r.ZIPCODE_ROWID_TH == groupIdListMain
                                            && r.SERVICE_TYPE == command.Service_Type
                                          select r;

                                if (dup.Any())
                                    command.FlagDup = true;

                                else
                                    FTTxCreate(command);
                            }
                            else
                            {
                                var dup = from r in _FBB_COVERAGE_REGION.Get()
                                          where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                           && r.ACTIVEFLAG == "Y" && r.ZIPCODE_ROWID_TH == groupIdListMainTH
                                            && r.SERVICE_TYPE == command.Service_Type
                                          select r;

                                if (dup.Any())
                                    command.FlagDup = true;

                                var dup2 = from r in _FBB_COVERAGE_REGION.Get()
                                           where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                            && r.ACTIVEFLAG == "Y" && r.ZIPCODE_ROWID_EN == groupIdListMainEN
                                             && r.SERVICE_TYPE == command.Service_Type
                                           select r;

                                if (dup2.Any())
                                    command.FlagDup = true;

                                if (command.FlagDup != true)
                                    FTTxCreate(command);
                            }


                        }



                    }

                    else if (command.Action == "Update")
                    {
                        if (command.tower_th != "" && command.tower_en != "")
                        {
                            var dup2 = from r in _FBB_COVERAGE_REGION.Get()
                                       where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                        && r.ACTIVEFLAG == "Y" && r.GROUP_AMPHUR == groupIdListMain && r.FTTX_ID != command.Fttx_id
                                         && r.TOWER_EN == command.tower_en && r.TOWER_TH == command.tower_th && r.SERVICE_TYPE == command.Service_Type
                                       select r;
                            if (dup2.Any())
                                command.FlagDup = true;

                            else
                                FTTxCreate(command);

                        }
                        else
                        {
                            if (command.Tumbon == "")
                            {
                                var dup = from r in _FBB_COVERAGE_REGION.Get()
                                          where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                           && r.ACTIVEFLAG == "Y" && r.GROUP_AMPHUR == groupIdListMain && r.FTTX_ID != command.Fttx_id
                                            && r.SERVICE_TYPE == command.Service_Type
                                          select r;
                                if (dup.Any())
                                    command.FlagDup = true;

                                else
                                    FTTxCreate(command);
                            }
                            else
                            {

                                var dup = from r in _FBB_COVERAGE_REGION.Get()
                                          where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                           && r.ACTIVEFLAG == "Y" && r.FTTX_ID != command.Fttx_id && r.ZIPCODE_ROWID_TH == groupIdListMainTH
                                            && r.SERVICE_TYPE == command.Service_Type
                                          select r;

                                if (dup.Any())
                                    command.FlagDup = true;

                                var dup2 = from r in _FBB_COVERAGE_REGION.Get()
                                           where r.OWNER_PRODUCT == command.OwnerProduct && r.OWNER_TYPE == command.OwnerType
                                            && r.ACTIVEFLAG == "Y" && r.FTTX_ID != command.Fttx_id && r.ZIPCODE_ROWID_EN == groupIdListMainEN
                                             && r.SERVICE_TYPE == command.Service_Type
                                           select r;

                                if (dup2.Any())
                                    command.FlagDup = true;

                                if (command.FlagDup != true)
                                    FTTxCreate(command);
                            }


                        }
                    }

                    else if (command.Action == "Delete")
                    {
                        FTTxDisabled(command);
                        _uow.Persist();
                    }

                    if (command.FlagDup != true)
                        _uow.Persist();
                }
                //end try
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
            }

        }

    }
}
