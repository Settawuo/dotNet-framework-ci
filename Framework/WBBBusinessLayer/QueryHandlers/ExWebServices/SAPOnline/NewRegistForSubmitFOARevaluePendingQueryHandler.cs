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
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{
    public class NewRegistForSubmitFOARevaluePendingQueryHandler : IQueryHandler<NewRegistForSubmitFOARevaluePendingQuery, NewRegistForSubmitFOAResponse>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<InsertFoaInfoParmModel> _objFoaService;
        private readonly IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> _subContractor;
        //  private readonly IEntityRepository<FBSS_FOA_SYMPTOM> _symptom;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_REASON> _AssetReason;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_SYMPTOM> _symptom;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> _tran;
        public NewRegistForSubmitFOARevaluePendingQueryHandler(
            ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig,
            IEntityRepository<object> objService,
            IEntityRepository<InsertFoaInfoParmModel> objFoaService,
            IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> subContractor
            , IEntityRepository<FBSS_FIXED_ASSET_SYMPTOM> symptom
              , IEntityRepository<FBSS_FIXED_ASSET_REASON> AssetReason
              , IEntityRepository<FBSS_FIXED_ASSET_TRAN_LOG> tran
            )
        {
            _logger = logger;
            _intfLog = intfLog;
            _hisLog = hisLog;
            _uow = uow;
            _cfgLov = cfgLov;
            _fixAssConfig = fixAssConfig;
            _objService = objService;
            _objFoaService = objFoaService;
            _subContractor = subContractor;
            _symptom = symptom;
            _AssetReason = AssetReason;
            _tran = tran;
        }

        public NewRegistForSubmitFOAResponse Handle(NewRegistForSubmitFOARevaluePendingQuery model)
        {
            NewRegistForSubmitFOAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAResponse();
            InterfaceLogPayGCommand log = null;
            HistoryLogCommand hLog = new HistoryLogCommand();
            bool flagSap = true;
            bool flagSff = true;
            string SAPError = "";


            var resultLov = from item in _cfgLov.Get()
                            where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET"
                            select item;
            var urlEndpoint = resultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
            var userCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
            var passwordCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();


            var REVresultLov = from item in _cfgLov.Get()
                               where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET_REVALUE"
                               select item;
            var REVurlEndpoint = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
            var REVuserCredential = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
            var REVpasswordCredential = REVresultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();


            var MAresultLov = from item in _cfgLov.Get()
                              where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET_MAINTAIN"
                              select item;
            var MAurlEndpoint = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
            var MAuserCredential = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
            var MApasswordCredential = MAresultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();




            var resultSffLov = from item in _cfgLov.Get()
                               where item.LOV_TYPE == "CREDENTIAL_SFF_FIXED_ASSET"
                               select item;
            var urlSffEndpoint = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "URL");
            var userSffCredential = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
            var passwordSffCredential = resultSffLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();

            var resultFixAssConfig = from item in _fixAssConfig.Get()
                                     where item.PROGRAM_CODE == "P001" || item.PROGRAM_CODE == "P009" || item.PROGRAM_CODE == "P010"
                                     select item;
            var fixAssetExpense = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P001" && item.COM_CODE == "1800");
            var fixAssetCallSap = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P009");
            var fixAssetCallSff = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P010");

            var CheckRollBack = (from r in _cfgLov.Get()
                                 where r.LOV_NAME == "ROLLBACK" && r.LOV_TYPE == "FBBPAYG_CERTSAP"
                                 select r.ACTIVEFLAG).FirstOrDefault().ToSafeString();

            _logger.Info("Call NewRegistForSubmitFOARevaluePendingQueryHandler");

            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.ParameterName = "ret_msg";
            ret_msg.Size = 2000;
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Direction = ParameterDirection.Output;
            String cDOC_DATE = null;
            if (model.DOC_DATE != null && model.DOC_DATE != "")
            {
                try
                {
                    DateTime dtDOC_DATE = DateTime.ParseExact(model.DOC_DATE, "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
                    cDOC_DATE = dtDOC_DATE.ToString("yyyyMMdd");
                }
                catch
                {
                    DateTime dtDOC_DATE = DateTime.ParseExact(model.DOC_DATE, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    cDOC_DATE = dtDOC_DATE.ToString("yyyyMMdd");
                }
            }

            // update response from sap
            string result_code = "0";
            string result_msg = "";
            string main_access = "";
            string ACTION = model.ACTION;
            string INTERNET_NO = model.ACCESS_NUMBER;
            string MAIN_ASSET = model.MAIN_ASSET;
            string RUN_GROUP = model.RUN_GROUP;
            string SUBNUMBER = model.SUB_NUMBER;
            string TRANS_ID = model.TRANS_ID;
            string ASSET_VALUE_DATE = cDOC_DATE;
            string COM_CODE = model.COM_CODE;
            string DOC_DATE = cDOC_DATE;
            string ITEM_TEXT = model.ITEM_TEXT;
            string POST_DATE = cDOC_DATE;
            string REF_DOC_NO = getOrderNo(TRANS_ID);
            string NUMBER_MESSAGE = model.ERR_CODE;
            string ERROR_MESSAGE = model.ERR_MSG;
            hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "Resend Revalue");
            if (flagSap)
            {
                List<FBSS_SubmitFOARevalueResponse> listoutSAP = new List<FBSS_SubmitFOARevalueResponse>();

                ZFBSS_FIXEDASSET_ASSET_OUT_REVALUE[] OUT_REVALUE_ASSET = null;
                // InterfaceLogPayGCommand sap_log = null;
                // sap_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_in.ToString(), transactionID, "call SAP.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");
                //!* log
                try
                {
                    List<XDocument> listXDocument = new List<XDocument>();
                    listXDocument.Add(XDocument.Parse(TRANS_ID.DumpToXml()));
                    if (model != null)
                    {
                        listXDocument.Add(XDocument.Parse(model.DumpToXml()));
                    }



                    XDocument xml_master_in = null;
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
                    UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToSafeString(), hLog, "", "IN_SAP", null, "Resend Revalue");

                    if (fixAssetCallSap.COM_CODE == "Y")
                    {
                        if (REVuserCredential == null) return new NewRegistForSubmitFOAResponse() { result = "08", errorReason = "Endpoint" };
                        var _SAP = new FBBSAPOnlineRevalue.SI_Z_FIAM_FBSS_REVALUATION_ASSETService();
                        _SAP.UseDefaultCredentials = true;

                        if (REVurlEndpoint != null) _SAP.Url = REVurlEndpoint.DISPLAY_VAL;

                        if (CheckRollBack == "Y")
                            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                        _SAP.Credentials = new NetworkCredential(REVuserCredential.DISPLAY_VAL.ToSafeString(), REVpasswordCredential.DISPLAY_VAL.ToSafeString());

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

                        var xml_master_out = XDocument.Parse(OUT_REVALUE_ASSET.DumpToXml());


                        //    InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_out.ToString(), sap_log, "Success", "", "");

                        //!* log
                        UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToString(), hLog, "", "OUT_SAP", null, "Resend Revalue");

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
                                if (element.NUMBER_MESSAGE.ToSafeString() == "000")
                                {
                                    main_access = "success";
                                }
                                else
                                {
                                    main_access = "";
                                }

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
                                    p_ORDER_NO = model.ORDER_NO,
                                    ret_code = ret_code,
                                    ret_msg = ret_msg

                                }).ToList();

                                _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_code.Value.ToString());
                                _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_msg.Value.ToString());
                            }

                        }



                    }
                    else
                    {
                        //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                    }
                }
                catch (Exception ex)
                {
                    SAPError = ex.GetErrorMessage().ToSafeString();
                    // InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                    // InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", sap_log, "Fail", ex.GetErrorMessage().ToSafeString(), "");
                }
            }


            string msg = "";
            int indexSuccess = 0;
            int indexError = 0;

            NewRegistForSubmitFOAResponseResult.result = main_access;// result_code;
            NewRegistForSubmitFOAResponseResult.errorReason = result_msg;
            if (main_access != "")
                //if ((result_code.Equals("0") || result_code.Equals("000")) && result_msg.Trim().Equals("")) result_msg = "Success";
                //if (result_code == "0" || result_code == "000" || result_code == "") indexSuccess += 1;
                if (main_access != "") indexSuccess += 1;
                else indexError += 1;
            if (indexSuccess > 0 || indexError > 0)
            {
                msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
            }
            //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, msg, "");
            if (SAPError != "")
            {
                result_msg = SAPError;
            }
            //!* log
            UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);

            return NewRegistForSubmitFOAResponseResult;
        }
        public class GetORDER
        {
            public string ORDER_NO { get; set; }
        }
        public string getOrderNo(string tranID)
        {
            string _ord = string.Empty;


            var _hgroup = (from c in _tran.Get() select c);

            _hgroup = (from c in _hgroup where (c.TRANS_ID == tranID) select c);


            List<GetORDER> result = (from c in _hgroup
                                     select new GetORDER
                                     {
                                         ORDER_NO = c.ORDER_NO,

                                     }).ToList();
            if (result.Count > 0)
            {

                foreach (var dd in result)
                {

                    _ord = dd.ORDER_NO;
                }

            }



            return _ord;
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
            //uow.Persist();

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
