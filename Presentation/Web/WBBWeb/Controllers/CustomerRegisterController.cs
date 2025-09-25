using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Xml.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.Commons;
using WBBContract.Queries.Commons.Master;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBContract.Queries.WebServices.FBSS;
using WBBContract.Queries.WebServices.WTTX;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.WebServiceModels;
using WBBWeb.Extension;
using WBBWeb.Models;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Controllers
{
    [CustomHandleError]
    public class CustomerRegisterController : WBBController
    {
        //
        // GET: /Process/   

        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ReserveTimeSlotCommand> _commandReserveTimeslot;
        private readonly ICommandHandler<UpdateSplitterDummy3bbCommand> _updateSplitterDummy3bbCommand;

        public CustomerRegisterController(IQueryProcessor queryProcessor,
            ILogger logger,
            ICommandHandler<ReserveTimeSlotCommand> commandReserveTimeslot,
            ICommandHandler<UpdateSplitterDummy3bbCommand> updateSplitterDummy3bbCommand)
        {
            _queryProcessor = queryProcessor;
            _commandReserveTimeslot = commandReserveTimeslot;
            _updateSplitterDummy3bbCommand = updateSplitterDummy3bbCommand;
            base.Logger = logger;
        }

        public JsonResult CustomerRegisTestLog(string txtLog)
        {
            Logger.Info(txtLog);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimeSlotConfigSelectDate(string technology, string subtype, string flowflag, string splitterflagFirst, string splitterflag,
            string aisAirNumber = "", string timeStamp = "", string subAccessMode = "", string timeSlotRegisterFlag = "", string timeSlotPBOldFlag = "")
        {
            if (timeSlotRegisterFlag == "Y")
            {
                // 19.8 Access Mode is PREMIUM
                if (subAccessMode == "PREMIUM")
                {
                    var ConfigTSPremium = from tm in base.LovData
                                          where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                          && tm.Name == subAccessMode
                                          && tm.Text.ToUpper() == technology.ToUpper()
                                          select tm;

                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                            && tm.Name == (string.IsNullOrEmpty(subtype) ? "Customer" : subtype)
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                    if (!ConfigTSbySubtype.Any())
                    {
                        ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                            && tm.Name == "Default_Subtype"
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;
                    }

                    if (ConfigTSPremium != null && ConfigTSbySubtype.Any())
                    {
                        ConfigTSPremium.FirstOrDefault().LovValue5 = ConfigTSbySubtype.FirstOrDefault().LovValue5.ToSafeString();
                    }

                    if (ConfigTSPremium.Any())
                    {
                        base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                        base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                        base.Logger.Info("aisAirNumber : " + aisAirNumber);
                        base.Logger.Info("ASSIGN RULE : " + ConfigTSPremium.FirstOrDefault().LovValue5);
                        base.Logger.Info("subtype : " + subtype);

                        return Json(ConfigTSPremium.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                    }

                    base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + "");
                    base.Logger.Info("subtype : " + subtype);

                    return Json(ConfigTSPremium.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var typeTIME_SLOT = "TIME_SLOT_BY_SUBTYPE_HR";
                    if (timeSlotPBOldFlag == "N") typeTIME_SLOT = "TIME_SLOT_PB";

                    // TODO: R19.7 Check Flag Register Flow
                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == typeTIME_SLOT
                                            && tm.Name == (string.IsNullOrEmpty(subtype) ? "Customer" : subtype)
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                    if (!ConfigTSbySubtype.Any())
                    {
                        ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == typeTIME_SLOT
                                            && tm.Name == "Default_Subtype"
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                        if (ConfigTSbySubtype.Any())
                        {
                            base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                            base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                            base.Logger.Info("aisAirNumber : " + aisAirNumber);
                            base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                            base.Logger.Info("subtype : " + subtype);

                            return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                        }
                    }

                    base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                    base.Logger.Info("subtype : " + subtype);

                    return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                string nameTimeslot = subAccessMode == "PREMIUM" ? "L_TIME_SLOT_PREMIUM" : "L_TIME_SLOT";

                //TODO: Splitter Management
                #region Splitter Management

                if (!string.IsNullOrEmpty(flowflag) && technology == "FTTH")
                {
                    string caseName = null;
                    if (!string.IsNullOrEmpty(splitterflagFirst) && string.IsNullOrEmpty(splitterflag))
                    {
                        //if ((splitterflagFirst != "1") && (splitterflagFirst != "2"))
                        //{
                        //    return Json(new LovValueModel(), JsonRequestBehavior.AllowGet);
                        //}

                        //caseName = splitterflagFirst == "2" ? "L_TIME_SLOT_CASE_2" : nameTimeslot ;


                    }
                    else
                    {
                        //if (splitterflag == "2")
                        //    caseName = "L_TIME_SLOT_CASE_2";
                    }

                    LovValueModel defaultConfigSm;
                    //if ((splitterflagFirst == "1" || splitterflagFirst == "3") && (splitterflag != "2") && (!string.IsNullOrEmpty(subtype)) && subAccessMode != "PREMIUM")
                    if ((splitterflagFirst == "1" || splitterflagFirst == "2" || splitterflag == "3") && (!string.IsNullOrEmpty(subtype)) && subAccessMode != "PREMIUM")
                    {
                        defaultConfigSm =
                            LovData.FirstOrDefault(
                                item =>
                                    item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                    item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }
                    else
                    {
                        defaultConfigSm =
                            LovData.FirstOrDefault(
                                item => item.Name == caseName && item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }

                    if (defaultConfigSm == null)
                    {
                        defaultConfigSm = LovData.FirstOrDefault(
                            item => item.Name == nameTimeslot && item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }

                    // R19.2 Premium Install - Set Assign Rule at LovValue5
                    //if ((nameTimeslot == "L_TIME_SLOT_PREMIUM") && (splitterflagFirst == "1" || splitterflagFirst == "3") && (splitterflag != "2") && (!string.IsNullOrEmpty(subtype)))
                    if ((nameTimeslot == "L_TIME_SLOT_PREMIUM") && (splitterflagFirst == "1" || splitterflag == "2" || splitterflagFirst == "3") && (!string.IsNullOrEmpty(subtype)))
                    {
                        var LovValue5forPremium =
                               LovData.FirstOrDefault(
                                   item => item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                           item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());

                        if (LovValue5forPremium != null)
                        {
                            defaultConfigSm.LovValue5 = LovValue5forPremium.LovValue5.ToSafeString();
                        }
                    }//----End R19.2---

                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + defaultConfigSm != null ? defaultConfigSm.LovValue5 : "");
                    base.Logger.Info("subtype : " + subtype);

                    return defaultConfigSm == null ? Json("", JsonRequestBehavior.AllowGet) : Json(defaultConfigSm, JsonRequestBehavior.AllowGet);
                }
                #endregion Splitter Management

                if (!string.IsNullOrEmpty(subtype) && subAccessMode != "PREMIUM")
                {
                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE"
                                            && tm.Name == subtype
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;
                    if (ConfigTSbySubtype != null)
                    {
                        if (ConfigTSbySubtype.Any())
                        {
                            base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                            base.Logger.Info("aisAirNumber : " + aisAirNumber);
                            base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                            base.Logger.Info("subtype : " + subtype);
                            return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                var DefaultConfigTS = from tm in base.LovData
                                          //where tm.Name == "L_TIME_SLOT"
                                      where tm.Name == nameTimeslot
                                      && tm.Text.ToUpper() == technology.ToUpper()
                                      select tm;

                // R19.2 Premium Install - Set Assign Rule at LovValue5
                if (nameTimeslot == "L_TIME_SLOT_PREMIUM" && (!string.IsNullOrEmpty(subtype)))
                {
                    var LovValue5forPremium =
                        LovData.FirstOrDefault(
                            item => item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                    item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());

                    if (LovValue5forPremium != null)
                    {
                        DefaultConfigTS.FirstOrDefault().LovValue5 = LovValue5forPremium.LovValue5.ToSafeString();
                    }
                }//----End R19.2---

                if (DefaultConfigTS != null)
                {
                    if (DefaultConfigTS.Any())
                    {
                        return Json(DefaultConfigTS.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                    }
                }

                base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                base.Logger.Info("aisAirNumber : " + aisAirNumber);
                base.Logger.Info("ASSIGN RULE : " + "");
                base.Logger.Info("subtype : " + subtype);

            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        //TODO: Splitter Management
        //[OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult ReservedSplitterManagement(string installDate, string timeSlot, string languageValue,
            string flowFlag, string accessMode, string splitterFlagFirstTime,
            string splitterFlag, string splitterList, string splitterTransactionId, string appointmentDate,
            string reservationid, string addressid)
        {
            try
            {
                base.Logger.Info(splitterTransactionId.ToSafeString() + " Start Splitter Management Get ResReserved");

                const string showTimeSlot = "N";
                var timeSlotMessage = string.Empty;
                var reservationIdResult = string.Empty;
                var installDateResult = installDate ?? string.Empty;
                var timeSlotResult = timeSlot ?? string.Empty;
                var appointmentDateResult = appointmentDate ?? string.Empty;
                splitterList = WebUtility.HtmlDecode(splitterList ?? string.Empty);

                if (string.IsNullOrEmpty(flowFlag) || accessMode != "FTTH")
                {
                    base.Logger.Info(splitterTransactionId.ToSafeString() + " Splitter Management flowFlag=" + flowFlag.ToSafeString() + "accessMode=" + accessMode.ToSafeString());
                    return Json(new
                    {
                        RESULT = showTimeSlot,
                        splitterFlag,
                        reservationIdResult,
                        installDateResult,
                        timeSlotResult,
                        timeSlotMessage,
                        appointmentDateResult
                    }, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(splitterFlag) && !string.IsNullOrEmpty(reservationid))
                {
                    base.Logger.Info(splitterTransactionId.ToSafeString() + " Splitter Management Get ResReserved splitterFlag not NullOrEmpty");
                    return Json(new
                    {
                        RESULT = showTimeSlot,
                        splitterFlag,
                        reservationid,
                        installDateResult,
                        timeSlotResult,
                        appointmentDateResult
                    }, JsonRequestBehavior.AllowGet);
                }
                SplitterInfoList splitterListResult = new SplitterInfoList();
                List<SPLITTER_INFO> splitterListData = new List<SPLITTER_INFO>();

                if (!string.IsNullOrEmpty(splitterList))
                {
                    var serializer = new XmlSerializer(typeof(SplitterInfoList));
                    using (TextReader reader = new StringReader(splitterList))
                    {
                        splitterListResult = (SplitterInfoList)serializer.Deserialize(reader);
                    }
                }

                if (splitterListResult.Splitter != null)
                {
                    splitterListData = splitterListResult.Splitter.ConvertAll(x => new SPLITTER_INFO
                    {
                        Splitter_Name = x.Name.ToSafeString(),
                        Distance = x.Distance.ToSafeDecimal(),
                        Distance_Type = x.DistanceType.ToSafeString()
                    });
                }

                var queryResReservedQuery = new ZTEResReservedQuery
                {
                    PRODUCT = accessMode,
                    LISTOFSPLITTER = splitterListData.ToArray(),
                    TRANSACTION_ID = splitterTransactionId,
                    ADDRESS_ID = addressid
                };
                var resultResReserved = _queryProcessor.Execute(queryResReservedQuery);
                if (resultResReserved != null)
                {
                    splitterFlag = resultResReserved.RETURN_CASECODE;
                    reservationIdResult = resultResReserved.RESERVATION_ID;
                }
                else
                    splitterFlag = string.IsNullOrEmpty(splitterFlag) ? "3" : splitterFlag;

                Logger.Info(splitterTransactionId.ToSafeString() + " Splitter Management Return splitterFlagFirstTime= " +
                            splitterFlagFirstTime.ToSafeString() + " splitterFlag= " + splitterFlag.ToSafeString());

                //Splitter Manangement Change
                if (splitterFlagFirstTime != "3" && splitterFlagFirstTime != "2" && splitterFlag != "3" &&
                    splitterFlag != "2")
                    return Json(new
                    {
                        RESULT = showTimeSlot,
                        splitterFlag,
                        reservationIdResult,
                        installDateResult,
                        timeSlotResult,
                        timeSlotMessage,
                        appointmentDateResult
                    }, JsonRequestBehavior.AllowGet);

                installDateResult = string.Empty;
                timeSlotResult = string.Empty;
                appointmentDateResult = string.Empty;

                return Json(new
                {
                    RESULT = showTimeSlot,
                    splitterFlag,
                    reservationIdResult,
                    installDateResult,
                    timeSlotResult,
                    timeSlotMessage,
                    appointmentDateResult
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Info(splitterTransactionId.ToSafeString() + "Exception Splitter Management Get ResReserved: " + ex.GetErrorMessage());
                return Json(new { RESULT = "N", RESULT_DESC = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ReservedSplitter3bbManagement(string installDate, string timeSlot, string languageValue,
            string flowFlag, string accessMode, string splitterFlagFirstTime,
            string splitterFlag, string splitterList, string splitterTransactionId, string appointmentDate,
            string reservationid, string addressid, string homeLatitude, string homeLongitude, string mobileNo)
        {
            
            try
            {
                const string showTimeSlot = "N";
                var timeSlotMessage = string.Empty;
                var reservationIdResult = string.Empty;
                var installDateResult = installDate ?? string.Empty;
                var timeSlotResult = timeSlot ?? string.Empty;
                var appointmentDateResult = appointmentDate ?? string.Empty;
                splitterList = WebUtility.HtmlDecode(splitterList ?? string.Empty);
                if (string.IsNullOrEmpty(flowFlag) || accessMode != "FTTH")
                {
                    return Json(new
                    {
                        RESULT = showTimeSlot,
                        splitterFlag,
                        reservationIdResult,
                        installDateResult,
                        timeSlotResult,
                        timeSlotMessage,
                        appointmentDateResult
                    });
                }
                if (!string.IsNullOrEmpty(splitterFlag) && !string.IsNullOrEmpty(reservationid))
                {
                    return Json(new
                    {
                        RESULT = showTimeSlot,
                        splitterFlag,
                        reservationid,
                        installDateResult,
                        timeSlotResult,
                        appointmentDateResult
                    });
                }

                List<SplitterInfo3bb> splitter3bbListResult = new List<SplitterInfo3bb>();

                if (!string.IsNullOrEmpty(splitterList))
                {
                    string SplitterJson = Decrypt(splitterList);
                    splitter3bbListResult = new JavaScriptSerializer().Deserialize<List<SplitterInfo3bb>>(SplitterJson);
                }
                bool IsReservePort3bb = false;
                string referenceIdReserve = "";
                string splitterCodeReserve = "";
                string splitterAliasReserve = "";
                string distanceReserve = "";
                string splitterPortReserve = "";
                string splitterLatitudeReserve = "";
                string splitterLongitudeReserve = "";
                string mdfNameReserve = "";
                string mdfPortReserve = "";
                string isHomeReserve = "";


                if (splitter3bbListResult != null && splitter3bbListResult.Count > 0)
                {
                    foreach (var itemSplitter in splitter3bbListResult)
                    {
                        ReservePort3bbQuery reservePort3bbQuery = new ReservePort3bbQuery()
                        {
                            buildingAddressId = "",
                            transactionId = mobileNo.ToSafeString(),
                            homeLatitude = homeLatitude.ToSafeString(),
                            homeLongitude = homeLongitude.ToSafeString(),
                            splitterCode = itemSplitter.SplitterCode,
                        };
                        ReservePort3bbQueryModel reservePort3bbQueryModel = ReservePort3bb(reservePort3bbQuery);
                        if (reservePort3bbQueryModel != null)
                        {
                            if (reservePort3bbQueryModel.returnCode == "00000")
                            {
                                IsReservePort3bb = true;
                                referenceIdReserve = reservePort3bbQueryModel.referenceId;
                                splitterCodeReserve = reservePort3bbQueryModel.splitterCode;
                                splitterAliasReserve = reservePort3bbQueryModel.splitterAlias;
                                distanceReserve = reservePort3bbQueryModel.distance;
                                splitterPortReserve = reservePort3bbQueryModel.splitterPort;
                                splitterLatitudeReserve = reservePort3bbQueryModel.splitterLatitude;
                                splitterLongitudeReserve = reservePort3bbQueryModel.splitterLongitude;
                                mdfNameReserve = reservePort3bbQueryModel.mdfName;
                                mdfPortReserve = reservePort3bbQueryModel.mdfPort;
                                isHomeReserve = reservePort3bbQueryModel.isHome;
                                break;
                            }
                        }
                    }

                    if (!IsReservePort3bb)
                    {
                        return Json(new
                        {
                            RESULT = showTimeSlot,
                            splitterFlag,
                            reservationid,
                            installDateResult,
                            timeSlotResult,
                            appointmentDateResult
                        });
                    }

                    ZTEResReservedQuery queryResReservedQuery = new ZTEResReservedQuery
                    {
                        PRODUCT = accessMode,
                        LISTOFSPLITTER = null,
                        TRANSACTION_ID = mobileNo,
                        ADDRESS_ID = addressid,
                        PHONE_FLAG = isHomeReserve
                    };
                    var resultResReserved = _queryProcessor.Execute(queryResReservedQuery);

                    if (resultResReserved != null)
                    {
                        splitterFlag = resultResReserved.RETURN_CASECODE;
                        if (!string.IsNullOrEmpty(resultResReserved.RESERVATION_ID))
                        {
                            reservationIdResult = resultResReserved.RESERVATION_ID;

                        }
                        if (splitterFlag.Equals("3"))
                        {
                            ReleaseReservePort3bbQuery releaseReservePort3bbQuery = new ReleaseReservePort3bbQuery()
                            {
                                referenceId = referenceIdReserve
                            };
                            ReleaseReservePort3bbQueryModel reservePort3bbQueryModel = _queryProcessor.Execute(releaseReservePort3bbQuery);
                            throw new Exception();
                        }
                    }
                    else
                    {
                        ReleaseReservePort3bbQuery releaseReservePort3bbQuery = new ReleaseReservePort3bbQuery()
                        {
                            referenceId = referenceIdReserve
                        };
                        ReleaseReservePort3bbQueryModel reservePort3bbQueryModel = _queryProcessor.Execute(releaseReservePort3bbQuery);
                        throw new Exception();
                    }


                    #region 16/12/2022 comment getSplitterDummy3bbQuery,ZTEResReservedQuery,UpdateSplitterDummy3bbCommand
                    //GetSplitterDummy3bbQuery getSplitterDummy3bbQuery = new GetSplitterDummy3bbQuery()
                    //{
                    //    TransactionId = mobileNo,
                    //    p_address_id = addressid
                    //};
                    //GetSplitterDummy3bbQueryModel getSplitterDummy3bbModel = _queryProcessor.Execute(getSplitterDummy3bbQuery);

                    //if (getSplitterDummy3bbModel != null && getSplitterDummy3bbModel.Data != null
                    //    && getSplitterDummy3bbModel.Data.Count > 0)
                    //{
                    //    string SplitterNoLastCheck = "";
                    //    foreach (var SplitterDummy3bbItem in getSplitterDummy3bbModel.Data)
                    //    {
                    //        SplitterNoLastCheck = SplitterDummy3bbItem.splitter_no;
                    //        List<SPLITTER_INFO> splitterDataList = new List<SPLITTER_INFO>();
                    //        SPLITTER_INFO splitterData = new SPLITTER_INFO()
                    //        {
                    //            Splitter_Name = SplitterDummy3bbItem.splitter_no,
                    //            Distance = distanceReserve.ToSafeDecimal(),
                    //            Distance_Type = "",
                    //            Resource_Type = ""
                    //        };
                    //        splitterDataList.Add(splitterData);

                    //        ZTEResReservedQuery queryResReservedQuery = new ZTEResReservedQuery
                    //        {
                    //            PRODUCT = accessMode,
                    //            LISTOFSPLITTER = splitterDataList.ToArray(),
                    //            TRANSACTION_ID = mobileNo,
                    //            ADDRESS_ID = addressid,
                    //            PHONE_FLAG = isHomeReserve
                    //        };
                    //        var resultResReserved = _queryProcessor.Execute(queryResReservedQuery);
                    //        if (resultResReserved != null)
                    //        {
                    //            splitterFlag = resultResReserved.RETURN_CASECODE;
                    //            if (!string.IsNullOrEmpty(resultResReserved.RESERVATION_ID))
                    //            {
                    //                reservationIdResult = resultResReserved.RESERVATION_ID;
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    UpdateSplitterDummy3bbCommand updateSplitterDummy3bbCommand = new UpdateSplitterDummy3bbCommand()
                    //    {
                    //        p_address_id = addressid,
                    //        p_splitter_no = SplitterNoLastCheck,
                    //        Transaction_Id = mobileNo
                    //    };
                    //    _updateSplitterDummy3bbCommand.Handle(updateSplitterDummy3bbCommand);                       
                    //}
                    #endregion

                    return Json(new
                    {
                        RESULT = "Y",
                        splitterFlag,
                        reservationIdResult,
                        installDateResult,
                        timeSlotResult,
                        timeSlotMessage,
                        appointmentDateResult,
                        referenceIdReserve,
                        splitterCodeReserve,
                        splitterAliasReserve,
                        distanceReserve,
                        splitterPortReserve,
                        splitterLatitudeReserve,
                        splitterLongitudeReserve,
                        mdfNameReserve,
                        mdfPortReserve,
                        isHomeReserve
                    });
                }

                splitterFlag = "2";
                installDateResult = string.Empty;
                timeSlotResult = string.Empty;
                appointmentDateResult = string.Empty;

                return Json(new
                {
                    RESULT = showTimeSlot,
                    splitterFlag,
                    reservationIdResult,
                    installDateResult,
                    timeSlotResult,
                    timeSlotMessage,
                    appointmentDateResult
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                 
                return Json(new { RESULT = "N", RESULT_DESC = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ReservedDslamManagement(string accessMode, string dslamList, string phoneFlag, string TransactionId, string InCaseCode, string AddressId)
        {
            List<DSLAM_INFO> RESULTDSLAM_LIST;
            if (dslamList != null && dslamList != "")
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var routes_list = (object[])json_serializer.DeserializeObject(dslamList);

                RESULTDSLAM_LIST = null;
                if (routes_list != null)
                {
                    RESULTDSLAM_LIST = new List<DSLAM_INFO>();
                    foreach (Dictionary<string, object> item in routes_list)
                    {
                        if (item.ContainsKey("ResourceName"))
                        {
                            DSLAM_INFO resource_info = new DSLAM_INFO();
                            resource_info.Dslam_Name = (string)item["ResourceName"];
                            RESULTDSLAM_LIST.Add(resource_info);
                        }
                    }
                }
            }
            else
            {
                RESULTDSLAM_LIST = null;
            }

            if (TransactionId == "|")
            {
                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                TransactionId = "|" + ipAddress;
            }

            var reservationIdResult = string.Empty;

            var queryResReservedQuery = new ZTEResReservedQuery
            {
                PRODUCT = accessMode,
                LISTOFDSLAM = RESULTDSLAM_LIST.ToArray(),
                TRANSACTION_ID = TransactionId,
                PHONE_FLAG = phoneFlag,
                ADDRESS_ID = AddressId
            };
            var resultResReserved = _queryProcessor.Execute(queryResReservedQuery);
            if (resultResReserved != null)
            {
                if (resultResReserved.RETURN_CODE == "-1" && (InCaseCode == "6" || InCaseCode == "7"))
                {
                    string newPhoneFlag = "";
                    if (phoneFlag == "Y")
                    {
                        newPhoneFlag = "N";
                    }
                    else
                    {
                        newPhoneFlag = "Y";
                    }

                    var reQueryResReservedQuery = new ZTEResReservedQuery
                    {
                        PRODUCT = accessMode,
                        LISTOFDSLAM = RESULTDSLAM_LIST.ToArray(),
                        TRANSACTION_ID = TransactionId,
                        PHONE_FLAG = newPhoneFlag,
                        ADDRESS_ID = AddressId
                    };
                    var reResultResReserved = _queryProcessor.Execute(reQueryResReservedQuery);
                    if (reResultResReserved != null)
                    {
                        reservationIdResult = reResultResReserved.RESERVATION_ID;
                    }
                }
                else
                {
                    reservationIdResult = resultResReserved.RESERVATION_ID;
                }
            }

            return Json(new
            {
                ReservationIdResult = reservationIdResult,
            }, JsonRequestBehavior.AllowGet);

        }

        //public void CustomerRegisterOnLoad()
        //{
        //    var MasterDataController = Bootstrapper.GetInstance<MasterDataController>();
        //    //ViewBag.GetCustomerCardType = MasterDataController.GetCustomerCardType();
        //}

        public JsonResult GetCustomerSubCat(string idCardName)
        {
            var a = base.LovData.Where(p => p.Name == idCardName && p.Type == WebConstants.LovConfigName.CardType);
            LovValueModel model = new LovValueModel();
            if (a.Any())
            {
                model.LovValue3 = a.FirstOrDefault().LovValue3;
                model.LovValue4 = a.FirstOrDefault().LovValue4;
                model.LovValue5 = a.FirstOrDefault().LovValue5;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetAppointment(string installation_Date, string access_Mode = "", string address_Id = "",
            string serviceCode = "", string district = "", string subDistrict = "", string province = "", string postalCode = "", int lineSelect = 1,
            long days = 0, string productSpecCode = "",
            bool isThai = true, string timeSlotId = "", bool smallSize = false, string AisAirNumber = "", string SubAccessMode = "", string SetDefault = "", string taskType = "",
            string timeSlotRegisterFlag = "", string timeSlotRegisterHR = "", string timeSlotRegisterFlowActionNo = "", string installation_DateTimeNow = "",
            string serviceLevel = "", string areaRegion = "", string eventRule = "", string SpecialSkilL = "", string PlayBoxCountOld = "", string PlayBoxCountNew = "",
            string configLineLOV = "", string symptonCodePBreplace = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            StringBuilder strBuilder = new StringBuilder();
            StringBuilder strRemarkBuilder = new StringBuilder();
            DateTime installationDate = new DateTime();
            DateTime CerrentTime = DateTime.Now;

            string strFBSSTimeSlot = "";
            string firstInstallDate = "";
            string firstTimeSlot = "";
            string dllTimeFlag = "";

            try
            {

                #region Get IP Address Interface Log (Update 17.2)

                var defaultRegisterTimeSlot = new DateTime?();
                string transactionId = "";

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = AisAirNumber + ipAddress;

                #endregion

                if (isThai)
                {
                    DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, thDtfi);
                }
                else
                {
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, usDtfi);
                }

                /// check access_Mode for set FBSSTimeSlot

                // transform to null safe string commpare.
                var LovConstant = (from t in base.LovData
                                   where t.Type == "FBB_CONSTANT"
                                   && t.Name == "DEFAULT_TIMESLOT"
                                   && t.Text == access_Mode
                                   select t.LovValue1).ToList();
                if (LovConstant != null && LovConstant.Count > 0)
                {
                    strFBSSTimeSlot = LovConstant.FirstOrDefault().ToString();
                }

                /// End

                List<ASSIGN_CONDITION_ATTR> assignConditionList = new List<ASSIGN_CONDITION_ATTR>
                {
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SERVICE_LEVEL", VALUE = serviceLevel },
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "AREA_REGION", VALUE = areaRegion},
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "EVENT_RULE", VALUE = eventRule },
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SPECIAL_SKILL", VALUE = SpecialSkilL }
                };

                var actionTimeSlot = string.IsNullOrEmpty(timeSlotRegisterFlowActionNo) ? "" : (timeSlotRegisterFlowActionNo.Length > 1 ? timeSlotRegisterFlowActionNo.Split('|')[0] : "");
                var numTimeSlot = string.IsNullOrEmpty(timeSlotRegisterFlowActionNo) ? "" : (timeSlotRegisterFlowActionNo.Length > 1 ? timeSlotRegisterFlowActionNo.Split('|')[1] : "");

                GetFBSSAppointment query = new GetFBSSAppointment()
                {
                    AccessMode = access_Mode,
                    AddressId = address_Id,
                    Days = days,
                    ExtendingAttributes = "",
                    InstallationDate = installationDate.ToString("yyyy-MM-dd"),
                    ProductSpecCode = productSpecCode,
                    District = district,
                    Language = isThai ? "T" : "E",
                    Postal_Code = postalCode,
                    Province = province,
                    Service_Code = serviceCode,
                    SubDistrict = subDistrict,
                    LineSelect = (LineType)lineSelect,
                    Transaction_Id = transactionId,
                    SubAccessMode = SubAccessMode,
                    TaskType = taskType,
                    FullUrl = FullUrl,
                    ASSIGN_CONDITION_LIST = assignConditionList,
                    AisAirNumber = AisAirNumber,
                    PlayBoxCountOld = PlayBoxCountOld,
                    PlayBoxCountNew = PlayBoxCountNew,
                    TimeAdd = timeSlotRegisterHR,
                    ActionTimeSlot = actionTimeSlot,
                    NumTimeSlot = numTimeSlot,
                    SymptonCodePBreplace = symptonCodePBreplace
                };

                #region Data test
                //Random rnd = new Random();
                //List<FBSSTimeSlot> data = new List<FBSSTimeSlot>();

                //for (int i = 1; i <= days; i++)
                //{
                //    FBSSTimeSlot timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "08:00 - 10:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "10:00 - 12:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "13:00 - 15:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "15:00 - 18:00";
                //    data.Add(timeSlot);

                //    timeSlot = new FBSSTimeSlot();
                //    timeSlot.AppointmentDate = installationDate;
                //    timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    timeSlot.TimeSlot = "18:00 - 20:00";
                //    data.Add(timeSlot);

                //    //timeSlot = new FBSSTimeSlot();
                //    //timeSlot.AppointmentDate = installationDate;
                //    //timeSlot.InstallationCapacity = rnd.Next(0, 11).ToString() + "/10";
                //    //timeSlot.TimeSlot = "20:00-22:00";
                //    //listTimeSlot.Add(timeSlot);

                //    installationDate = installationDate.AddDays(1);
                //}
                #endregion

                var data = _queryProcessor.Execute(query);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        List<FBSSTimeSlot> fbssTimeSlotData = data;
                        #region TimeSlot Register

                        DateTime StartTimslot = CerrentTime;
                        DateTime StartTimslotAfternoon = CerrentTime;

                        string StartTimslotStr = base.LovData.FirstOrDefault(p => p.Type == "FBB_CONSTANT" &&
                                p.Name == "FTTB_START_TIMESLOT" + configLineLOV).LovValue1.ToSafeString();
                        string StartTimslotAfternoonStr = base.LovData.FirstOrDefault(p => p.Type == "FBB_CONSTANT" &&
                                p.Name == "START_TIMESLOT_AFTERNOON" + configLineLOV).LovValue1.ToSafeString();

                        if (StartTimslotStr != "")
                        {
                            StartTimslot = ConvertTimeSlotToTime(CerrentTime, StartTimslotStr);
                        }
                        if (StartTimslotAfternoonStr != "")
                        {
                            StartTimslotAfternoon = ConvertTimeSlotToTime(CerrentTime, StartTimslotAfternoonStr);
                        }

                        if (access_Mode == "VDSL")
                        {
                            fbssTimeSlotData = fbssTimeSlotData.Where(p => ConvertTimeSlotToTime(CerrentTime, p.TimeSlot) >= StartTimslot).ToList();
                        }

                        foreach (var itemTimeSlot in fbssTimeSlotData)
                        {
                            if (ConvertTimeSlotToTime(CerrentTime, itemTimeSlot.TimeSlot) < StartTimslotAfternoon)
                            {
                                itemTimeSlot.TimeFlag = "M";
                            }
                            else
                            {
                                itemTimeSlot.TimeFlag = "A";
                            }
                            itemTimeSlot.OrderByTimeSlot = ConvertTimeSlotToTime(CerrentTime, itemTimeSlot.TimeSlot);
                        }

                        string CheckShowTimeFlag = "M";

                        //R.19.7 Default Time Slot Register
                        if (timeSlotRegisterFlag == "Y")
                        {
                            var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                                ? timeSlotRegisterHR.ToSafeInteger() / 60
                                : 24;

                            var inputDateTimeStr = string.Format("{0} {1}",
                                installation_DateTimeNow,
                                DateTime.Now.ToDisplayText("HH:mm"));

                            DateTime inputDateTime;
                            if (isThai)
                            {
                                var thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                                inputDateTime = Convert.ToDateTime(inputDateTimeStr, thDtfi);
                            }
                            else
                            {
                                var usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                                inputDateTime = Convert.ToDateTime(inputDateTimeStr, usDtfi);
                            }

                            var dateBegin = inputDateTime.AddHours(addHr);
                            //var timeBegin = inputDateTime.AddHours(addHr).ToDisplayText("HH:mm");

                            List<FBSSTimeSlot> startTimeslot;
                            var timeSlotRegisterFlowAction = timeSlotRegisterFlowActionNo.Length > 1
                                ? timeSlotRegisterFlowActionNo.Split('|')[0] : "P";

                            var timeSlotRegisterFlowNo = timeSlotRegisterFlowActionNo.Length > 1
                                ? timeSlotRegisterFlowActionNo.Split('|')[1] : "1";

                            if (timeSlotRegisterFlowAction == "F")
                            {
                                startTimeslot = fbssTimeSlotData.Where(w =>
                                    ToDtByAppointmenTimeSlot(w.AppointmentDate, w.TimeSlot) > dateBegin)
                                    .OrderBy(o => o.AppointmentDate).ThenBy(t => t.TimeSlot)
                                    .ToList();
                            }
                            else
                            {
                                startTimeslot = fbssTimeSlotData.Where(w =>
                                    ToDtByAppointmenTimeSlot(w.AppointmentDate, w.TimeSlot) <= dateBegin)
                                    .OrderByDescending(o => o.AppointmentDate).ThenByDescending(t => t.TimeSlot)
                                    .ToList();
                            }

                            //R.19.8 Default Time Slot Register Previous
                            var FlowNo = (timeSlotRegisterFlowNo.ToSafeInteger() > 0
                                ? timeSlotRegisterFlowNo.ToSafeInteger()
                                : 1) - 1;

                            var rowSkipTimeslot = startTimeslot.Skip(FlowNo).FirstOrDefault();

                            var rowTimeslot = rowSkipTimeslot ?? startTimeslot.FirstOrDefault();
                            if (rowTimeslot != null)
                            {
                                defaultRegisterTimeSlot = ToDtByAppointmenTimeSlot(
                                    rowTimeslot.AppointmentDate,
                                    rowTimeslot.TimeSlot);
                            }
                        }

                        #endregion TimeSlot Register

                        #region Time column
                        var listDay = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_WEEK_OF_DAY")).OrderBy(p => p.OrderBy).ToList();

                        var listMonth = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_MONTH_OF_YEAR")).OrderBy(p => p.OrderBy).ToList();

                        var lblTime = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue1 : base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue2;

                        var oldDate = fbssTimeSlotData[0].AppointmentDate;
                        int remarkTop = 0;

                        //R21.8
                        //Temp Distinct ListTimeSlot
                        List<FBSSTimeSlot> tmpListTimeSlot = new List<FBSSTimeSlot>();
                        tmpListTimeSlot = fbssTimeSlotData.DistinctBy(d => d.TimeSlot)
                                                    .Select(s => new FBSSTimeSlot { TimeSlot = s.TimeSlot, OrderByTimeSlot = s.OrderByTimeSlot })
                                                    .OrderBy(o => o.OrderByTimeSlot).ToList();
                        //End R21.8---

                        //L_INSTALL_TIME
                        strBuilder.Append("<div style=\"float:left;\">");
                        strBuilder.Append("<div class=\"select-header-show\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div class=\"time-header\">");
                        strBuilder.Append("<label>" + lblTime + "</label> ");
                        strBuilder.Append("</div>");

                        //R21.8
                        for (int i = 0; i < tmpListTimeSlot.Count; i++)
                        {
                            string[] tmpTimeSlot1 = tmpListTimeSlot[i].TimeSlot.Split('-');
                            string lblTimeSlot = "";
                            if (tmpTimeSlot1.Count() > 0)
                            {
                                lblTimeSlot = tmpTimeSlot1[0].ToSafeString();
                            }

                            if (ConvertTimeSlotToTime(CerrentTime, tmpListTimeSlot[i].TimeSlot) < StartTimslotAfternoon)
                            {
                                strBuilder.Append("<div class=\"time-item slot-m \">");
                            }
                            else
                            {
                                strBuilder.Append("<div class=\"time-item slot-a \">");
                            }
                            strBuilder.Append("<label>" + lblTimeSlot + "</label>");
                            strBuilder.Append("</div>");
                            remarkTop++;
                        }
                        //End R21.8---

                        //for (int i = 0; i < fbssTimeSlotData.Count; i++)
                        //{
                        //    if (oldDate == fbssTimeSlotData[i].AppointmentDate)
                        //    {
                        //        string[] tmpTimeSlot1 = fbssTimeSlotData[i].TimeSlot.Split('-');
                        //        string lblTimeSlot = "";
                        //        if (tmpTimeSlot1.Count() > 0)
                        //        {
                        //            lblTimeSlot = tmpTimeSlot1[0].ToSafeString();
                        //        }

                        //        if (fbssTimeSlotData[i].TimeFlag == "M")
                        //        {
                        //            strBuilder.Append("<div class=\"time-item slot-m \">");
                        //        }
                        //        else
                        //        {
                        //            strBuilder.Append("<div class=\"time-item slot-a \">");
                        //        }
                        //        strBuilder.Append("<label>" + lblTimeSlot + "</label>");
                        //        strBuilder.Append("</div>");
                        //        remarkTop++;
                        //    }
                        //}

                        strBuilder.Append("<div class=\"time-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");

                        #endregion

                        #region R21.8 Add Lost Time
                        //Temp List fbssTimeSlotData
                        List<FBSSTimeSlot> tmpListfbssTimeSlotData = new List<FBSSTimeSlot>();
                        //Temp Distinct List AppointmentDate
                        List<FBSSTimeSlot> tmpListAppointmentDate = new List<FBSSTimeSlot>();
                        tmpListAppointmentDate = fbssTimeSlotData.DistinctBy(d => d.AppointmentDate)
                                                .Select(s => new FBSSTimeSlot { AppointmentDate = s.AppointmentDate }).ToList();

                        for (int i = 0; i < tmpListAppointmentDate.Count; i++)
                        {
                            for (int j = 0; j < tmpListTimeSlot.Count; j++)
                            {
                                FBSSTimeSlot tmpFBSSTimeSlot = new FBSSTimeSlot();
                                var k = fbssTimeSlotData.Where(w => w.AppointmentDate == tmpListAppointmentDate[i].AppointmentDate && w.TimeSlot == tmpListTimeSlot[j].TimeSlot);

                                if (k != null && k.Any())
                                {
                                    tmpFBSSTimeSlot = k.FirstOrDefault();
                                }
                                else
                                {
                                    tmpFBSSTimeSlot.AppointmentDate = tmpListAppointmentDate[i].AppointmentDate;
                                    tmpFBSSTimeSlot.TimeSlot = tmpListTimeSlot[j].TimeSlot;
                                    tmpFBSSTimeSlot.DayOffFlag = "N";
                                    tmpFBSSTimeSlot.ActiveFlag = "N";
                                    tmpFBSSTimeSlot.InstallationCapacity = "0/0";

                                    if (ConvertTimeSlotToTime(CerrentTime, tmpListTimeSlot[j].TimeSlot) < StartTimslotAfternoon)
                                        tmpFBSSTimeSlot.TimeFlag = "M";
                                    else
                                        tmpFBSSTimeSlot.TimeFlag = "A";
                                }

                                tmpListfbssTimeSlotData.Add(tmpFBSSTimeSlot);
                            }

                        }

                        fbssTimeSlotData = tmpListfbssTimeSlotData;

                        #endregion

                        #region Day column
                        int LIMIT_TIME_FOR_FAST_JOB = 0;
                        var tmp_limittime = base.LovData.Where(p => p.Name == "LIMIT_TIME_FOR_FAST_JOB").OrderBy(p => p.OrderBy).ToList();
                        if (tmp_limittime != null)
                            if (tmp_limittime.Any())
                                LIMIT_TIME_FOR_FAST_JOB = int.Parse(tmp_limittime.FirstOrDefault().LovValue1);
                        DateTime Curr_dt = DateTime.Now;

                        bool setDefaultFlag = false;
                        bool isDefaultTimeSlot = false;
                        bool setDefaultFirstFlag = false;

                        for (int i = 0; i < fbssTimeSlotData.Count; i++)
                        {
                            if (oldDate != fbssTimeSlotData[i].AppointmentDate || i == 0)
                            {
                                oldDate = fbssTimeSlotData[i].AppointmentDate;

                                if (i != 0)
                                {
                                    strBuilder.Append("<div class=\"day-footer-show\">");
                                    strBuilder.Append("<div class=\"day-footer\">");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                }

                                var dayNum = fbssTimeSlotData[i].AppointmentDate.Value.Day.ToString();
                                var dayName = isThai ? listDay[(int)fbssTimeSlotData[i].AppointmentDate.Value.DayOfWeek].LovValue1 : listDay[(int)fbssTimeSlotData[i].AppointmentDate.Value.DayOfWeek].LovValue2;
                                var monthName = isThai ? listMonth[fbssTimeSlotData[i].AppointmentDate.Value.Month - 1].LovValue1 : listMonth[fbssTimeSlotData[i].AppointmentDate.Value.Month - 1].LovValue2;

                                strBuilder.Append("<div style=\"float:left;\">");
                                strBuilder.Append("<div class=\"select-header-show\">");
                                strBuilder.Append("<div class=\"select-header\">");
                                strBuilder.Append("<span style=\"color:white;\">" + dayName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"day-header\">");
                                strBuilder.Append("<div class=\"header-label\">");
                                strBuilder.Append("<span>" + dayName + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 5px;\"></p>");
                                strBuilder.Append("<span>" + dayNum + " " + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"header-label-hidden\">");
                                strBuilder.Append("<span>" + dayNum + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 1px;\"></p>");
                                strBuilder.Append("<span>" + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }

                            if (oldDate == fbssTimeSlotData[i].AppointmentDate)
                            {
                                // set format date
                                var day = fbssTimeSlotData[i].AppointmentDate.Value.ToString("dd");
                                var month = fbssTimeSlotData[i].AppointmentDate.Value.ToString("MM");
                                var year = fbssTimeSlotData[i].AppointmentDate.Value.ToString("yyyy");
                                var appointment_date = year + "/" + month + "/" + day;
                                var install_date = day + "/" + month + "/" + year;
                                var slot = fbssTimeSlotData[i].InstallationCapacity.Split('/');

                                if (fbssTimeSlotData[i].TimeFlag == "M")
                                {
                                    strBuilder.Append("<div title=\"" + fbssTimeSlotData[i].TimeSlot + "\" class=\"day-item slot-m\" onclick=\"onDayItemClick(this,'" + timeSlotId + "');\">");
                                }
                                else
                                {
                                    strBuilder.Append("<div title=\"" + fbssTimeSlotData[i].TimeSlot + "\" class=\"day-item slot-a\" onclick=\"onDayItemClick(this,'" + timeSlotId + "');\">");
                                }
                                strBuilder.Append("<div class=\"day-item-circle\">");

                                //R21.3
                                var dayOffFlag = fbssTimeSlotData[i].DayOffFlag;
                                var ActiveFlag = fbssTimeSlotData[i].ActiveFlag;

                                if (Convert.ToInt32(slot[0]) <= 0 && dayOffFlag == "N")
                                {
                                    strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");

                                    if (timeSlotRegisterFlag == "Y")
                                    {
                                        //R.19.7 Default Time Slot Register
                                        var timeslotPeriodSlot0 = new DateTime(fbssTimeSlotData[i].AppointmentDate.Value.Year,
                                        fbssTimeSlotData[i].AppointmentDate.Value.Month,
                                        fbssTimeSlotData[i].AppointmentDate.Value.Day,
                                        int.Parse(fbssTimeSlotData[i].TimeSlot.Substring(0, 2)),
                                        int.Parse(fbssTimeSlotData[i].TimeSlot.Substring(3, 2)),
                                        0);
                                    }
                                }
                                else
                                {
                                    DateTime TimeslotPeriod = new DateTime(fbssTimeSlotData[i].AppointmentDate.Value.Year,
                                        fbssTimeSlotData[i].AppointmentDate.Value.Month,
                                        fbssTimeSlotData[i].AppointmentDate.Value.Day,
                                        int.Parse(fbssTimeSlotData[i].TimeSlot.Substring(0, 2)),
                                        int.Parse(fbssTimeSlotData[i].TimeSlot.Substring(3, 2)),
                                        0);

                                    if (timeSlotRegisterFlag == "Y")
                                    {

                                        if (TimeslotPeriod > Curr_dt.AddMinutes(LIMIT_TIME_FOR_FAST_JOB))
                                        {

                                            var dayOfWeek = fbssTimeSlotData[i].AppointmentDate.Value.DayOfWeek;
                                            if (!setDefaultFirstFlag && SetDefault == "Y" &&
                                                dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Saturday &&
                                                dayOffFlag != "Y" && ActiveFlag != "N")
                                            {
                                                // set FIRST_INSTALL_DATE and FIRST_TIME_SLOT
                                                firstInstallDate = install_date;
                                                firstTimeSlot = fbssTimeSlotData[i].TimeSlot;
                                                setDefaultFirstFlag = true;
                                            }

                                            if (!setDefaultFlag &&
                                                dayOfWeek != DayOfWeek.Sunday &&
                                                dayOfWeek != DayOfWeek.Saturday &&
                                                SetDefault == "Y" &&
                                                dayOffFlag != "Y" && ActiveFlag != "N") //isDefaultTimeSlot
                                            {
                                                CheckShowTimeFlag = fbssTimeSlotData[i].TimeFlag;
                                                strBuilder.Append("<i class=\"fa fa-circle color-green\"></i>");
                                                setDefaultFlag = true;
                                            }
                                            else if (dayOffFlag == "Y")
                                            {
                                                strBuilder.Append("<i class=\"fa fa-circle color-holiday\"></i>");
                                            }
                                            else
                                            {   //// dayOffFlag = N,null,empty
                                                //// color-gray คือ สีขาว [dayOffFlag = N,null,empty] [ActiveFlag = Y,null,empty]
                                                //// color -darkgrey คือ สีเทา [dayOffFlag = N,null,empty] [ActiveFlag = N]

                                                if ((dayOffFlag == "N" || string.IsNullOrEmpty(dayOffFlag)) && ActiveFlag != "N")
                                                    strBuilder.Append("<i class=\"fa fa-circle color-gray\"></i>");
                                                else
                                                    strBuilder.Append("<i class=\"fa fa-circle color-darkgrey\"></i>");
                                            }
                                        }
                                        else
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                                        }
                                    }
                                    else
                                    {
                                        // R.19.2 Default Time Slot

                                        if (defaultRegisterTimeSlot == null)
                                        {
                                            isDefaultTimeSlot = true;
                                        }

                                        var dayOfWeek = fbssTimeSlotData[i].AppointmentDate.Value.DayOfWeek;
                                        //R20.10 Day Off Flag

                                        if (TimeslotPeriod > Curr_dt.AddMinutes(LIMIT_TIME_FOR_FAST_JOB))
                                        {
                                            if (!setDefaultFirstFlag && SetDefault == "Y" && isDefaultTimeSlot &&
                                                dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Saturday && dayOffFlag != "Y")
                                            {
                                                // set FIRST_INSTALL_DATE and FIRST_TIME_SLOT
                                                firstInstallDate = install_date;
                                                firstTimeSlot = fbssTimeSlotData[i].TimeSlot;
                                                setDefaultFirstFlag = true;
                                            }

                                            if (!setDefaultFlag && dayOfWeek != DayOfWeek.Sunday &&
                                                dayOfWeek != DayOfWeek.Saturday && SetDefault == "Y" && dayOffFlag != "Y")
                                            {
                                                CheckShowTimeFlag = fbssTimeSlotData[i].TimeFlag;
                                                strBuilder.Append("<i class=\"fa fa-circle color-green\"></i>");
                                                setDefaultFlag = true;
                                            }
                                            else
                                            {
                                                if (dayOffFlag == "Y")
                                                    strBuilder.Append("<i class=\"fa fa-circle color-holiday\"></i>");
                                                else
                                                    strBuilder.Append("<i class=\"fa fa-circle color-gray\"></i>");
                                            }
                                        }
                                        else
                                        {
                                            strBuilder.Append("<i class=\"fa fa-circle color-red\"></i>");
                                        }
                                    }
                                }

                                strBuilder.Append("<div style=\"display:none;\">");
                                strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"" + fbssTimeSlotData[i].TimeSlotId + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + appointment_date + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + fbssTimeSlotData[i].TimeSlot + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"" + fbssTimeSlotData[i].InstallationCapacity + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + install_date + "\"/>");
                                strBuilder.Append("</div>");

                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }
                        }

                        strBuilder.Append("<div class=\"day-footer-show\">");
                        strBuilder.Append("<div class=\"day-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div style=\"clear: both;\"></div>");
                        #endregion

                        #region Remark
                        string insSelected = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_SELECTED").Select(p => p.LovValue2).FirstOrDefault();
                        string insAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();
                        string insNotAvailable = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_NOT_ AVAILABLE").Select(p => p.LovValue2).FirstOrDefault();
                        string insDisabled = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_DISABLED").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_DISABLED").Select(p => p.LovValue2).FirstOrDefault();
                        string insDayOff = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_DAYOFF").Select(p => p.LovValue1).FirstOrDefault() : base.LovData.Where(p => p.Name == "L_INSTALL_DAYOFF").Select(p => p.LovValue2).FirstOrDefault();

                        strRemarkBuilder.Append("<fieldset class=\"timeslot-remark center-block\"");

                        //R20.11 issue remark time slot tracking
                        if (days == 6)
                        {
                            if (SubAccessMode == "SCPE")
                                strRemarkBuilder.Append("style=\"width: 645px; margin-right: 165px;\"");
                            else
                                strRemarkBuilder.Append("style=\"width: 645px; margin-right: 275px;\"");
                        }

                        strRemarkBuilder.Append(">");
                        strRemarkBuilder.Append("<div style=\"margin-top:3px;\">");
                        strRemarkBuilder.Append("<i class=\"fa fa-circle color-green-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insSelected + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-gray-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insAvailable + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-red-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insNotAvailable + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-darkgrey-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insDisabled + "</label>");
                        strRemarkBuilder.Append("&nbsp&nbsp<i class=\"fa fa-circle color-holiday-remark\"></i>");
                        strRemarkBuilder.Append("&nbsp&nbsp<label>" + insDayOff + "</label>");
                        strRemarkBuilder.Append("</div>");
                        strRemarkBuilder.Append("</fieldset>");

                        #endregion                      

                        // Get Value For Gen dll TimeFlag
                        List<LovValueModel> dllTimeFlagData = base.LovData.Where(p => p.Type == "SCREEN" && p.Name == "DROPDOWN_FILTER_TIMESLOT" + configLineLOV).ToList();
                        dllTimeFlag = "";

                        dllTimeFlag += "<select class=\"select\" style=\"width: 100%\" id=\"dll_TimeFlag\" onchange=\"onChangeTimeFlag(this,'" + timeSlotId + "');\">";

                        foreach (var dllTimeFlageItem in dllTimeFlagData)
                        {
                            string dllTimeFlagText = "";
                            if (isThai)
                            {
                                dllTimeFlagText = dllTimeFlageItem.LovValue1.ToSafeString();
                            }
                            else
                            {
                                dllTimeFlagText = dllTimeFlageItem.LovValue2.ToSafeString();
                            }

                            if (dllTimeFlageItem.LovValue3 == CheckShowTimeFlag)
                            {
                                dllTimeFlag += "<option value=\"" + dllTimeFlageItem.LovValue3 + "\" selected>" + dllTimeFlagText + "</option>";
                            }
                            else
                            {
                                dllTimeFlag += "<option value=\"" + dllTimeFlageItem.LovValue3 + "\">" + dllTimeFlagText + "</option>";
                            }
                        }
                        dllTimeFlag += "</select>";

                    }
                    else
                    {
                        strBuilder.Append("<div style=\"display:none;\"> List data = 0");

                        if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                        {
                            var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                                ? timeSlotRegisterHR.ToSafeInteger()
                                : 24;

                            if (isThai)
                            {
                                strBuilder.Append(", installation_Date = " + installationDate.AddHours(addHr).AddYears(543).ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                strBuilder.Append(", installation_Date = " + installationDate.AddHours(addHr).ToString("dd/MM/yyyy"));
                            }

                        }
                        else
                        {
                            if (isThai)
                            {
                                strBuilder.Append(", installation_Date = " + installationDate.AddYears(543).ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                strBuilder.Append(", installation_Date = " + installationDate.ToString("dd/MM/yyyy"));
                            }
                        }

                        strBuilder.Append(", access_Mode = " + access_Mode);
                        strBuilder.Append(", address_Id = " + address_Id);
                        strBuilder.Append(", days = " + days);
                        strBuilder.Append(", productSpecCode =" + productSpecCode);
                        strBuilder.Append(", ExtendingAttributes = ");
                        strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");

                        if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                        {
                            var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                                ? timeSlotRegisterHR.ToSafeInteger()
                                : 24;

                            strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.AddHours(addHr).ToString("yyyy/MM/dd") + "\"/>");
                        }
                        else
                        {
                            strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                        }

                        strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");

                        if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                        {
                            var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                                ? timeSlotRegisterHR.ToSafeInteger()
                                : 24;

                            strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.AddHours(addHr).ToString("dd/MM/yyyy") + "\"/>");
                        }
                        else
                        {
                            strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                        }

                        strBuilder.Append("</div>");
                    }
                }
                else
                {
                    strBuilder.Append("<div style=\"display:none;\"> Data null");

                    if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                    {
                        var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                            ? timeSlotRegisterHR.ToSafeInteger()
                            : 24;

                        if (isThai)
                        {
                            strBuilder.Append(", installation_Date = " + installationDate.AddHours(addHr).AddYears(543).ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            strBuilder.Append(", installation_Date = " + installationDate.AddHours(addHr).ToString("dd/MM/yyyy"));
                        }

                    }
                    else
                    {
                        if (isThai)
                        {
                            strBuilder.Append(", installation_Date = " + installationDate.AddYears(543).ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            strBuilder.Append(", installation_Date = " + installationDate.ToString("dd/MM/yyyy"));
                        }
                    }

                    strBuilder.Append(", access_Mode = " + access_Mode);
                    strBuilder.Append(", address_Id = " + address_Id);
                    strBuilder.Append(", days = " + days);
                    strBuilder.Append(", productSpecCode =" + productSpecCode);
                    strBuilder.Append(", ExtendingAttributes = ");
                    strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");

                    if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                    {
                        var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                            ? timeSlotRegisterHR.ToSafeInteger()
                            : 24;

                        strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.AddHours(addHr).ToString("yyyy/MM/dd") + "\"/>");
                    }
                    else
                    {
                        strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                    }

                    strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");

                    if (timeSlotRegisterFlag == "Y" && installation_Date == installation_DateTimeNow)
                    {
                        var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                            ? timeSlotRegisterHR.ToSafeInteger()
                            : 24;

                        strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.AddHours(addHr).ToString("yyyy/MM/dd") + "\"/>");
                    }
                    else
                    {
                        strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                    }

                    strBuilder.Append("</div>");
                }

                return Json(
                    new
                    {
                        timeSlotData = strBuilder.ToSafeString(),
                        timeSlotRemark = strRemarkBuilder.ToSafeString(),
                        firstInstallDate = firstInstallDate.ToSafeString(),
                        firstTimeSlot = firstTimeSlot.ToSafeString(),
                        dllTimeFlag = dllTimeFlag
                    },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.RenderExceptionMessage());
                strBuilder.Append("<div style=\"display:none;\"> Data error " + ex.ToString());
                strBuilder.Append(", installation_Date = " + installation_Date);
                strBuilder.Append(", access_Mode = " + access_Mode);
                strBuilder.Append(", address_Id = " + address_Id);
                strBuilder.Append(", days = " + days);
                strBuilder.Append(", productSpecCode =" + productSpecCode);
                strBuilder.Append(", ExtendingAttributes = ");
                strBuilder.Append("<input type=\"text\" name=\"TimeSlotId\" value=\"\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSAppointmentDate\" value=\"" + installationDate.ToString("yyyy/MM/dd") + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallationCapacity\" value=\"0/1\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSInstallDate\" value=\"" + installationDate.ToString("dd/MM/yyyy") + "\"/>");
                strBuilder.Append("</div>");
            }

            return Json(
                new
                {
                    timeSlotData = strBuilder.ToSafeString(),
                    timeSlotRemark = strRemarkBuilder.ToSafeString(),
                    firstInstallDate = firstInstallDate.ToSafeString(),
                    firstTimeSlot = firstTimeSlot.ToSafeString(),
                    dllTimeFlag = dllTimeFlag
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAppointmentFirstTimeSlot(string installation_Date, string access_Mode = "", string address_Id = "",
            string serviceCode = "", string district = "", string subDistrict = "", string province = "", string postalCode = "", int lineSelect = 1,
            long days = 0, string productSpecCode = "",
            bool isThai = true, string timeSlotId = "", bool smallSize = false, string AisAirNumber = "", string SubAccessMode = "", string SetDefault = "", string taskType = "",
            string timeSlotRegisterFlag = "", string timeSlotRegisterHR = "", string timeSlotRegisterFlowActionNo = "", string installation_DateTimeNow = "",
            string serviceLevel = "", string areaRegion = "", string eventRule = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            DateTime installationDate = new DateTime();
            FBSSTimeSlot result = new FBSSTimeSlot()
            {
                FirstInstallDate = "",
                FirstTimeSlot = ""
            };

            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                var defaultRegisterTimeSlot = new DateTime?();
                string transactionId = "";

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = AisAirNumber + ipAddress;

                #endregion

                if (isThai)
                {
                    DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, thDtfi);
                }
                else
                {
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    installationDate = Convert.ToDateTime(installation_Date, usDtfi);
                }

                List<ASSIGN_CONDITION_ATTR> assignConditionList = new List<ASSIGN_CONDITION_ATTR>
                {
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SERVICE_LEVEL", VALUE = serviceLevel },
                    new ASSIGN_CONDITION_ATTR(){  ATTR_NAME = "AREA_REGION", VALUE = areaRegion},
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "EVENT_RULE", VALUE = eventRule }
                };

                //R21.3
                var actionTimeSlot = string.IsNullOrEmpty(timeSlotRegisterFlowActionNo) ? "" : (timeSlotRegisterFlowActionNo.Length > 1 ? timeSlotRegisterFlowActionNo.Split('|')[0] : "");
                var numTimeSlot = string.IsNullOrEmpty(timeSlotRegisterFlowActionNo) ? "" : (timeSlotRegisterFlowActionNo.Length > 1 ? timeSlotRegisterFlowActionNo.Split('|')[1] : "");

                GetFBSSAppointment query = new GetFBSSAppointment()
                {
                    AccessMode = access_Mode,
                    AddressId = address_Id,
                    Days = days,
                    ExtendingAttributes = "",
                    InstallationDate = installationDate.ToString("yyyy-MM-dd"),
                    ProductSpecCode = productSpecCode,
                    District = district,
                    Language = isThai ? "T" : "E",
                    Postal_Code = postalCode,
                    Province = province,
                    Service_Code = serviceCode,
                    SubDistrict = subDistrict,
                    LineSelect = (LineType)lineSelect,
                    Transaction_Id = transactionId,
                    SubAccessMode = SubAccessMode,
                    TaskType = taskType,
                    FullUrl = FullUrl,
                    ASSIGN_CONDITION_LIST = assignConditionList,
                    //R21.3
                    TimeAdd = timeSlotRegisterHR,
                    ActionTimeSlot = actionTimeSlot,
                    NumTimeSlot = numTimeSlot
                };

                var data = _queryProcessor.Execute(query);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        #region TimeSlot Register

                        //R.19.7 Default Time Slot Register
                        if (timeSlotRegisterFlag == "Y")
                        {
                            var addHr = timeSlotRegisterHR.ToSafeInteger() >= 0
                                ? timeSlotRegisterHR.ToSafeInteger()
                                : 24;

                            var inputDateTimeStr = string.Format("{0} {1}",
                                installation_DateTimeNow,
                                DateTime.Now.ToDisplayText("HH:mm"));

                            DateTime inputDateTime;
                            if (isThai)
                            {
                                var thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                                inputDateTime = Convert.ToDateTime(inputDateTimeStr, thDtfi);
                            }
                            else
                            {
                                var usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                                inputDateTime = Convert.ToDateTime(inputDateTimeStr, usDtfi);
                            }

                            var dateBegin = inputDateTime.AddHours(addHr);

                            List<FBSSTimeSlot> TimeslotData = new List<FBSSTimeSlot>();

                            TimeslotData = data.Where(w => w.TimeSlot != "08:00-10:00").OrderBy(o => o.AppointmentDate).ThenBy(t => t.TimeSlot).ToList();

                            if (TimeslotData != null && TimeslotData.Count > 0)
                            {
                                foreach (var item in TimeslotData)
                                {
                                    string[] InstallationCapacityValue = item.InstallationCapacity.Split('/');
                                    if (InstallationCapacityValue.Count() == 2)
                                    {
                                        int fCapacityValue = 0;
                                        int bCapacityValue = 0;
                                        if (int.TryParse(InstallationCapacityValue[0], out fCapacityValue) && int.TryParse(InstallationCapacityValue[1], out bCapacityValue))
                                        {
                                            if (fCapacityValue > 0)
                                            {
                                                var ddd = item.AppointmentDate;
                                                var day = item.AppointmentDate.Value.ToString("dd");
                                                var month = item.AppointmentDate.Value.ToString("MM");
                                                var year = item.AppointmentDate.Value.ToString("yyyy");
                                                var install_date = day + "/" + month + "/" + year;
                                                result = new FBSSTimeSlot()
                                                {
                                                    FirstTimeSlot = item.TimeSlot,
                                                    FirstInstallDate = install_date
                                                };
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion TimeSlot Register                                                               
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(
                result,
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCheckEventCode(string event_Code)
        {
            EventCodeModel data;
            var query = new GetCheckEventCodeQuery()
            {
                Event_Code = event_Code
            };

            data = _queryProcessor.Execute(query);



            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventDetail(string EventCode, string DatePeriodTH = "", string DatePeriodEN = "", string IDCardNo = "", string Technology = "")
        {
            //EventCodeModel EventDetail = new EventCodeModel();
            var query = new GetEventDetailQuery()
            {
                DatePeriodTH = DatePeriodTH,
                DatePeriodEN = DatePeriodEN,
                EventCode = EventCode,
                IDCardNo = IDCardNo,
                Language = SiteSession.CurrentUICulture.IsEngCulture(),
                Technology = Technology
            };

            var EventDetail = _queryProcessor.Execute(query);

            return Json(EventDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCheckCPE(string CPECode = "", string playbox = "", string idcardNo = "", string AisAirNumber = "")
        {
            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = AisAirNumber + ipAddress;

            #endregion

            // 17.6 Interface Log Add Url
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            var query = new GetFBSSCheckCPEQuery()
            {
                CPE = CPECode.Trim(),
                playbox = playbox.Trim(),
                IN_ID_CARD_NO = idcardNo.Trim(),
                Transaction_Id = transactionId,
                FullUrl = FullUrl
            };

            var CPEListDetail = _queryProcessor.Execute(query);

            List<CPEINFO> CPE_Info = new List<CPEINFO>();

            int rowNo = 0;
            foreach (var item in CPEListDetail)
            {
                rowNo += 1;

                var s_display_val = CheckCPE_TYPE(item.CPE_TYPE);
                string cpe_err = "";
                string cpe_type = "";

                cpe_type = s_display_val;
                if (s_display_val == "CPE")
                {
                    cpe_err = "";
                    if (rowNo == 2)
                    {
                        cpe_type = "PLAYBOX";
                        cpe_err = Get_CPEERROR();
                    }

                }
                else if (s_display_val == "PLAYBOX")
                {
                    cpe_err = "";
                    if (CPEListDetail.Count == 2)
                    {
                        if (rowNo == 1)
                        {
                            if (cpe_type == "PLAYBOX")
                            {
                                cpe_type = "CPE";
                                cpe_err = Get_CPEERROR();
                            }
                        }
                    }


                }
                else
                {
                    cpe_err = Get_CPEERROR();
                    if (string.IsNullOrEmpty(s_display_val))
                    {
                        if (rowNo == 1)
                        {
                            cpe_type = "CPE";
                        }
                        else
                        {
                            cpe_type = "PLAYBOX";
                        }
                    }

                }

                CPEINFO CPEitem = new CPEINFO();

                if (item.SN == CPECode.Trim())
                {
                    //check cpe
                    if (cpe_err == "")
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = ("CPE");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = "";
                        CPEitem.SN_PATTERN = item.SN_PATTERN;

                        //20.4
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                    else
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = cpe_type;

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = cpe_err;
                        CPEitem.SN_PATTERN = item.SN_PATTERN;

                        //20.4
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                }
                else
                {
                    //check playbox

                    if (cpe_err == "")
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = ("PLAYBOX");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = "";
                        CPEitem.SN_PATTERN = item.SN_PATTERN;

                        //20.4
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                    else
                    {
                        CPEitem.SN = item.SN;
                        CPEitem.CPE_MAC_ADDR = item.CPE_MAC_ADDR;
                        CPEitem.cpe_type = cpe_type;//("PLAYBOX");

                        CPEitem.Check_Result_Code = item.Check_Result_Code;
                        CPEitem.Check_Result_Desc = item.Check_Result_Desc;
                        CPEitem.STATUS_DESC = item.STATUS_DESC;
                        CPEitem.Status_ID = item.Status_ID;
                        CPEitem.CPE_TYPE_ERR = cpe_err;
                        CPEitem.SN_PATTERN = item.SN_PATTERN;

                        //20.4
                        CPEitem.CPE_MODEL_NAME = item.CPE_MODEL_NAME;
                        CPEitem.CPE_COMPANY_CODE = item.CPE_COMPANY_CODE;
                        CPEitem.CPE_PLANT = item.CPE_PLANT;
                        CPEitem.CPE_STORAGE_LOCATION = item.CPE_STORAGE_LOCATION;
                        CPEitem.CPE_MATERIAL_CODE = item.CPE_MATERIAL_CODE;
                        CPEitem.REGISTER_DATE = item.REGISTER_DATE;
                        CPEitem.FIBRENET_ID = item.FIBRENET_ID;
                        CPEitem.SHIP_TO = item.SHIP_TO;
                        CPEitem.WARRANTY_START_DATE = item.WARRANTY_START_DATE;
                        CPEitem.WARRANTY_END_DATE = item.WARRANTY_END_DATE;
                        CPEitem.MAC_ADDRESS = item.MAC_ADDRESS;
                    }
                }
                CPE_Info.Add(CPEitem);
            }



            return Json(CPE_Info, JsonRequestBehavior.AllowGet);
        }

        public string CheckCPE_TYPE(string cpe_type)
        {
            List<LovValueModel> lov = base.LovData.Where(l => l.LovValue1 == cpe_type && l.Type == "CHECK_CPE" && l.Name == "CPE_TYPE").ToList();
            var s_display_val = lov.Select(i => i.Text).FirstOrDefault();

            return s_display_val;
        }

        public String Get_CPEERROR()
        {
            string result = "";
            var langFlg = Session[WebConstants.SessionKeys.CurrentUICulture].ToSafeString() == "1" ? "TH" : "EN";
            LovValueModel msgLov;
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            msgLov = masterController.GetLovList("SCREEN", "L_ERROR_CPE").ToList().FirstOrDefault();
            if (langFlg == "TH")
            {
                result = msgLov.LovValue1;
            }
            else
            {
                result = msgLov.LovValue2;
            }
            return result;
        }

        public JsonResult Get_ScanBarcode()
        {
            List<LovValueModel> config = base.LovData.Where(l => l.Name == "C_BARCODE" && l.Type == "SCREEN").ToList();

            var c_Barcode = config.Select(i => i.LovValue1).FirstOrDefault();

            return Json(c_Barcode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReserveTimeSlot(Guid transactionGuid, decimal timeSlotId = 0)
        {
            var command = new ReserveTimeSlotCommand
            {
                IdReserve = transactionGuid,
                ReserveDTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeSlotId = timeSlotId
            };

            _commandReserveTimeslot.Handle(command);

            return Json(command.Return_Code, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // 16/06/2015 by john
        /// </summary>

        public JsonResult CheckBlackList(string idcardNo = "")
        {

            var query = new evOMServiceIVRCheckBlackListQuery
            {
                inCardNo = idcardNo
            };
            var a = _queryProcessor.Execute(query);


            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckInputMobileList(string lMobile = "")
        {
            bool data = true;

            try
            {
                var query = new GetLovQuery
                {
                    LovType = "FBB_CONSTANT",
                    LovName = "INPUT_MOBILE"
                };

                var result = _queryProcessor.Execute(query);
                var chkResult = result.Where(t => t.LovValue1 == lMobile);
                if (chkResult.Count() > 0)
                {
                    data = true;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    data = false;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = false)]
        [HttpPost]
        public JsonResult GetOrderDup(string p_id_card = "", string LanguagePage = "")
        {
            // ส่ง access mode list ที่ได้มาจาก GetFBSSFeasibilityCheckHandler
            //var accessModeModel = new JavaScriptSerializer().Deserialize<List<OrderDupModel>>(OrderDup);
            p_id_card = DecryptStringAES(p_id_card, "fbbwebABCDEFGHIJ");
            var query = new GetOrderDupQuery
            {
                p_id_card = p_id_card,
                p_eng_flag = base.GetCurrentCulture().IsEngCulture().ToYesNoFlgString(),
            };

            var orders = _queryProcessor.Execute(query);

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConfirmDeleteMSG()
        {
            LovValueModel msgLov;
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            msgLov = masterController.GetLovList("SCREEN", "L_CONFIRM_CANCEL_DUP").ToList().FirstOrDefault();
            return Json(msgLov, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConfirmCancelMSG()
        {
            LovValueModel msgLov;
            var masterController = Bootstrapper.GetInstance<MasterDataController>();
            msgLov = masterController.GetLovList("SCREEN", "L_BUTTON_CANCEL_ORD").ToList().FirstOrDefault();
            return Json(msgLov, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadTest()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> Arr_files = new List<string>();
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        if (file.ContentLength > 0)
                        {
                            Arr_files.Add(files.AllKeys[i]);
                        }

                        //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        //file.SaveAs(fname);
                    }
                    return Json(Arr_files);
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            else
            {
            TODO: // check image from card reader
                var listOfBase64Photo = Session["base64photo"] as Dictionary<string, string>;
                if (listOfBase64Photo == null || !listOfBase64Photo.Any())
                {
                    return Json("");
                }
                else
                {
                    return Json("UploadTest fileIDCard : Complete");
                }

            }
        }

        public JsonResult GetReservedTimeslotParameter(string nonMobile, string addressID, string promotionMain, string appointmentDate, string timeSlot, string language,
            string installAddress1, string installAddress2, string installAddress3, string installAddress4, string installAddress5, string outServiceLevel, string optionMesh)
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = nonMobile + ipAddress;

            #endregion

            var query = new GetMeshParameterReserveQuery
            {
                nonMobile = nonMobile,
                p_addressID = addressID,
                promotionMain = promotionMain,
                appointmentDate = appointmentDate,
                timeSlot = timeSlot,
                language = language,
                installAddress1 = installAddress1,
                installAddress2 = installAddress2,
                installAddress3 = installAddress3,
                installAddress4 = installAddress4,
                installAddress5 = installAddress5,
                outServiceLevel = outServiceLevel,
                optionMesh = optionMesh
            };

            var model = _queryProcessor.Execute(query);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeshAppointmentParameter(string nonMobile, string addressID, string Channel, string installDate, string language,
            string installAddress1, string installAddress2, string installAddress3, string installAddress4, string installAddress5, string outServiceLevel, string optionMesh,
            string officerChannel, string flag_call)
        {

            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();

            #region Get IP Address Interface Log (Update 17.2)

            // Get IP Address
            string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string transactionId = nonMobile + ipAddress;

            #endregion

            var query2 = new GetMeshParameterAppointmentQuery
            {
                nonMobile = nonMobile,//"8800023695",
                p_addressID = addressID,
                Channel = Channel,
                installDate = installDate,
                language = language,
                installAddress1 = installAddress1,
                installAddress2 = installAddress2,
                installAddress3 = installAddress3,
                installAddress4 = installAddress4,
                installAddress5 = installAddress5,
                outServiceLevel = outServiceLevel,
                optionMesh = optionMesh,
                officerChannel = officerChannel,//R21.10
                flag_call = flag_call//R21.10
            };

            var model = _queryProcessor.Execute(query2);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult ReservedTimeslot(string APPOINTMENT_DATE, string TIME_SLOT, string ACCESS_MODE, string PROD_SPEC_CODE, string ADDRESS_ID, string LOCATION_CODE, string SUBDISTRICT, string POSTAL_CODE, string ASSIGN_RULE, string AISAIRNUMBER, string SUB_ACCESS_MODE,
            string TASK_TYPE = "", string SERVICE_LEVEL = "", string AREA_REGION = "", string EVENT_RULE = "", string SPECIAL_SKILL = "", string PlayBoxCountOld = "", string PlayBoxCountNew = "", string SymptonCodePBreplace = "")
        {
            string FullUrl = "";
            if (Session["FullUrl"] != null)
                FullUrl = Session["FullUrl"].ToSafeString();
            try
            {
                #region Get IP Address Interface Log (Update 17.2)

                string transactionId = "";

                // Get IP Address
                string ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                transactionId = AISAIRNUMBER + ipAddress;

                #endregion

                List<ASSIGN_CONDITION_ATTR> assignConditionList = new List<ASSIGN_CONDITION_ATTR>
                {
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SERVICE_LEVEL", VALUE = SERVICE_LEVEL },
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "AREA_REGION", VALUE = AREA_REGION},
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "EVENT_RULE", VALUE = EVENT_RULE },
                    new ASSIGN_CONDITION_ATTR(){ ATTR_NAME = "SPECIAL_SKILL", VALUE = SPECIAL_SKILL },
                };
                ZTEReservedTimeslotQuery query = new ZTEReservedTimeslotQuery()
                {
                    ACCESS_MODE = ACCESS_MODE,
                    ADDRESS_ID = ADDRESS_ID,
                    APPOINTMENT_DATE = APPOINTMENT_DATE.Replace("/", "-"),
                    LOCATION_CODE = LOCATION_CODE,
                    PROD_SPEC_CODE = PROD_SPEC_CODE,
                    TIME_SLOT = TIME_SLOT,
                    SUBDISTRICT = SUBDISTRICT,
                    POSTAL_CODE = POSTAL_CODE,
                    ASSIGN_RULE = ASSIGN_RULE,
                    TRANSACTION_ID = transactionId,
                    SUB_ACCESS_MODE = SUB_ACCESS_MODE,
                    TASK_TYPE = TASK_TYPE,
                    FullUrl = FullUrl,
                    ASSIGN_CONDITION_LIST = assignConditionList,
                    AssignFlag = "A",
                    AisAirNumber = AISAIRNUMBER,
                    PlayBoxCountOld = PlayBoxCountOld,
                    PlayBoxCountNew = PlayBoxCountNew,
                    SymptonCodePBreplace = SymptonCodePBreplace
                };
                ZTEReservedTimeslotRespModel zte_result = _queryProcessor.Execute(query);

                return Json(zte_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ZTEReservedTimeslotRespModel() { RESULT = "-1", RESULT_DESC = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //20.2 WTTx
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult ReservedWTTx(string gridId, string incrementHours, string mobileNo)
        {
            try
            {
                int intIncrementHours = 0;
                try
                {
                    intIncrementHours = int.Parse(incrementHours);
                }
                catch
                {

                }
                DateTime expTime = DateTime.Now.AddHours(intIncrementHours);
                GetWTTXReserveQuery query = new GetWTTXReserveQuery()
                {
                    transaction_id = mobileNo,
                    gridId = gridId,
                    reservedExpTime = expTime.ToString("dd/MM/yyyy HH:mm:ss")
                };
                WTTXReserveModel wttx_result = _queryProcessor.Execute(query);

                return Json(wttx_result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new WTTXReserveModel() { RESULT_CODE = "-1", RESULT_MESSAGE = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IsFastJob(string AppointmentDate, string AccessMode, string RegisterFlag)
        {
            string showPopup = "FALSE";
            int LimitDay = 0;

            if (RegisterFlag == "Y")
            {
                var a = base.LovData.Where(p => p.Name == "IS_FAST_JOB_REGISTER" && p.Text == AccessMode);
                if (a != null)
                    if (a.Any())
                    {
                        LimitDay = int.Parse(a.FirstOrDefault().LovValue1);
                    }
                DateTime appointment_dt;
                if (DateTime.TryParseExact(AppointmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out appointment_dt))
                {
                    DateTime curr_dt = DateTime.Now.Date;
                    if (curr_dt.AddDays(LimitDay) == appointment_dt)
                        showPopup = "TRUE";
                }
            }
            else
            {
                var a = base.LovData.Where(p => p.Name == "IS_FAST_JOB" && p.Text == AccessMode);
                if (a != null)
                    if (a.Any())
                    {
                        LimitDay = int.Parse(a.FirstOrDefault().LovValue1);
                    }
                DateTime appointment_dt;
                if (DateTime.TryParseExact(AppointmentDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out appointment_dt))
                {
                    DateTime curr_dt = DateTime.Now.Date;
                    if (curr_dt.AddDays(LimitDay) > appointment_dt)
                        showPopup = "TRUE";
                }
            }

            return Json(
                    new { Popup = showPopup },
                    JsonRequestBehavior.AllowGet);
        }

        //R17.10 Cancel Order
        [HttpPost]
        public JsonResult GetCancelOrder(string templist, string idcard)
        {
            if (templist.ToSafeString() != "")
            {
                var cancellist = new JavaScriptSerializer().Deserialize<List<string>>(templist);
                var query = new GetCancelOrderQuery
                {
                    ID_Card_No = idcard,
                    ListOrder = cancellist
                };

                var result = _queryProcessor.Execute(query);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        //DOPA CheckCardByLaser
        [HttpPost]
        public JsonResult CheckCardByLaser(DataFromIDCardVerify dataVerify)
        {
            var birthdate = "";

            CheckCardByLaserMappingModel result;
            try
            {
                var lang = "";

                if (base.GetCurrentCulture().IsThaiCulture())
                {
                    lang = "T";

                    birthdate = dataVerify.birthyear +
                                string.Format("{0:00}", Convert.ToInt32(dataVerify.birthmonth)) +
                                string.Format("{0:00}", Convert.ToInt32(dataVerify.birthday));
                }
                else
                {
                    lang = "E";

                    birthdate = string.Format("{0:0000}", Convert.ToInt32(dataVerify.birthyear) + 543) +
                                string.Format("{0:00}", Convert.ToInt32(dataVerify.birthmonth)) +
                                string.Format("{0:00}", Convert.ToInt32(dataVerify.birthday));
                }

                var resultlov = base.LovData.Where(lov => lov.Type == "DOPA_CHK_IDCARD");

                var result_xssborigin = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-origin") ?? new LovValueModel();
                var result_xssbserviceorigin = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-service-origin") ?? new LovValueModel();
                var result_xssborderchannel = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-order-channel") ?? new LovValueModel();
                var result_xssbversion = resultlov.FirstOrDefault(lov => lov.Name == "x-ssb-version") ?? new LovValueModel();
                var result_Url = resultlov.FirstOrDefault(lov => lov.Name == "url") ?? new LovValueModel();
                var result_Method = resultlov.FirstOrDefault(lov => lov.Name == "command") ?? new LovValueModel();

                var uuid = GenerateRandomNo();
                var transaction = DateTime.Now.ToString("yyyyMMddHHmmfff");

                var result_xssbtransactionid = transaction + uuid;

                var query = new CheckCardByLaserQuery
                {
                    xssborigin = result_xssborigin.LovValue1,
                    xssbserviceorigin = result_xssbserviceorigin.LovValue1,
                    xssbtransactionid = result_xssbtransactionid,
                    xssborderchannel = result_xssborderchannel.LovValue1,
                    xssbversion = result_xssbversion.LovValue1,
                    Url = result_Url.LovValue1,
                    Method = result_Method.LovValue1,
                    Lang = lang,
                    Body = new CheckCardByLaserBody
                    {
                        firstName = dataVerify.firstname,
                        lastName = dataVerify.lastname,
                        idCardNo = dataVerify.idcardno,
                        laserID = dataVerify.idcardlaser,
                        birthday = birthdate
                    }
                };

                result = _queryProcessor.Execute(query);
            }
            catch (Exception)
            {
                result = new CheckCardByLaserMappingModel();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        //R19.4 Redcap
        public JsonResult GetPreSurveyTimeSlot(string Appointment_Date, string Pre_Survey_Date, bool isThai = true, string preSurveyTimeSlotId = "", bool smallSize = false)
        {
            var strBuilder = new StringBuilder();
            var AppointmentDate = new DateTime();
            var PreSurveyDate = new DateTime();
            var strFBSSTimeSlot = "";

            try
            {
                if (isThai)
                {
                    DateTimeFormatInfo thDtfi = new CultureInfo("th-TH", false).DateTimeFormat;
                    AppointmentDate = Convert.ToDateTime(Appointment_Date, thDtfi);
                    PreSurveyDate = Convert.ToDateTime(Pre_Survey_Date, thDtfi);
                }
                else
                {
                    DateTimeFormatInfo usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    AppointmentDate = Convert.ToDateTime(Appointment_Date, usDtfi);
                    PreSurveyDate = Convert.ToDateTime(Pre_Survey_Date, usDtfi);
                }

                //Create Date Range (Period)
                var data = CreateDataPreSurveyTimeSlot(AppointmentDate, PreSurveyDate);

                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        #region Time column
                        var listDay = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_WEEK_OF_DAY")).OrderBy(p => p.OrderBy).ToList();

                        var listMonth = base.LovData.Where(p => p.LovValue5 == WebConstants.LovConfigName.CustomerRegisterPageCode &&
                            p.Name.Contains("L_INSTALL_MONTH_OF_YEAR")).OrderBy(p => p.OrderBy).ToList();

                        var lblTime = isThai ? base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue1 : base.LovData.Where(p => p.Name == "L_INSTALL_TIME").FirstOrDefault().LovValue2;

                        var oldDate = data[0].PreSurveyDate;
                        int remarkTop = 0;

                        strBuilder.Append("<div style=\"float:left;\">");
                        strBuilder.Append("<div class=\"select-header-show\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div class=\"time-header\">");
                        strBuilder.Append("<label>" + lblTime + "</label> ");
                        strBuilder.Append("</div>");

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (oldDate == data[i].PreSurveyDate)
                            {
                                strBuilder.Append("<div class=\"time-item\">");
                                strBuilder.Append("<label>" + data[i].PreSurveyTimeSlot + "</label>");
                                strBuilder.Append("</div>");
                                remarkTop++;
                            }
                        }

                        strBuilder.Append("<div class=\"time-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");

                        #endregion

                        #region Day column

                        var tmp_limittime = AppointmentDate.Subtract(DateTime.Now).TotalDays - 1;

                        DateTime Curr_dt = DateTime.Now;

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (oldDate != data[i].PreSurveyDate || i == 0)
                            {
                                oldDate = data[i].PreSurveyDate;

                                if (i != 0)
                                {
                                    strBuilder.Append("<div class=\"day-footer-show\">");
                                    strBuilder.Append("<div class=\"day-footer\">");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                    strBuilder.Append("</div>");
                                }

                                var dayNum = data[i].PreSurveyDate.Value.Day.ToString();
                                var dayName = isThai ? listDay[(int)data[i].PreSurveyDate.Value.DayOfWeek].LovValue1 : listDay[(int)data[i].PreSurveyDate.Value.DayOfWeek].LovValue2;
                                var monthName = isThai ? listMonth[data[i].PreSurveyDate.Value.Month - 1].LovValue1 : listMonth[data[i].PreSurveyDate.Value.Month - 1].LovValue2;

                                strBuilder.Append("<div style=\"float:left;\">");
                                strBuilder.Append("<div class=\"select-header-show\">");
                                strBuilder.Append("<div class=\"select-header\">");
                                strBuilder.Append("<span style=\"color:white;\">" + dayName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"day-header\">");
                                strBuilder.Append("<div class=\"header-label\">");
                                strBuilder.Append("<span>" + dayName + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 5px;\"></p>");
                                strBuilder.Append("<span>" + dayNum + " " + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("<div class=\"header-label-hidden\">");
                                strBuilder.Append("<span>" + dayNum + "</span>");
                                strBuilder.Append("<p style=\"margin: 0 0 1px;\"></p>");
                                strBuilder.Append("<span>" + monthName + "</span>");
                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }

                            if (oldDate == data[i].PreSurveyDate)
                            {

                                strBuilder.Append("<div class=\"day-item\" onclick=\"onDayPreSurveyItemClick(this,'" + preSurveyTimeSlotId + "');\">");
                                strBuilder.Append("<div class=\"day-item-circle\">");

                                strBuilder.Append("<i class=\"fa fa-circle color-gray\"></i>");
                                // set format date
                                var day = data[i].PreSurveyDate.Value.ToString("dd");
                                var month = data[i].PreSurveyDate.Value.ToString("MM");
                                var year = data[i].PreSurveyDate.Value.ToString("yyyy");
                                var pre_survey_date = day + "/" + month + "/" + year;
                                var appointment_date = day + "/" + month + "/" + year;

                                strBuilder.Append("<div style=\"display:none;\">");
                                strBuilder.Append("<input type=\"text\" name=\"PreSurveyTimeSlotId\" value=\"" + data[i].PreSurveyTimeSlotId + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyDate\" value=\"" + pre_survey_date + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyTimeSlot\" value=\"" + data[i].PreSurveyTimeSlot + "\"/>");
                                strBuilder.Append("<input type=\"text\" name=\"AppointmentDate\" value=\"" + appointment_date + "\"/>");
                                strBuilder.Append("</div>");

                                strBuilder.Append("</div>");
                                strBuilder.Append("</div>");
                            }
                        }

                        strBuilder.Append("<div class=\"day-footer-show\">");
                        strBuilder.Append("<div class=\"day-footer\">");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("</div>");
                        strBuilder.Append("<div style=\"clear: both;\"></div>");
                        #endregion

                    }
                    else
                    {
                        strBuilder.Append("<div style=\"display:none;\"> List data = 0");
                        strBuilder.Append(", Pre_Survey_Date = " + Pre_Survey_Date);
                        strBuilder.Append("<input type=\"text\" name=\"PreSurveyTimeSlotId\" value=\"\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyDate\" value=\"" + PreSurveyDate.ToString("dd/MM/yyyy") + "\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                        strBuilder.Append("<input type=\"text\" name=\"AppointmentDate\" value=\"" + AppointmentDate.ToString("dd/MM/yyyy") + "\"/>");
                        strBuilder.Append("</div>");
                    }
                }
                else
                {
                    strBuilder.Append("<div style=\"display:none;\"> Data null");
                    strBuilder.Append(", Pre_Survey_Date = " + Pre_Survey_Date);
                    strBuilder.Append("<input type=\"text\" name=\"PreSurveyTimeSlotId\" value=\"\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyDate\" value=\"" + AppointmentDate.ToString("dd/MM/yyyy") + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                    strBuilder.Append("<input type=\"text\" name=\"AppointmentDate\" value=\"" + AppointmentDate.ToString("dd/MM/yyyy") + "\"/>");
                    strBuilder.Append("</div>");
                }


                return Json(new { preSurveyTimeSlotData = strBuilder.ToSafeString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                base.Logger.Info(ex.RenderExceptionMessage());
                strBuilder.Append("<div style=\"display:none;\"> Data error " + ex.ToString());
                strBuilder.Append(", Pre_Survey_Date = " + Pre_Survey_Date);
                strBuilder.Append("<input type=\"text\" name=\"PreSurveyTimeSlotId\" value=\"\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyDate\" value=\"" + AppointmentDate.ToString("yyyy/MM/dd") + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"FBSSPreSurveyTimeSlot\" value=\"" + strFBSSTimeSlot + "\"/>");
                strBuilder.Append("<input type=\"text\" name=\"AppointmentDate\" value=\"" + AppointmentDate.ToString("dd/MM/yyyy") + "\"/>");
                strBuilder.Append("</div>");

                return Json(new { preSurveyTimeSlotData = strBuilder.ToSafeString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<FBSSTimeSlotPreSurvey> CreateDataPreSurveyTimeSlot(DateTime AppointmentDate, DateTime PreSurveyDate)
        {
            try
            {
                var listTimeSlotPreSurvey = new List<FBSSTimeSlotPreSurvey>();

                DateTime DateNow = DateTime.Now;
                var tomorrowDate = DateNow.AddDays(1);
                var diffPreSurveyDate = Convert.ToInt16(Math.Round(AppointmentDate.Subtract(Convert.ToDateTime(tomorrowDate.ToString("MM/dd/yyyy"))).TotalDays));

                var defaultConfigPre = base.LovData.Where(item => item.Name == "PRE_SURVEY_TIMESLOT")
                                                    .Select(item => new { Timeslot = item.LovValue1 }).ToList();

                for (int i = 0; i < diffPreSurveyDate; i++)
                {
                    foreach (var j in defaultConfigPre)
                    {
                        var tmpTimeslot = new FBSSTimeSlotPreSurvey();

                        tmpTimeslot.PreSurveyDate = Convert.ToDateTime(tomorrowDate.AddDays(i).ToString("MM/dd/yyyy"));
                        tmpTimeslot.PreSurveyTimeSlot = j.Timeslot.ToSafeString();

                        listTimeSlotPreSurvey.Add(tmpTimeslot);
                    }
                }

                var periodPreSurveyDate = PreSurveyDate.AddDays(6);

                var result = listTimeSlotPreSurvey.Where(item => item.PreSurveyDate >= PreSurveyDate && item.PreSurveyDate <= periodPreSurveyDate)
                                                .Select(item =>
                                                new FBSSTimeSlotPreSurvey
                                                {
                                                    PreSurveyDate = item.PreSurveyDate,
                                                    PreSurveyTimeSlot = item.PreSurveyTimeSlot
                                                }).ToList();

                return result;
            }
            catch (Exception ex) { return new List<FBSSTimeSlotPreSurvey>(); }
        }

        public List<DropdownModel> GetConfigPreSurveyRecurringCharge(string lov_name)
        {
            return base.LovData.Where(l => l.Type == "SCREEN" && l.Name == lov_name)
                                .Select(l => new DropdownModel
                                {
                                    Text = l.Name,
                                    Value = l.LovValue1,
                                    Value2 = l.LovValue2,
                                    Value3 = l.LovValue3
                                    ,
                                    Value4 = l.LovValue4,
                                    Value5 = l.LovValue5,
                                    DefaultValue = l.DefaultValue
                                }).ToList();
        }

        private DateTime ToDtByAppointmenTimeSlot(DateTime? inputDate, string inputTime)
        {
            DateTime resultDate;
            try
            {
                var valDate = inputDate.ToDisplayText("dd/MM/yyyy");

                var valTime = "";
                var timeSplit = inputTime.Split('-');
                if (timeSplit.Any())
                {
                    valTime = timeSplit[0].ToSafeString().Trim();
                }
                var inputDateTimeStr = string.Format("{0} {1}", valDate, valTime);
                var usDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                resultDate = Convert.ToDateTime(inputDateTimeStr, usDtfi);
            }
            catch (Exception ex)
            {
                base.Logger.Info("ToTimeBegin : " + ex.Message);
                resultDate = new DateTime();
            }

            return resultDate;
        }

        public ReservePort3bbQueryModel ReservePort3bb(ReservePort3bbQuery query)
        {
            ReservePort3bbQueryModel reservePort3bbQueryModel = new ReservePort3bbQueryModel();
            try
            {
                reservePort3bbQueryModel = _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return reservePort3bbQueryModel;
        }
        #region IPCAMERA
        public JsonResult GetTimeSlotConfigSelectDateIpCamera(string technology, string subtype, string flowflag, string splitterflagFirst, string splitterflag,
        string aisAirNumber = "", string timeStamp = "", string subAccessMode = "", string timeSlotRegisterFlag = "", string timeSlotPBOldFlag = "")
        {
            if (timeSlotRegisterFlag == "Y")
            {
                // 19.8 Access Mode is PREMIUM
                if (subAccessMode == "PREMIUM")
                {
                    var ConfigTSPremium = from tm in base.LovData
                                          where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                          && tm.Name == subAccessMode
                                          && tm.Text.ToUpper() == technology.ToUpper()
                                          select tm;

                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                            && tm.Name == (string.IsNullOrEmpty(subtype) ? "Customer" : subtype)
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                    if (!ConfigTSbySubtype.Any())
                    {
                        ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE_HR"
                                            && tm.Name == "Default_Subtype"
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;
                    }

                    if (ConfigTSPremium != null && ConfigTSbySubtype.Any())
                    {
                        ConfigTSPremium.FirstOrDefault().LovValue5 = ConfigTSbySubtype.FirstOrDefault().LovValue5.ToSafeString();
                    }

                    if (ConfigTSPremium.Any())
                    {
                        base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                        base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                        base.Logger.Info("aisAirNumber : " + aisAirNumber);
                        base.Logger.Info("ASSIGN RULE : " + ConfigTSPremium.FirstOrDefault().LovValue5);
                        base.Logger.Info("subtype : " + subtype);

                        return Json(ConfigTSPremium.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                    }

                    base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + "");
                    base.Logger.Info("subtype : " + subtype);

                    return Json(ConfigTSPremium.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var typeTIME_SLOT = "TIME_SLOT_BY_SUBTYPE_HR";
                    if (timeSlotPBOldFlag == "N") typeTIME_SLOT = "TIME_SLOT_IPCamera";

                    // TODO: R19.7 Check Flag Register Flow
                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == typeTIME_SLOT
                                            && tm.Name == (string.IsNullOrEmpty(subtype) ? "Customer" : subtype)
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                    if (!ConfigTSbySubtype.Any())
                    {
                        ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == typeTIME_SLOT
                                            && tm.Name == "Default_Subtype"
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;

                        if (ConfigTSbySubtype.Any())
                        {
                            base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                            base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                            base.Logger.Info("aisAirNumber : " + aisAirNumber);
                            base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                            base.Logger.Info("subtype : " + subtype);

                            return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                        }
                    }

                    base.Logger.Info("timeSlotRegisterFlag : " + timeSlotRegisterFlag);
                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                    base.Logger.Info("subtype : " + subtype);

                    return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                string nameTimeslot = subAccessMode == "PREMIUM" ? "L_TIME_SLOT_PREMIUM" : "L_TIME_SLOT";

                //TODO: Splitter Management
                #region Splitter Management

                if (!string.IsNullOrEmpty(flowflag) && technology == "FTTH")
                {
                    string caseName = null;
                    if (!string.IsNullOrEmpty(splitterflagFirst) && string.IsNullOrEmpty(splitterflag))
                    {
                        //if ((splitterflagFirst != "1") && (splitterflagFirst != "2"))
                        //{
                        //    return Json(new LovValueModel(), JsonRequestBehavior.AllowGet);
                        //}

                        //caseName = splitterflagFirst == "2" ? "L_TIME_SLOT_CASE_2" : nameTimeslot ;


                    }
                    else
                    {
                        //if (splitterflag == "2")
                        //    caseName = "L_TIME_SLOT_CASE_2";
                    }

                    LovValueModel defaultConfigSm;
                    //if ((splitterflagFirst == "1" || splitterflagFirst == "3") && (splitterflag != "2") && (!string.IsNullOrEmpty(subtype)) && subAccessMode != "PREMIUM")
                    if ((splitterflagFirst == "1" || splitterflagFirst == "2" || splitterflag == "3") && (!string.IsNullOrEmpty(subtype)) && subAccessMode != "PREMIUM")
                    {
                        defaultConfigSm =
                            LovData.FirstOrDefault(
                                item =>
                                    item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                    item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }
                    else
                    {
                        defaultConfigSm =
                            LovData.FirstOrDefault(
                                item => item.Name == caseName && item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }

                    if (defaultConfigSm == null)
                    {
                        defaultConfigSm = LovData.FirstOrDefault(
                            item => item.Name == nameTimeslot && item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());
                    }

                    // R19.2 Premium Install - Set Assign Rule at LovValue5
                    //if ((nameTimeslot == "L_TIME_SLOT_PREMIUM") && (splitterflagFirst == "1" || splitterflagFirst == "3") && (splitterflag != "2") && (!string.IsNullOrEmpty(subtype)))
                    if ((nameTimeslot == "L_TIME_SLOT_PREMIUM") && (splitterflagFirst == "1" || splitterflag == "2" || splitterflagFirst == "3") && (!string.IsNullOrEmpty(subtype)))
                    {
                        var LovValue5forPremium =
                               LovData.FirstOrDefault(
                                   item => item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                           item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());

                        if (LovValue5forPremium != null)
                        {
                            defaultConfigSm.LovValue5 = LovValue5forPremium.LovValue5.ToSafeString();
                        }
                    }//----End R19.2---

                    base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                    base.Logger.Info("aisAirNumber : " + aisAirNumber);
                    base.Logger.Info("ASSIGN RULE : " + defaultConfigSm != null ? defaultConfigSm.LovValue5 : "");
                    base.Logger.Info("subtype : " + subtype);

                    return defaultConfigSm == null ? Json("", JsonRequestBehavior.AllowGet) : Json(defaultConfigSm, JsonRequestBehavior.AllowGet);
                }
                #endregion Splitter Management

                if (!string.IsNullOrEmpty(subtype) && subAccessMode != "PREMIUM")
                {
                    var ConfigTSbySubtype = from tm in base.LovData
                                            where tm.Type == "TIME_SLOT_BY_SUBTYPE"
                                            && tm.Name == subtype
                                            && tm.Text.ToUpper() == technology.ToUpper()
                                            select tm;
                    if (ConfigTSbySubtype != null)
                    {
                        if (ConfigTSbySubtype.Any())
                        {
                            base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                            base.Logger.Info("aisAirNumber : " + aisAirNumber);
                            base.Logger.Info("ASSIGN RULE : " + ConfigTSbySubtype.FirstOrDefault().LovValue5);
                            base.Logger.Info("subtype : " + subtype);
                            return Json(ConfigTSbySubtype.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                var DefaultConfigTS = from tm in base.LovData
                                          //where tm.Name == "L_TIME_SLOT"
                                      where tm.Name == nameTimeslot
                                      && tm.Text.ToUpper() == technology.ToUpper()
                                      select tm;

                // R19.2 Premium Install - Set Assign Rule at LovValue5
                if (nameTimeslot == "L_TIME_SLOT_PREMIUM" && (!string.IsNullOrEmpty(subtype)))
                {
                    var LovValue5forPremium =
                        LovData.FirstOrDefault(
                            item => item.Type == "TIME_SLOT_BY_SUBTYPE" && item.Name == subtype &&
                                    item.Text.ToSafeString().ToUpper() == technology.ToSafeString().ToUpper());

                    if (LovValue5forPremium != null)
                    {
                        DefaultConfigTS.FirstOrDefault().LovValue5 = LovValue5forPremium.LovValue5.ToSafeString();
                    }
                }//----End R19.2---

                if (DefaultConfigTS != null)
                {
                    if (DefaultConfigTS.Any())
                    {
                        return Json(DefaultConfigTS.FirstOrDefault(), JsonRequestBehavior.AllowGet);
                    }
                }

                base.Logger.Info("GetTimeSlotConfigSelectDate : " + timeStamp);
                base.Logger.Info("aisAirNumber : " + aisAirNumber);
                base.Logger.Info("ASSIGN RULE : " + "");
                base.Logger.Info("subtype : " + subtype);

            }

            return Json("", JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}
