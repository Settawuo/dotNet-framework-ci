using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBBusinessLayer.FBBSAPOnlineRevalue;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class FBSSInvSendTerminateQueryHandler : IQueryHandler<FBSSInvSendTerminateQuery, FBSSInvSendTerminateReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IEntityRepository<FBSSInvSendTerminateModel> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        HistoryLogCommand hLog = null;

        public FBSSInvSendTerminateQueryHandler(
            ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBSSInvSendTerminateModel> objService,
           IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<FBB_HISTORY_LOG> historyLog,
           IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog
               )
        {
            _logger = logger;
            _uow = uow;
            _historyLog = historyLog;
            _hisLog = hisLog;
            _objService = objService;
            _cfgLov = cfgLov;
            _fixAssConfig = fixAssConfig;

        }
        public FBSSInvSendTerminateReturn Handle(FBSSInvSendTerminateQuery query)
        {

            HistoryLogCommand hLog = null;
            FBSSInvSendTerminateReturn FBSSInvSendTerminateResponseResult = new FBSSInvSendTerminateReturn();
            FBSSInvSendTerminateWriteLog WriteHisLog = new FBSSInvSendTerminateWriteLog();
            // FBSSInvSendTerminateReturn ResultLogOutFoa = new FBSSInvSendTerminateReturn();
            var historyLog = new FBB_HISTORY_LOG();
            var fixAssConfig = new FBSS_FIXED_ASSET_CONFIG();

            try
            {


                #region OracleParameter
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };
                var ret_msg = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Output
                };
                var p_ws_revalue_cur = new OracleParameter
                {
                    ParameterName = "p_ws_revalue_cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };
                #endregion

                var fixAssetCallSap = (from item in _fixAssConfig.Get()
                                       where item.PROGRAM_NAME == "FLAG_FBSS_TERMINATE"
                                       select item).FirstOrDefault();
                //----------query.p_term_date 

                #region Get Data Config Lov And Check Date Modify 

                if (fixAssetCallSap.MODIFY_DATETIME != null && fixAssetCallSap.MODIFY_DATETIME.ToSafeString() != "")
                {
                    #region  fixAssetCallSap.COM_CODE

                    if (fixAssetCallSap.COM_CODE == "Y")
                    {
                        //R19.03
                        #region Set Date Modify To parameter
                        var culture = CultureInfo.GetCultureInfo("en-US");
                        try
                        {

                            query.p_term_date = fixAssetCallSap.MODIFY_DATETIME.Value.ToString("dd/MM/yyyy HH:mm:ss", culture);
                        }
                        catch (Exception ex)
                        {
                            historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                            historyLog.ACTION = ActionHistory.ADD.ToString();
                            historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                            historyLog.CREATED_BY = "FBSSInvSendTerminate";
                            historyLog.CREATED_DATE = DateTime.Now;
                            historyLog.DESCRIPTION = "Error Config Datetime FbssFixedAssetConfig  DateModify " + ex.GetErrorMessage();
                            historyLog.REF_KEY = "FBSSInvSendTerminate";
                            historyLog.REF_NAME = "NODEID";
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                            return FBSSInvSendTerminateResponseResult;
                        }
                        #endregion
                        #region Get Data From FBSSInvSendTerminateResponseResult.p_ws_revalue_cur
                        FBSSInvSendTerminateResponseResult.p_ws_revalue_cur = _objService.ExecuteReadStoredProc(
                            "WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_terminatation",
                            new
                            {

                                query.p_term_date,
                                //---
                                p_ws_revalue_cur,
                                ret_code,
                                ret_msg
                            }).ToList();
                        FBSSInvSendTerminateResponseResult.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "1";
                        FBSSInvSendTerminateResponseResult.ret_msg = ret_msg.Value != null ? ret_msg.Value.ToString() : "";
                        _logger.Info("Call WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_terminatation: ret_code : " + ret_code + " ret_msg:" + ret_msg);

                        #endregion
                        //End R19.03
                        #region  Count p_ws_revalue_cur > 0

                        if (FBSSInvSendTerminateResponseResult.p_ws_revalue_cur.Count > 0)
                        {

                            var REVresultLov = from item in _cfgLov.Get()
                                               where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET_REVALUE"
                                               select item;
                            var REVurlEndpoint = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
                            var REVuserCredential = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
                            var REVpasswordCredential = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();

                            var _SAP = new FBBSAPOnlineRevalue.SI_Z_FIAM_FBSS_REVALUATION_ASSETService();
                            _SAP.UseDefaultCredentials = true;

                            try
                            {

                                if (REVurlEndpoint != null) _SAP.Url = REVurlEndpoint.DISPLAY_VAL;

                                var CheckRollBack = (from r in _cfgLov.Get()
                                                     where r.LOV_NAME == "ROLLBACK" && r.LOV_TYPE == "FBBPAYG_CERTSAP"
                                                     select r.ACTIVEFLAG).FirstOrDefault().ToSafeString();

                                if (CheckRollBack == "Y")
                                    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                                _SAP.Credentials = new NetworkCredential(REVuserCredential.DISPLAY_VAL.ToSafeString(), REVpasswordCredential.DISPLAY_VAL.ToSafeString());


                                foreach (var main in FBSSInvSendTerminateResponseResult.p_ws_revalue_cur)
                                {
                                    #region forFBSSInvSendTerminateResponseResult

                                    #region Notify variable
                                    string ACTION = string.Empty;
                                    string INTERNET_NO = string.Empty;
                                    string MAIN_ASSET = string.Empty;
                                    string RUN_GROUP = string.Empty;
                                    string SUBNUMBER = string.Empty;
                                    string TRANS_ID = string.Empty;
                                    string ASSET_VALUE_DATE = string.Empty;
                                    string COM_CODE = string.Empty;
                                    string DOC_DATE = string.Empty;
                                    string ITEM_TEXT = string.Empty;
                                    string POST_DATE = string.Empty;
                                    string REF_DOC_NO = string.Empty;
                                    string NUMBER_MESSAGE = string.Empty;
                                    string ERROR_MESSAGE = string.Empty;
                                    string result_code = string.Empty;
                                    string result_msg = string.Empty;
                                    string LogDesc = "";
                                    string SAPError = "";
                                    #endregion

                                    string transactionID = main.TRANS_ID != null ? main.TRANS_ID.ToSafeString() : "";



                                    //ResultLogOutFoa.ret_code = FBSSInvSendTerminateResponseResult.ret_code;
                                    //ResultLogOutFoa.ret_msg = FBSSInvSendTerminateResponseResult.ret_msg;
                                    //ResultLogOutFoa.p_ws_revalue_cur = FBSSInvSendTerminateResponseResult.p_ws_revalue_cur.Where(x => x.INTERNET_NO == main.INTERNET_NO && x.TRANS_ID == main.TRANS_ID && x.RUN_GROUP == main.RUN_GROUP && x.MAIN_ASSET == main.MAIN_ASSET).ToList();
                                    var result = FBSSInvSendTerminateResponseResult.p_ws_revalue_cur.Where(x => x.INTERNET_NO == main.INTERNET_NO && x.TRANS_ID == main.TRANS_ID && x.RUN_GROUP == main.RUN_GROUP && x.MAIN_ASSET == main.MAIN_ASSET).ToList().FirstOrDefault();

                                    #region setwritehislog
                                    WriteHisLog.Access_No = result.INTERNET_NO;
                                    WriteHisLog.Trans_id = result.TRANS_ID;
                                    WriteHisLog.Run_Group = result.RUN_GROUP;
                                    WriteHisLog.Action = result.ACTION;
                                    WriteHisLog.Main_Asset = result.MAIN_ASSET;
                                    WriteHisLog.SubNumber = result.SUBNUMBER;
                                    WriteHisLog.CompanyCode = result.COM_CODE;
                                    WriteHisLog.DocDate = result.DOC_DATE;
                                    WriteHisLog.PostDate = result.POST_DATE;
                                    WriteHisLog.AssetDate = result.ASSET_VALUE_DATE;
                                    WriteHisLog.OrderNumber = result.REF_DOC_NO;
                                    WriteHisLog.ItemText = result.ITEM_TEXT;
                                    #endregion

                                    hLog = StartHistoryLog(_uow, _hisLog, WriteHisLog, "", "IN_FOA", null, "Batch FBSSInvSendTerminate");
                                    UpdateHistoryLog(_uow, _hisLog, "", hLog, transactionID, "OUT_FOA", result_msg, null);

                                    #region Notify Object variable And Setting variable  Sap
                                    ZFBSS_FIXEDASSET_ASSET_OUT_REVALUE[] OUT_REVALUE_ASSET = null;
                                    ZFBSS_REVALUE_ASSET[] CREATE_ASSET_REVALUE = null;
                                    List<FBSS_SubmitFOARevalueResponse> listoutSAP = new List<FBSS_SubmitFOARevalueResponse>();
                                    List<FBSS_SubmitFOARevalueResponse> listMain = new List<FBSS_SubmitFOARevalueResponse>();

                                    XDocument listXDocument = new XDocument();
                                    ACTION = main.ACTION;
                                    INTERNET_NO = main.INTERNET_NO;
                                    MAIN_ASSET = main.MAIN_ASSET;
                                    RUN_GROUP = main.RUN_GROUP;
                                    SUBNUMBER = main.SUBNUMBER;
                                    TRANS_ID = main.TRANS_ID;
                                    ASSET_VALUE_DATE = main.ASSET_VALUE_DATE;
                                    COM_CODE = main.COM_CODE;
                                    DOC_DATE = main.DOC_DATE;
                                    ITEM_TEXT = main.ITEM_TEXT;
                                    POST_DATE = main.POST_DATE;
                                    REF_DOC_NO = main.REF_DOC_NO;

                                    LogDesc = " ACTION = " + ACTION;
                                    LogDesc += ",INTERNET_NO=" + INTERNET_NO;
                                    LogDesc += ",MAIN_ASSET=" + MAIN_ASSET;
                                    LogDesc += ",RUN_GROUP=" + RUN_GROUP;
                                    LogDesc += ",SUBNUMBER=" + SUBNUMBER;
                                    LogDesc += ",TRANS_ID=" + TRANS_ID;

                                    #endregion
                                    try
                                    {
                                        #region Create  CREATE_ASSET_REVALUE To Array for ZFBSS_REVALUE_ASSET   From listMain

                                        listMain.Add(new FBSS_SubmitFOARevalueResponse
                                        {
                                            TRANS_ID = TRANS_ID.ToSafeString(),
                                            ACTION = ACTION.ToSafeString(),
                                            RUN_GROUP = RUN_GROUP.ToSafeString(),
                                            INTERNET_NO = INTERNET_NO.ToSafeString(),
                                            MAIN_ASSET = MAIN_ASSET.ToSafeString(),
                                            SUBNUMBER = SUBNUMBER.ToSafeString(),


                                            ASSET_VALUE_DATE = ASSET_VALUE_DATE.ToSafeString(),
                                            COM_CODE = COM_CODE.ToSafeString(),
                                            DOC_DATE = DOC_DATE.ToSafeString(),

                                            ITEM_TEXT = ITEM_TEXT.ToSafeString(),
                                            POST_DATE = POST_DATE.ToSafeString(),
                                            REF_DOC_NO = REF_DOC_NO.ToSafeString(),
                                        }
                                     );


                                        if (listMain != null && listMain.Count > 0)
                                        {

                                            CREATE_ASSET_REVALUE = listMain.Select(ins => new ZFBSS_REVALUE_ASSET()
                                            {


                                                TRANS_ID = ins.TRANS_ID.ToSafeString(),
                                                ACTION = ins.ACTION.ToSafeString(),
                                                RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                                                INTERNET_NO = ins.INTERNET_NO.ToSafeString(),
                                                MAIN_ASSET = ins.MAIN_ASSET.ToSafeString(),
                                                SUBNUMBER = ins.SUBNUMBER.ToSafeString(),


                                                ASSET_VALUE_DATE = ins.ASSET_VALUE_DATE.ToSafeString(),
                                                COM_CODE = ins.COM_CODE.ToSafeString(),
                                                DOC_DATE = ins.DOC_DATE.ToSafeString(),

                                                ITEM_TEXT = ins.ITEM_TEXT.ToSafeString(),
                                                POST_DATE = ins.POST_DATE.ToSafeString(),
                                                REF_DOC_NO = ins.REF_DOC_NO.ToSafeString(),

                                            }).ToArray();
                                        }

                                        // -------------------IN  SAP 

                                        if (CREATE_ASSET_REVALUE != null && CREATE_ASSET_REVALUE.Count() > 0)
                                        {

                                            listXDocument = XDocument.Parse(CREATE_ASSET_REVALUE.DumpToXml());
                                        }
                                        #endregion
                                        UpdateHistoryRawLog(_uow, _hisLog, listXDocument.ToString(), hLog, "", "IN_SAP", null, null);
                                        //--------------------- END IN  FOA 
                                        UpdateHistoryLog(_uow, _hisLog, "", hLog, transactionID, "INSTALLATION", null, null);

                                        #region Call SapServicesRevalue And Check Output Sap

                                        var sapResult = _SAP.SI_Z_FIAM_FBSS_REVALUATION_ASSET(
                                              ref ACTION,
                                              ref INTERNET_NO,
                                              ref MAIN_ASSET,
                                              ref RUN_GROUP,
                                              ref SUBNUMBER,
                                              ref TRANS_ID,
                                               ASSET_VALUE_DATE,
                                               COM_CODE,
                                               DOC_DATE,
                                               ITEM_TEXT,
                                               POST_DATE,
                                               REF_DOC_NO,
                                              out NUMBER_MESSAGE

                                        );

                                        listoutSAP.Add(new FBSS_SubmitFOARevalueResponse { TRANS_ID = TRANS_ID, ACTION = ACTION, RUN_GROUP = RUN_GROUP, INTERNET_NO = INTERNET_NO, MAIN_ASSET = MAIN_ASSET, SUBNUMBER = SUBNUMBER, NUMBER_MESSAGE = NUMBER_MESSAGE, ERROR_MESSAGE = sapResult });

                                        if (listoutSAP != null && listoutSAP.Count > 0)
                                        {
                                            OUT_REVALUE_ASSET = listoutSAP.Select(ins => new ZFBSS_FIXEDASSET_ASSET_OUT_REVALUE()
                                            {


                                                TRANS_ID = ins.TRANS_ID.ToSafeString(),
                                                ACTION = ins.ACTION.ToSafeString(),
                                                RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                                                INTERNET_NO = ins.INTERNET_NO.ToSafeString(),
                                                ASSET_CODE = ins.MAIN_ASSET.ToSafeString(),
                                                SUBNUMBER = ins.SUBNUMBER.ToSafeString(),
                                                NUMBER_MESSAGE = ins.NUMBER_MESSAGE.ToSafeString(),
                                                MESSAGE = ins.ERROR_MESSAGE.ToSafeString(),

                                            }).ToArray();
                                        }


                                        if (OUT_REVALUE_ASSET.Length > 0)
                                        {

                                            string ins_msg = string.Empty;
                                            foreach (var element in OUT_REVALUE_ASSET)
                                            {
                                                if (SAPResponseValid(element.NUMBER_MESSAGE.ToSafeString(), element.NUMBER_MESSAGE.ToSafeString()) && result_msg == "")
                                                {
                                                    result_code = element.NUMBER_MESSAGE.ToSafeString();
                                                    result_msg = element.MESSAGE.ToSafeString();
                                                }

                                                XDocument xml_master_out = new XDocument();
                                                xml_master_out = XDocument.Parse(OUT_REVALUE_ASSET.DumpToXml());

                                                //OUT_SAP
                                                if (element.NUMBER_MESSAGE == "000")
                                                {
                                                    element.MESSAGE = "Success";
                                                }
                                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToString(), hLog, "", "OUT_SAP", element.MESSAGE, null);
                                                // UpdateHistoryLog(_uow, _hisLog, ResultLogOutFoa, hLog, transactionID, "OUT_FOA", element.MESSAGE, null);
                                                // UPDATE MESSAGE
                                                UpdateHistoryLog(_uow, _hisLog, "", hLog, transactionID, "INSTALLATION", element.MESSAGE, null);
                                                //--------------------- END INSTALLATION fro update  transactionID

                                                #region revalue_response
                                                var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_revalue_response",
                                                    new
                                                    {
                                                        p_TRANS_ID = element.TRANS_ID.ToSafeString(),
                                                        p_ACTION = element.ACTION.ToSafeString(),
                                                        p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),
                                                        p_INTERNET_NO = element.INTERNET_NO.ToSafeString(),
                                                        p_ASSET_CODE = element.ASSET_CODE.ToSafeString(),
                                                        p_SUBNUMBER = element.SUBNUMBER.ToSafeString(),
                                                        p_ERR_CODE = element.NUMBER_MESSAGE.ToSafeString(),
                                                        p_ERR_MSG = element.MESSAGE.ToSafeString(),
                                                        p_ORDER_NO = REF_DOC_NO,
                                                        ret_code = ret_code,
                                                        ret_msg = ret_msg

                                                    }).ToList();

                                                _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_code.Value.ToString());
                                                _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_msg.Value.ToString());
                                                #endregion
                                            } // end if check OUT_REVALUE_ASSET

                                        }
                                        #endregion

                                    }
                                    catch (Exception ex)
                                    {

                                        SAPError = ex.GetErrorMessage().ToSafeString();
                                        if (string.IsNullOrEmpty(SUBNUMBER) || string.IsNullOrEmpty(MAIN_ASSET))
                                        {
                                            SAPError += " Main Asset  is null and Subnumber";
                                        }

                                        UpdateHistoryLog(_uow, _hisLog, WriteHisLog, hLog, transactionID, "OUT_FOA", SAPError, null);
                                        try
                                        {


                                            var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_revalue_response",
                                                       new
                                                       {
                                                           p_TRANS_ID = TRANS_ID.ToSafeString(),
                                                           p_ACTION = ACTION.ToSafeString(),
                                                           p_RUN_GROUP = RUN_GROUP.ToSafeString(),
                                                           p_INTERNET_NO = INTERNET_NO.ToSafeString(),
                                                           p_ASSET_CODE = MAIN_ASSET,
                                                           p_SUBNUMBER = SUBNUMBER.ToSafeString(),
                                                           p_ERR_CODE = NUMBER_MESSAGE.ToSafeString(),
                                                           p_ERR_MSG = SAPError.ToSafeString(),
                                                           p_ORDER_NO = REF_DOC_NO,
                                                           ret_code = ret_code,
                                                           ret_msg = ret_msg

                                                       }).ToList();


                                        }
                                        catch (Exception exs)
                                        {
                                            _logger.Info("catch pkg_fbb_foa_order_management.p_revalue_response " + exs.GetErrorMessage());
                                        }

                                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                                        historyLog.ACTION = ActionHistory.ADD.ToString();
                                        historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                                        historyLog.CREATED_BY = "FBSSInvSendTerminate";
                                        historyLog.CREATED_DATE = DateTime.Now;
                                        historyLog.DESCRIPTION = SAPError + LogDesc;
                                        historyLog.REF_KEY = "FBSSInvSendTerminate";
                                        historyLog.REF_NAME = "NODEID";
                                        _historyLog.Create(historyLog);
                                        _uow.Persist();
                                        _logger.Info("Error FBSSInvSendTerminate :" + SAPError);
                                    }

                                    #endregion

                                    TimeSpan.FromSeconds(5);
                                } // end foreach p_ws_revalue_cur


                                // 
                            }
                            catch (Exception ex)
                            {


                                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                                historyLog.ACTION = ActionHistory.ADD.ToString();
                                historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                                historyLog.CREATED_BY = "FBSSInvSendTerminate";
                                historyLog.CREATED_DATE = DateTime.Now;
                                historyLog.DESCRIPTION = ex.GetErrorMessage().ToSafeString();
                                historyLog.REF_KEY = "FBSSInvSendTerminate";
                                historyLog.REF_NAME = "NODEID";
                                _historyLog.Create(historyLog);
                                _uow.Persist();
                                _logger.Info("Error Call Sap Services :" + ex.GetErrorMessage().ToSafeString());
                                // InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                                // InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", sap_log, "Fail", ex.GetErrorMessage().ToSafeString(), "");
                            }

                        }
                        else
                        {
                            historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                            historyLog.ACTION = ActionHistory.ADD.ToString();
                            historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                            historyLog.CREATED_BY = "FBSSInvSendTerminate";
                            historyLog.CREATED_DATE = DateTime.Now;
                            historyLog.DESCRIPTION = "PKG_FBB_FOA_ORDER_MANAGEMENT p_get_terminatation  Data Not Found";
                            historyLog.REF_KEY = "FBSSInvSendTerminate";
                            historyLog.REF_NAME = "NODEID";
                            _historyLog.Create(historyLog);
                            _uow.Persist();
                            _logger.Info("  FBSSInvSendTerminateQueryHandler call PKG_FBB_FOA_ORDER_MANAGEMENT p_get_terminatation   Data Not Found");
                        }
                        //end if check Data > from  PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_terminatation
                        #endregion
                        return FBSSInvSendTerminateResponseResult;


                    }
                    else
                    {
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                        historyLog.CREATED_BY = "FBSSInvSendTerminate";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = "Config FbssFixedAssetConfig   COM_CODE Not Equals Y";
                        historyLog.REF_KEY = "FBSSInvSendTerminate";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                        _logger.Info("Error fixAssetCallSap.COM_CODE Not Equals Y");

                    }
                    // end if fixAssetCallSap.COM_CODE == "Y" 
                    return FBSSInvSendTerminateResponseResult;
                    #endregion
                }
                else
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                    historyLog.CREATED_BY = "FBSSInvSendTerminate";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = "Error Config Datetime FbssFixedAssetConfig  DateModify Data is Null";
                    historyLog.REF_KEY = "FBSSInvSendTerminate";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                    return FBSSInvSendTerminateResponseResult;

                }
                #endregion

            }
            catch (Exception ex)
            {

                _logger.Info("Error FBSSInvSendTerminateQueryHandler call PKG_FBB_FOA_ORDER_MANAGEMENT p_get_terminatation handles : " + ex.GetErrorMessage());
                FBSSInvSendTerminateResponseResult.ret_code = "1";
                FBSSInvSendTerminateResponseResult.ret_msg = "PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_terminatation Error : " + ex.GetErrorMessage();

                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Batch FBSSInvSendTerminate";
                historyLog.CREATED_BY = "FBSSInvSendTerminate";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Error FBSSInvSendTerminateQueryHandler " + ex.GetErrorMessage();
                historyLog.REF_KEY = "FBSSInvSendTerminate";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
                return FBSSInvSendTerminateResponseResult;
            }
        }
        public static bool SAPResponseValid(string code, string msg)
        {
            bool c = true;
            if (code.Equals("000") && msg.Equals("")) return false;
            return c;
        }
        public static HistoryLogCommand StartHistoryLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog, T query,
         string transactionId, string actionName, string msg, string createBy)
        {
            var dbIntfCmd = new HistoryLogCommand
            {
                ActionBy = actionName,
                TRANSACTION_ID = transactionId,
                IN_FOA = query.DumpToXml(),
                INSTALLATION = query.DumpToXml(),
                IN_SAP = query.DumpToXml(),
                OUT_SAP = query.DumpToXml(),
                OUT_FOA = query.DumpToXml(),
                REQUEST_STATUS = msg,
                CREATED_BY = createBy,
            };

            var log = HistoryLogHelper.Log(uow, historyLog, dbIntfCmd);
            // uow.Persist();

            dbIntfCmd.HISTORY_ID = log.HISTORY_ID;
            return dbIntfCmd;
        }

        public static void UpdateHistoryLog<T>(IWBBUnitOfWork uow, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog, T query,
          HistoryLogCommand dbIntfCmd, string transactionId, string actionName, string msg, string createBy)
        {
            dbIntfCmd.ActionBy = actionName;
            dbIntfCmd.TRANSACTION_ID = transactionId;
            dbIntfCmd.IN_FOA = query.DumpToXml();
            dbIntfCmd.INSTALLATION = query.DumpToXml();
            dbIntfCmd.IN_SAP = query.DumpToXml();
            dbIntfCmd.OUT_SAP = query.DumpToXml();
            dbIntfCmd.OUT_FOA = query.DumpToXml();
            dbIntfCmd.REQUEST_STATUS = msg;
            HistoryLogHelper.Log(uow, historyLog, dbIntfCmd);
            //uow.Persist();
        }

        public static void UpdateHistoryRawLog(IWBBUnitOfWork uow, IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> historyLog, string query,
          HistoryLogCommand dbIntfCmd, string transactionId, string actionName, string msg, string createBy)
        {
            dbIntfCmd.ActionBy = actionName;
            dbIntfCmd.TRANSACTION_ID = transactionId;
            dbIntfCmd.IN_FOA = query;
            dbIntfCmd.INSTALLATION = query;
            dbIntfCmd.IN_SAP = query;
            dbIntfCmd.OUT_SAP = query;
            dbIntfCmd.OUT_FOA = query;
            dbIntfCmd.REQUEST_STATUS = msg;
            HistoryLogHelper.Log(uow, historyLog, dbIntfCmd);
            //uow.Persist();
        }


    }
}
