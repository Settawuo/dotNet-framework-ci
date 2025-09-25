using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
using WBBBusinessLayer.SAPFixedAssetService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{
    public class NewRegistForSubmitFOAMainAssetHandler : IQueryHandler<NewRegistForSubmitFOAMainAssetQuery, NewRegistForSubmitFOAMainAssetModel>
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
        public NewRegistForSubmitFOAMainAssetHandler(
            ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG_PAYG> intfLog,
            IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> hisLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> cfgLov,
            IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig,
            IEntityRepository<object> objService,
            IEntityRepository<InsertFoaInfoParmModel> objFoaService,
            IEntityRepository<FBBPAYG_WFM_SUBCONTRACTOR> subContractor
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
        }
        public NewRegistForSubmitFOAMainAssetModel Handle(NewRegistForSubmitFOAMainAssetQuery model)
        {
            InterfaceLogPayGCommand log = null;
            HistoryLogCommand hLog = null;
            bool flagSap = true;
            bool flagSff = true;
            NewRegistForSubmitFOAMainAssetModel NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAMainAssetModel();
            PkgFbbFoaOrderManagementResponse executeResults = new PkgFbbFoaOrderManagementResponse();
            //string StoreName = "";
            try
            {
                var resultLov = from item in _cfgLov.Get()
                                where item.LOV_TYPE == "CREDENTIAL_SAP_FIXED_ASSET"
                                select item;
                var urlEndpoint = resultLov.FirstOrDefault(item => item.LOV_NAME == "URL");
                var userCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "USER") ?? new FBB_CFG_LOV();
                var passwordCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "PASSWORD") ?? new FBB_CFG_LOV();

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

                _logger.Info("Call NewRegistForSubmitFOAMainAssetHandler");


                log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package.", "NewRegistForSubmitFOAMainAsset", "", "Fixed Asset", "Exs.");
                hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB");



                //Parameter Input

                var p_ORDER_NO = new OracleParameter();
                p_ORDER_NO.ParameterName = "p_ORDER_NO";
                p_ORDER_NO.Size = 2000;
                p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_NO.Direction = ParameterDirection.Input;
                p_ORDER_NO.Value = model.p_ORDER_NO;

                var p_INTERNET_NO = new OracleParameter();
                p_INTERNET_NO.ParameterName = "p_INTERNET_NO";
                p_INTERNET_NO.Size = 2000;
                p_INTERNET_NO.OracleDbType = OracleDbType.Varchar2;
                p_INTERNET_NO.Direction = ParameterDirection.Input;
                p_INTERNET_NO.Value = model.p_INTERNET_NO;

                var p_COM_CODE_OLD = new OracleParameter();
                p_COM_CODE_OLD.ParameterName = "p_COM_CODE_OLD";
                p_COM_CODE_OLD.Size = 2000;
                p_COM_CODE_OLD.OracleDbType = OracleDbType.Varchar2;
                p_COM_CODE_OLD.Direction = ParameterDirection.Input;
                p_COM_CODE_OLD.Value = model.p_COM_CODE_OLD;

                var p_COM_CODE_NEW = new OracleParameter();
                p_COM_CODE_NEW.ParameterName = "p_COM_CODE_NEW";
                p_COM_CODE_NEW.Size = 2000;
                p_COM_CODE_NEW.OracleDbType = OracleDbType.Varchar2;
                p_COM_CODE_NEW.Direction = ParameterDirection.Input;
                p_COM_CODE_NEW.Value = model.p_COM_CODE_NEW;

                var p_ASSET_CLASS = new OracleParameter();
                p_ASSET_CLASS.ParameterName = "p_ASSET_CLASS";
                p_ASSET_CLASS.Size = 2000;
                p_ASSET_CLASS.OracleDbType = OracleDbType.Varchar2;
                p_ASSET_CLASS.Direction = ParameterDirection.Input;
                p_ASSET_CLASS.Value = model.p_ASSET_CLASS;

                var p_COSTCENTER = new OracleParameter();
                p_COSTCENTER.ParameterName = "p_COSTCENTER";
                p_COSTCENTER.Size = 2000;
                p_COSTCENTER.OracleDbType = OracleDbType.Varchar2;
                p_COSTCENTER.Direction = ParameterDirection.Input;
                p_COSTCENTER.Value = model.p_COSTCENTER;

                var p_PRODUCT = new OracleParameter();
                p_PRODUCT.ParameterName = "p_PRODUCT";
                p_PRODUCT.Size = 2000;
                p_PRODUCT.OracleDbType = OracleDbType.Varchar2;
                p_PRODUCT.Direction = ParameterDirection.Input;
                p_PRODUCT.Value = model.p_PRODUCT;

                var p_USER_CODE = new OracleParameter();
                p_USER_CODE.ParameterName = "p_USER_CODE";
                p_USER_CODE.Size = 2000;
                p_USER_CODE.OracleDbType = OracleDbType.Varchar2;
                p_USER_CODE.Direction = ParameterDirection.Input;
                p_USER_CODE.Value = model.p_USER_CODE;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var p_ws_main_cur = new OracleParameter();
                p_ws_main_cur.ParameterName = "p_ws_main_cur";
                p_ws_main_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_main_cur.Direction = ParameterDirection.Output;

                var p_ws_inv_cur = new OracleParameter();
                p_ws_inv_cur.ParameterName = "p_ws_inv_cur";
                p_ws_inv_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_inv_cur.Direction = ParameterDirection.Output;

                var p_ws_ins_cur = new OracleParameter();
                p_ws_ins_cur.ParameterName = "p_ws_ins_cur";
                p_ws_ins_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_ins_cur.Direction = ParameterDirection.Output;

                var p_ws_sff_cur = new OracleParameter();
                p_ws_sff_cur.ParameterName = "p_ws_sff_cur";
                p_ws_sff_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_sff_cur.Direction = ParameterDirection.Output;

                _logger.Info("Start pkg_fbb_foa_order_management " + executeResults.ret_msg);
                //  T-T
                //string _Momentype = "";
                // foreach (var a in model.ProductList)
                // { _Momentype = a.MovementType.ToSafeString(); }
                //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallIN");


                //var result = _objService.ExecuteStoredProcMultipleCursor(StoreName,
                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_resend_main_asset",
                      new object[]
                      {
                          //Parameter Input
                          p_ORDER_NO,
                          p_INTERNET_NO,
                          p_COM_CODE_OLD,
                          p_COM_CODE_NEW,
                          p_ASSET_CLASS,
                          p_COSTCENTER,
                          p_PRODUCT,
                          p_USER_CODE,

                          p_ws_main_cur,
                          p_ws_inv_cur,
                          p_ws_ins_cur,
                          p_ws_sff_cur,
                          ret_code
                      });

                if (result != null)
                {
                    executeResults.ret_code = result[4] != null ? result[4].ToString() : "-1";
                    executeResults.ret_msg = result[5] != null ? result[5].ToString() : "";

                    NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToString();
                    NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToString();

                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, "", "");

                    if (!executeResults.ret_code.Equals("0"))
                    {
                        UpdateHistoryLog(_uow, _hisLog, result, hLog, "", "ERROR", "ReturnCode:" + executeResults.ret_code + " ReturnMSG:" + executeResults.ret_msg, null);
                        return NewRegistForSubmitFOAResponseResult;
                    }

                    DataTable dtSubmitFOAMainRespones = (DataTable)result[0];
                    List<FBSS_SubmitFOAMainRespones> ListSubmitFOARespones = dtSubmitFOAMainRespones.DataTableToList<FBSS_SubmitFOAMainRespones>();
                    executeResults.p_ws_main_cur = ListSubmitFOARespones;

                    DataTable dtSubmitFOAInvRespones = (DataTable)result[1];
                    List<FBSS_SubmitFOAInvRespones> ListSubmitFOAInvRespones = dtSubmitFOAInvRespones.DataTableToList<FBSS_SubmitFOAInvRespones>();
                    executeResults.p_ws_inv_cur = ListSubmitFOAInvRespones;

                    DataTable dtSubmitFOAInsRespones = (DataTable)result[2];
                    List<FBSS_SubmitFOAInsRespones> ListSubmitFOAInsRespones = dtSubmitFOAInsRespones.DataTableToList<FBSS_SubmitFOAInsRespones>();
                    executeResults.p_ws_ins_cur = ListSubmitFOAInsRespones;

                    DataTable dtFBSS_SFFRespones = (DataTable)result[3];
                    List<FBSS_SFFRespones> ListSFFRespones = dtFBSS_SFFRespones.DataTableToList<FBSS_SFFRespones>();
                    executeResults.p_ws_sff_cur = ListSFFRespones;

                    if (executeResults.p_ws_main_cur.Count > 0)
                    {
                        int countRowMain = 0;
                        foreach (var item in executeResults.p_ws_main_cur)
                        {
                            if (item.TRANS_ID == null || item.TRANS_ID == "")
                            {
                                countRowMain++;
                            }
                        }
                        if (countRowMain.Equals(executeResults.p_ws_main_cur.Count))
                        {
                            executeResults.p_ws_main_cur = new List<FBSS_SubmitFOAMainRespones>();
                        }
                    }

                    if (executeResults.p_ws_inv_cur.Count > 0)
                    {
                        int countRowInv = 0;
                        foreach (var item in executeResults.p_ws_inv_cur)
                        {
                            if (item.TRANS_ID == null || item.TRANS_ID == "")
                            {
                                countRowInv++;
                            }
                        }

                        if (countRowInv.Equals(executeResults.p_ws_inv_cur.Count))
                        {
                            executeResults.p_ws_inv_cur = new List<FBSS_SubmitFOAInvRespones>();
                        }
                    }
                    if (executeResults.p_ws_ins_cur.Count > 0)
                    {
                        int countRowIns = 0;
                        foreach (var item in executeResults.p_ws_ins_cur)
                        {
                            if (item.TRANS_ID == null || item.TRANS_ID == "")
                            {
                                countRowIns++;
                            }
                        }
                        if (countRowIns.Equals(executeResults.p_ws_ins_cur.Count))
                        {

                            executeResults.p_ws_ins_cur = new List<FBSS_SubmitFOAInsRespones>();
                        }
                    }
                    if (executeResults.p_ws_main_cur.Count <= 0 && executeResults.p_ws_inv_cur.Count <= 0 && executeResults.p_ws_ins_cur.Count <= 0)
                    {
                        flagSap = false;
                    }

                    if (executeResults.p_ws_sff_cur.Count > 0)
                    {
                        int countRowSff = 0;
                        foreach (var item in executeResults.p_ws_sff_cur)
                        {
                            if (item.ACCESS == null || item.ACCESS == "")
                            {
                                countRowSff++;
                            }
                        }
                        if (countRowSff.Equals(executeResults.p_ws_sff_cur.Count))
                        {
                            executeResults.p_ws_sff_cur = new List<FBSS_SFFRespones>(); ;
                        }
                    }

                    if (executeResults.p_ws_sff_cur.Count <= 0)
                    {
                        flagSff = false;
                    }
                    _logger.Info("End pkg_fbb_foa_order_management " + executeResults.ret_msg);

                }
                else
                {
                    NewRegistForSubmitFOAResponseResult.result = "-1";//"Package Data Return Null!";
                    UpdateHistoryLog(_uow, _hisLog, "", hLog, "", "Data Null", "Package Data Return Null!", null);
                    return NewRegistForSubmitFOAResponseResult;
                }

                FBSS_SubmitFOAMainRespones mainRespone = new FBSS_SubmitFOAMainRespones();
                FBSS_SubmitFOAInsRespones insRespone = new FBSS_SubmitFOAInsRespones();
                FBSS_SubmitFOAInvRespones invRespone = new FBSS_SubmitFOAInvRespones();

                if (executeResults.p_ws_main_cur.Count > 0)
                {
                    mainRespone = executeResults.p_ws_main_cur.ElementAt(0);
                }
                if (executeResults.p_ws_ins_cur.Count > 0)
                {
                    insRespone = executeResults.p_ws_ins_cur.ElementAt(0);
                }
                if (executeResults.p_ws_inv_cur.Count > 0)
                {
                    invRespone = executeResults.p_ws_inv_cur.ElementAt(0);
                }

                string transactionID = mainRespone.TRANS_ID != null ? mainRespone.TRANS_ID.ToSafeString() : insRespone.TRANS_ID != null ? insRespone.TRANS_ID.ToSafeString()
                    : invRespone.TRANS_ID != null ? invRespone.TRANS_ID.ToSafeString() : "";

                string orderNO = null;
                if (invRespone.REF_DOC_NO != null && invRespone.REF_DOC_NO == "COMPLETE")
                {
                    orderNO = insRespone.REF_DOC_NO.ToSafeString();
                }
                else if (invRespone.REF_DOC_NO != null)
                {
                    orderNO = invRespone.REF_DOC_NO.ToSafeString();
                }
                else
                {
                    orderNO = insRespone.REF_DOC_NO.ToSafeString();
                }
                string accessNO = mainRespone.INTERNET_NO != null ? mainRespone.INTERNET_NO.ToSafeString() : invRespone.INTERNET_NO != null ? invRespone.INTERNET_NO.ToSafeString()
                    : insRespone.INTERNET_NO.ToSafeString();

                //!* log
                UpdateHistoryLog(_uow, _hisLog, executeResults, hLog, transactionID, "INSTALLATION", null, null);

                //var User = "PIAPPLUSER_FBB";
                //var password = "FBB@1234";
                if (model.ORDER_TYPE.ToUpper().Equals("RENEW"))
                {
                    UpdateHistoryLog(_uow, _hisLog, result, hLog, "", "SUCCESS", null, null);
                    return NewRegistForSubmitFOAResponseResult;
                }

                ZFBSS_FIXEDASSET_ASSET[] CREATE_ASSET_MASTER = null;
                if (executeResults.p_ws_main_cur.Count > 0)
                {
                    List<FBSS_SubmitFOAMainRespones> listMain = new List<FBSS_SubmitFOAMainRespones>();
                    foreach (var main in executeResults.p_ws_main_cur)
                    {
                        if (main.TRANS_ID != null && main.TRANS_ID != "")
                        {
                            listMain.Add(main);
                        }

                    }
                    if (listMain != null && listMain.Count > 0)
                    {
                        CREATE_ASSET_MASTER = listMain.Select(ins => new ZFBSS_FIXEDASSET_ASSET()
                        {
                            FLAG_TYPE = ins.FLAG_TYPE.ToSafeString(),
                            TRANS_ID = ins.TRANS_ID.ToSafeString(),
                            REC_TYPE = ins.REC_TYPE.ToSafeString(),
                            RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                            COM_CODE = ins.COM_CODE.ToSafeString(),
                            ASSET_CLASS = ins.ASSET_CLASS.ToSafeString(),
                            MAIN_ASSET = ins.MAIN_ASSET.ToSafeString(),
                            SUBNUMBER = ins.SUBNUMBER.ToSafeString(),
                            ASSET_DESC1 = ins.PRODUCT_NAME.ToSafeString(),
                            ASSET_DESC2 = ins.ASSET_DESC2.ToSafeString(),
                            INTERNET_NO = ins.INTERNET_NO.ToSafeString(),
                            COSTCENTER = ins.COSTCENTER.ToSafeString(),
                            EVA4 = ins.EVA4.ToSafeString(),
                            EVA5 = ins.EVA5.ToSafeString()
                        }).ToArray();
                    }
                }

                //List GI
                ZFBSS_FIXEDASSET_INSTALL_IN[] INSTALL_TRANS_POST = null;
                if (executeResults.p_ws_ins_cur.Count > 0)
                {
                    List<FBSS_SubmitFOAInsRespones> listIns = new List<FBSS_SubmitFOAInsRespones>();
                    foreach (var ins in executeResults.p_ws_ins_cur)
                    {
                        if (ins.TRANS_ID != null && ins.TRANS_ID != "")
                        {
                            listIns.Add(ins);
                        }

                    }
                    var curAmount = checkAmount(model.SUBCONTRACT_NAME.ToSafeString(), listIns.ElementAt(0).FLAG_TYPE.ToSafeString());
                    if (listIns != null && listIns.Count > 0)
                    {
                        INSTALL_TRANS_POST = listIns.Select(ins => new ZFBSS_FIXEDASSET_INSTALL_IN()
                        {
                            FLAG_TYPE = ins.FLAG_TYPE.ToSafeString(),
                            TRANS_ID = ins.TRANS_ID.ToSafeString(),
                            REC_TYPE = ins.REC_TYPE.ToSafeString(),
                            RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                            COM_CODE = ins.COM_CODE.ToSafeString(),
                            ASSET_CLASS = ins.ASSET_CLASS.ToSafeString(),
                            MAIN_ASSET = ins.MAIN_ASSET.ToSafeString(),
                            SUBNUMBER = ins.SUBNUMBER.ToSafeString(),
                            ASSET_DESC1 = ins.ASSET_DESC1.ToSafeString(),
                            ASSET_DESC2 = ins.ASSET_DESC2.ToSafeString(),
                            INTERNET_NO = ins.INTERNET_NO.ToSafeString(),
                            COSTCENTER = ins.COSTCENTER.ToSafeString(),
                            EVA4 = ins.EVA4.ToSafeString(),
                            EVA5 = ins.EVA5.ToSafeString(),
                            DOC_TYPE = ins.DOCUMENT_TYPE.ToSafeString(),
                            DOC_DATE = ins.DOC_DATE.ToSafeString(),
                            POST_DATE = ins.POST_DATE.ToSafeString(),
                            REF_DOC_NO = ins.REF_DOC_NO.ToSafeString(),
                            XREF1_HD = ins.XREF1_HD.ToSafeString(),
                            CURRENCY = ins.CURRENCY.ToSafeString(),
                            RATE = ins.RATE.ToSafeDecimal(),
                            RATESpecified = curAmount,
                            TRANSLA_DATE = ins.TRANSLATION_DATE.ToSafeString(),
                            ACCOUNT = ins.ACCOUNT.ToSafeString(),
                            AMOUNT = ins.AMOUNT.ToSafeDecimal(),
                            AMOUNTSpecified = curAmount,
                            REF_KEY1 = ins.REFERENCE_KEY1.ToSafeString(),
                            REF_KEY2 = ins.REFERENCE_KEY2.ToSafeString(),
                            REF_KEY3 = ins.REFERENCE_KEY3.ToSafeString(),
                            ITEM_TEXT = ins.ITEM_TEXT.ToSafeString(),
                            ASSIGNMENT = ins.ASSIGNMENT.ToSafeString()
                        }).ToArray();
                    }
                }


                //List GI
                ZFBSS_FIXEDASSET_INVENTORY_IN[] INVENTORY_TRANS_POST = null;
                if (executeResults.p_ws_inv_cur.Count > 0)
                {
                    List<FBSS_SubmitFOAInvRespones> listInv = new List<FBSS_SubmitFOAInvRespones>();
                    foreach (var inv in executeResults.p_ws_inv_cur)
                    {
                        if (inv.TRANS_ID != null && inv.TRANS_ID != "")
                        {
                            listInv.Add(inv);
                        }

                    }

                    if (listInv != null && listInv.Count > 0)
                    {
                        INVENTORY_TRANS_POST = listInv.Select(inv => new ZFBSS_FIXEDASSET_INVENTORY_IN()
                        {
                            FLAG_TYPE = inv.FLAG_TYPE.ToSafeString(),
                            TRANS_ID = inv.TRANS_ID.ToSafeString(),
                            REC_TYPE = inv.REC_TYPE.ToSafeString(),
                            RUN_GROUP = inv.RUN_GROUP.ToSafeString(),
                            DOC_DATE = inv.DOC_DATE.ToSafeString(),
                            POST_DATE = inv.POST_DATE.ToSafeString(),
                            INTERNET_NO = inv.INTERNET_NO.ToSafeString(),
                            REF_DOC_NO = inv.REF_DOC_NO.ToSafeString(),
                            COM_CODE = inv.COM_CODE.ToSafeString(),
                            MAIN_ASSET = inv.MAIN_ASSET.ToSafeString(),
                            SUBNUMBER = inv.SUBNUMBER.ToSafeString(),
                            MATERIAL_NO = inv.MATERIAL_NO.ToSafeString(),
                            PLANT_FROM = inv.PLANT.ToSafeString(),
                            SLOC_FROM = inv.STORAGE_LOCATION.ToSafeString(),
                            QUANTITY = inv.QUANTITY.ToSafeString(),
                            MOVEMENT_TYPE = inv.MOVEMENT_TYPE.ToSafeString(),
                            ITEM_TEXT = inv.ITEM_TEXT.ToSafeString(),
                            SERIAL_NO = inv.SERIAL_NO.ToSafeString(),
                            XREF1_HD = inv.XREF1_HD.ToSafeString()
                        }).ToArray();
                    }
                }


                ZFBSS_FIXEDASSET_ASSET_OUT[] OUT_CREATE_ASSET = null;
                ZFBSS_FIXEDASSET_INS_OUT[] OUT_INS_TRANS_POST = null;
                ZFBSS_FIXEDASSET_INV_OUT[] OUT_INV_TRANS_POST = null;

                List<XDocument> listXDocument = new List<XDocument>();
                if (CREATE_ASSET_MASTER != null && CREATE_ASSET_MASTER.Count() > 0)
                {
                    listXDocument.Add(XDocument.Parse(CREATE_ASSET_MASTER.DumpToXml()));
                }

                if (INSTALL_TRANS_POST != null && INSTALL_TRANS_POST.Count() > 0)
                {
                    listXDocument.Add(XDocument.Parse(INSTALL_TRANS_POST.DumpToXml()));
                }
                if (INVENTORY_TRANS_POST != null && INVENTORY_TRANS_POST.Count() > 0)
                {
                    listXDocument.Add(XDocument.Parse(INVENTORY_TRANS_POST.DumpToXml()));
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

                // update response from sap
                string result_code = "0";
                string result_msg = "";
                string main_access = "";

                if (flagSap)
                {
                    InterfaceLogPayGCommand sap_log = null;
                    sap_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_in.ToString(), transactionID, "call SAP.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");
                    //!* log
                    UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToString(), hLog, "", "IN_SAP", null, null);

                    if (fixAssetCallSap.COM_CODE == "Y")
                    {
                        if (userCredential == null) return new NewRegistForSubmitFOAMainAssetModel() { result = "08", errorReason = "Endpoint" };

                        var _sap = new SAPFixedAssetService.SI_Z_FIAM_FBSS_FIXEDASSETService();
                        _sap.UseDefaultCredentials = true;

                        if (urlEndpoint != null) _sap.Url = urlEndpoint.DISPLAY_VAL;

                        if (CheckRollBack == "Y")
                            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                        _sap.Credentials = new NetworkCredential(userCredential.DISPLAY_VAL.ToSafeString(), passwordCredential.DISPLAY_VAL.ToSafeString());

                        _sap.SI_Z_FIAM_FBSS_FIXEDASSET(
                            ref CREATE_ASSET_MASTER,
                            ref INSTALL_TRANS_POST,
                            ref INVENTORY_TRANS_POST,
                            ref OUT_CREATE_ASSET,
                            ref OUT_INS_TRANS_POST,
                            ref OUT_INV_TRANS_POST);

                        var xml_master_out = XDocument.Parse(OUT_CREATE_ASSET.DumpToXml());
                        var xml_install_out = XDocument.Parse(OUT_INS_TRANS_POST.DumpToXml());
                        var xml_GI_out = XDocument.Parse(OUT_INV_TRANS_POST.DumpToXml());
                        xml_master_out.Root.Add(xml_install_out.Root.Elements());
                        xml_master_out.Root.Add(xml_GI_out.Root.Elements());
                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_out.ToString(), sap_log, "Success", "", "");

                        //!* log
                        UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToString(), hLog, "", "OUT_SAP", null, null);

                        if (OUT_CREATE_ASSET.Length > 0)
                        {
                            string ins_msg = string.Empty;

                            foreach (var element in OUT_CREATE_ASSET)
                            {
                                if (SAPResponseValid(element.NUMBER_MESSAGE.ToSafeString(), element.NUMBER_MESSAGE.ToSafeString()) && result_msg == "")
                                {
                                    result_code = element.NUMBER_MESSAGE.ToSafeString();
                                    result_msg = element.MESSAGE.ToSafeString();
                                }

                                var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_upsert_foa_response",
                                new
                                {
                                    p_TRANS_ID = element.TRANS_ID.ToSafeString(),
                                    p_REC_TYPE = element.REC_TYPE.ToSafeString(),
                                    p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),
                                    p_INTERNET_NO = element.REFERENCE.ToSafeString(),
                                    p_ORDER_NO = orderNO,
                                    p_COM_CODE = element.COMPANY.ToSafeString(),
                                    p_ASSET_CODE = element.MAIN_ASSET.ToSafeString(),
                                    p_SUBNUMBER = element.SUBNUMBER.ToSafeString(),
                                    p_MATERIAL_NO = "",
                                    p_SERIAL_NO = "",
                                    p_MATERIAL_DOC = "",
                                    p_DOC_YEAR = "",
                                    p_ERR_CODE = element.NUMBER_MESSAGE.ToSafeString(),
                                    p_ERR_MSG = element.MESSAGE.ToSafeString(),

                                    ret_code = ret_code,
                                    ret_msg = ""

                                }).ToList();

                                _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToString());
                                //_logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_msg.Value.ToString());
                            }

                        }

                        if (OUT_INS_TRANS_POST.Length > 0)
                        {
                            string ins_msg = string.Empty;

                            foreach (var element in OUT_INS_TRANS_POST)
                            {
                                if (SAPResponseValid(element.NUMBER_MESSAGE.ToSafeString(), element.NUMBER_MESSAGE.ToSafeString()) && result_msg == "")
                                {
                                    result_code = element.NUMBER_MESSAGE.ToSafeString();
                                    result_msg = element.MESSAGE.ToSafeString();
                                }

                                var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_upsert_foa_response",
                                new
                                {
                                    p_TRANS_ID = element.TRANS_ID.ToSafeString(),
                                    p_REC_TYPE = element.REC_TYPE.ToSafeString(),
                                    p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),
                                    p_INTERNET_NO = accessNO,
                                    p_ORDER_NO = orderNO,
                                    p_COM_CODE = element.COMPANY.ToSafeString(),
                                    p_ASSET_CODE = element.MAIN_ASSET.ToSafeString(),
                                    p_SUBNUMBER = element.SUBNUMBER.ToSafeString(),
                                    p_MATERIAL_NO = "",
                                    p_SERIAL_NO = "",
                                    p_MATERIAL_DOC = element.DOCNO.ToSafeString(),
                                    p_DOC_YEAR = element.YEAR.ToSafeString(),
                                    p_ERR_CODE = element.NUMBER_MESSAGE.ToSafeString(),
                                    p_ERR_MSG = element.MESSAGE.ToSafeString(),

                                    ret_code = ret_code,
                                    ret_msg = ""

                                }).ToList();

                                _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToString());
                                //_logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_msg.Value.ToString());
                            }
                        }

                        if (OUT_INV_TRANS_POST.Length > 0)
                        {
                            string ins_mat = string.Empty;
                            string ins_serial = string.Empty;
                            string ins_msg = string.Empty;

                            foreach (var element in OUT_INV_TRANS_POST)
                            {
                                ;
                                if (SAPResponseValid(element.ERR_CODE.ToSafeString(), element.ERR_CODE.ToSafeString()) && result_msg == "")
                                {
                                    result_code = element.ERR_CODE.ToSafeString();
                                    result_msg = element.ERR_MSG.ToSafeString();
                                    main_access = element.MAIN_ASSET.ToSafeString();
                                }

                                if (!ins_mat.Equals(element.MATERIAL_NO) && !ins_serial.Equals(element.SERIAL_NO))
                                {
                                    ins_msg = element.ERR_MSG.ToSafeString();
                                }
                                ins_mat = element.MATERIAL_NO.ToSafeString();
                                ins_serial = element.SERIAL_NO.ToSafeString();

                                var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_upsert_foa_response",
                                    new
                                    {
                                        p_TRANS_ID = element.TRANS_ID.ToSafeString(),
                                        p_REC_TYPE = element.REC_TYPE.ToSafeString(),
                                        p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),
                                        p_INTERNET_NO = element.INTERNET_NO.ToSafeString(),
                                        p_ORDER_NO = orderNO,
                                        p_COM_CODE = element.COMPANY.ToSafeString(),
                                        p_ASSET_CODE = element.MAIN_ASSET.ToSafeString(),
                                        p_SUBNUMBER = element.SUBNUMBER.ToSafeString(),
                                        p_MATERIAL_NO = element.MATERIAL_NO.ToSafeString(),
                                        p_SERIAL_NO = element.SERIAL_NO.ToSafeString(),
                                        p_MATERIAL_DOC = element.MATERIAL_DOC.ToSafeString(),
                                        p_DOC_YEAR = element.DOC_YEAR.ToSafeString(),
                                        p_ERR_CODE = element.ERR_CODE.ToSafeString(),
                                        p_ERR_MSG = element.ERR_MSG.ToSafeString(),

                                        ret_code = ret_code,
                                        ret_msg = ""

                                    }).ToList();
                                _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToString());
                            }

                        }
                    }
                    else
                    {
                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                    }
                }

                //TODO: R17.10 smsSurveyFOA
                #region smsSurveyFOA

                try
                {
                    var resultFoaLov = from item in _cfgLov.Get()
                                       where item.LOV_TYPE == "CREDENTIAL_SMSSURVEYFOA"
                                       select item;

                    var urlSmsFoaEndpoint = resultFoaLov.FirstOrDefault(item => item.LOV_NAME == "URL");

                    var resultFixAssConfigSmsFoa = from item in _fixAssConfig.Get()
                                                   where item.PROGRAM_CODE == "P012"
                                                   && item.PROGRAM_NAME == "SMSSURVEYFOA"
                                                   select item;
                    var fixAssetCallSmsFoa = resultFixAssConfigSmsFoa.FirstOrDefault();


                    if (fixAssetCallSmsFoa != null && fixAssetCallSmsFoa.COM_CODE == "Y")
                    {
                        var ordNo = model.p_ORDER_NO;
                        var nonMobile = model.p_INTERNET_NO;
                        var ordType = model.ORDER_TYPE;
                        var productName = model.p_PRODUCT;

                        var subcontractorName = model.SUBCONTRACT_NAME.ToSafeString();
                        var orgIDrow = from itemSubcontractorName in _subContractor.Get()
                                       where itemSubcontractorName.SUB_CONTRACTOR_NAME_TH == subcontractorName
                                       select itemSubcontractorName;

                        var orgId = orgIDrow.Any() ? orgIDrow.FirstOrDefault().ORG_ID.ToSafeString() : string.Empty;


                        var submitDate = string.Empty;
                        if (!string.IsNullOrEmpty(model.FOA_SUBMIT_DATE))
                        {
                            try
                            {
                                DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_SUBMIT_DATE, "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
                                submitDate = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                            }
                            catch
                            {
                                DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_SUBMIT_DATE, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                submitDate = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss");
                            }
                        }


                        var foaRequert = new SmsSurveyFOAWebService.FOAParam
                        {
                            ordNo = ordNo,
                            nonMobile = nonMobile,
                            ordType = ordType,
                            productName = productName,
                            orgID = orgId,
                            //rejectReason = rejectReason,
                            submitDate = submitDate
                        };

                        var smsSurveyFoa = new SmsSurveyFOAWebService.smsSurveyFOA
                        {
                            UseDefaultCredentials = true
                        };

                        if (urlSmsFoaEndpoint != null) smsSurveyFoa.Url = urlSmsFoaEndpoint.DISPLAY_VAL;

                        //ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                        //smsSurveyFoa.Credentials = new NetworkCredential(userSmsFoaCredential.DISPLAY_VAL.ToSafeString(), passwordSmsFoaCredential.DISPLAY_VAL.ToSafeString());

                        var interfaceNode = String.Format("{0}|{1}", "FOA", smsSurveyFoa.Url.ToSafeString());
                        var foaLog = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, foaRequert,
                            model.p_ORDER_NO, "insertFOAInfo", "SmsSurveyFOA", nonMobile, interfaceNode, "Exs.");

                        try
                        {
                            var smsSurveyFoaResult = smsSurveyFoa.insertFOAInfo(foaRequert);

                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, smsSurveyFoaResult, foaLog,
                                smsSurveyFoaResult.IndexOf("Success", StringComparison.Ordinal) > 0 ? "Success" : "Failed",
                                "", "Exs.");
                        }
                        catch (Exception ex)
                        {
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex.GetBaseException(), foaLog,
                                "Error", ex.Message, "Exs.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var foaLog = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, new SmsSurveyFOAWebService.FOAParam(),
                             model.p_ORDER_NO, "insertFOAInfo", "SmsSurveyFOA", model.p_INTERNET_NO, "FOA", "Exs.");

                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex.GetBaseException(), foaLog,
                                "Error GetConfig", ex.Message, "Exs.");
                }

                #endregion smsSurveyFOA

                if (flagSff)
                {
                    XDocument xml_SFF_in = null;
                    if (executeResults.p_ws_sff_cur != null && executeResults.p_ws_sff_cur.Count() > 0)
                    {
                        xml_SFF_in = XDocument.Parse(executeResults.p_ws_sff_cur.DumpToXml());
                    }

                    InterfaceLogPayGCommand sff_log = null;
                    sff_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_SFF_in.ToString(), transactionID, "call SFF.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                    if (fixAssetCallSff.COM_CODE == "Y")
                    {
                        if (executeResults.p_ws_sff_cur != null && executeResults.p_ws_sff_cur.Count() > 0)
                        {
                            if (userSffCredential == null) return new NewRegistForSubmitFOAMainAssetModel() { result = "08", errorReason = "Endpoint" };

                            var _sff = new SFFServices.SFFServiceService();
                            _sff.UseDefaultCredentials = true;

                            if (urlSffEndpoint != null) _sff.Url = urlSffEndpoint.DISPLAY_VAL;
                            //if (CheckRollBack == "Y")
                            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                            _sff.Credentials = new NetworkCredential(userSffCredential.DISPLAY_VAL.ToSafeString(), passwordSffCredential.DISPLAY_VAL.ToSafeString());


                            var request = new SFFServices.SffRequest();
                            request.Event = "evOMCreatePendingTaskFBB";

                            var paramArray = new SFFServices.Parameter[3];
                            var param0 = new SFFServices.Parameter();
                            var param1 = new SFFServices.Parameter();
                            var param2 = new SFFServices.Parameter();
                            var param3 = new SFFServices.Parameter();

                            param0.Name = "processType";
                            param0.Value = "NewRegister3GFMC";
                            param1.Name = "simSerial";
                            param1.Value = executeResults.p_ws_sff_cur[0].SN;
                            param2.Name = "fbbId";
                            param2.Value = param2.Value = executeResults.p_ws_sff_cur[0].ACCESS;

                            paramArray[0] = param0;
                            paramArray[1] = param1;
                            paramArray[2] = param2;

                            var paramList = new SFFServices.ParameterList();
                            paramList.Parameter = paramArray;

                            request.ParameterList = paramList;

                            var objResp = _sff.ExecuteService(request);

                            if (objResp != null)
                            {
                                if (objResp.ErrorMessage == null)
                                {
                                    string returncode = "";
                                    string retuenmsg = "";

                                    var response = new SFFServices.SffResponse();
                                    foreach (var itemData in objResp.ParameterList.Parameter)
                                    {
                                        if (itemData.Name == "result")
                                        {
                                            returncode = itemData.Value;

                                        }
                                        else if (itemData.Name == "errorReason")
                                        {
                                            retuenmsg = itemData.Value;
                                        }
                                    }

                                    if (returncode == "Fail")
                                    {
                                        _logger.Info("End Call SFF  Result" + "-1");
                                        _logger.Info("End Call SFF  ErrorReason" + "Service Sff is " + retuenmsg.ToString());
                                    }
                                    else
                                    {
                                        _logger.Info("End Call SFF  Result" + "0");
                                        _logger.Info("End Call SFF  ErrorReason" + retuenmsg.ToString());
                                    }

                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, objResp, sff_log, returncode, "", "");
                                }
                            }
                        }
                    }
                    else
                    {
                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sff_log, "Block out", "", "");
                    }
                }
                string msg = "";
                int indexSuccess = 0;
                int indexError = 0;

                NewRegistForSubmitFOAResponseResult.result = main_access;// result_code;
                NewRegistForSubmitFOAResponseResult.errorReason = result_msg;
                if (main_access != "") result_msg = "Success";
                //if ((result_code.Equals("0") || result_code.Equals("000")) && result_msg.Trim().Equals("")) result_msg = "Success";
                //if (result_code == "0" || result_code == "000" || result_code == "") indexSuccess += 1;
                if (main_access != "") indexSuccess += 1;
                else indexError += 1;
                if (indexSuccess > 0 || indexError > 0)
                {
                    msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToString() + " Order. " : msg + "";
                    msg = indexError > 0 ? msg + "Error " + indexError.ToString() + " Order. " : msg + "";
                }
                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, msg, "");
                //!* log
                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);

            }
            catch (Exception ex)
            {
                NewRegistForSubmitFOAResponseResult.result = "-1";
                NewRegistForSubmitFOAResponseResult.errorReason = ex.GetErrorMessage();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, NewRegistForSubmitFOAResponseResult, log, "Fail", ex.GetErrorMessage().ToSafeString(), "");
                //!* log
                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "ERROR", ex.GetErrorMessage().ToSafeString(), null);
                //StartHistoryLog(_uow, _hisLog, result, "", "ERROR", ex.GetErrorMessage().ToSafeString(), "");
            }

            return NewRegistForSubmitFOAResponseResult;

        }

        public static bool GIValid(string flagType, string matCode)
        {
            if (matCode == null || matCode.Equals("")) return false;
            if (flagType.Equals("C") || flagType.Equals("F")) return false;
            return true;
        }

        public static bool checkAmount(string subname, string flagtype)
        {

            var c = true;
            //if (!subname.ToUpper().Equals("CS AIRNET")) c = false;
            if (flagtype.ToUpper().Equals("G"))
            {
                c = false;
                if (!subname.ToUpper().Equals("CS AIRNET")) c = true;
            }

            return c;
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
