using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
//using WBBBusinessLayer.FBBSAPOnlineMA;
using WBBBusinessLayer.FBBSAPOnlineMA2;
using WBBBusinessLayer.FBBSAPOnlineRevalue;
using WBBBusinessLayer.SAPFixedAssetService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class NewRegistForSubmitFOAQueryHandler : IQueryHandler<NewRegistForSubmitFOAQuery, NewRegistForSubmitFOAResponse>
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
        private readonly IEntityRepository<FBBPAYG_VENDOR> _paygv;
        private readonly ICommandHandler<UpdateSubmitFoaErrorLogCommand> _UpdateSubmitFoaError;

        public NewRegistForSubmitFOAQueryHandler(
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
             , IEntityRepository<FBBPAYG_VENDOR> paygv
            , ICommandHandler<UpdateSubmitFoaErrorLogCommand> UpdateSubmitFoaError

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
            _paygv = paygv;
            _UpdateSubmitFoaError = UpdateSubmitFoaError;
            // __UpdateSubmitFoaError = UpdateSubmitFoaError;
        }

        public NewRegistForSubmitFOAResponse Handle(NewRegistForSubmitFOAQuery model)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            HistoryLogCommand hLog = new HistoryLogCommand();
            bool flagSap = true; bool flagRevalueSap = true; bool flagMASap = true;
            bool flagSff = true;
            string packageMsg = "";
            string sn_no = "";
            NewRegistForSubmitFOAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAResponse();
            PkgFbbFoaOrderManagementResponse executeResults = new PkgFbbFoaOrderManagementResponse();
            //string StoreName = "";
            string SAPError = "";
            string errMsg = ""; string errCode = "000";
            try
            {
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
                //var fixAssetCallSap = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P009");
                var fixAssetCallSff = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P010");
                var FixAssConfig = from item in _fixAssConfig.Get()
                                   where item.PROGRAM_NAME == "FLAG_CALL_SAP" || item.PROGRAM_NAME == "FLAG_CALL_SAP_MA" || item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE"
                                   select item;
                var fixAssetCallSap = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP");
                var fixAssetCallSapRevalue = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE");
                var fixAssetCallSapMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_MA");

                var CheckRollBack = (from r in _cfgLov.Get()
                                     where r.LOV_NAME == "ROLLBACK" && r.LOV_TYPE == "FBBPAYG_CERTSAP"
                                     select r.ACTIVEFLAG).FirstOrDefault().ToSafeString();

                _logger.Info("Call NewRegistForSubmitFOAQueryHandler");

                var packageMappingObjectModel = new PackageMappingObjectModel();
                if (model.ProductList != null)
                {
                    packageMappingObjectModel.FBB_PRODUCT_LIST = model.ProductList.Select(m => new Product_Mapping_ArrayMapping
                    {
                        SerialNumber = m.SerialNumber.ToSafeString(),
                        MaterialCode = m.MaterialCode.ToSafeString(),
                        CompanyCode = m.CompanyCode.ToSafeString(),
                        Plant = m.Plant.ToSafeString(),
                        StorageLocation = m.StorageLocation.ToSafeString(),
                        SNPattern = m.SNPattern.ToSafeString(),
                        MovementType = m.MovementType.ToSafeString()
                    }).ToArray();
                }
                //comment 23.01.2019
                //model.Address_ID = model.Address_ID.ToSafeString();
                //model.ORG_ID = model.ORG_ID.ToSafeString();
                //// add new 17.12.21
                //model.Reuse_Flag = model.Reuse_Flag.ToSafeString();
                //model.Event_Flow_Flag = model.Event_Flow_Flag.ToSafeString();
                // End add new 17.12.21
                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Product_List", "WBB.FBB_PRODUCT_LIST", packageMappingObjectModel);


                String FOA_Submit_date = null;
                var culture = CultureInfo.GetCultureInfo("en-US");
                if (model.FOA_Submit_date != null && model.FOA_Submit_date != "")
                {
                    try
                    {
                        DateTime dt;
                        if (DateTime.TryParseExact(model.FOA_Submit_date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture, DateTimeStyles.None, out dt))
                        {
                            DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_Submit_date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture);
                            FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss", culture);
                        }
                        else if (DateTime.TryParseExact(model.FOA_Submit_date, "dd/MM/yyyy HH:mm:ss", culture, DateTimeStyles.None, out dt))
                        {
                            DateTime dtFOA_Submit_date = DateTime.ParseExact(model.FOA_Submit_date, "dd/MM/yyyy HH:mm:ss", culture);
                            FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss", culture);
                        }

                    }
                    catch (Exception ex)
                    {
                        FOA_Submit_date = null;
                        _logger.Info("FOA_Submit_date Exception : " + ex.Message);
                    }
                }

                // R17.11
                String Post_Date = null;
                if (model.Post_Date != null && model.Post_Date != "")
                {
                    try
                    {

                        DateTime dt;
                        if (DateTime.TryParseExact(model.Post_Date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture, DateTimeStyles.None, out dt))
                        {
                            DateTime dtPost_Date = DateTime.ParseExact(model.Post_Date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture);
                            Post_Date = dtPost_Date.ToString("dd/MM/yyyy HHmmss", culture);
                        }
                        else if (DateTime.TryParseExact(model.Post_Date, "dd/MM/yyyy HH:mm:ss", culture, DateTimeStyles.None, out dt))
                        {
                            DateTime dtPost_Date = DateTime.ParseExact(model.Post_Date, "dd/MM/yyyy HH:mm:ss", culture);
                            Post_Date = dtPost_Date.ToString("dd/MM/yyyy HHmmss", culture);
                        }
                        else if (DateTime.TryParseExact(model.Post_Date, "dd/MM/yyyy", culture, DateTimeStyles.None, out dt))
                        {
                            DateTime dtPost_Date = DateTime.ParseExact(model.Post_Date, "dd/MM/yyyy", culture);
                            Post_Date = dtPost_Date.ToString("dd/MM/yyyy HHmmss", culture);
                        }

                    }
                    catch (Exception ex)
                    {
                        Post_Date = null;
                        _logger.Info("Post_Date Exception : " + ex.Message);
                    }
                }

                //R01.08.2021
                if (model.ProductList.Count != 0)
                {
                    sn_no = model.ProductList.First().SerialNumber.ToSafeString();
                }

                if (model.Access_No != null && (model.Product_Owner == null || model.Product_Owner == ""))
                {
                    if (model.Access_No.StartsWith("888"))
                    {
                        model.Product_Owner = "3BB";
                    }
                    else
                    {
                        model.Product_Owner = "FBB";
                    }
                }

                #region Parameter Input
                var p_ACCESS_NUMBER = new OracleParameter();
                p_ACCESS_NUMBER.ParameterName = "p_ACCESS_NUMBER";
                p_ACCESS_NUMBER.Size = 2000;
                p_ACCESS_NUMBER.OracleDbType = OracleDbType.Varchar2;
                p_ACCESS_NUMBER.Direction = ParameterDirection.Input;
                p_ACCESS_NUMBER.Value = model.Access_No;

                var p_ORDER_NO = new OracleParameter();
                p_ORDER_NO.ParameterName = "p_ORDER_NO";
                p_ORDER_NO.Size = 2000;
                p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_NO.Direction = ParameterDirection.Input;
                p_ORDER_NO.Value = model.OrderNumber;

                var p_ORDER_TYPE = new OracleParameter();
                p_ORDER_TYPE.ParameterName = "p_ORDER_TYPE";
                p_ORDER_TYPE.Size = 2000;
                p_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
                p_ORDER_TYPE.Direction = ParameterDirection.Input;
                p_ORDER_TYPE.Value = model.OrderType;

                var p_SUBCONTRACT_CODE = new OracleParameter();
                p_SUBCONTRACT_CODE.ParameterName = "p_SUBCONTRACT_CODE";
                p_SUBCONTRACT_CODE.Size = 2000;
                p_SUBCONTRACT_CODE.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONTRACT_CODE.Direction = ParameterDirection.Input;
                p_SUBCONTRACT_CODE.Value = model.SubcontractorCode;

                var p_SUBCONTRACT_NAME = new OracleParameter();
                p_SUBCONTRACT_NAME.ParameterName = "p_SUBCONTRACT_NAME";
                p_SUBCONTRACT_NAME.Size = 2000;
                p_SUBCONTRACT_NAME.OracleDbType = OracleDbType.Varchar2;
                p_SUBCONTRACT_NAME.Direction = ParameterDirection.Input;
                p_SUBCONTRACT_NAME.Value = model.SubcontractorName;

                var p_PRODUCT_NAME = new OracleParameter();
                p_PRODUCT_NAME.ParameterName = "p_PRODUCT_NAME";
                p_PRODUCT_NAME.Size = 2000;
                p_PRODUCT_NAME.OracleDbType = OracleDbType.Varchar2;
                p_PRODUCT_NAME.Direction = ParameterDirection.Input;
                p_PRODUCT_NAME.Value = model.ProductName;

                var p_SERVICE_LIST = new OracleParameter();
                p_SERVICE_LIST.ParameterName = "p_SERVICE_LIST";
                p_SERVICE_LIST.Size = 2000;
                p_SERVICE_LIST.OracleDbType = OracleDbType.Varchar2;
                p_SERVICE_LIST.Direction = ParameterDirection.Input;
                p_SERVICE_LIST.Value = model.ServiceList != null ? string.Join(",", model.ServiceList.Select(m => m.ServiceName).ToArray()) : "";

                var p_Product_List = new OracleParameter();
                p_Product_List.ParameterName = "p_Product_List";
                p_Product_List.OracleDbType = OracleDbType.RefCursor;
                p_Product_List.Direction = ParameterDirection.Input;
                p_Product_List.Value = packageMapping;

                var p_SUBMIT_FLAG = new OracleParameter();
                p_SUBMIT_FLAG.ParameterName = "p_SUBMIT_FLAG";
                p_SUBMIT_FLAG.Size = 2000;
                p_SUBMIT_FLAG.OracleDbType = OracleDbType.Varchar2;
                p_SUBMIT_FLAG.Direction = ParameterDirection.Input;
                p_SUBMIT_FLAG.Value = model.SubmitFlag;

                var p_Reject_reason = new OracleParameter();
                p_Reject_reason.ParameterName = "p_Reject_reason";
                p_Reject_reason.Size = 2000;
                p_Reject_reason.OracleDbType = OracleDbType.Varchar2;
                p_Reject_reason.Direction = ParameterDirection.Input;
                p_Reject_reason.Value = model.RejectReason;

                var p_foa_submit_date = new OracleParameter();
                p_foa_submit_date.ParameterName = "p_foa_submit_date";
                p_foa_submit_date.Size = 2000;
                p_foa_submit_date.OracleDbType = OracleDbType.Varchar2;
                p_foa_submit_date.Direction = ParameterDirection.Input;
                p_foa_submit_date.Value = FOA_Submit_date;

                var p_post_date = new OracleParameter();
                p_post_date.ParameterName = "p_post_date";
                p_post_date.Size = 2000;
                p_post_date.OracleDbType = OracleDbType.Varchar2;
                p_post_date.Direction = ParameterDirection.Input;
                p_post_date.Value = Post_Date;

                var p_olt_name = new OracleParameter();
                p_olt_name.ParameterName = "p_olt_name";
                p_olt_name.Size = 2000;
                p_olt_name.OracleDbType = OracleDbType.Varchar2;
                p_olt_name.Direction = ParameterDirection.Input;
                p_olt_name.Value = model.OLT_NAME;

                var p_building_name = new OracleParameter();
                p_building_name.ParameterName = "p_building_name";
                p_building_name.Size = 2000;
                p_building_name.OracleDbType = OracleDbType.Varchar2;
                p_building_name.Direction = ParameterDirection.Input;
                p_building_name.Value = model.BUILDING_NAME;

                var p_mobile_contact = new OracleParameter();
                p_mobile_contact.ParameterName = "p_mobile_contact";
                p_mobile_contact.Size = 2000;
                p_mobile_contact.OracleDbType = OracleDbType.Varchar2;
                p_mobile_contact.Direction = ParameterDirection.Input;
                p_mobile_contact.Value = model.Mobile_Contact;
                //add new 
                var p_addess_id = new OracleParameter();
                p_addess_id.ParameterName = "p_addess_id";
                p_addess_id.Size = 2000;
                p_addess_id.OracleDbType = OracleDbType.Varchar2;
                p_addess_id.Direction = ParameterDirection.Input;
                p_addess_id.Value = model.Address_ID.ToSafeString();

                //add new 17.12.21
                var p_Reuse_Flag = new OracleParameter();
                p_Reuse_Flag.ParameterName = "p_reuse_flag";
                p_Reuse_Flag.Size = 2000;
                p_Reuse_Flag.OracleDbType = OracleDbType.Varchar2;
                p_Reuse_Flag.Direction = ParameterDirection.Input;
                p_Reuse_Flag.Value = model.Reuse_Flag.ToSafeString();

                var p_Event_Flow_Flag = new OracleParameter();
                p_Event_Flow_Flag.ParameterName = "p_event_flow_flag";
                p_Event_Flow_Flag.Size = 2000;
                p_Event_Flow_Flag.OracleDbType = OracleDbType.Varchar2;
                p_Event_Flow_Flag.Direction = ParameterDirection.Input;
                p_Event_Flow_Flag.Value = model.Event_Flow_Flag.ToSafeString();

                //En add new 17.12.21

                ////add new 18.06.28
                var p_Subcontract_Type = new OracleParameter();
                p_Subcontract_Type.ParameterName = "p_subcontract_type";
                p_Subcontract_Type.Size = 2000;
                p_Subcontract_Type.OracleDbType = OracleDbType.Varchar2;
                p_Subcontract_Type.Direction = ParameterDirection.Input;
                p_Subcontract_Type.Value = model.Subcontract_Type.ToSafeString();

                var p_Subcontract_Sub_Type = new OracleParameter();
                p_Subcontract_Sub_Type.ParameterName = "p_subcontract_sub_type";
                p_Subcontract_Sub_Type.Size = 2000;
                p_Subcontract_Sub_Type.OracleDbType = OracleDbType.Varchar2;
                p_Subcontract_Sub_Type.Direction = ParameterDirection.Input;
                p_Subcontract_Sub_Type.Value = model.Subcontract_Sub_Type.ToSafeString();

                var p_Request_Sub_Flag = new OracleParameter();
                p_Request_Sub_Flag.ParameterName = "p_request_sub_flag";
                p_Request_Sub_Flag.Size = 2000;
                p_Request_Sub_Flag.OracleDbType = OracleDbType.Varchar2;
                p_Request_Sub_Flag.Direction = ParameterDirection.Input;
                p_Request_Sub_Flag.Value = model.Request_Sub_Flag.ToSafeString();

                var p_Sub_Access_Mode = new OracleParameter();
                p_Sub_Access_Mode.ParameterName = "p_sub_access_mode";
                p_Sub_Access_Mode.Size = 2000;
                p_Sub_Access_Mode.OracleDbType = OracleDbType.Varchar2;
                p_Sub_Access_Mode.Direction = ParameterDirection.Input;
                p_Sub_Access_Mode.Value = model.Sub_Access_Mode.ToSafeString();

                var p_product_owner = new OracleParameter();
                p_product_owner.ParameterName = "p_product_owner";
                p_product_owner.Size = 2000;
                p_product_owner.OracleDbType = OracleDbType.Varchar2;
                p_product_owner.Direction = ParameterDirection.Input;
                p_product_owner.Value = model.Product_Owner.ToSafeString();

                var p_main_promo_code = new OracleParameter();
                p_main_promo_code.ParameterName = "p_main_promo_code";
                p_main_promo_code.Size = 2000;
                p_main_promo_code.OracleDbType = OracleDbType.Varchar2;
                p_main_promo_code.Direction = ParameterDirection.Input;
                p_main_promo_code.Value = model.Main_Promo_Code.ToSafeString();

                var p_team_id = new OracleParameter();
                p_team_id.ParameterName = "p_team_id";
                p_team_id.Size = 2000;
                p_team_id.OracleDbType = OracleDbType.Varchar2;
                p_team_id.Direction = ParameterDirection.Input;
                p_team_id.Value = model.Team_ID.ToSafeString();

                //end add new 18.06.28

                var p_org_id = new OracleParameter();
                p_org_id.ParameterName = "p_org_id";
                p_org_id.Size = 2000;
                p_org_id.OracleDbType = OracleDbType.Varchar2;
                p_org_id.Direction = ParameterDirection.Input;
                p_org_id.Value = model.ORG_ID.ToSafeString();

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

                var p_ws_revalue_cur = new OracleParameter();
                p_ws_revalue_cur.ParameterName = "p_ws_revalue_cur";
                p_ws_revalue_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_revalue_cur.Direction = ParameterDirection.Output;


                var p_ws_maintain_cur = new OracleParameter();
                p_ws_maintain_cur.ParameterName = "p_ws_maintain_cur";
                p_ws_maintain_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_maintain_cur.Direction = ParameterDirection.Output;

                #endregion

                _logger.Info("Start pkg_fbb_foa_order_management " + executeResults.ret_msg);

                string _symptomgroup = "NEW"; string _humamgroup = "HUMAN_NEW"; string _jointype = "NEW";
                string _upf = "NEW"; string _MESH = "N";
                if (model.OrderType.ToUpper() == "NEW")
                {
                    int wfcount = 0;
                    int iotcount = 0;

                    //  _symptomgroup = "NEW";
                    if (packageMappingObjectModel.FBB_PRODUCT_LIST != null)
                    {
                        string cpetype = string.Empty;
                        foreach (var d in packageMappingObjectModel.FBB_PRODUCT_LIST)
                        {

                            if (d.MaterialCode.ToSafeString() != "")
                            {
                                cpetype = getcPEtYPE(d.MaterialCode.ToSafeString());
                                if (cpetype.ToUpper() == "WIFI ROUTER")
                                {
                                    wfcount = wfcount + 1;
                                }
                                if (cpetype.ToUpper() == "IOT")
                                {
                                    iotcount = iotcount + 1;
                                }

                                //if (cpetype == "Wifi router")
                                //{
                                //	wfcount = wfcount + 1;
                                //}
                            }
                        }
                    }
                    if (wfcount >= 2)
                    {
                        _MESH = "Y";
                    }
                    else
                    {
                        if (iotcount > 0 && packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.SNPattern == "S"))
                        {
                            _MESH = "Y"; //Borrow parameter for IOT equipment sn pattern = S >> call p_upsert_maintanance_order 
                        }
                        else
                        {
                            _MESH = "N";
                        }

                        //_MESH = "N";
                    }
                }

                if (model.OrderType.ToUpper() == "UPDATEPROFILE")
                {
                    string ctype = string.Empty; string salep = string.Empty;
                    if (packageMappingObjectModel.FBB_PRODUCT_LIST.Count() == 1)
                    {
                        if (packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.MovementType == "Old SN"))
                        {
                            _upf = "MA";
                        }
                        else
                        {
                            foreach (var d in packageMappingObjectModel.FBB_PRODUCT_LIST)
                            {
                                salep = d.SNPattern.ToSafeString();
                                ctype = getcPEtYPE(d.MaterialCode.ToSafeString());
                                if (ctype == "Wifi router" && salep == "S")
                                {
                                    _upf = "MESH";
                                }
                                else
                                {
                                    _upf = "NEW";

                                }

                            }
                        }

                    }
                    else
                    {


                        //  _symptomgroup = "NEW";
                        if (packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.MovementType == "Old SN"))
                        {
                            _upf = "MAMESH";
                        }
                        else
                        {

                            foreach (var d in packageMappingObjectModel.FBB_PRODUCT_LIST)
                            {
                                salep = d.SNPattern.ToSafeString();
                                ctype = getcPEtYPE(d.MaterialCode.ToSafeString());
                                if (ctype == "Wifi router" && salep == "S")
                                {
                                    _upf = "MESH";
                                }
                                else
                                {
                                    _upf = "MAMESH";
                                    break;
                                }

                            }


                        }
                    }
                }

                if (model.OrderType.ToUpper() == "HUMANERROR")
                {
                    var _hgroup = (from c in _AssetReason.Get() select c);
                    if ((model.RejectReason != null) || (model.RejectReason != ""))
                    {
                        _hgroup = (from c in _hgroup where (c.LONG_VALUE == model.RejectReason) select c);
                    }
                    List<SYMPTOM> result = (from c in _hgroup
                                            select new SYMPTOM
                                            {
                                                SYMPTOM_CODE = c.REASON_CODE,
                                                LONG_VALUE = c.LONG_VALUE,
                                                DISPLAY_VALUE = c.DISPLAY_VALUE,
                                                SYMPTOM_GROUP = c.SYMPTOM_GROUP
                                            }).ToList();
                    if (result.Count > 0)
                    {
                        foreach (var dd in result)
                        {
                            _humamgroup = dd.SYMPTOM_GROUP;
                        }
                    }
                    else
                    {
                        _humamgroup = "";
                    }
                }

                if (model.OrderType.ToUpper() == "MA")
                {
                    var config = (from c in _symptom.Get() select c);

                    if ((model.RejectReason != null) || (model.RejectReason != ""))
                    {

                        config = (from c in config where (c.SYMPTOM_NAME == model.RejectReason) select c);

                    }

                    List<SYMPTOM> result = (from c in config
                                            select new SYMPTOM
                                            {
                                                SYMPTOM_CODE = c.SYMPTOM_CODE,
                                                LONG_VALUE = c.SUB_CATEGORY,
                                                DISPLAY_VALUE = c.CATEGORY,
                                                SYMPTOM_GROUP = c.SYMPTOM_GROUP
                                            }).ToList();

                    if (result.Count > 0)
                    {
                        foreach (var dd in result)
                        {
                            _symptomgroup = dd.SYMPTOM_GROUP;
                        }
                        if (_symptomgroup == "MA_CAMP")
                        {
                            int oldsn = 0; int newsn = 0; string mtype = string.Empty;
                            foreach (var d in packageMappingObjectModel.FBB_PRODUCT_LIST)
                            {
                                mtype = d.MovementType.ToSafeString();
                                if (mtype != "")
                                {
                                    if (mtype == "MA (Old SN)")
                                    {
                                        oldsn = oldsn + 1;
                                    }
                                    else
                                    {
                                        newsn = newsn + 1;
                                    }
                                }
                            }
                            if (oldsn > 0 && newsn == 0)
                            {
                                _symptomgroup = "MA_REAL";
                            }
                            if (oldsn == 0 && newsn > 0)
                            {
                                if (packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.SNPattern == "S"))
                                {
                                    _symptomgroup = "MA_CAMP";
                                }
                                else
                                {
                                    _symptomgroup = "NEW";
                                }

                                //_symptomgroup = "NEW";
                            }
                            if (oldsn > 0 && newsn > 0)
                            {
                                _symptomgroup = "MA_CAMP";
                            }
                        }


                    }
                    else
                    {

                        var flag = _cfgLov.Get().FirstOrDefault(w =>
                        w.LOV_TYPE.Equals("WS_NewRegistForSubmitFOA") &&
                        w.LOV_NAME.Equals("SOA_EIR") &&
                        w.ACTIVEFLAG.Equals("Y"));
                        if (flag != null)
                        {
                            _symptomgroup = "MA_REAL";
                        }
                        else
                        {
                            //  _symptomgroup = "NEW";
                            if (packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.MovementType == "MA (Old SN)"))
                            {
                                _symptomgroup = "MA_REAL";
                            }
                            else
                            {
                                // _symptomgroup = "NEW";
                                hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB_MA");

                                UpdateHistoryLog(_uow, _hisLog, model, hLog, "", "OUT_FOA", "ProductList not Match", null);
                                UpdateSubmitFoaErrorLog(model.OrderNumber, model.RejectReason, model.OrderType, sn_no, model.Access_No, model.UserName);
                                return NewRegistForSubmitFOAResponseResult;
                            }
                        }

                    }




                }
                if (model.OrderType.ToUpper() == "JOIN")
                {

                    //  _symptomgroup = "NEW";
                    if (packageMappingObjectModel.FBB_PRODUCT_LIST.Any(m => m.MovementType == "911"))
                    {
                        _jointype = "OLD";
                    }
                    else
                    {
                        _jointype = "NEW";
                    }

                }


                if ((model.OrderType.ToUpper().ToSafeString() == "MA" && _symptomgroup == "MA_REAL") ||
                    (model.OrderType.ToUpper().ToSafeString() == "MA" && _symptomgroup == "MA_CAMP") ||
                    (model.OrderType.ToUpper().ToSafeString() == "JOIN" && _jointype == "OLD") ||
                      //model.OrderType.ToUpper().ToSafeString() == "RENEW_T" ||
                     model.OrderType.ToUpper().ToSafeString() == "RELOCATE" ||
                      model.OrderType.ToUpper().ToSafeString() == "WRITEOFF" ||
                    (model.OrderType.ToUpper().ToSafeString() == "HUMANERROR" && (!_humamgroup.Equals("HUMAN_NEW"))) ||
                     (model.OrderType.ToUpper().ToSafeString() == "NEW" && _MESH != "N") ||
                      (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "MA") ||
                      (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "MAMESH") ||
                       (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "MESH") ||
                    model.OrderType.ToUpper().ToSafeString() == "RETURNED")

                {
                    hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB_MA");

                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package : p_upsert_maintenance_order", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                    #region WebService >>> p_upsert_Maintain_order


                    var result = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_upsert_maintenance_order",
                          new object[]
                      {
                          //Parameter Input
                          p_ACCESS_NUMBER,
                          p_ORDER_NO,
                          p_ORDER_TYPE,
                          p_SUBCONTRACT_CODE,
                          p_SUBCONTRACT_NAME,
                          p_PRODUCT_NAME,
                          p_SERVICE_LIST,

                          //!* Product Array
                          packageMapping,

                          p_SUBMIT_FLAG,
                          p_Reject_reason,
                          p_foa_submit_date,
                          p_post_date,
                          p_olt_name,
                          p_building_name,
                          p_mobile_contact,
                          p_addess_id,
                          p_org_id,
                          p_Reuse_Flag,
                          p_Event_Flow_Flag,
                          
                          ////add new 18.06.28
                          p_Subcontract_Type,
                          p_Subcontract_Sub_Type,
                          p_Request_Sub_Flag,
                          p_Sub_Access_Mode,
                          ////end add new 18.06.28
                          ///
                          ///New Param
                          ///   
                          p_product_owner,
                          p_main_promo_code,
                          p_team_id,


                          //Parameter Output
                          
                          p_ws_maintain_cur,

                          ret_code,
                          ret_msg
                      });
                    string Trans_ID = getTran(model.Access_No, model.OrderNumber);

                    UpdateHistoryLog(_uow, _hisLog, "", hLog, Trans_ID, "INSTALLATION", null, null);
                    if (result != null)
                    {
                        executeResults.ret_code = result[1] != null ? result[1].ToSafeString() : "-1";
                        executeResults.ret_msg = result[2] != null ? result[2].ToSafeString() : "";
                        packageMsg = executeResults.ret_msg.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();

                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, "", "");

                        if (!executeResults.ret_code.Equals("0"))
                        {
                            UpdateHistoryLog(_uow, _hisLog, result, hLog, Trans_ID, "ERROR", "ReturnCode:" + executeResults.ret_code + " ReturnMSG:" + executeResults.ret_msg, null);
                            //return NewRegistForSubmitFOAResponseResult;
                        }

                        DataTable dtMaintainFOARespones = (DataTable)result[0];
                        List<FBSS_SubmitFOAMaintainResponse> ListMaintainFOARespones = new List<FBSS_SubmitFOAMaintainResponse>();
                        ListMaintainFOARespones = dtMaintainFOARespones.DataTableToList<FBSS_SubmitFOAMaintainResponse>();
                        executeResults.p_ws_maintain_cur = ListMaintainFOARespones;


                        if (executeResults.p_ws_maintain_cur.Count > 0)
                        {
                            int countRowMain = 0;
                            foreach (var item in executeResults.p_ws_maintain_cur)
                            {
                                if (item.TRANS_ID == null || item.TRANS_ID == "")
                                {
                                    countRowMain++;
                                }
                            }
                            if (countRowMain.Equals(executeResults.p_ws_maintain_cur.Count))
                            {
                                executeResults.p_ws_maintain_cur = new List<FBSS_SubmitFOAMaintainResponse>();
                            }
                        }


                        if (executeResults.p_ws_maintain_cur.Count <= 0)
                        {
                            flagMASap = false;
                        }


                        _logger.Info("End pkg_fbb_foa_order_management " + executeResults.ret_msg);

                    }
                    else
                    {
                        NewRegistForSubmitFOAResponseResult.result = "Package Data Return Null!";
                        UpdateHistoryLog(_uow, _hisLog, "", hLog, "", "Data Null", ret_msg.Value.ToSafeString(), null);
                        NewRegistForSubmitFOAResponseResult.result = ""; //clear
                                                                         //return NewRegistForSubmitFOAResponseResult;
                    }

                    FBSS_SubmitFOAMaintainResponse maintainRespone = new FBSS_SubmitFOAMaintainResponse();


                    if (executeResults.p_ws_maintain_cur.Count > 0)
                    {
                        foreach (var d in executeResults.p_ws_maintain_cur)
                        {
                            maintainRespone.TRANS_ID = d.TRANS_ID.ToSafeString();
                        }
                        // maintainRespone = executeResults.p_ws_maintain_cur.ElementAt(0);
                    }


                    string transactionID = string.Empty;
                    if (maintainRespone.TRANS_ID != null || maintainRespone.TRANS_ID.ToSafeString() != "")
                    {
                        transactionID = maintainRespone.TRANS_ID != null ? maintainRespone.TRANS_ID.ToSafeString() : "";
                    }
                    else
                    {
                        transactionID = Trans_ID;
                    }

                    string orderNO = model.OrderNumber;

                    //!* log
                    UpdateHistoryLog(_uow, _hisLog, executeResults, hLog, transactionID, "INSTALLATION", null, null);


                    if (model.OrderType.ToUpper().Equals("RENEW"))
                    {
                        UpdateHistoryLog(_uow, _hisLog, result, hLog, "", "SUCCESS", null, null);
                        // return NewRegistForSubmitFOAResponseResult;
                    }

                    ZINPUT_INVENTORY_TRANS_STRUC[] INPUT_INVENTORY_TRANS = null;
                    if (executeResults.p_ws_maintain_cur.Count > 0)
                    {
                        List<FBSS_SubmitFOAMaintainResponse> listMain = new List<FBSS_SubmitFOAMaintainResponse>();
                        foreach (var main in executeResults.p_ws_maintain_cur)
                        {
                            if (main.TRANS_ID != null && main.TRANS_ID != "")
                            {
                                listMain.Add(main);
                            }

                        }
                        if (listMain != null && listMain.Count > 0)
                        {
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
                                //
                                QUANTITY = ins.QUANTITY.ToSafeString(),
                                UOM = ins.UOM.ToSafeString(),
                                AMOUNT = Convert.ToDecimal(ins.AMOUNT),
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
                        }
                    }





                    ZOUTPUT_TRANS_ERROR_STRUC[] OUTPUT_TRANS_ERROR = null;


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

                    // update response from sap
                    string result_code = "0";
                    string result_msg = "";
                    string main_access = "";



                    if (flagMASap)
                    {
                        InterfaceLogPayGCommand sap_log = new InterfaceLogPayGCommand();
                        sap_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_in.ToSafeString(), transactionID, "call SAP.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");
                        //!* log
                        try
                        {

                            UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToSafeString(), hLog, "", "IN_SAP", null, null);

                            //  if (fixAssetCallSapMA.COM_CODE == "Y")
                            if (checkFlagByOrderType(model.OrderType, "MA") == "Y")
                            {

                                if (MAuserCredential == null) return new NewRegistForSubmitFOAResponse() { result = "08", errorReason = "Endpoint" };
                                var _sapMA = new FBBSAPOnlineMA2.SI_Z_MMIM_FBSS_TRANS_POSTService();


                                _sapMA.UseDefaultCredentials = true;
                                _sapMA.Timeout = 180000;

                                if (MAurlEndpoint != null) _sapMA.Url = MAurlEndpoint.DISPLAY_VAL;

                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                                if (CheckRollBack == "Y")
                                    ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                                
                                _sapMA.Credentials = new NetworkCredential(MAuserCredential.DISPLAY_VAL.ToSafeString(), MApasswordCredential.DISPLAY_VAL.ToSafeString());
                                var OUTPUT_TRANS_COMPLETE = _sapMA.SI_Z_MMIM_FBSS_TRANS_POST(INPUT_INVENTORY_TRANS, out OUTPUT_TRANS_ERROR);
                                //var xml_master_out = (Object)null;
                                //write xml outout 
                                //if (OUTPUT_TRANS_ERROR.Length > 0)
                                //{
                                //    xml_master_out = XDocument.Parse(OUTPUT_TRANS_ERROR.DumpToXml());
                                //}
                                //if (OUTPUT_TRANS_COMPLETE.Length > 0)
                                //{
                                //    xml_master_out = XDocument.Parse(OUTPUT_TRANS_COMPLETE.DumpToXml());


                                //}

                                List<XDocument> listMADocument = new List<XDocument>();
                                // List<XDocument> listXDocument = new List<XDocument>();
                                if (OUTPUT_TRANS_ERROR != null)
                                {
                                    listMADocument.Add(XDocument.Parse(OUTPUT_TRANS_ERROR.DumpToXml()));
                                }
                                if (OUTPUT_TRANS_COMPLETE != null)
                                {
                                    listMADocument.Add(XDocument.Parse(OUTPUT_TRANS_COMPLETE.DumpToXml()));
                                }


                                XDocument xml_master_out = null;
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



                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_out.ToSafeString(), sap_log, "Success", "", "");

                                //!* log
                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", null, null);

                                if (OUTPUT_TRANS_ERROR.Length > 0)
                                {

                                    string ins_msg = string.Empty; string tranid = string.Empty;
                                    foreach (var errorelement in OUTPUT_TRANS_ERROR)
                                    {
                                        tranid = errorelement.TRANS_ID.ToSafeString();
                                        errCode = errorelement.ERR_CODE.ToSafeString();
                                        errMsg = errorelement.ERR_MSG.ToSafeString();
                                        if (errCode == "9999" || errCode == "999" || errCode == "053")
                                        {
                                            NewRegistForSubmitFOAResponseResult.new_ErrorMsg += "|" + errMsg.ToSafeString();
                                        }
                                        if (SAPResponseValid(errCode, errMsg) && result_msg == "")
                                        {
                                            result_code = errCode;
                                            result_msg = errMsg;
                                            if (errCode == "9999" || errCode == "999")
                                            {
                                                NewRegistForSubmitFOAResponseResult.new_response = errCode;
                                            }
                                        }
                                        if (orderNO.ToSafeString() == "" || orderNO.ToSafeString() == null)
                                        {
                                            orderNO = getOrderNo(tranid);
                                        }
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

                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_code.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_msg.Value.ToSafeString());
                                        errMsg = "";
                                        errCode = "000";
                                    }

                                }



                                if (OUTPUT_TRANS_COMPLETE.Length > 0)
                                {
                                    string ins_msg = string.Empty; string tranid = string.Empty;

                                    foreach (var element in OUTPUT_TRANS_COMPLETE)
                                    {
                                        tranid = element.TRANS_ID.ToSafeString();

                                        //return value
                                        if (element.DOC_YEAR.ToSafeString() != "" && element.MATERIAL_DOC.ToSafeString() != "")
                                        {
                                            main_access = "success";
                                        }
                                        else
                                        {
                                            main_access = "";
                                        }

                                        // if (SAPResponseValid(errCode, errMsg) && result_msg == "")
                                        //{
                                        //     result_code = "0000";
                                        //     result_msg = errMsg;
                                        //}
                                        if (orderNO.ToSafeString() == "" || orderNO.ToSafeString() == null)
                                        {
                                            orderNO = getOrderNo(tranid);
                                        }
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

                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_code.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_maintenance_response " + ret_msg.Value.ToSafeString());
                                    }

                                }



                            }
                            else
                            {
                                SAPError = "Blockout FLAG CALL SAP MA";
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                            }
                        }
                        catch (Exception ex)
                        {

                            SAPError = ex.GetErrorMessage().ToSafeString();
                            //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", sap_log, "Fail", ex.GetErrorMessage().ToSafeString(), "");

                        }
                    }


                    string msg = "";
                    int indexSuccess = 0;
                    int indexError = 0;
                    if (result_msg == "" || result_msg == null || result_msg == "null")
                    {
                        result_msg = packageMsg;
                    }
                    NewRegistForSubmitFOAResponseResult.result = main_access;// result_code;
                    NewRegistForSubmitFOAResponseResult.errorReason = result_msg;
                    if (main_access != "") { result_msg = "Success"; } else { result_msg = errMsg; }
                    //if ((result_code.Equals("0") || result_code.Equals("000")) && result_msg.Trim().Equals("")) result_msg = "Success";
                    //if (result_code == "0" || result_code == "000" || result_code == "") indexSuccess += 1;
                    if (main_access != "") indexSuccess += 1;
                    else indexError += 1;
                    if (indexSuccess > 0 || indexError > 0)
                    {
                        msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToSafeString() + " Order. " : msg + "";
                        msg = indexError > 0 ? msg + "Error " + indexError.ToSafeString() + " Order. " : msg + "";
                    }
                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code == "0" ? "Success" : executeResults.ret_msg, msg, "");
                    //!* log
                    if (SAPError != "")
                    {
                        result_msg = SAPError;
                        SAPError = "";
                    }
                    UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);






                    #endregion

                }
                if (model.OrderType.ToUpper().ToSafeString() == "TERMINATION" ||
                    model.OrderType.ToUpper().ToSafeString() == "RELOCATE" ||
                    model.OrderType.ToUpper().ToSafeString() == "JOIN" && _jointype == "OLD" ||
                    // model.OrderType.ToUpper().ToSafeString() == "MA" && _symptomgroup == "MA_CAMP" ||
                    // model.OrderType.ToUpper().ToSafeString() == "RENEW_T" ||
                    model.OrderType.ToUpper().ToSafeString() == "RETURNED")
                {
                    hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB_REVALUE");

                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package : p_upsert_revalue_order", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                    #region WebService >>> p_upsert_revalue_order

                    var result = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_upsert_revalue_order",
                          new object[]
                      {
                          //Parameter Input
                          p_ACCESS_NUMBER,
                          p_ORDER_NO,
                          p_ORDER_TYPE,
                          p_SUBCONTRACT_CODE,
                          p_SUBCONTRACT_NAME,
                          p_PRODUCT_NAME,
                          p_SERVICE_LIST,

                          //!* Product Array
                          packageMapping,

                          p_SUBMIT_FLAG,
                          p_Reject_reason,
                          p_foa_submit_date,
                          p_post_date,
                          p_olt_name,
                          p_building_name,
                          p_mobile_contact,
                          p_addess_id,
                          p_org_id,
                          p_Reuse_Flag,
                          p_Event_Flow_Flag,
                          
                          ////add new 18.06.28
                          p_Subcontract_Type,
                          p_Subcontract_Sub_Type,
                          p_Request_Sub_Flag,
                          p_Sub_Access_Mode,
                          ////end add new 18.06.28
                          ///
                          ///New Param
                          ///   
                          p_product_owner,
                          p_main_promo_code,
                          p_team_id,
                          //Parameter Output
                          
                          p_ws_revalue_cur,

                          ret_code,
                          ret_msg
                      });

                    // update response from sap
                    string result_code = "0";
                    string result_msg = "";
                    string main_access = "";

                    if (result != null)
                    {
                        executeResults.ret_code = result[1] != null ? result[1].ToSafeString() : "-1";
                        executeResults.ret_msg = result[2] != null ? result[2].ToSafeString() : "";
                        packageMsg = executeResults.ret_msg.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();

                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, "", "");

                        if (!executeResults.ret_code.Equals("0"))
                        {
                            UpdateHistoryLog(_uow, _hisLog, result, hLog, "", "ERROR", "ReturnCode:" + executeResults.ret_code + " ReturnMSG:" + executeResults.ret_msg, null);
                        }

                        var dtSubmitFOARevalueRespones = (DataTable)result[0];
                        var ListRevalueFOARespones = new List<FBSS_SubmitFOARevalueResponse>();
                        ListRevalueFOARespones = dtSubmitFOARevalueRespones.DataTableToList<FBSS_SubmitFOARevalueResponse>();
                        executeResults.p_ws_revalue_cur = ListRevalueFOARespones;

                        executeResults.p_ws_revalue_cur.RemoveAll(
                            item => string.IsNullOrWhiteSpace(item.INTERNET_NO)
                                    || string.IsNullOrWhiteSpace(item.MAIN_ASSET));

                        if (executeResults.p_ws_revalue_cur.Count > 0)
                        {
                            var RevalueRespone = executeResults.p_ws_revalue_cur.ElementAt(0);
                            string transactionID = RevalueRespone.TRANS_ID != null ? RevalueRespone.TRANS_ID.ToSafeString() : "";
                            string orderNO = RevalueRespone.ORDER_NO != null ? RevalueRespone.ORDER_NO.ToSafeString() : "";
                            string accessNO = RevalueRespone.INTERNET_NO != null ? RevalueRespone.INTERNET_NO.ToSafeString() : "";

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

                            UpdateHistoryLog(_uow, _hisLog, executeResults, hLog, transactionID, "INSTALLATION", null, null);

                            ZFBSS_REVALUE_ASSET[] CREATE_ASSET_REVALUE = null;
                            var listMain = new List<FBSS_SubmitFOARevalueResponse>();
                            foreach (var main in executeResults.p_ws_revalue_cur)
                            {
                                if (main.TRANS_ID != null && main.TRANS_ID != "")
                                {
                                    listMain.Add(main);
                                }
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
                            }
                            if (listMain != null && listMain.Count > 0)
                            {

                                CREATE_ASSET_REVALUE = listMain.Select(ins => new ZFBSS_REVALUE_ASSET()
                                {
                                    TRANS_ID = ins.TRANS_ID.ToSafeString(),
                                    INTERNET_NO = ins.INTERNET_NO.ToSafeString(),
                                    ACTION = ins.ACTION.ToSafeString(),
                                    RUN_GROUP = ins.RUN_GROUP.ToSafeString(),
                                    MAIN_ASSET = ins.MAIN_ASSET.ToSafeString(),
                                    SUBNUMBER = ins.SUBNUMBER.ToSafeString(),
                                    COM_CODE = ins.COM_CODE.ToSafeString(),
                                    DOC_DATE = ins.DOC_DATE.ToSafeString(),
                                    POST_DATE = ins.POST_DATE.ToSafeString(),
                                    ASSET_VALUE_DATE = ins.ASSET_VALUE_DATE.ToSafeString(),
                                    REF_DOC_NO = ins.REF_DOC_NO.ToSafeString(),
                                    ITEM_TEXT = ins.ITEM_TEXT.ToSafeString()
                                }).ToArray();
                            }

                            ZFBSS_FIXEDASSET_ASSET_OUT_REVALUE[] OUT_REVALUE_ASSET = null;
                            var listXDocument = new List<XDocument>
                            {
                                XDocument.Parse(TRANS_ID.DumpToXml())
                            };

                            if (CREATE_ASSET_REVALUE != null && CREATE_ASSET_REVALUE.Any())
                            {
                                listXDocument.Add(XDocument.Parse(CREATE_ASSET_REVALUE.DumpToXml()));
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

                            List<FBSS_SubmitFOARevalueResponse> listoutSAP = new List<FBSS_SubmitFOARevalueResponse>();

                            InterfaceLogPayGCommand sap_log = null;
                            sap_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_in.ToSafeString(), transactionID, "call SAP.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                            try
                            {
                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToSafeString(), hLog, "", "IN_SAP", null, null);

                                if (checkFlagByOrderType(model.OrderType, "REVALUE") == "Y")
                                {
                                    if (REVuserCredential == null) return new NewRegistForSubmitFOAResponse() { result = "08", errorReason = "Endpoint" };
                                    var _SAP = new FBBSAPOnlineRevalue.SI_Z_FIAM_FBSS_REVALUATION_ASSETService
                                    {
                                        UseDefaultCredentials = true
                                    };
                                    _SAP.Timeout = 180000;

                                    if (REVurlEndpoint != null) _SAP.Url = REVurlEndpoint.DISPLAY_VAL;

                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                    
                                    if (CheckRollBack == "Y")
                                        ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                                    _SAP.Credentials = new NetworkCredential(REVuserCredential.DISPLAY_VAL.ToSafeString(), REVpasswordCredential.DISPLAY_VAL.ToSafeString());
                                    if (CREATE_ASSET_REVALUE.Length > 0)
                                    {
                                        string ins_msg = string.Empty;

                                        foreach (var element in CREATE_ASSET_REVALUE)
                                        {
                                            ACTION = element.ACTION;
                                            INTERNET_NO = element.INTERNET_NO;
                                            MAIN_ASSET = element.MAIN_ASSET;
                                            RUN_GROUP = element.RUN_GROUP;
                                            SUBNUMBER = element.SUBNUMBER;
                                            TRANS_ID = element.TRANS_ID;
                                            ASSET_VALUE_DATE = element.ASSET_VALUE_DATE;
                                            COM_CODE = element.COM_CODE;
                                            DOC_DATE = element.DOC_DATE;
                                            ITEM_TEXT = element.ITEM_TEXT;
                                            POST_DATE = element.POST_DATE;
                                            REF_DOC_NO = element.REF_DOC_NO;

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
                                        }
                                    }
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

                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_out.ToSafeString(), sap_log, "Success", "", "");

                                    UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", null, null);

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

                                            main_access = element.NUMBER_MESSAGE.ToSafeString() == "000" ? "success" : "";

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
                                                p_ORDER_NO = orderNO,
                                                ret_code = ret_code,
                                                ret_msg = ret_msg

                                            }).ToList();

                                            _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_code.Value.ToSafeString());
                                            _logger.Info("End pkg_fbb_foa_order_management.p_revalue_response " + ret_msg.Value.ToSafeString());
                                        }
                                    }
                                }
                                else
                                {
                                    SAPError = "Blockout FLAG CALL SAP_Revalue";
                                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                                }
                                if (result_code == "000")
                                {
                                    result_msg = "Success";
                                }

                                string msg = "";
                                int indexSuccess = 0;
                                int indexError = 0;
                                if (string.IsNullOrEmpty(result_msg) || result_msg == "null")
                                {
                                    result_msg = packageMsg;
                                }
                                NewRegistForSubmitFOAResponseResult.result = main_access;// result_code;
                                NewRegistForSubmitFOAResponseResult.errorReason = result_msg;
                                if (main_access != "")
                                {
                                    result_msg = "Success";
                                    indexSuccess += 1;
                                }
                                else
                                {
                                    result_msg = errMsg;
                                    indexError += 1;
                                }

                                if (indexSuccess > 0 || indexError > 0)
                                {
                                    msg = indexSuccess > 0 ? msg + "Success " + indexSuccess + " Order. " : msg + "";
                                    msg = indexError > 0 ? msg + "Error " + indexError + " Order. " : msg + "";
                                }
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, result_msg, msg, "");
                                if (SAPError != "")
                                {
                                    result_msg = SAPError;
                                    SAPError = "";
                                }
                                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);
                            }
                            catch (Exception ex)
                            {
                                SAPError = ex.GetErrorMessage().ToSafeString();
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", sap_log, "Fail", ex.GetErrorMessage().ToSafeString(), "");
                            }
                        }

                        _logger.Info("End pkg_fbb_foa_order_management " + executeResults.ret_msg);
                    }
                    else
                    {
                        NewRegistForSubmitFOAResponseResult.result = "Package Data Return Null!";
                        UpdateHistoryLog(_uow, _hisLog, "", hLog, "", "Data Null", "Package Data Return Null!", null);
                        NewRegistForSubmitFOAResponseResult.result = "";
                    }

                    #endregion
                }

                if (model.OrderType.ToUpper().ToSafeString() == "PRE-TERMINATION" ||
                    (model.OrderType.ToUpper().ToSafeString() == "NEW" && _MESH == "Y") ||
                  (model.OrderType.ToUpper().ToSafeString() == "NEW" && _MESH == "N") ||
                   model.OrderType.ToUpper().ToSafeString() == "RENEW" ||
                   model.OrderType.ToUpper().ToSafeString() == "RENEW_R" ||
                    model.OrderType.ToUpper().ToSafeString() == "RELOCATE" ||
                    model.OrderType.ToUpper().ToSafeString() == "RESEND_INS" ||
                  model.OrderType.ToUpper().ToSafeString() == "RENEW_T" ||
                   (model.OrderType.ToUpper().ToSafeString() == "JOIN" && _jointype == "OLD") ||
                 (model.OrderType.ToUpper().ToSafeString() == "JOIN" && _jointype == "NEW") ||
                (model.OrderType.ToUpper().ToSafeString() == "MA" && _symptomgroup == "NEW") ||
                (model.OrderType.ToUpper().ToSafeString() == "MA" && _symptomgroup == "MA_CAMP") ||
                  (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "MAMESH") ||
                  (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "NEW") ||
                (model.OrderType.ToUpper().ToSafeString() == "HUMANERROR" && (_humamgroup.Equals("HUMAN_NEW"))) ||
                (model.OrderType.ToUpper().ToSafeString() == "UPDATEPROFILE" && _upf == "MESH"))
                {
                    hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB_NEW");

                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package : p_upsert_foa_order", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                    #region WebService >>> p_upsert_foa_order


                    var result = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_upsert_foa_order",
                          new object[]
                      {
                          //Parameter Input
                          p_ACCESS_NUMBER,
                          p_ORDER_NO,
                          p_ORDER_TYPE,
                          p_SUBCONTRACT_CODE,
                          p_SUBCONTRACT_NAME,
                          p_PRODUCT_NAME,
                          p_SERVICE_LIST,

                          //!* Product Array
                          packageMapping,

                          p_SUBMIT_FLAG,
                          p_Reject_reason,
                          p_foa_submit_date,
                          p_post_date,
                          p_olt_name,
                          p_building_name,
                          p_mobile_contact,
                          p_addess_id,
                          p_org_id,
                          p_Reuse_Flag,
                          p_Event_Flow_Flag,
                          
                          ////add new 18.06.28
                          p_Subcontract_Type,
                          p_Subcontract_Sub_Type,
                          p_Request_Sub_Flag,
                          p_Sub_Access_Mode,
                          ////end add new 18.06.28
                          ///

                          ///New Param
                          ///   
                          p_product_owner,
                          p_main_promo_code,
                          p_team_id,
                          //Parameter Output
                          
                          p_ws_main_cur,
                          p_ws_inv_cur,
                          p_ws_ins_cur,
                          p_ws_sff_cur,
                          ret_code,
                          ret_msg
                      });

                    string Trans_ID = getTran(model.Access_No, model.OrderNumber);
                    UpdateHistoryLog(_uow, _hisLog, "", hLog, Trans_ID, "INSTALLATION", null, null);
                    // UpdateHistoryLog(_uow, _hisLog, "", hLog, Trans_ID, null, null, null);


                    if (result != null)
                    {

                        executeResults.ret_code = result[4] != null ? result[4].ToSafeString() : "-1";
                        executeResults.ret_msg = result[5] != null ? result[5].ToSafeString() : "";
                        packageMsg = executeResults.ret_msg.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();
                        NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();

                        InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, "p_upsert_foa_order", "p_upsert_foa_order");

                        if (!executeResults.ret_code.Equals("0"))
                        {
                            UpdateHistoryLog(_uow, _hisLog, result, hLog, Trans_ID, "ERROR", "ReturnCode:" + executeResults.ret_code + " ReturnMSG:" + executeResults.ret_msg, null);
                            //return NewRegistForSubmitFOAResponseResult;
                        }

                        DataTable dtSubmitFOAMainRespones = (DataTable)result[0];
                        List<FBSS_SubmitFOAMainRespones> ListSubmitFOARespones = new List<FBSS_SubmitFOAMainRespones>();
                        ListSubmitFOARespones = dtSubmitFOAMainRespones.DataTableToList<FBSS_SubmitFOAMainRespones>();
                        executeResults.p_ws_main_cur = ListSubmitFOARespones;

                        DataTable dtSubmitFOAInvRespones = (DataTable)result[1];
                        List<FBSS_SubmitFOAInvRespones> ListSubmitFOAInvRespones = new List<FBSS_SubmitFOAInvRespones>();
                        ListSubmitFOAInvRespones = dtSubmitFOAInvRespones.DataTableToList<FBSS_SubmitFOAInvRespones>();
                        executeResults.p_ws_inv_cur = ListSubmitFOAInvRespones;

                        DataTable dtSubmitFOAInsRespones = (DataTable)result[2];
                        List<FBSS_SubmitFOAInsRespones> ListSubmitFOAInsRespones = new List<FBSS_SubmitFOAInsRespones>();
                        ListSubmitFOAInsRespones = dtSubmitFOAInsRespones.DataTableToList<FBSS_SubmitFOAInsRespones>();
                        executeResults.p_ws_ins_cur = ListSubmitFOAInsRespones;

                        DataTable dtFBSS_SFFRespones = (DataTable)result[3];
                        List<FBSS_SFFRespones> ListSFFRespones = new List<FBSS_SFFRespones>();
                        ListSFFRespones = dtFBSS_SFFRespones.DataTableToList<FBSS_SFFRespones>();
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
                        NewRegistForSubmitFOAResponseResult.result = "Package Data Return Null!";
                        UpdateHistoryLog(_uow, _hisLog, "", hLog, Trans_ID, "Data Null", "Package Data Return Null!", null);
                        NewRegistForSubmitFOAResponseResult.result = ""; //clear
                                                                         //return NewRegistForSubmitFOAResponseResult;
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
                    string ordforsubnuber = null;
                    string orderNO = null;
                    if (invRespone.REF_DOC_NO != null && invRespone.REF_DOC_NO == "COMPLETE")
                    {
                        orderNO = insRespone.REF_DOC_NO.ToSafeString();
                        ordforsubnuber = insRespone.REF_DOC_NO.ToSafeString();
                    }
                    else if (invRespone.REF_DOC_NO != null)
                    {
                        orderNO = invRespone.REF_DOC_NO.ToSafeString();
                        ordforsubnuber = insRespone.REF_DOC_NO.ToSafeString();
                    }
                    else
                    {
                        orderNO = insRespone.REF_DOC_NO.ToSafeString();
                        ordforsubnuber = insRespone.REF_DOC_NO.ToSafeString();
                    }
                    string accessNO = mainRespone.INTERNET_NO != null ? mainRespone.INTERNET_NO.ToSafeString() : invRespone.INTERNET_NO != null ? invRespone.INTERNET_NO.ToSafeString()
                        : insRespone.INTERNET_NO.ToSafeString();
                    if (transactionID == "" || transactionID == null)
                    {
                        transactionID = Trans_ID;
                    }

                    //!* log
                    UpdateHistoryLog(_uow, _hisLog, executeResults, hLog, transactionID, "INSTALLATION", null, null);

                    //var User = "PIAPPLUSER_FBB";
                    //var password = "FBB@1234";
                    if (model.OrderType.ToUpper().Equals("RENEW"))
                    {
                        UpdateHistoryLog(_uow, _hisLog, result, hLog, "", "SUCCESS", null, null);
                        //return NewRegistForSubmitFOAResponseResult;
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
                                REF_DOC_NO = ins.ORDER_NO.ToSafeString(),
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
                        var curAmount = checkAmount(model.SubcontractorName.ToSafeString(), listIns.ElementAt(0).FLAG_TYPE.ToSafeString());
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
                                // ORDER_NO = inv.ORDER_NO.ToSafeString(),
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
                        try
                        {

                            UpdateHistoryRawLog(_uow, _hisLog, xml_master_in.ToSafeString(), hLog, "", "IN_SAP", null, null);

                            if (checkFlagByOrderType(model.OrderType, "NEW") == "Y")
                            {
                                if (userCredential == null) return new NewRegistForSubmitFOAResponse() { result = "08", errorReason = "Endpoint" };

                                var _sap = new SAPFixedAssetService.SI_Z_FIAM_FBSS_FIXEDASSETService();
                                _sap.UseDefaultCredentials = true;
                                _sap.Timeout = 180000;

                                if (urlEndpoint != null) _sap.Url = urlEndpoint.DISPLAY_VAL;

                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

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
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, xml_master_out.ToSafeString(), sap_log, "Success", "", "");
                                string SUBNUMBER = "0000";
                                //!* log
                                UpdateHistoryRawLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", null, null);

                                if (OUT_CREATE_ASSET.Length > 0)
                                {
                                    string ins_msg = string.Empty;

                                    foreach (var element in OUT_CREATE_ASSET)
                                    {
                                        if (element.NUMBER_MESSAGE.ToSafeString() == "9999" || element.NUMBER_MESSAGE.ToSafeString() == "999" || element.NUMBER_MESSAGE.ToSafeString() == "053")
                                        {
                                            NewRegistForSubmitFOAResponseResult.new_ErrorMsg += "|" + element.MESSAGE.ToSafeString();
                                        }
                                        if (SAPResponseValid(element.NUMBER_MESSAGE.ToSafeString(), element.NUMBER_MESSAGE.ToSafeString()) && result_msg == "")
                                        {
                                            result_code = element.NUMBER_MESSAGE.ToSafeString();
                                            result_msg = element.MESSAGE.ToSafeString();
                                            if (element.NUMBER_MESSAGE.ToSafeString() == "9999" || element.NUMBER_MESSAGE.ToSafeString() == "999")
                                            {
                                                NewRegistForSubmitFOAResponseResult.new_response = element.NUMBER_MESSAGE.ToSafeString();
                                            }
                                        }

                                        if (ordforsubnuber == "COMPLETE")
                                        {
                                            SUBNUMBER = "COMPLETE";
                                        }
                                        else
                                        {
                                            SUBNUMBER = "0000";
                                        }
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallOUT");
                                        //var updateResponse = _objService.ExecuteReadStoredProc(StoreName,
                                        if (orderNO == "COMPLETE")
                                        {
                                            orderNO = getOrderNo(element.TRANS_ID.ToSafeString());


                                        }



                                        var updateResponse = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_upsert_foa_response",
                                        new
                                        {
                                            p_TRANS_ID = element.TRANS_ID.ToSafeString(),
                                            p_REC_TYPE = element.REC_TYPE.ToSafeString(),
                                            p_RUN_GROUP = element.RUN_GROUP.ToSafeString(),
                                            //p_INTERNET_NO = executeResult.ElementAt(0).INTERNET_NO.ToSafeString(),
                                            p_INTERNET_NO = element.REFERENCE.ToSafeString(),
                                            p_ORDER_NO = orderNO,
                                            p_COM_CODE = element.COMPANY.ToSafeString(),
                                            p_ASSET_CODE = element.MAIN_ASSET.ToSafeString(),
                                            p_SUBNUMBER = SUBNUMBER.ToSafeString(),

                                            p_MATERIAL_NO = "",
                                            p_SERIAL_NO = "",
                                            p_MATERIAL_DOC = "",
                                            p_DOC_YEAR = "",
                                            p_ERR_CODE = element.NUMBER_MESSAGE.ToSafeString(),
                                            p_ERR_MSG = element.MESSAGE.ToSafeString(),

                                            //R21.03.2021
                                            p_REF_DOC_NO = "",
                                            ret_code = ret_code,
                                            ret_msg = ret_msg

                                        }).ToList();
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallLOG");
                                        //_logger.Info(StoreName + ret_code.Value.ToSafeString());
                                        //_logger.Info(StoreName + ret_msg.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_msg.Value.ToSafeString());
                                    }
                                    if (result_msg == "" || result_msg == null || result_msg == "null")
                                    {
                                        result_msg = "Success.";
                                    }
                                    UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);
                                    result_msg = "";
                                }

                                if (OUT_INS_TRANS_POST.Length > 0)
                                {
                                    string ins_msg = string.Empty;

                                    foreach (var element in OUT_INS_TRANS_POST)
                                    {
                                        if (element.NUMBER_MESSAGE.ToSafeString() == "9999" || element.NUMBER_MESSAGE.ToSafeString() == "999" || element.NUMBER_MESSAGE.ToSafeString() == "053")
                                        {
                                            NewRegistForSubmitFOAResponseResult.new_ErrorMsg += "|" + element.MESSAGE.ToSafeString();
                                        }
                                        if (SAPResponseValid(element.NUMBER_MESSAGE.ToSafeString(), element.NUMBER_MESSAGE.ToSafeString()) && result_msg == "")
                                        {
                                            result_code = element.NUMBER_MESSAGE.ToSafeString();
                                            result_msg = element.MESSAGE.ToSafeString();
                                            if (element.NUMBER_MESSAGE.ToSafeString() == "9999" || element.NUMBER_MESSAGE.ToSafeString() == "999")
                                            {
                                                NewRegistForSubmitFOAResponseResult.new_response = element.NUMBER_MESSAGE.ToSafeString();
                                            }
                                        }
                                        if (orderNO == "COMPLETE")
                                        {
                                            orderNO = getOrderNo(element.TRANS_ID.ToSafeString());

                                        }

                                        if (element.REF_DOC_NO.ToSafeString() == "COMPLETE")
                                        {
                                            SUBNUMBER = element.REF_DOC_NO.ToSafeString();
                                        }
                                        else
                                        {
                                            SUBNUMBER = "0000";
                                        }
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallOUT");
                                        //var updateResponse = _objService.ExecuteReadStoredProc(StoreName,
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
                                            p_SUBNUMBER = SUBNUMBER.ToSafeString(),
                                            p_MATERIAL_NO = "",
                                            p_SERIAL_NO = "",
                                            p_MATERIAL_DOC = element.DOCNO.ToSafeString(),
                                            p_DOC_YEAR = element.YEAR.ToSafeString(),
                                            p_ERR_CODE = element.NUMBER_MESSAGE.ToSafeString(),
                                            p_ERR_MSG = element.MESSAGE.ToSafeString(),

                                            //R21.03.2021
                                            p_REF_DOC_NO = element.REF_DOC_NO.ToSafeString(),
                                            ret_code = ret_code,
                                            ret_msg = ret_msg

                                        }).ToList();
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallLOG");
                                        //_logger.Info(StoreName + ret_code.Value.ToSafeString());
                                        //_logger.Info(StoreName + ret_msg.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_msg.Value.ToSafeString());
                                    }
                                    if (result_msg == "" || result_msg == null || result_msg == "null")
                                    {
                                        result_msg = "Success.";
                                    }
                                    UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);
                                    result_msg = "";
                                }

                                if (OUT_INV_TRANS_POST.Length > 0)
                                {
                                    string ins_mat = string.Empty;
                                    string ins_serial = string.Empty;
                                    string ins_msg = string.Empty;

                                    foreach (var element in OUT_INV_TRANS_POST)
                                    {
                                        if (element.ERR_CODE.ToSafeString() == "9999" || element.ERR_CODE.ToSafeString() == "999" || element.ERR_CODE.ToSafeString() == "053")
                                        {
                                            NewRegistForSubmitFOAResponseResult.new_ErrorMsg += "|" + element.ERR_MSG.ToSafeString();
                                        }
                                        if (SAPResponseValid(element.ERR_CODE.ToSafeString(), element.ERR_CODE.ToSafeString()) && result_msg == "")
                                        {
                                            result_code = element.ERR_CODE.ToSafeString();
                                            result_msg = element.ERR_MSG.ToSafeString();
                                            main_access = element.MAIN_ASSET.ToSafeString();
                                            if (element.ERR_CODE.ToSafeString() == "9999" || element.ERR_CODE.ToSafeString() == "999")
                                            {
                                                NewRegistForSubmitFOAResponseResult.new_response = element.ERR_CODE.ToSafeString();
                                            }
                                        }

                                        if (!ins_mat.Equals(element.MATERIAL_NO) && !ins_serial.Equals(element.SERIAL_NO))
                                        {
                                            ins_msg = element.ERR_MSG.ToSafeString();
                                        }
                                        if (orderNO == "COMPLETE")
                                        {
                                            orderNO = getOrderNo(element.TRANS_ID.ToSafeString());


                                        }

                                        if (element.REF_DOC_NO.ToSafeString() == "COMPLETE")
                                        {
                                            SUBNUMBER = element.REF_DOC_NO.ToSafeString();
                                        }
                                        else
                                        {
                                            SUBNUMBER = "0000";
                                        }
                                        ins_mat = element.MATERIAL_NO.ToSafeString();
                                        ins_serial = element.SERIAL_NO.ToSafeString();
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallOUT");
                                        //var updateResponse = _objService.ExecuteReadStoredProc(StoreName,
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
                                                p_SUBNUMBER = SUBNUMBER.ToSafeString(),
                                                p_MATERIAL_NO = element.MATERIAL_NO.ToSafeString(),
                                                p_SERIAL_NO = element.SERIAL_NO.ToSafeString(),
                                                p_MATERIAL_DOC = element.MATERIAL_DOC.ToSafeString(),
                                                p_DOC_YEAR = element.DOC_YEAR.ToSafeString(),
                                                p_ERR_CODE = element.ERR_CODE.ToSafeString(),
                                                p_ERR_MSG = element.ERR_MSG.ToSafeString(),

                                                //R21.03.2021
                                                p_REF_DOC_NO = element.REF_DOC_NO.ToSafeString(),
                                                ret_code = ret_code,
                                                ret_msg = ret_msg

                                            }).ToList();
                                        //StoreName = CallExecuteStored(model.OrderType.ToUpper().ToSafeString(), "CallLOG");
                                        //_logger.Info(StoreName + ret_code.Value.ToSafeString());
                                        //_logger.Info(StoreName + ret_msg.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_code.Value.ToSafeString());
                                        _logger.Info("End pkg_fbb_foa_order_management.p_upsert_foa_response " + ret_msg.Value.ToSafeString());
                                    }

                                }
                                if (result_msg == "" || result_msg == null || result_msg == "null")
                                {
                                    result_msg = "Success.";
                                }
                                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);
                                result_msg = "";
                                // UpdateHistoryLog(_uow, _hisLog, xml_master_out.ToSafeString(), hLog, "", "OUT_SAP", result_msg, null);
                            }
                            else
                            {
                                SAPError = "Blockout FLAG CALL SAP";
                                InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                            }
                        }
                        catch (Exception ex)
                        {
                            SAPError = ex.GetErrorMessage().ToSafeString();
                            //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, "", sap_log, "Fail", ex.GetErrorMessage().ToSafeString(), "");
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
                            var ordNo = model.OrderNumber;
                            var nonMobile = model.Access_No;
                            var ordType = model.OrderType;
                            var productName = model.ProductName;

                            var subcontractorName = model.SubcontractorName.ToSafeString();
                            var orgIDrow = from itemSubcontractorName in _subContractor.Get()
                                           where itemSubcontractorName.SUB_CONTRACTOR_NAME_TH == subcontractorName
                                           select itemSubcontractorName;

                            var orgId = orgIDrow.Any() ? orgIDrow.FirstOrDefault().ORG_ID.ToSafeString() : string.Empty;

                            var rejectReason = model.RejectReason.ToSafeString().ToUnicodeString();

                            //var submitDate = string.Empty;
                            //if (!string.IsNullOrEmpty(model.FOA_Submit_date))
                            //{
                            //    // DateTime dtsubmitDatee = DateTime.ParseExact(model.FOA_Submit_date, "yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
                            //    DateTime dtsubmitDatee = DateTime.ParseExact(model.FOA_Submit_date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            //    submitDate = dtsubmitDatee.ToSafeString("dd-MM-yyyy HH:mm:ss");
                            //}

                            var foaRequert = new SmsSurveyFOAWebService.FOAParam
                            {
                                ordNo = ordNo,
                                nonMobile = nonMobile,
                                ordType = ordType,
                                productName = productName,
                                orgID = orgId,
                                rejectReason = rejectReason,
                                submitDate = FOA_Submit_date
                            };

                            var smsSurveyFoa = new SmsSurveyFOAWebService.smsSurveyFOA
                            {
                                UseDefaultCredentials = true
                            };

                            if (urlSmsFoaEndpoint != null) smsSurveyFoa.Url = urlSmsFoaEndpoint.DISPLAY_VAL;

                            //ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                            //smsSurveyFoa.Credentials = new NetworkCredential(userSmsFoaCredential.DISPLAY_VAL.ToSafeString(), passwordSmsFoaCredential.DISPLAY_VAL.ToSafeString());

                            var interfaceNode = String.Format("{0}|{1}", "FOA", smsSurveyFoa.Url.ToSafeString());
                            //  var foaLog = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, foaRequert,
                            //      model.OrderNumber, "insertFOAInfo", "SmsSurveyFOA", nonMobile, interfaceNode, "Exs.");

                            try
                            {
                                var smsSurveyFoaResult = smsSurveyFoa.insertFOAInfo(foaRequert);

                                //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, smsSurveyFoaResult, foaLog,
                                //     smsSurveyFoaResult.IndexOf("Success", StringComparison.Ordinal) > 0 ? "Success" : "Failed",
                                //     "", "Exs.");
                            }
                            catch (Exception ex)
                            {
                                //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex.GetBaseException(), foaLog,
                                //     "Error", ex.Message, "Exs.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //  string msg = ex.ToSafeString();
                        //  var foaLog = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, new SmsSurveyFOAWebService.FOAParam(),
                        //   model.OrderNumber, "insertFOAInfo", "SmsSurveyFOA", model.Access_No, "FOA", "Exs.");

                        //    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, ex.GetBaseException(), foaLog,
                        //               "Error GetConfig", ex.Message, "Exs.");
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
                        //   sff_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, xml_SFF_in.ToSafeString(), transactionID, "call SFF.", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                        if (fixAssetCallSff.COM_CODE == "Y")
                        {
                            if (executeResults.p_ws_sff_cur != null && executeResults.p_ws_sff_cur.Count() > 0)
                            {
                                if (userSffCredential == null) return new NewRegistForSubmitFOAResponse() { result = "08", errorReason = "Endpoint" };

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
                                            _logger.Info("End Call SFF  ErrorReason" + "Service Sff is " + retuenmsg.ToSafeString());
                                        }
                                        else
                                        {
                                            _logger.Info("End Call SFF  Result" + "0");
                                            _logger.Info("End Call SFF  ErrorReason" + retuenmsg.ToSafeString());
                                        }

                                        //  InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, objResp, sff_log, returncode, "", "");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sff_log, "Block out", "", "");
                        }
                    }
                    string msg = "";
                    int indexSuccess = 0;
                    int indexError = 0;
                    if (result_msg == "" || result_msg == null || result_msg == "null")
                    {
                        if (packageMsg.ToSafeString() != "null")
                        {
                            result_msg = packageMsg;
                            UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);
                        }

                    }
                    NewRegistForSubmitFOAResponseResult.result = main_access;// result_code;
                    NewRegistForSubmitFOAResponseResult.errorReason = result_msg;
                    if (main_access != "") result_msg = "Success";
                    //if ((result_code.Equals("0") || result_code.Equals("000")) && result_msg.Trim().Equals("")) result_msg = "Success";
                    //if (result_code == "0" || result_code == "000" || result_code == "") indexSuccess += 1;
                    if (main_access != "") indexSuccess += 1;
                    else indexError += 1;
                    if (indexSuccess > 0 || indexError > 0)
                    {
                        msg = indexSuccess > 0 ? msg + "Success " + indexSuccess.ToSafeString() + " Order. " : msg + "";
                        msg = indexError > 0 ? msg + "Error " + indexError.ToSafeString() + " Order. " : msg + "";
                    }
                    //  var errorLog = string.Empty;
                    if (SAPError != "")
                    {
                        result_msg = SAPError;
                        SAPError = "";
                        UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "OUT_FOA", result_msg, null);

                    }

                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, result, log, executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg, msg, "");
                    //!* log

                    #endregion
                }


            }
            catch (Exception ex)
            {

                NewRegistForSubmitFOAResponseResult.result = "-1";
                NewRegistForSubmitFOAResponseResult.errorReason = ex.GetErrorMessage();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, NewRegistForSubmitFOAResponseResult, log, "PackageFail:" + model.OrderNumber, ex.GetErrorMessage().ToSafeString(), "");
                //!* log

                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "ERROR", ex.GetErrorMessage().ToSafeString(), null);
                //StartHistoryLog(_uow, _hisLog, result, "", "ERROR", ex.GetErrorMessage().ToSafeString(), "");
                NewRegistForSubmitFOAResponseResult.result = ""; //clear
            }
            UpdateSubmitFoaErrorLog(model.OrderNumber, model.RejectReason, model.OrderType, sn_no, model.Access_No, model.UserName);
            return NewRegistForSubmitFOAResponseResult;

        }
        public class getCPETYPE
        {
            public string CPE_TYPE { get; set; }
        }
        public string getcPEtYPE(string matcode)
        {
            string _cpetypey = string.Empty;


            var _ctype = (from c in _paygv.Get() select c);

            _ctype = (from c in _ctype where (c.MATERIAL_CODE == matcode) select c);


            var result = (from c in _ctype
                          select new getCPETYPE
                          {
                              CPE_TYPE = c.CPE_TYPE,

                          }).FirstOrDefault();
            if (result != null)
            {
                _cpetypey = result.CPE_TYPE.ToSafeString();
            }
            else
            {
                _cpetypey = "";
            }




            return _cpetypey;
        }
        public class GetORDER
        {
            public string ORDER_NO { get; set; }
        }
        public class GetTRANSID
        {
            public string TRANSID { get; set; }
            public DateTime? CREATE_DATE { get; set; }
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

        public string getTran(string accressno, string orderno)
        {
            string _tranID = string.Empty;
            var _hgroup = (from c in _tran.Get() select c);
            _hgroup = (from c in _hgroup where (c.INTERNET_NO == accressno && c.ORDER_NO == orderno && c.NEXT_TRAN_ID == null) select c);

            var Tranresult = (from c in _hgroup
                              select new GetTRANSID
                              {
                                  TRANSID = c.TRANS_ID,
                                  CREATE_DATE = (c.CREATE_DATE != null ? c.CREATE_DATE : c.MODIFY_DATE)
                              });

            if (Tranresult != null && Tranresult.Any())
            {
                _tranID = Tranresult.OrderByDescending(o => o.CREATE_DATE).FirstOrDefault().TRANSID.ToSafeString();
            }
            else
            {
                _tranID = "";
            }
            return _tranID;
        }
        public string checkFlagByOrderType(string ORDTYPE, string type)
        {
            var FixAssConfig = from item in _fixAssConfig.Get()
                               where item.PROGRAM_NAME == "FLAG_CALL_SAP" || item.PROGRAM_NAME == "FLAG_CALL_SAP_MA" || item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE" || item.PROGRAM_NAME == "FLAG_CALL_SAP_RETURN_MA" || item.PROGRAM_NAME == "FLAG_CALL_SAP_RETURN_REVALUE"
                               select item;
            var fixAssetCallSap = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP");
            var fixAssetCallSapRevalue = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE");
            var fixAssetCallSapMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_MA");
            var fixAssetCallSapRETURNMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_RETURN_MA");
            var fixAssetCallSapRETURNREVALUE = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_RETURN_REVALUE");


            string CHKFLAG = "Y";
            if (ORDTYPE == "RETURNED")
            {
                if (type == "MA")
                {
                    CHKFLAG = fixAssetCallSapRETURNMA.COM_CODE.ToSafeString();
                }
                if (type == "REVALUE")
                {
                    CHKFLAG = fixAssetCallSapRETURNREVALUE.COM_CODE.ToSafeString();
                }
            }
            else
            {
                if (type == "NEW")
                {
                    CHKFLAG = fixAssetCallSap.COM_CODE.ToSafeString();
                }
                if (type == "MA")
                {
                    CHKFLAG = fixAssetCallSapMA.COM_CODE.ToSafeString();
                }
                if (type == "REVALUE")
                {
                    CHKFLAG = fixAssetCallSapRevalue.COM_CODE.ToSafeString();
                }

            }
            return CHKFLAG;
        }

        #region Mapping PACKAGE_MAPPING_ARRAY Type Oracle

        public class PackageMappingObjectModel : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public Product_Mapping_ArrayMapping[] FBB_PRODUCT_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageMappingObjectModel Null
            {
                get
                {
                    PackageMappingObjectModel obj = new PackageMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, FBB_PRODUCT_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                FBB_PRODUCT_LIST = (Product_Mapping_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("WBB.FBB_PRODUCT_REC")]
        public class Package_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new Product_Mapping_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.FBB_PRODUCT_LIST")]
        public class ProductMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new Product_Mapping_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class Product_Mapping_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("SN")]
            public string SerialNumber { get; set; }

            [OracleObjectMappingAttribute("MATERIAL_CODE")]
            public string MaterialCode { get; set; }

            [OracleObjectMappingAttribute("COMPANY_CODE")]
            public string CompanyCode { get; set; }

            [OracleObjectMappingAttribute("PLANT")]
            public string Plant { get; set; }

            [OracleObjectMappingAttribute("STORAGE_LOCATION")]
            public string StorageLocation { get; set; }

            [OracleObjectMappingAttribute("SN_PATTERN")]
            public string SNPattern { get; set; }

            [OracleObjectMappingAttribute("MOVEMENT_TYPE")]
            public string MovementType { get; set; }

            #endregion Attribute Mapping

            public static Product_Mapping_ArrayMapping Null
            {
                get
                {
                    Product_Mapping_ArrayMapping obj = new Product_Mapping_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "SN", SerialNumber);
                OracleUdt.SetValue(con, udt, "MATERIAL_CODE", MaterialCode);
                OracleUdt.SetValue(con, udt, "COMPANY_CODE", CompanyCode);
                OracleUdt.SetValue(con, udt, "PLANT", Plant);
                OracleUdt.SetValue(con, udt, "STORAGE_LOCATION", StorageLocation);
                OracleUdt.SetValue(con, udt, "SN_PATTERN", SNPattern);
                OracleUdt.SetValue(con, udt, "MOVEMENT_TYPE", MovementType);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  PACKAGE_MAPPING_ARRAY Type Oracle


        #region Mapping PackageList_MAPPING_ARRAY Type Oracle

        public class PackageListMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public PkgMapping_ArrayMapping[] PAYG_PACKAGE_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageListMappingObjectModel Null
            {
                get
                {
                    PackageListMappingObjectModel obj = new PackageListMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, PAYG_PACKAGE_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                PAYG_PACKAGE_LIST = (PkgMapping_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("WBB.payg_package_rec")]
        public class Pkg_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new PkgMapping_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.PAYG_PACKAGE_LIST")]
        public class PkgMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageListMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new PkgMapping_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class PkgMapping_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("PACKAGE_CODE")]
            public string PackageCode { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_NAME")]
            public string PackageName { get; set; }

            [OracleObjectMappingAttribute("PACKAGE_CLASS")]
            public string PackageClass { get; set; }

            [OracleObjectMappingAttribute("IS_NEW")]
            public string IsNew { get; set; }

            [OracleObjectMappingAttribute("ORDER_NO")]
            public string OrderNo { get; set; }

            [OracleObjectMappingAttribute("ACCESS_NO")]
            public string AccessNo { get; set; }

            #endregion Attribute Mapping

            public static PkgMapping_ArrayMapping Null
            {
                get
                {
                    PkgMapping_ArrayMapping obj = new PkgMapping_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "PACKAGE_CODE", PackageCode);
                OracleUdt.SetValue(con, udt, "PACKAGE_NAME", PackageName);
                OracleUdt.SetValue(con, udt, "PACKAGE_CLASS", PackageClass);
                OracleUdt.SetValue(con, udt, "IS_NEW", IsNew);
                OracleUdt.SetValue(con, udt, "ORDER_NO", OrderNo);
                OracleUdt.SetValue(con, udt, "ACCESS_NO", AccessNo);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  PACKAGE_MAPPING_ARRAY Type Oracle


        #region Mapping PackageList_MAPPING_ARRAY Type Oracle

        public class PackageFeeItemMappingObjectModel : INullable, IOracleCustomType
        {
            [OracleArrayMapping()]
            public PkgFeeitemMapping_ArrayMapping[] PAYG_FEE_LIST { get; set; }

            private bool objectIsNull;

            public bool IsNull
            {
                get { return objectIsNull; }
            }

            public static PackageFeeItemMappingObjectModel Null
            {
                get
                {
                    PackageFeeItemMappingObjectModel obj = new PackageFeeItemMappingObjectModel();
                    obj.objectIsNull = true;
                    return obj;
                }
            }

            public void FromCustomObject(OracleConnection con, object udt)
            {
                OracleUdt.SetValue(con, udt, 0, PAYG_FEE_LIST);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                PAYG_FEE_LIST = (PkgFeeitemMapping_ArrayMapping[])OracleUdt.GetValue(con, udt, 0);
            }
        }

        [OracleCustomTypeMappingAttribute("WBB.PAYG_FEE_REC")]
        public class PkgFeeItem_MappingOracleTypeMappingFactory : IOracleCustomTypeFactory
        {
            public IOracleCustomType CreateObject()
            {
                return new PkgFeeitemMapping_ArrayMapping();
            }
        }

        [OracleCustomTypeMapping("WBB.PAYG_FEE_LIST")]
        public class PkgFeeItemMappingObjectModelFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
        {
            #region IOracleCustomTypeFactory Members

            public IOracleCustomType CreateObject()
            {
                return new PackageFeeItemMappingObjectModel();
            }

            #endregion IOracleCustomTypeFactory Members

            #region IOracleArrayTypeFactory Members

            public Array CreateArray(int numElems)
            {
                return new PkgFeeitemMapping_ArrayMapping[numElems];
            }

            public Array CreateStatusArray(int numElems)
            {
                return null;
            }

            #endregion IOracleArrayTypeFactory Members
        }

        public class PkgFeeitemMapping_ArrayMapping : Oracle.ManagedDataAccess.Types.INullable, IOracleCustomType
        {
            private bool objectIsNull;

            #region Attribute Mapping

            [OracleObjectMappingAttribute("FEE_ID")]
            public string FeeId { get; set; }

            [OracleObjectMappingAttribute("FEE_ID_TYPE")]
            public string FeeIdType { get; set; }

            [OracleObjectMappingAttribute("FEE_NAME")]
            public string FeeName { get; set; }

            [OracleObjectMappingAttribute("ORDER_FEE_PRICE")]
            public decimal OrderFeePrice { get; set; }

            
            [OracleObjectMappingAttribute("FEE_ACTION")]
            public string FeeAction { get; set; }

            [OracleObjectMappingAttribute("ORDER_NO")]
            public string OrderNo { get; set; }

            [OracleObjectMappingAttribute("ACCESS_NO")]
            public string AccessNo { get; set; }

            #endregion Attribute Mapping

            public static PkgFeeitemMapping_ArrayMapping Null
            {
                get
                {
                    PkgFeeitemMapping_ArrayMapping obj = new PkgFeeitemMapping_ArrayMapping();
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
                OracleUdt.SetValue(con, udt, "FEE_ID", FeeId);
                OracleUdt.SetValue(con, udt, "FEE_ID_TYPE", FeeIdType);
                OracleUdt.SetValue(con, udt, "FEE_NAME", FeeName);
                OracleUdt.SetValue(con, udt, "ORDER_FEE_PRICE", OrderFeePrice);
                OracleUdt.SetValue(con, udt, "FEE_ACTION", FeeAction);
                OracleUdt.SetValue(con, udt, "ORDER_NO", OrderNo);
                OracleUdt.SetValue(con, udt, "ACCESS_NO", AccessNo);
            }

            public void ToCustomObject(OracleConnection con, object udt)
            {
                throw new NotImplementedException();
            }
        }

        #endregion Mapping  PACKAGE_MAPPING_ARRAY Type Oracle



  

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

        public void UpdateSubmitFoaErrorLog(string orderno, string rejectreason, string ordertype, string serialno, string accessno, string usename)
        {
            string resendstatus = "Y";
            string updateby = "PAYG";
            string updateddesc = "Y = Done";
            var culture = CultureInfo.GetCultureInfo("en-US");

            ///L = Done By Batch Lost Transaction
            if (usename == "FOARESENDBATCH")
            {
                resendstatus = "L";
                updateby = "FOARESENDBATCH";
                updateddesc = "L = Done By Batch Lost Transaction";
            }
            DateTime dt = DateTime.Now;
            string updatedate = dt.ToString("dd/MM/yyyy", culture);
            var update = new UpdateSubmitFoaErrorLogCommand()
            {
                order_no = orderno,
                order_type = ordertype,
                reject_reason = rejectreason,
                serial_no = serialno,
                access_number = accessno,
                resend_status = resendstatus,
                updated_by = updateby,
                updated_desc = updateddesc,
            };
            _UpdateSubmitFoaError.Handle(update);
        }


    }
}
