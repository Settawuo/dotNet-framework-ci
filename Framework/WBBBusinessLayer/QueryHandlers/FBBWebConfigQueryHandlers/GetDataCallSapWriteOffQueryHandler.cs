using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBBusinessLayer.FBBSAPOnlineMA2;
using WBBContract;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetDataCallSapWriteOffQueryHandler : IQueryHandler<GetDataCallSapWriteOffQuery, ReturnCallSapWriteOffModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<RespondGetDataForSapFOAWriteOff> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IWBBUnitOfWork _uow;
        HistoryLogCommand hLog = null;

        public GetDataCallSapWriteOffQueryHandler(ILogger logger, IEntityRepository<RespondGetDataForSapFOAWriteOff> objService
            , IEntityRepository<FBB_CFG_LOV> cfgLov
             , IEntityRepository<FBB_HISTORY_LOG> historyLog
            , IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig
             , IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog
            , IWBBUnitOfWork uow
            )
        {
            _logger = logger;
            _objService = objService;
            _cfgLov = cfgLov;
            _fixAssConfig = fixAssConfig;
            _historyLog = historyLog;
            _hisLog = hisLog;
            _uow = uow;
        }

        public ReturnCallSapWriteOffModel Handle(GetDataCallSapWriteOffQuery query)
        {
            ReturnCallSapWriteOffModel ReturnCallSapWriteOffModel = new ReturnCallSapWriteOffModel();
            ReturnCallSapWriteOffModel.ResSuccess = 0;
            ReturnCallSapWriteOffModel.ResError = 0;
            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                // ReturnCallSapWriteOffModel OUT_WriteOffModel = new ReturnCallSapWriteOffModel();
                // string SAPError = "";
                #region pamameter noteify
                string errMsg = "";
                string errCode = "000";



                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;
                cur.ParameterName = "cur";
                #endregion endpamameter noteify

                #region packageMappingObjectModel
                var packageMappingObjectModelWriteoffSap = new PackageMappingObjectModelWriteoff
                {
                    FBB_WRITE_OFF_LIST =
                        query.WriteOffQueryListModels.Select(
                            a => new FBB_WRITE_OFF_LISTMappingWriteoff
                            {
                                ACCESS_NUMBER = a.Access_No.ToSafeString(),
                                SN = a.SerialNumber.ToSafeString(),
                                MATERIAL_CODE = a.MaterialCode.ToSafeString(),
                                COMPANY_CODE = a.CompanyCode.ToSafeString(),
                                PLANT = a.Plant.ToSafeString(),
                                STORAGE_LOCATION = a.StorageLocation.ToSafeString(),
                                CREATE_BY = a.Create_by.ToSafeString()

                            }).ToArray()
                };
                var packageMappingWriteoff = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_writeoff_access_list", "FBB_WRITE_OFF_LIST", packageMappingObjectModelWriteoffSap);

                #endregion end packageMappingObjectModel

                //  #region Call Sap

                //if (ReturnCallSapWriteOffModel.p_ws_writeoff_cur.Count > 0)
                //{
                var fixAssetCallSap = (from item in _fixAssConfig.Get()
                                       where item.PROGRAM_NAME == "FLAG_CALL_SAP_MA"
                                       select item).FirstOrDefault();



                #region fixAssetCallSap.COM_CODE

                if (fixAssetCallSap.COM_CODE == "Y")
                {
                    #region Call get data from p_upsert_writeoff  and Check Data p_ws_writeoff_cur
                    try
                    {

                        ReturnCallSapWriteOffModel.p_ws_writeoff_cur = _objService.ExecuteReadStoredProc(
                               "WBB.PKG_FBB_Order_WriteOff.p_upsert_writeoff",
                               new
                               {
                                   packageMappingWriteoff,
                                   //return
                                   cur,
                                   ret_code,
                                   ret_msg

                               }).ToList();

                        ReturnCallSapWriteOffModel.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                        ReturnCallSapWriteOffModel.ret_msg = ret_msg.Value.ToString();

                    }
                    catch (Exception ex)
                    {
                        ReturnCallSapWriteOffModel.ret_code = "-1";
                        ReturnCallSapWriteOffModel.ResError = query.WriteOffQueryListModels.Count();
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "FBBConfig Write Off Foa";
                        historyLog.CREATED_BY = "FBBConfig Write Off Foa";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = "Error Call PK p_upsert_writeoff : " + ex.GetErrorMessage(); ;
                        historyLog.REF_KEY = "FBBConfig Write Off Foa";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                    }
                    if (ReturnCallSapWriteOffModel.p_ws_writeoff_cur.Count > 0)
                    {
                        //-------------------notetify sap

                        var MAresultLov = from item in _cfgLov.Get()
                                          where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET_MAINTAIN"
                                          select item;

                        var MAurlEndpoint = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
                        var MAuserCredential = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
                        var MApasswordCredential = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();
                        var _sapMA = new FBBSAPOnlineMA2.SI_Z_MMIM_FBSS_TRANS_POSTService();
                        _sapMA.UseDefaultCredentials = true;

                        if (MAurlEndpoint != null) _sapMA.Url = MAurlEndpoint.DISPLAY_VAL;
                        //-------------------end notetify sap


                        // ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                        //  _sapMA.Credentials = new NetworkCredential(MAuserCredential.DISPLAY_VAL.ToSafeString(), MApasswordCredential.DISPLAY_VAL.ToSafeString());

                        //ZFBSS_FIXEDASSET_ASSET_OUT[] OUT_CREATE_ASSET = null;
                        //ZFBSS_FIXEDASSET_INS_OUT[] OUT_INS_TRANS_POST = null;
                        //ZFBSS_FIXEDASSET_INV_OUT[] OUT_INV_TRANS_POST = null;
                        ZINPUT_INVENTORY_TRANS_STRUC[] INPUT_INVENTORY_TRANS = null;

                        #region  Loop Call Sap Services From Cur

                        foreach (var main in ReturnCallSapWriteOffModel.p_ws_writeoff_cur)
                        {
                            #region assign variable
                            List<RespondGetDataForSapFOAWriteOff> listMain = new List<RespondGetDataForSapFOAWriteOff>();
                            string DOC_DATE = string.Empty;
                            string POST_DATE = string.Empty;
                            string TRANS_ID = string.Empty;
                            string REF_DOC = string.Empty;
                            string RUN_GROUP = string.Empty;
                            string MOVEMENT_TYPE = string.Empty;
                            string MATERIAL_NO = string.Empty;
                            string PLANT_FROM = string.Empty;
                            string SLOC_FROM = string.Empty;
                            //add 09042019
                            string PLANT_TO = string.Empty;
                            string SLOC_TO = string.Empty;

                            string QUANTITY = string.Empty;
                            string UOM = string.Empty;
                            string AMOUNT = string.Empty;
                            string COST_CENTER = string.Empty;
                            string GL_ACCT = string.Empty;
                            string GOODS_RECIPIENT = string.Empty;
                            string SERIAL_NO = string.Empty;
                            string MATERIAL_DOC = string.Empty;
                            string DOC_YEAR = string.Empty;
                            string ITEM_TEXT = string.Empty;
                            string REF_DOC_FBSS = string.Empty;
                            string XREF1_HD = string.Empty;
                            //------------------------------------------------------
                            DOC_DATE = main.DOC_DATE;
                            POST_DATE = main.POST_DATE;
                            TRANS_ID = main.TRANS_ID;
                            REF_DOC = main.REF_DOC;
                            RUN_GROUP = main.RUN_GROUP;
                            MOVEMENT_TYPE = main.MOVEMENT_TYPE;
                            MATERIAL_NO = main.MATERIAL_NO;
                            PLANT_FROM = main.PLANT_FROM;
                            SLOC_FROM = main.SLOC_FROM;
                            //add 09042019
                            PLANT_TO = main.PLANT_TO;
                            SLOC_TO = main.SLOC_TO;

                            QUANTITY = main.QUANTITY;
                            UOM = main.UOM;
                            // AMOUNT=main.AMOUNT;
                            COST_CENTER = main.COST_CENTER;
                            GL_ACCT = main.GL_ACCT;
                            GOODS_RECIPIENT = main.GOODS_RECIPIENT;
                            SERIAL_NO = main.SERIAL_NO;
                            MATERIAL_DOC = main.MATERIAL_DOC;
                            DOC_YEAR = main.DOC_YEAR;
                            ITEM_TEXT = main.ITEM_TEXT;
                            REF_DOC_FBSS = main.REF_DOC_FBSS;
                            XREF1_HD = main.XREF1_HD;

                            listMain.Add(new RespondGetDataForSapFOAWriteOff
                            {
                                DOC_DATE = DOC_DATE.ToSafeString(),
                                POST_DATE = POST_DATE.ToSafeString(),
                                TRANS_ID = TRANS_ID.ToSafeString(),
                                REF_DOC = REF_DOC.ToSafeString(),
                                RUN_GROUP = RUN_GROUP.ToSafeString(),
                                MOVEMENT_TYPE = MOVEMENT_TYPE.ToSafeString(),
                                MATERIAL_NO = MATERIAL_NO.ToSafeString(),
                                PLANT_FROM = PLANT_FROM.ToSafeString(),
                                SLOC_FROM = SLOC_FROM.ToSafeString(),
                                //add 09042019
                                PLANT_TO = PLANT_TO.ToSafeString(),
                                SLOC_TO = SLOC_TO.ToSafeString(),

                                QUANTITY = QUANTITY.ToSafeString(),
                                UOM = UOM.ToSafeString(),

                                //AMOUNTSpecified = false,
                                COST_CENTER = COST_CENTER.ToSafeString(),
                                GL_ACCT = GL_ACCT.ToSafeString(),
                                GOODS_RECIPIENT = GOODS_RECIPIENT.ToSafeString(),
                                SERIAL_NO = SERIAL_NO.ToSafeString(),
                                MATERIAL_DOC = MATERIAL_DOC.ToSafeString(),
                                DOC_YEAR = DOC_YEAR.ToSafeString(),
                                ITEM_TEXT = ITEM_TEXT.ToSafeString(),
                                REF_DOC_FBSS = REF_DOC_FBSS.ToSafeString(),
                                XREF1_HD = XREF1_HD.ToSafeString()
                            }
                             );

                            INPUT_INVENTORY_TRANS = listMain.Select(ins => new ZINPUT_INVENTORY_TRANS_STRUC()
                            {
                                DOC_DATE = ins.DOC_DATE.ToSafeString(),
                                POST_DATE = ins.POST_DATE.ToSafeString(),
                                TRANS_ID = ins.TRANS_ID.ToSafeString(),
                                REF_DOC = ins.REF_DOC.ToSafeString(),
                                RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                                MOVEMENT_TYPE = ins.MOVEMENT_TYPE.ToSafeString(),
                                MATERIAL_NO = ins.MATERIAL_NO.ToSafeString(),
                                PLANT_FROM = ins.PLANT_FROM.ToSafeString(),
                                SLOC_FROM = ins.SLOC_FROM.ToSafeString(),
                                //add 09042019
                                PLANT_TO = ins.PLANT_TO.ToSafeString(),
                                SLOC_TO = ins.SLOC_TO.ToSafeString(),

                                QUANTITY = ins.QUANTITY.ToSafeString(),
                                UOM = ins.UOM.ToSafeString(),
                                // AMOUNT = Convert.ToDecimal(ins.AMOUNT),
                                AMOUNTSpecified = false,
                                COST_CENTER = ins.COST_CENTER.ToSafeString(),
                                GL_ACCT = ins.GL_ACCT.ToSafeString(),
                                GOODS_RECIPIENT = ins.GOODS_RECIPIENT.ToSafeString(),
                                SERIAL_NO = ins.SERIAL_NO.ToSafeString(),
                                MATERIAL_DOC = ins.MATERIAL_DOC.ToSafeString(),
                                DOC_YEAR = ins.DOC_YEAR.ToSafeString(),
                                ITEM_TEXT = ins.ITEM_TEXT.ToSafeString(),
                                REF_DOC_FBSS = ins.REF_DOC_FBSS.ToSafeString(),
                                XREF1_HD = ins.XREF1_HD.ToSafeString()
                            }).ToArray();

                            ZOUTPUT_TRANS_ERROR_STRUC[] OUTPUT_TRANS_ERROR = null;

                            #endregion End Assign variable
                            #region write his log sap
                            var INFOA_QueryInFoa = query.WriteOffQueryListModels.Where(x => x.Access_No == main.REF_DOC_FBSS && x.SerialNumber == main.SERIAL_NO).ToList().FirstOrDefault();
                            hLog = StartHistoryLog(_uow, _hisLog, INFOA_QueryInFoa, "", "IN_FOA", null, "FBB WRITE OFF");
                            UpdateHistoryLog(_uow, _hisLog, "", hLog, main.TRANS_ID, "INSTALLATION", null, null);

                            //-------------OUT_WriteOffModel
                            var OUT_WriteOffModel = ReturnCallSapWriteOffModel.p_ws_writeoff_cur.Where(x => x.REF_DOC_FBSS == main.REF_DOC_FBSS && x.SERIAL_NO == main.SERIAL_NO && x.TRANS_ID == main.TRANS_ID).ToList().FirstOrDefault();
                            UpdateHistoryLog(_uow, _hisLog, OUT_WriteOffModel, hLog, main.TRANS_ID, "OUT_FOA", ReturnCallSapWriteOffModel.ret_msg, null);
                            #endregion write his log sap
                            try
                            {
                                // in_sap
                                #region write his log in_sap
                                List<XDocument> listXDocument = new List<XDocument>();
                                if (INPUT_INVENTORY_TRANS != null && INPUT_INVENTORY_TRANS.Count() > 0)
                                {
                                    listXDocument.Add(XDocument.Parse(INPUT_INVENTORY_TRANS.DumpToXml()));
                                }



                                XDocument xml_master_in = new XDocument();
                                for (int i = 0; i < listXDocument.Count; i++)
                                {
                                    var XDoc = listXDocument.ElementAt(i);
                                    if (i == 0)
                                    {
                                        xml_master_in = XDoc;
                                    }
                                    else
                                    {
                                        xml_master_in.Root.Add(XDoc.Root.Elements());
                                    }
                                }
                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToSafeString(), hLog, "", "IN_SAP", null, null);

                                #endregion write his log in_sap

                                #region call Sap function
                                var CheckRollBack = (from r in _cfgLov.Get()
                                                     where r.LOV_NAME == "ROLLBACK" && r.LOV_TYPE == "FBBPAYG_CERTSAP"
                                                     select r.ACTIVEFLAG).FirstOrDefault().ToSafeString();

                                if (CheckRollBack == "Y")
                                    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                                _sapMA.Credentials = new NetworkCredential(MAuserCredential.DISPLAY_VAL.ToSafeString(), MApasswordCredential.DISPLAY_VAL.ToSafeString());
                                var OUTPUT_TRANS_COMPLETE = _sapMA.SI_Z_MMIM_FBSS_TRANS_POST(INPUT_INVENTORY_TRANS, out OUTPUT_TRANS_ERROR);

                                //var resultSapMa=  _sapMA.SI_Z_MMIM_FBSS_TRANS_POST(
                                //      ref DOC_DATE,
                                //      ref POST_DATE,

                                //      );
                                #endregion end call sap function
                                //-----------------Out Sap
                                #region Out Sap and Create xml

                                List<XDocument> listMADocument = new List<XDocument>();
                                if (OUTPUT_TRANS_ERROR != null && OUTPUT_TRANS_ERROR.Count() > 0)
                                {
                                    listMADocument.Add(XDocument.Parse(OUTPUT_TRANS_ERROR.DumpToXml()));
                                }
                                if (OUTPUT_TRANS_COMPLETE != null && OUTPUT_TRANS_COMPLETE.Count() > 0)
                                {
                                    listMADocument.Add(XDocument.Parse(OUTPUT_TRANS_COMPLETE.DumpToXml()));
                                }

                                XDocument xml_master_out = null;
                                if (listMADocument.Count > 0)
                                {


                                    for (int i = 0; i < listMADocument.Count; i++)
                                    {
                                        var XDoc = listMADocument.ElementAt(i);
                                        if (i == 0)
                                        {
                                            xml_master_out = XDoc;
                                        }
                                        else
                                        {
                                            xml_master_out.Root.Add(XDoc.Root.Elements());
                                        }
                                    }
                                    ReturnCallSapWriteOffModel.ResSuccess += 1;
                                }
                                else
                                {
                                    ReturnCallSapWriteOffModel.ResError += 1;
                                }
                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", null, null);
                                #endregion Out Sap and Create xml
                                // update response from sap
                                string result_code = "0";
                                string result_msg = "";
                                // string main_access = "";
                                string orderNO = main.ITEM_TEXT;
                                #region check error from output sap services


                                //--------------------Error-------------------------

                                if (OUTPUT_TRANS_ERROR.Length > 0)
                                {

                                    string ins_msg = string.Empty; string tranid = string.Empty;


                                    foreach (var errorelement in OUTPUT_TRANS_ERROR)
                                    {
                                        tranid = errorelement.TRANS_ID.ToSafeString();

                                        if (string.IsNullOrEmpty(errorelement.ERR_CODE.ToSafeString()))
                                        {
                                            errCode = "0000";
                                            errMsg = "Success";
                                            result_code = errCode;
                                            result_msg = errMsg;
                                        }
                                        else
                                        {
                                            errCode = errorelement.ERR_CODE.ToSafeString();
                                            errMsg = errorelement.ERR_MSG.ToSafeString();
                                            result_code = errCode;
                                            result_msg = errMsg;
                                        }
                                        UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToString(), hLog, "", "OUT_SAP", result_msg, null);

                                        var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_maintenance_response",
                                        new
                                        {
                                            p_TRANS_ID = errorelement.TRANS_ID.ToSafeString(),

                                            p_RUN_GROUP = errorelement.RUN_GROUP.ToSafeString(),

                                            p_INTERNET_NO = errorelement.REF_DOC_FBSS.ToSafeString(),
                                            p_ORDER_NO = orderNO.ToSafeString(),
                                            p_MATERIAL_NO = errorelement.MATERIAL_NO.ToSafeString(),
                                            p_SERIAL_NO = errorelement.SERIAL_NO.ToSafeString(),
                                            p_MATERIAL_DOC = "",
                                            p_DOC_YEAR = "",

                                            p_ERR_CODE = errCode,
                                            p_ERR_MSG = errMsg,

                                            ret_code = errCode,
                                            ret_msg = errMsg



                                        }).ToList();

                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_code.Value.ToString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_msg.Value.ToString());
                                        //errMsg = "";
                                        //errCode = "000";
                                        //  ReturnCallSapWriteOffModel.ResError += 1;
                                    }
                                    //   ReturnCallSapWriteOffModel.ResError += 1; 

                                }
                                #endregion check error from output sap services

                                //--------------------Success-------------------------
                                #region check Success from output sap services
                                if (OUTPUT_TRANS_COMPLETE.Length > 0)
                                {
                                    string ins_msg = string.Empty; string tranid = string.Empty;

                                    foreach (var element in OUTPUT_TRANS_COMPLETE)
                                    {
                                        tranid = element.TRANS_ID.ToSafeString();

                                        //return value
                                        //if (element.DOC_YEAR.ToSafeString() != "" && element.MATERIAL_DOC.ToSafeString() != "")
                                        //{
                                        //    main_access = "success";
                                        //}
                                        //else
                                        //{
                                        //    main_access = "";
                                        //}

                                        if (SAPResponseValid(errCode, errMsg) && result_msg == "")
                                        {
                                            result_code = errCode;
                                            result_msg = errMsg;

                                        }
                                        else
                                        {
                                            result_code = "0000";
                                            result_msg = "Success";
                                        }
                                        UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToString(), hLog, "", "OUT_SAP", "Success", null);
                                        //if (orderNO.ToSafeString() == "" || orderNO.ToSafeString() == null)
                                        //{
                                        //    orderNO = getOrderNo(tranid);
                                        //}
                                        var outp = new List<object>();
                                        var paramOut = outp.ToArray();
                                        var updateResponse = _objService.ExecuteStoredProc("WBB.pkg_fbb_foa_order_management.p_maintenance_response",
                                          out paramOut,
                                        new
                                        {
                                            p_TRANS_ID = element.TRANS_ID.ToSafeString(),

                                            p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),

                                            p_INTERNET_NO = element.REF_DOC_FBSS.ToSafeString(),
                                            p_ORDER_NO = orderNO.ToSafeString(),
                                            p_MATERIAL_NO = element.MATERIAL_NO.ToSafeString(),
                                            p_SERIAL_NO = element.SERIAL_NO.ToSafeString(),
                                            p_MATERIAL_DOC = element.MATERIAL_DOC.ToSafeString(),
                                            p_DOC_YEAR = element.DOC_YEAR.ToSafeString(),

                                            p_ERR_CODE = "0000",
                                            p_ERR_MSG = "Success",

                                            ret_code = "0000",
                                            ret_msg = "Success"

                                        });

                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_code.Value.ToString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_msg.Value.ToString());



                                    }

                                }

                                #endregion check Success from output sap services

                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", result_msg, null);

                            }
                            catch (Exception ex)
                            {
                                ReturnCallSapWriteOffModel.ret_code = "-1";
                                ReturnCallSapWriteOffModel.ResError += 1;
                                //ReturnCallSapWriteOffModel.ResError = ReturnCallSapWriteOffModel.p_ws_writeoff_cur.Count();
                                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                                historyLog.ACTION = ActionHistory.ADD.ToString();
                                historyLog.APPLICATION = "FBBConfig Write Off Foa";
                                historyLog.CREATED_BY = "FBBConfig Write Off Foa";
                                historyLog.CREATED_DATE = DateTime.Now;
                                historyLog.DESCRIPTION = ex.GetErrorMessage();
                                historyLog.REF_KEY = "FBBConfig Write Off Foa";
                                historyLog.REF_NAME = "NODEID";
                                _historyLog.Create(historyLog);
                                _uow.Persist();
                                _logger.Info("Error foreach call sap :" + ex.GetErrorMessage());
                            }
                        }
                        #endregion  End Loop Call Sap Services From Cur
                    }
                    else
                    {
                        ReturnCallSapWriteOffModel.ret_code = "-1";
                        ReturnCallSapWriteOffModel.ResError = query.WriteOffQueryListModels.Count();
                        historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                        historyLog.ACTION = ActionHistory.ADD.ToString();
                        historyLog.APPLICATION = "FBBConfig Write Off Foa";
                        historyLog.CREATED_BY = "FBBConfig Write Off Foa";
                        historyLog.CREATED_DATE = DateTime.Now;
                        historyLog.DESCRIPTION = "Data Not Found";
                        historyLog.REF_KEY = "FBBConfig Write Off Foa";
                        historyLog.REF_NAME = "NODEID";
                        _historyLog.Create(historyLog);
                        _uow.Persist();
                        _logger.Info("FBBConfig Write Off Foa Data Not Found");
                    }
                    #endregion Call get data from p_upsert_writeoff  and Check Data p_ws_writeoff_cur
                }
                else
                {
                    ReturnCallSapWriteOffModel.ret_code = "-1";
                    ReturnCallSapWriteOffModel.ResError = ReturnCallSapWriteOffModel.p_ws_writeoff_cur.Count();
                    ReturnCallSapWriteOffModel.ret_msg = "Write Off Foa Flag Sap Services Close.";

                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "FBBConfig Write Off Foa";
                    historyLog.CREATED_BY = "FBBConfig Write Off Foa";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = "Call Sap Services   fixAssetCallSap ComCode Not Equals Y";
                    historyLog.REF_KEY = "FBBConfig Write Off Foa";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                    _logger.Info("Call Sap Services   fixAssetCallSap ComCode Not Equals Y");
                }
                #endregion End iF fixAssetCallSap.COM_CODE

                // return 
                return ReturnCallSapWriteOffModel;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                //  query.ret_code = "-1";
                // query.ret_msg = "PKG_FBB_Order_WriteOff.p_upsert_writeoff Error : " + ex.Message;
                ReturnCallSapWriteOffModel.ret_code = "-1";
                ReturnCallSapWriteOffModel.ret_msg = "Write off Not Success.";
                ReturnCallSapWriteOffModel.ResError = query.WriteOffQueryListModels.Count();

                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "FBBConfig Write Off Foa";
                historyLog.CREATED_BY = "FBBConfig Write Off Foa";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = ex.GetErrorMessage();
                historyLog.REF_KEY = "FBBConfig Write Off Foa";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                return ReturnCallSapWriteOffModel;
            }
        }
        #region function check and create update his log
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
        public static bool SAPResponseValid(string code, string msg)
        {
            bool c = true;
            if (code.Equals("000") && msg.Equals("")) return false;
            return c;
        }

        #endregion function check and create update his log
    }


    public class PackageMappingObjectModelWriteoff : INullable, IOracleCustomType
    {
        [OracleArrayMapping()]
        public FBB_WRITE_OFF_LISTMappingWriteoff[] FBB_WRITE_OFF_LIST { get; set; }

        private bool objectIsNull;

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PackageMappingObjectModelWriteoff Null
        {
            get
            {
                var obj = new PackageMappingObjectModelWriteoff();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, 0, FBB_WRITE_OFF_LIST);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            FBB_WRITE_OFF_LIST = (FBB_WRITE_OFF_LISTMappingWriteoff[])OracleUdt.GetValue(con, udt, 0);
        }
    }


    [OracleCustomTypeMapping("FBB_WRITE_OFF_REC")]
    public class FBB_WRITE_OFF_LIST_MappingWriteoffOracleTypeMappingFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new FBB_WRITE_OFF_LISTMappingWriteoff();
        }
    }

    [OracleCustomTypeMapping("FBB_WRITE_OFF_LIST")]
    public class FBB_WRITE_OFF_LISTMappingWriteoffObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new PackageMappingObjectModelWriteoff();
        }

        #endregion IOracleCustomTypeFactory Members

        #region IOracleArrayTypeFactory Members

        public Array CreateArray(int numElems)
        {
            return new FBB_WRITE_OFF_LISTMappingWriteoff[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion IOracleArrayTypeFactory Members
    }

    public class FBB_WRITE_OFF_LISTMappingWriteoff : INullable, IOracleCustomType
    {
        private bool objectIsNull;

        #region Attribute Mapping

        [OracleObjectMapping("ACCESS_NUMBER")]
        public string ACCESS_NUMBER { get; set; }

        // [OracleObjectMapping("ORDER_TYPE")]
        //  public string ORDER_TYPE { get; set; }

        [OracleObjectMapping("SN")]
        public string SN { get; set; }

        [OracleObjectMapping("MATERIAL_CODE")]
        public string MATERIAL_CODE { get; set; }

        [OracleObjectMapping("COMPANY_CODE")]
        public string COMPANY_CODE { get; set; }

        [OracleObjectMapping("PLANT")]
        public string PLANT { get; set; }

        [OracleObjectMapping("STORAGE_LOCATION")]
        public string STORAGE_LOCATION { get; set; }

        // [OracleObjectMapping("SNPATTERN")]
        //public string SNPATTERN { get; set; }

        //[OracleObjectMapping("SN_STATUS")]
        //public string SN_STATUS { get; set; }

        [OracleObjectMapping("CREATE_BY")]
        public string CREATE_BY { get; set; }

        #endregion Attribute Mapping

        public static FBB_WRITE_OFF_LISTMappingWriteoff Null
        {
            get
            {
                var obj = new FBB_WRITE_OFF_LISTMappingWriteoff();
                obj.objectIsNull = true;
                return obj;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "ACCESS_NUMBER", ACCESS_NUMBER);
            // OracleUdt.SetValue(con, udt, "ORDER_TYPE", ORDER_TYPE);
            OracleUdt.SetValue(con, udt, "SN", SN);
            OracleUdt.SetValue(con, udt, "MATERIAL_CODE", MATERIAL_CODE);
            OracleUdt.SetValue(con, udt, "COMPANY_CODE", COMPANY_CODE);
            OracleUdt.SetValue(con, udt, "PLANT", PLANT);
            OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", STORAGE_LOCATION);
            //OracleUdt.SetValue(con, udt, "SNPATTERN", SNPATTERN);
            //OracleUdt.SetValue(con, udt, "SN_STATUS", SN_STATUS);
            OracleUdt.SetValue(con, udt, "CREATE_BY", CREATE_BY);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            throw new NotImplementedException();
        }
    }
}
