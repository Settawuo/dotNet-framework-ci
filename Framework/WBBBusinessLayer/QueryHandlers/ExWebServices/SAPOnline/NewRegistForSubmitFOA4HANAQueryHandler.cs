using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Org.BouncyCastle.Utilities.Collections;
using Renci.SshNet.Messages.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Model;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Xml.Linq;
using WBBBusinessLayer.CommandHandlers.ExWebServices.SAPFixedAsset;
//using WBBBusinessLayer.FBBSAPOnlineMA;
using WBBBusinessLayer.FBBSAPOnlineMA2;
using WBBBusinessLayer.FBBSAPOnlineRevalue;
using WBBBusinessLayer.SAPFixedAssetService;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.SAPFixedAsset;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Model;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;
using static iTextSharp.text.pdf.AcroFields;
using static WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory.NewRegistForSubmitFOAQueryHandler;
using static WBBBusinessLayer.QueryHandlers.WebServices.GoodsMovementKAFKAQueryHandler;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory
{
    public class NewRegistForSubmitFOA4HANAQueryHandler : IQueryHandler<NewRegistForSubmitFOA4HANAQuery, NewRegistForSubmitFOAS4HANAResponse>
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
        private readonly IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> _queryProcessorGoodsMovementHandler;
        private readonly IEntityRepository<FBB_CFG_QUERY_REPORT> _cfgQueryReport;
        private readonly IFBBHVREntityRepository<object> _fbBhvREntity;

        public NewRegistForSubmitFOA4HANAQueryHandler(
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
            , IQueryHandler<GoodsMovementKAFKAQuery, GoodsMovementKAFKAQueryModel> queryProcessorGoodsMovementHandle
            , IEntityRepository<FBB_CFG_QUERY_REPORT> cfgQueryReport
            , IFBBHVREntityRepository<object> fbBhvREntity
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
            _queryProcessorGoodsMovementHandler = queryProcessorGoodsMovementHandle;
            _cfgQueryReport = cfgQueryReport;
            _fbBhvREntity = fbBhvREntity;
        }

        public NewRegistForSubmitFOAS4HANAResponse Handle(NewRegistForSubmitFOA4HANAQuery model)
        {
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            HistoryLogCommand hLog = new HistoryLogCommand();
            bool flagSap = true; bool flagRevalueSap = true; bool flagMASap = true;
            bool flagSff = true;
            string packageMsg = "";
            string sn_no = "";
            NewRegistForSubmitFOAS4HANAResponse NewRegistForSubmitFOAResponseResult = new NewRegistForSubmitFOAS4HANAResponse();
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
                //var fixAssetCallSff = resultFixAssConfig.FirstOrDefault(item => item.PROGRAM_CODE == "P010");
                //var FixAssConfig = from item in _fixAssConfig.Get()
                //                   where item.PROGRAM_NAME == "FLAG_CALL_SAP" || item.PROGRAM_NAME == "FLAG_CALL_SAP_MA" || item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE"
                //                   select item;
                //var fixAssetCallSap = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP");
                //var fixAssetCallSapRevalue = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_REVALUE");
                //var fixAssetCallSapMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "FLAG_CALL_SAP_MA");

                var CheckRollBack = (from r in _cfgLov.Get()
                                     where r.LOV_NAME == "ROLLBACK" && r.LOV_TYPE == "FBBPAYG_CERTSAP"
                                     select r.ACTIVEFLAG).FirstOrDefault().ToSafeString();

                // ----------------- Change Home Device ----------------------
                // date: 2025-09-02
                // name: New feature
                // owner: yotap
                // รอ condition, fix logic
                // -----------------------------------------------------------
                var feeitem = new FeeModel();
                var productitem = new List<ProductModel>();
                var feelist = new List<FeeListModel>();

                var pchange_main = "N";
                var prejectReason = false;

                var packagelistMappingObjectModel = new PackageListMappingObjectModel();
                var packageFeeItemMappingObjectModel = new PackageFeeItemMappingObjectModel();

                var cfgQueryReport = _cfgQueryReport.Get().Where(w => w.SHEET_NAME == "QUERY_PACKAGE_FEE").FirstOrDefault();

                if (model.SubmitFlag != "WEB_RESEND_FOA" && 
                    model.SubmitFlag != "WEB_RESEND_FOA_EQUIPMENT")
                {
                    //--------------- add on cr ---------------------
                    if (model.OrderType.ToSafeString().ToUpper() == "MA" && (model.RejectReason.ToSafeString() == ""))
                        prejectReason = true;

                    //_logger.Info($"Call NewRegistForSubmitFOAQueryHandler with cfgQueryReport: {cfgQueryReport.QUERY_1}");

                    var QueryForOrderNo = cfgQueryReport.QUERY_1.Replace("{order_no}", model.OrderNumber);
                    var hvr = new DataTable();

                    if (cfgQueryReport.QUERY_2.ToUpper() == "N")
                    {
                        //service_no,request_no,type,value
                        hvr.Columns.Add("service_no");
                        hvr.Columns.Add("request_no");
                        hvr.Columns.Add("type");
                        hvr.Columns.Add("value");
                    }
                    else
                    {
                      hvr = _fbBhvREntity.ExecuteToDataTable(QueryForOrderNo, "TableHRV");
                    }



                    var getlovChd = _cfgLov.Get().FirstOrDefault(l => l.LOV_TYPE == "QUERY_CONDITION" && l.LOV_NAME == "PAYG_TYPE" && l.ACTIVEFLAG == "Y");

                    var fieldsfeetxt = getlovChd.LOV_VAL1.Split(',');



                    var dataHVR = hvr.AsEnumerable()
                                     .Where(r => fieldsfeetxt.Any(f =>
                                                  r.Field<string>("type").Contains(f)))
                                    .Select(r => (Name: r.Field<string>("type"), Value: r.Field<string>("value")))
                                    .ToList();



                    foreach (var item in dataHVR)
                    {
                        if (item.Name == "FEE")
                        {
                            feeitem = JsonConvert.DeserializeObject<FeeModel>(item.Value);
                        }
                        else if (item.Name == "PRODUCT")
                            productitem = JsonConvert.DeserializeObject<List<ProductModel>>(item.Value);
                        else
                            feelist = JsonConvert.DeserializeObject<List<FeeListModel>>(item.Value);
                    }

                    // ----- สั้นลง: pivot → dict แล้ว map ใส่ model -----
                    var productList = productitem.AsEnumerable().Select(s => new ProductModel
                    {
                        PRODUCT_CLASS = s.PRODUCT_CLASS,
                        PRODUCT_NAME = s.PRODUCT_NAME,
                        PRODUCT_CODE = s.PRODUCT_CODE,
                        IS_NEW = s.IS_NEW,
                        ORDER_NO = model.OrderNumber,
                        ACCESS_NO = model.Access_No,
                    }).ToList();

                    var listFeeitem = new List<FeeItemModel>();

                    if (feeitem.Fee_ItemEntry_Fee != null)
                    {
                        var feetopkg = new FeeItemModel
                        {
                            FEE_NAME = feeitem.Fee_ItemEntry_Fee.Split('_')[1],
                            FEE_ID = feeitem.Fee_ItemEntry_Fee.Split('_')[0],
                            FEE_ID_TYPE = null,
                            ORDER_FEE_PRICE = feeitem.Fee_ItemEntry_Fee.Split('_').LastOrDefault().ToSafeDecimal(),
                            ORDER_NO = model.OrderNumber,
                            ACCESS_NO = model.Access_No,
                        };
                        listFeeitem.Add(feetopkg);

                    }
                    if (feeitem.Fee_ItemSoundbar_Fee != null)
                    {
                        var feeItemSoundbar = new FeeItemModel
                        {
                            FEE_NAME = feeitem.Fee_ItemSoundbar_Fee.Split('_')[1],
                            FEE_ID = feeitem.Fee_ItemSoundbar_Fee.Split('_')[0],
                            FEE_ID_TYPE = null,
                            ORDER_FEE_PRICE = feeitem.Fee_ItemSoundbar_Fee.Split('_').LastOrDefault().ToSafeDecimal(),
                            ORDER_NO = model.OrderNumber,
                            ACCESS_NO = model.Access_No,
                        };
                        listFeeitem.Add(feeItemSoundbar);

                    }
                    if (feeitem.Fee_ItemFiberhome_Fee != null)
                    {
                        var feeItemFiber = new FeeItemModel
                        {
                            FEE_NAME = feeitem.Fee_ItemFiberhome_Fee.Split('_')[1],
                            FEE_ID = feeitem.Fee_ItemFiberhome_Fee.Split('_')[0],
                            FEE_ID_TYPE = null,
                            ORDER_FEE_PRICE = feeitem.Fee_ItemFiberhome_Fee.Split('_').LastOrDefault().ToSafeDecimal(),
                            ORDER_NO = model.OrderNumber,
                            ACCESS_NO = model.Access_No,
                        };
                        listFeeitem.Add(feeItemFiber);

                    }


                    var feeItemtopkg = feelist.Select(s => new FeeItemModel
                    {
                        FEE_ID_TYPE = s.idType,
                        FEE_ID = s.idValue,
                        FEE_NAME =  null,
                        FEE_ACTION = s.action,
                        ORDER_FEE_PRICE = s.orderFeePrice,
                        ORDER_NO = model.OrderNumber,
                        ACCESS_NO = model.Access_No,
                    }).ToList();

                    //listFeeitem.Concat(feeItemtopkg).ToList();
                    listFeeitem.AddRange(feeItemtopkg);

                    // loop หา PackageType = Main และ  IsNew = Y
                    if (productList != null)
                    {
                        foreach (var item in productitem)
                        {
                            if (item.PRODUCT_CLASS.ToUpper() == "MAIN" && item.IS_NEW == "Y")
                            {
                                pchange_main = "Y";
                                model.Main_Promo_Code = item.PRODUCT_CODE;
                                break;
                            }
                        }
                    }

                    if (productList != null)
                    {
                        packagelistMappingObjectModel.PAYG_PACKAGE_LIST = productitem.Select(m => new PkgMapping_ArrayMapping
                        {
                            PackageClass = m.PRODUCT_CLASS.ToSafeString(),
                            PackageCode = m.PRODUCT_CODE.ToSafeString(),
                            PackageName = m.PRODUCT_NAME.ToSafeString(),
                            IsNew = m.IS_NEW.ToSafeString(),
                            OrderNo = model.OrderNumber.ToSafeString(),
                            AccessNo = model.Access_No.ToSafeString(),
                        }).ToArray();
                    }

                    if (listFeeitem != null)
                    {
                        packageFeeItemMappingObjectModel.PAYG_FEE_LIST = listFeeitem.Select(m=> new PkgFeeitemMapping_ArrayMapping
                        {
                            FeeId = m.FEE_ID.ToSafeString(),
                            FeeIdType = m.FEE_ID_TYPE.ToSafeString(),
                            FeeAction = m.FEE_ACTION.ToSafeString(),
                            FeeName = m.FEE_NAME.ToSafeString(),
                            OrderFeePrice = m.ORDER_FEE_PRICE,
                            OrderNo = model.OrderNumber.ToSafeString(),
                            AccessNo = model.Access_No.ToSafeString(),
                        }).ToArray();
                    }

                }
                else
                {
                    pchange_main = "N";
                    model.Main_Promo_Code = null;  // set p_main_promo_code = null
                }


                //if (packagelistMappingObjectModel == null)
                //{
                //    packagelistMappingObjectModel.PAYG_PACKAGE_LIST = new PkgMapping_ArrayMapping[0]
                //    {
                //            PackageClass = null,
                //            PackageCode = null
                //            PackageName = null,
                //            IsNew =null,
                //            OrderNo = null
                //            AccessNo = null
                //    };
                //}

                var packagelistMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_package_list", "WBB.PAYG_PACKAGE_LIST", packagelistMappingObjectModel);
                var packagefeeItemMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_fee_list", "WBB.PAYG_FEE_LIST", packageFeeItemMappingObjectModel);
              

                  // ----------------- End Change Home Device ----------------------

                  _logger.Info("Call NewRegistForSubmitFOAQueryHandler");



                var packageMappingObjectModel = new WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory.NewRegistForSubmitFOAQueryHandler.PackageMappingObjectModel();
                if (model.ProductList != null)
                {
                    packageMappingObjectModel.FBB_PRODUCT_LIST = model.ProductList.Select(m => new WBBBusinessLayer.QueryHandlers.ExWebServices.SAPInventory.NewRegistForSubmitFOAQueryHandler.Product_Mapping_ArrayMapping
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

                if (model.ProductList.Count != 0)
                {
                    sn_no = model.ProductList.First().SerialNumber.ToSafeString();
                }

                if (model.Access_No != null && (string.IsNullOrWhiteSpace(model.Product_Owner)))
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

                if (string.IsNullOrWhiteSpace(model.ProductName))
                {
                    model.ProductName = "FTTH";
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
                p_Reject_reason.Value = prejectReason ? "DEFAULT SYMPTOM" : model.RejectReason;

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

                //for pkg p_upsert_foa_goodsmovement

                var p_Reuse_Flag_foa = new OracleParameter();
                p_Reuse_Flag_foa.ParameterName = "p_Reuse_Flag";
                p_Reuse_Flag_foa.Size = 2000;
                p_Reuse_Flag_foa.OracleDbType = OracleDbType.Varchar2;
                p_Reuse_Flag_foa.Direction = ParameterDirection.Input;
                p_Reuse_Flag_foa.Value = model.Reuse_Flag.ToSafeString();

                var p_Event_Flow_Flag_foa = new OracleParameter();
                p_Event_Flow_Flag_foa.ParameterName = "p_Event_Flow_Flag";
                p_Event_Flow_Flag_foa.Size = 2000;
                p_Event_Flow_Flag_foa.OracleDbType = OracleDbType.Varchar2;
                p_Event_Flow_Flag_foa.Direction = ParameterDirection.Input;
                p_Event_Flow_Flag_foa.Value = model.Event_Flow_Flag.ToSafeString();

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

                var p_ws_header_cur = new OracleParameter();
                p_ws_header_cur.ParameterName = "p_ws_header_cur";
                p_ws_header_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_header_cur.Direction = ParameterDirection.Output;

                var p_ws_item_cur = new OracleParameter();
                p_ws_item_cur.ParameterName = "p_ws_item_cur";
                p_ws_item_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_item_cur.Direction = ParameterDirection.Output;

                // ----------Change Home Device -----------
                var p_package_list = new OracleParameter();
                p_package_list.ParameterName = "p_package_list"; // p_package_list
                p_package_list.OracleDbType = OracleDbType.RefCursor;
                p_package_list.Direction = ParameterDirection.Input;
                p_package_list.Value = packagelistMapping;


                var p_fee_list = new OracleParameter();
                p_fee_list.ParameterName = "p_fee_list"; // p_fee_list
                p_fee_list.OracleDbType = OracleDbType.RefCursor;
                p_fee_list.Direction = ParameterDirection.Input;
                p_fee_list.Value = packagefeeItemMapping;


                var p_change_main = new OracleParameter();
                p_change_main.ParameterName = "p_change_main";  //p_change_main
                p_change_main.Size = 1;
                p_change_main.OracleDbType = OracleDbType.Varchar2;
                p_change_main.Direction = ParameterDirection.Input;
                p_change_main.Value = pchange_main; // Y,N
               // ----------End Change Home Device -----------



                #endregion

                _logger.Info("Start pkg_fbb_foa_order_management " + executeResults.ret_msg);

                string _symptomgroup = "NEW"; string _humamgroup = "HUMAN_NEW"; string _jointype = "NEW";
                string _upf = "NEW"; string _MESH = "N";

                if (model.OrderType.ToUpper().ToSafeString() == "TERMINATION")
                {
                    hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB_REVALUE");

                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package : p_upsert_foa_revalue", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");

                    #region WebService >>> p_upsert_foa_revalue

                    var result = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_upsert_foa_revalue",
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
                          p_product_owner,
                          p_main_promo_code,
                          p_team_id,
                          ////end add new 18.06.28
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

                string orderType = model.OrderType.ToUpper().ToSafeString();

                if (
                    orderType == "MA" ||
                    orderType == "JOIN" ||
                    orderType == "RELOCATE" ||
                    orderType == "HUMANERROR" ||
                    orderType == "NEW" ||
                    orderType == "UPDATEPROFILE" ||
                    orderType == "RETURNED" ||
                    orderType == "PRE-TERMINATION" ||
                    orderType == "RENEW" ||
                    orderType == "RENEW_R" ||
                    orderType == "RENEW_T" ||
                    orderType == "CHANGE HOME DEVICE"
                )
                {
                    #region WebService >>> p_upsert_foa_goodsmovement
                    GoodsMovementKAFKAQuery response = new GoodsMovementKAFKAQuery();
                    log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, model, "Resend by [" + model.UserName + "]", "call package : p_upsert_foa_goodsmovement", "NewRegistForSubmitFOA", "", "Fixed Asset", "Exs.");
                    try
                    {
                        var result_upsert_foa_goodsmovement = _objService.ExecuteStoredProcMultipleCursor("WBB.pkg_fbb_foa_order_management.p_upsert_foa_goodsmovement",
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
                                          p_Reuse_Flag_foa,
                                          p_Event_Flow_Flag_foa,
                          
                                          ////add new 18.06.28
                                          p_Subcontract_Type,
                                          p_Subcontract_Sub_Type,
                                          p_Request_Sub_Flag,
                                          p_Sub_Access_Mode,
                                          ////end add new 18.06.28
                                          //Parameter Output

                          
                                          p_product_owner,
                                          p_main_promo_code,  // model.Main_Promo_Code.ToSafeString(); set from main package of product list
                                          p_team_id,

                                        // ----------- Change Home Device -----------
                                        // add new 9.25
                                          // p_package_list, // send package_list all
                                          //p_fee_list, // send fee_list all
                                          //p_change_main, // hvr_value != null ? "Y" : "N", // set Y if have main package and is new
                                          packagelistMapping,
                                          packagefeeItemMapping,
                                          p_change_main,

                                          
                                          // ----------- Change Home Device -----------
                                          

                                          p_ws_header_cur,
                                          p_ws_item_cur,

                                          ret_code,
                                          ret_msg
                          });
                        InterfaceLogPayGCommand sap_log = null;

                        sap_log = InterfaceLogPayGServiceHelper.StartInterfaceLogPayGRawXml(_uow, _intfLog, model.ToSafeString(), "", "call SAP.", "NewRegistForSubmitFOA4HANAQueryHandler", "", "Fixed Asset", "Exs.");
                        if (checkFlagByOrderType(model.OrderType, "NEW") == "Y")
                        {
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Success", "", "");

                            if (result_upsert_foa_goodsmovement != null)
                            {
                                if (result_upsert_foa_goodsmovement[3] != null && result_upsert_foa_goodsmovement[3].ToSafeString() != null)
                                {
                                    executeResults.ret_msg = result_upsert_foa_goodsmovement[3].ToSafeString();
                                }

                                if (result_upsert_foa_goodsmovement != null && result_upsert_foa_goodsmovement[2].ToSafeString() != "1")
                                {
                                    executeResults.ret_code = result_upsert_foa_goodsmovement[2] != null ? result_upsert_foa_goodsmovement[2].ToSafeString() : "-1";
                                    executeResults.ret_msg = result_upsert_foa_goodsmovement[3] != null ? result_upsert_foa_goodsmovement[3].ToSafeString() : "";
                                    NewRegistForSubmitFOAResponseResult.result = ret_code.Value.ToSafeString();
                                    NewRegistForSubmitFOAResponseResult.errorReason = executeResults.ret_code.Equals("0") ? "Success" : executeResults.ret_msg.ToSafeString();



                                    var converterDataTable = new DataTableToXmlConverter();
                                    string headerXml = result_upsert_foa_goodsmovement[0] is DataTable dt ? converterDataTable.ConvertDataTableToCustomHeaderXml(dt) : "";
                                    string itemsXml = result_upsert_foa_goodsmovement[1] is DataTable dt2 ? converterDataTable.ConvertDataTableToCustomBodyXml(dt2) : "";

                                    if (!string.IsNullOrEmpty(headerXml) && !string.IsNullOrEmpty(itemsXml))
                                    {
                                        XmlToJsonConverter converter = new XmlToJsonConverter();
                                        List<string> jsonResult = new List<string>();

                                        var headerKAFKA = new JObject();
                                        string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                                        var ESB_KAFKA_HEADER = from item in _cfgLov.Get()
                                                               where item.LOV_TYPE == "ESB_KAFKA_HEADER" && item.ACTIVEFLAG == "Y"
                                                               orderby item.LOV_ID ascending
                                                               select item;

                                        foreach (var item in ESB_KAFKA_HEADER)
                                        {
                                            if (item.LOV_NAME == "groupTags")
                                            {
                                                if (item.LOV_VAL1 != null)
                                                {
                                                    string jsonContent = "[" + item.LOV_VAL1 + "]";
                                                    headerKAFKA[item.LOV_NAME] = JArray.Parse(jsonContent);
                                                }
                                                else
                                                {
                                                    headerKAFKA[item.LOV_NAME] = new JArray { };
                                                }
                                            }
                                            else if (item.LOV_NAME == "identity")
                                            {
                                                if (item.LOV_VAL1 != null)
                                                {
                                                    string jsonContent = "{" + item.LOV_VAL1 + "}";
                                                    headerKAFKA[item.LOV_NAME] = JObject.Parse(jsonContent);
                                                }
                                                else
                                                {
                                                    headerKAFKA[item.LOV_NAME] = new JObject { };
                                                }
                                            }
                                            else
                                            {
                                                headerKAFKA[item.LOV_NAME] = item.LOV_VAL1 != null ? item.LOV_VAL1 : string.Empty;
                                            }
                                        }

                                        jsonResult = converter.ConvertXmlToJson(headerXml, itemsXml, headerKAFKA);


                                        foreach (var item in jsonResult)
                                        {
                                            InterfaceLogPayGCommand logItem = new InterfaceLogPayGCommand();
                                            var jsonObject = JObject.Parse(item);

                                            // Access the PartnerMessageID
                                            string partnerMessageID = jsonObject["body"]?["PartnerMessageID"]?.ToString();
                                            string internetNo = jsonObject["body"]?["GRGISlipNo"]?.ToString();


                                            List<Dictionary<string, object>> headerList =
                                                result_upsert_foa_goodsmovement[0] is DataTable dtHeader ? ConvertCursorToList(dtHeader) : new List<Dictionary<string, object>>();

                                            List<Dictionary<string, object>> itemList =
                                                result_upsert_foa_goodsmovement[1] is DataTable dtItem ? ConvertCursorToList(dtItem) : new List<Dictionary<string, object>>();

                                            var logData = new HistoryLogModel
                                            {
                                                header = headerList,
                                                body = itemList,
                                                ReturnCode = result_upsert_foa_goodsmovement[2]?.ToSafeString() ?? "-1",
                                                ReturnMessage = result_upsert_foa_goodsmovement[3]?.ToSafeString() ?? ""
                                            };

                                            string jsonLogData = JsonConvert.SerializeObject(logData, Formatting.Indented);

                                            //insert interface log
                                            logItem = InterfaceLogPayGServiceHelper.StartInterfaceLogPayG(_uow, _intfLog, item, partnerMessageID, "Call GoodsMovementKAFKA", "GoodsMovementKAFKAQueryHandler", internetNo, "Fixed Asset", "Exs.");

                                            //insert history log
                                            hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "FBB");


                                            UpdateHistoryLog(_uow, _hisLog, jsonLogData, hLog, partnerMessageID, "INSTALLATION", "", "");

                                            var response_out_foa = new NewRegistForSubmitFOAResponse
                                            {
                                                result = "Success",
                                                errorReason = string.Empty
                                            };
                                            UpdateHistoryLog(_uow, _hisLog, response_out_foa, hLog, partnerMessageID, "OUT_FOA", "", "");

                                        }

                                        var query = new GoodsMovementKAFKAQuery()
                                        {
                                            action = "PRODUCER",
                                            item_json = jsonResult
                                        };
                                        _queryProcessorGoodsMovementHandler.Handle(query);

                                        response = new GoodsMovementKAFKAQuery
                                        {
                                            Return_Code = "Success"
                                        };
                                    }
                                    else
                                    {
                                        var err_msg = executeResults.ret_msg.ToSafeString() != null
                                            ? executeResults.ret_msg.ToSafeString()
                                            : "Package p_upsert_foa_goodsmovement Data Header and Item is Null";
                                        NewRegistForSubmitFOAResponseResult.result = "";
                                        response = new GoodsMovementKAFKAQuery
                                        {
                                            Return_Code = "Error",
                                            Return_Message = err_msg
                                        };
                                    }
                                }
                                else
                                {

                                    var err_msg = executeResults.ret_msg.ToSafeString() != null
                                        ? executeResults.ret_msg.ToSafeString()
                                        : "Package p_upsert_foa_goodsmovement Data Return Null";
                                    NewRegistForSubmitFOAResponseResult.result = "";
                                    response = new GoodsMovementKAFKAQuery
                                    {
                                        Return_Code = "Error",
                                        Return_Message = err_msg
                                    };
                                }
                            }
                            else
                            {
                                response = new GoodsMovementKAFKAQuery
                                {
                                    Return_Code = "Error",
                                    Return_Message = "Package p_upsert_foa_goodsmovement Data Return Null"
                                };
                                NewRegistForSubmitFOAResponseResult.new_response = "999";
                                NewRegistForSubmitFOAResponseResult.new_ErrorMsg = "Data Return Null";
                            }
                        }
                        else
                        {
                            SAPError = "Blockout FLAG CALL SAP";
                            InterfaceLogPayGServiceHelper.EndInterfaceLogPayGRawXml(_uow, _intfLog, "", sap_log, "Block out", "", "");
                        }


                    }
                    catch (Exception e)
                    {
                        response = new GoodsMovementKAFKAQuery
                        {
                            Return_Code = "Error",
                            Return_Message = e.Message.ToSafeString()
                        };
                        NewRegistForSubmitFOAResponseResult.new_response = "999";
                        NewRegistForSubmitFOAResponseResult.new_ErrorMsg = e.GetErrorMessage();
                    }
                    hLog = StartHistoryLog(_uow, _hisLog, model, "", "IN_FOA", null, "Goodsmovement");
                    UpdateHistoryLog(_uow, _hisLog, "", hLog, "", "INSTALLATION", executeResults.ret_msg, "");
                    #endregion
                    InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, response, log, response.Return_Code, (response.Return_Code == "Success") ? "" : response.Return_Message, "");
                }
            }
            catch (Exception ex)
            {

                NewRegistForSubmitFOAResponseResult.result = "-1";
                NewRegistForSubmitFOAResponseResult.errorReason = ex.GetErrorMessage();

                InterfaceLogPayGServiceHelper.EndInterfaceLogPayG(_uow, _intfLog, NewRegistForSubmitFOAResponseResult, log, "PackageFail:" + model.OrderNumber, ex.GetErrorMessage().ToSafeString(), "");
                //!* log

                UpdateHistoryLog(_uow, _hisLog, NewRegistForSubmitFOAResponseResult, hLog, "", "ERROR", ex.GetErrorMessage().ToSafeString(), null);
                NewRegistForSubmitFOAResponseResult.result = ""; //clear
            }
            UpdateSubmitFoaErrorLog(model.OrderNumber, model.RejectReason, model.OrderType, sn_no, model.Access_No, model.UserName);
            return NewRegistForSubmitFOAResponseResult;

        }

        public class HistoryLogModel
        {
            public List<Dictionary<string, object>> header { get; set; }
            public List<Dictionary<string, object>> body { get; set; }
            public string ReturnCode { get; set; }
            public string ReturnMessage { get; set; }
        }


        public List<Dictionary<string, object>> ConvertCursorToList(DataTable cursor)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in cursor.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in cursor.Columns)
                {
                    object value = row[col];

                    if (value is string strValue)
                    {
                        string decodedValue = WebUtility.HtmlDecode(strValue);

                        try
                        {
                            var xml = XElement.Parse(decodedValue);

                            if (col.ColumnName.Equals("SerialNumber", StringComparison.OrdinalIgnoreCase))
                            {
                                dict[col.ColumnName] = xml;
                            }
                            else
                            {
                                dict[col.ColumnName] = xml.ToString(SaveOptions.DisableFormatting);
                            }
                        }
                        catch
                        {
                            dict[col.ColumnName] = decodedValue;
                        }
                    }
                    else
                    {
                        dict[col.ColumnName] = value;
                    }
                }

                list.Add(dict);
            }

            return list;
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
                               where item.PROGRAM_NAME == "S4_FLAG_CALL_SAP" || item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_MA" || item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_REVALUE" || item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_RETURN_MA" || item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_RETURN_REVALUE"
                               select item;
            var fixAssetCallSap = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "S4_FLAG_CALL_SAP");
            var fixAssetCallSapRevalue = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_REVALUE");
            var fixAssetCallSapMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_MA");
            var fixAssetCallSapRETURNMA = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_RETURN_MA");
            var fixAssetCallSapRETURNREVALUE = FixAssConfig.FirstOrDefault(item => item.PROGRAM_NAME == "S4_FLAG_CALL_SAP_RETURN_REVALUE");


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
                if (type == "NEW")
                {
                    CHKFLAG = fixAssetCallSap.COM_CODE.ToSafeString();
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
        bool IsValidJsonObject(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            return value.StartsWith("{") && value.EndsWith("}");
        }

        bool IsValidJsonArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();
            return value.StartsWith("[") && value.EndsWith("]");
        }

    }
}


public class ProductModel
{
    public string IS_NEW { get; set; }
    public string PRODUCT_CLASS { get; set; }
    public string PRODUCT_NAME { get; set; }
    public string PRODUCT_CODE { get; set; }
    public string ORDER_NO { get; set; }
    public string ACCESS_NO { get; set; }
}
public class FeeItemModel
{
    public string FEE_ID_TYPE { get; set; }
    public string FEE_ID { get; set; }
    public string FEE_ACTION { get; set; }
    public decimal ORDER_FEE_PRICE { get; set; }
    public string FEE_NAME { get; set; }
    public string ORDER_NO { get; set; }
    public string ACCESS_NO { get; set; }

}

public class FeeModel
{
    public string Fee_ItemEntry_Fee { get; set; }
    public string Fee_ItemSoundbar_Fee { get; set; }
    public string Fee_ItemFiberhome_Fee { get; set; }
    public int Total_Fee { get; set; }

}

public class FeeListModel
{
    public string idType { get; set; }
    public string idValue { get; set; }
    public string action { get; set; }
    public decimal orderFeePrice { get; set; }
    public string type { get; set; }

}
