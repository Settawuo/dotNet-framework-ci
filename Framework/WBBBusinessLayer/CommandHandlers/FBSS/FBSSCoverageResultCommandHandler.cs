using LinqKit;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBSS
{
    public class FBSSCoverageResultCommandHandler : ICommandHandler<FBSSCoverageResultCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageResult;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public FBSSCoverageResultCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageResult,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _coverageResult = coverageResult;
            _objService = objService;
            _intfLog = intfLog;
        }

        public void Handle(FBSSCoverageResultCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                _logger.Info("Start FBSSCoverageResultCommandHandler");

                if (command.ActionType == ActionType.Insert)
                {
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.TRANSACTION_ID, "FBSSCoverageResult", "FBSSCoverageResult Insert", "", "FBB", "WEB");
                    _logger.Info("Inserting Coverage Result.");

                    #region Insert FBB_COVERAGEAREA_RESULT

                    var data = new FBB_FBSS_COVERAGEAREA_RESULT();
                    data.ADDRRESS_TYPE = command.ADDRRESS_TYPE;
                    data.POSTAL_CODE = command.POSTAL_CODE;
                    data.SUB_DISTRICT_NAME = command.SUB_DISTRICT_NAME;
                    data.LANGUAGE = command.LANGUAGE;
                    data.BUILDING_NAME = command.BUILDING_NAME;
                    data.BUILDING_NO = command.BUILDING_NO;
                    data.PHONE_FLAG = command.PHONE_FLAG;
                    data.FLOOR_NO = command.FLOOR_NO;
                    data.ADDRESS_NO = command.ADDRESS_NO;
                    data.MOO = command.MOO;
                    data.SOI = command.SOI;
                    data.ROAD = command.ROAD;
                    data.LATITUDE = command.LATITUDE;
                    data.LONGITUDE = command.LONGITUDE;
                    data.COVERAGE = command.COVERAGE;
                    data.ADDRESS_ID = command.ADDRESS_ID;
                    data.ACCESS_MODE_LIST = command.ACCESS_MODE_LIST;
                    data.PLANNING_SITE_LIST = command.PLANNING_SITE_LIST;
                    data.IS_PARTNER = command.IS_PARTNER;
                    data.PARTNER_NAME = command.PARTNER_NAME;
                    data.PRODUCTTYPE = command.PRODUCTTYPE;
                    data.ZIPCODE_ROWID = command.ZIPCODE_ROWID;
                    data.OWNER_PRODUCT = command.OWNER_PRODUCT;
                    data.TRANSACTION_ID = command.TRANSACTION_ID;
                    data.CREATED_BY = string.IsNullOrEmpty(command.ActionBy) ? "CUSTOMER" : command.ActionBy;
                    data.CREATED_DATE = DateTime.Now;
                    data.LOCATION_CODE = command.LOCATION_CODE;
                    data.ASC_CODE = command.ASC_CODE;
                    data.EMPLOYEE_ID = command.EMPLOYEE_ID;
                    data.SALE_NAME = command.SALE_FIRSTNAME + " " + command.SALE_LASTNAME;
                    data.LOCATION_NAME = command.LOCATION_NAME;
                    data.SUB_REGION = command.SUB_REGION;
                    data.REGION_NAME = command.REGION;
                    data.ASC_NAME = command.ASC_NAME;
                    data.CHANNEL_NAME = command.CHANNEL_NAME;
                    data.SALE_CHANNEL = command.SALE_CHANNEL;
                    //R21.2
                    data.ADDRESS_TYPE_DTL = command.ADDRESS_TYPE_DTL;
                    data.REMARK = command.REMARK;
                    data.TECHNOLOGY = command.TECHNOLOGY;
                    data.PROJECTNAME = command.PROJECTNAME;

                    //onservice special
                    data.COVERAGE_AREA = command.COVERAGEAREA;
                    data.COVERAGE_STATUS = command.STATUS;
                    data.COVERAGE_SUBSTATUS = command.SUBSTATUS;
                    data.COVERAGE_CONTACTEMAIL = command.CONTACTEMAIL;
                    data.COVERAGE_CONTACTTEL = command.CONTACTTEL;
                    data.COVERAGE_GROUPOWNER = command.GROUPOWNER;
                    data.COVERAGE_CONTACTNAME = command.CONTACTNAME;
                    data.COVERAGE_NETWORKPROVIDER = command.NETWORKPROVIDER;
                    data.COVERAGE_FTTHDISPLAYMESSAGE = command.FTTHDISPLAYMESSAGE;
                    data.COVERAGE_WTTXDISPLAYMESSAGE = command.WTTXDISPLAYMESSAGE;

                    _coverageResult.Create(data);
                    _uow.Persist();
                    command.RESULTID = data.RESULTID;

                    #endregion Insert FBB_COVERAGEAREA_RESULT

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "Success", log, "Success", "", "");
                    _logger.Info("End FBSSCoverageResultCommandHandler");
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    #region Update
                    DateTime tmpCreatedDate = DateTime.Now;
                    tmpCreatedDate = tmpCreatedDate.AddDays(-1);
                    _logger.Info("Update FBSS Coverage Result.");

                    var predicate = PredicateBuilder.True<FBB_FBSS_COVERAGEAREA_RESULT>().And(t => t.CREATED_DATE >= tmpCreatedDate);

                    if (command.RESULTID > 0)
                    {
                        predicate = predicate.And(t => t.RESULTID == command.RESULTID);
                    }

                    if (!string.IsNullOrEmpty(command.TRANSACTION_ID))
                    {
                        predicate = predicate.And(t => !string.IsNullOrEmpty(t.TRANSACTION_ID)
                            && t.TRANSACTION_ID == command.TRANSACTION_ID);
                    }

                    var exCoverageResult = _coverageResult.Get(predicate).FirstOrDefault();
                    if (null == exCoverageResult)
                    {
                        _logger.Info("Cannot update FBB_FBSS_COVERAGEAREA_RESULT with result id = " + command.RESULTID);
                        return;
                    }
                    else
                    {
                        if (command.LOCATION_CODE.ToSafeString() == "")
                            command.LOCATION_CODE = exCoverageResult.LOCATION_CODE;
                        if (command.ASC_CODE.ToSafeString() == "")
                            command.ASC_CODE = exCoverageResult.ASC_CODE;
                        if (command.EMPLOYEE_ID.ToSafeString() == "")
                            command.EMPLOYEE_ID = exCoverageResult.EMPLOYEE_ID;
                        if (command.SALE_FIRSTNAME.ToSafeString() == "")
                            command.SALE_FIRSTNAME = exCoverageResult.SALE_NAME;
                        if (command.SALE_LASTNAME.ToSafeString() == "")
                            command.SALE_LASTNAME = "";
                        if (command.LOCATION_NAME.ToSafeString() == "")
                            command.LOCATION_NAME = exCoverageResult.LOCATION_NAME;
                        if (command.SUB_REGION.ToSafeString() == "")
                            command.SUB_REGION = exCoverageResult.SUB_REGION;
                        if (command.REGION.ToSafeString() == "")
                            command.REGION = exCoverageResult.REGION_NAME;
                        if (command.ASC_NAME.ToSafeString() == "")
                            command.ASC_NAME = exCoverageResult.ASC_NAME;
                        if (command.CHANNEL_NAME.ToSafeString() == "")
                            command.CHANNEL_NAME = exCoverageResult.CHANNEL_NAME;
                        if (command.SALE_CHANNEL.ToSafeString() == "")
                            command.SALE_CHANNEL = exCoverageResult.SALE_CHANNEL;
                        if (command.ADDRESS_TYPE_DTL.ToSafeString() == "")
                            command.ADDRESS_TYPE_DTL = exCoverageResult.ADDRESS_TYPE_DTL;
                        if (command.REMARK.ToSafeString() == "")
                            command.REMARK = exCoverageResult.REMARK;
                        if (command.TECHNOLOGY.ToSafeString() == "")
                            command.TECHNOLOGY = exCoverageResult.TECHNOLOGY;
                        if (command.PROJECTNAME.ToSafeString() == "")
                            command.PROJECTNAME = exCoverageResult.PROJECTNAME;                        
                    }

                    if (command.ADDRRESS_TYPE.ToSafeString() == "ESRI")
                    {
                        // R19.2PP Update Check Coverage By Package (ESRI)
                        log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.TRANSACTION_ID, "FBSSCoverageResult", "FBSSCoverageResult Update By Map", "", "FBB", "WEB");

                        try
                        {
                            var ret_code = new OracleParameter();
                            ret_code.OracleDbType = OracleDbType.Varchar2;
                            ret_code.Size = 2000;
                            ret_code.Direction = ParameterDirection.Output;

                            var ret_msg = new OracleParameter();
                            ret_msg.OracleDbType = OracleDbType.Varchar2;
                            ret_msg.Size = 2000;
                            ret_msg.Direction = ParameterDirection.Output;

                            var outp = new List<object>();
                            var paramOut = outp.ToArray();

                            _logger.Info("Start PROC_UPD_ADDR_CHECK_COVERAGE");

                            _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_UPD_ADDR_CHECK_COVERAGE",
                            out paramOut,
                              new
                              {
                                  p_transaction_id = command.TRANSACTION_ID.ToSafeString(),
                                  p_building_name = command.BUILDING_NAME.ToSafeString(),
                                  p_partner_name = command.PARTNER_NAME.ToSafeString(),
                                  p_owner_product = command.OWNER_PRODUCT.ToSafeString(),
                                  p_product_type = command.PRODUCTTYPE.ToSafeString(),
                                  p_created_by = string.IsNullOrEmpty(command.ActionBy.ToSafeString()) ? "FBBMOB" : command.ActionBy.ToSafeString(),
                                  p_updated_by = "From Process Register ESRI",
                                  p_address_id = command.ADDRESS_ID.ToSafeString(),
                                  p_address_no = command.ADDRESS_NO.ToSafeString(),
                                  p_moo = command.MOO.ToSafeString(),
                                  p_soi = command.SOI.ToSafeString(),
                                  p_road = command.ROAD.ToSafeString(),
                                  p_location_code = command.LOCATION_CODE.ToSafeString(),
                                  p_asc_code = command.ASC_CODE.ToSafeString(),
                                  p_employee_id = command.EMPLOYEE_ID.ToSafeString(),
                                  p_sale_firstname = command.SALE_FIRSTNAME.ToSafeString(),
                                  p_sale_lastname = command.SALE_LASTNAME.ToSafeString(),
                                  p_location_name = command.LOCATION_NAME.ToSafeString(),
                                  p_sub_region = command.SUB_REGION.ToSafeString(),
                                  p_region = command.REGION.ToSafeString(),
                                  p_asc_name = command.ASC_NAME.ToSafeString(),
                                  p_channel_name = command.CHANNEL_NAME.ToSafeString(),
                                  p_sale_channel = command.SALE_CHANNEL.ToSafeString(),
                                  //R21.2
                                  p_address_type_dtl = command.ADDRESS_TYPE_DTL.ToSafeString(),
                                  p_remark = command.REMARK.ToSafeString(),
                                  p_technology = command.TECHNOLOGY.ToSafeString(),
                                  p_projectname = command.PROJECTNAME.ToSafeString(),

                                  // out param
                                  ret_code = ret_code,
                                  ret_msg = ret_msg
                              });

                            if (ret_code.Value.ToSafeString() == "0")
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                            else
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ret_msg.Value.ToSafeString(), "");

                            _logger.Info("End PROC_UPD_ADDR_CHECK_COVERAGE");

                        }
                        catch (Exception ex)
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                            _logger.Info("Error PROC_UPD_ADDR_CHECK_COVERAGE");
                            _logger.Info(ex.GetErrorMessage() + "Inner: " + ex.InnerException);
                            _logger.Info(ex.StackTrace);
                        }

                        //-----------Comment Old Code----------------
                        //exCoverageResult.PARTNER_NAME = command.PARTNER_NAME;
                        //exCoverageResult.OWNER_PRODUCT = command.OWNER_PRODUCT;
                        //exCoverageResult.PRODUCTTYPE = command.PRODUCTTYPE;
                        //exCoverageResult.UPDATED_BY = "From Process Register ESRI";
                        //exCoverageResult.ADDRESS_ID = command.ADDRESS_ID;
                        //exCoverageResult.UPDATED_DATE = DateTime.Now;
                        //exCoverageResult.CREATED_BY = string.IsNullOrEmpty(command.ActionBy) ? "FBBMOB" : command.ActionBy;

                        //exCoverageResult.ADDRESS_NO = command.ADDRESS_NO;
                        //exCoverageResult.MOO = command.MOO;
                        //exCoverageResult.SOI = command.SOI;
                        //exCoverageResult.ROAD = command.ROAD;
                        //if (command.COVERAGE == "N")
                        //    exCoverageResult.BUILDING_NAME = command.BUILDING_NAME;

                        //_coverageResult.Update(exCoverageResult);
                        //_uow.Persist();
                    }
                    else
                    {
                        // R19.2 Update Check Coverage By Package
                        log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.RESULTID.ToSafeString(), "FBSSCoverageResult", "FBSSCoverageResult Update By Package", "", "FBB", "WEB");

                        try
                        {
                            var ret_code = new OracleParameter();
                            ret_code.OracleDbType = OracleDbType.Varchar2;
                            ret_code.Size = 2000;
                            ret_code.Direction = ParameterDirection.Output;

                            var ret_msg = new OracleParameter();
                            ret_msg.OracleDbType = OracleDbType.Varchar2;
                            ret_msg.Size = 2000;
                            ret_msg.Direction = ParameterDirection.Output;

                            var outp = new List<object>();
                            var paramOut = outp.ToArray();

                            _logger.Info("Start PROC_UPD_PROFILE_OUT_COVERAGE");

                            _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_UPD_PROFILE_OUT_COVERAGE",
                            out paramOut,
                              new
                              {
                                  p_result_id = command.RESULTID.ToSafeString(),
                                  p_created_by = command.ADDRESS_ID.ToSafeString(),
                                  p_prefix_name = command.PREFIXNAME.ToSafeString(),
                                  p_first_name = command.FIRSTNAME.ToSafeString(),
                                  p_last_name = command.LASTNAME.ToSafeString(),
                                  p_contact_number = command.CONTACTNUMBER.ToSafeString(),
                                  p_contact_email = command.EMAIL.ToSafeString(),
                                  p_contact_line_id = command.LINEID.ToSafeString(),
                                  p_return_code = command.RETURN_CODE.ToSafeString(),
                                  p_return_message = command.RETURN_MESSAGE.ToSafeString(),
                                  p_retuen_order = command.RETURN_ORDER.ToSafeString(),
                                  p_location_code = command.LOCATION_CODE.ToSafeString(),
                                  p_asc_code = command.ASC_CODE.ToSafeString(),
                                  p_employee_id = command.EMPLOYEE_ID.ToSafeString(),
                                  p_sale_firstname = command.SALE_FIRSTNAME.ToSafeString(),
                                  p_sale_lastname = command.SALE_LASTNAME.ToSafeString(),
                                  p_location_name = command.LOCATION_NAME.ToSafeString(),
                                  p_sub_region = command.SUB_REGION.ToSafeString(),
                                  p_region = command.REGION.ToSafeString(),
                                  p_asc_name = command.ASC_NAME.ToSafeString(),
                                  p_channel_name = command.CHANNEL_NAME.ToSafeString(),
                                  p_sale_channel = command.SALE_CHANNEL.ToSafeString(),
                                  //R21.2
                                  p_address_type_dtl = command.ADDRESS_TYPE_DTL.ToSafeString(),
                                  p_remark = command.REMARK.ToSafeString(),
                                  p_technology = command.TECHNOLOGY.ToSafeString(),
                                  p_projectname = command.PROJECTNAME.ToSafeString(),
                                  //onservice special
                                  //p_coveragearea = command.COVERAGEAREA.ToSafeString(),
                                  //p_status = command.STATUS.ToSafeString(),
                                  //p_substatus = command.SUBSTATUS.ToSafeString(),
                                  //p_contactemail = command.CONTACTEMAIL.ToSafeString(),
                                  //p_contacttel = command.CONTACTTEL.ToSafeString(),
                                  //p_groupowner = command.GROUPOWNER.ToSafeString(),
                                  //p_contactname = command.CONTACTNAME.ToSafeString(),
                                  //p_networkprovider = command.NETWORKPROVIDER.ToSafeString(),
                                  //p_ftthdisplaymessage = command.FTTHDISPLAYMESSAGE.ToSafeString(),
                                  //p_wttxdisplaymessage = command.WTTXDISPLAYMESSAGE.ToSafeString(),

                                  // out param
                                  ret_code = ret_code,
                                  ret_msg = ret_msg
                              });

                            if (ret_code.Value.ToSafeString() == "0")
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                            else
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ret_msg.Value.ToSafeString(), "");

                            _logger.Info("End PROC_UPD_PROFILE_OUT_COVERAGE");

                        }
                        catch (Exception ex)
                        {
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                            _logger.Info("Error PROC_UPD_PROFILE_OUT_COVERAGE");
                            _logger.Info(ex.GetErrorMessage() + "Inner: " + ex.InnerException);
                            _logger.Info(ex.StackTrace);
                        }

                        //-----------Comment Old Code----------------
                        //if (command.ADDRESS_ID == "FTTR_SELL_ROUTER")
                        //{
                        //    exCoverageResult.CREATED_BY = command.ADDRESS_ID;
                        //}

                        //exCoverageResult.PREFIXNAME = command.PREFIXNAME;
                        //exCoverageResult.FIRSTNAME = command.FIRSTNAME;
                        //exCoverageResult.LASTNAME = command.LASTNAME;
                        //exCoverageResult.CONTACTNUMBER = command.CONTACTNUMBER;
                        //exCoverageResult.CONTACT_EMAIL = command.EMAIL;
                        //exCoverageResult.CONTACT_LINE_ID = command.LINEID;

                        //exCoverageResult.RETURN_CODE = command.RETURN_CODE;
                        //exCoverageResult.RETURN_MESSAGE = command.RETURN_MESSAGE;
                        //exCoverageResult.RETURN_ORDER = command.RETURN_ORDER;
                        //--------End Comment Old Code---------------
                    }


                    //_coverageResult.Update(exCoverageResult);
                    //_uow.Persist();
                    #endregion Update

                    _logger.Info("End FBSSCoverageResultCommandHandler");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle FBSSCoverageResultCommandHandler");
                _logger.Info(ex.GetErrorMessage() + "Inner: " + ex.InnerException);
                _logger.Info(ex.StackTrace);
            }
        }
    }
}
