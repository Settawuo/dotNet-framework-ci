using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    public class FBBReturnedRevalueAssetS4HANAQueryHandler : IQueryHandler<FBBReturnedRevalueAssetS4HANAQuery, FBBReturnedRevalueAssetS4HANAReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_PAYG> _intfLog;
        private readonly IEntityRepository<FBB_FIXED_ASSET_HISTORY_LOG> _hisLog;
        private readonly IEntityRepository<FBBReturnedRevalueAssetS4HANAModel> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        HistoryLogCommand hLog = null;

        public FBBReturnedRevalueAssetS4HANAQueryHandler(
            ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBBReturnedRevalueAssetS4HANAModel> objService,
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
        public FBBReturnedRevalueAssetS4HANAReturn Handle(FBBReturnedRevalueAssetS4HANAQuery query)
        {

            HistoryLogCommand hLog = null;
            FBBReturnedRevalueAssetS4HANAReturn FBBReturnedRevalueAssetS4HANAResponseResult = new FBBReturnedRevalueAssetS4HANAReturn();
            FBBReturnedRevalueAssetS4HANAReturn FBSSInvSendTerminateResponseResult = new FBBReturnedRevalueAssetS4HANAReturn();
            FBBReturnedRevalueAssetS4HANAWriteLog WriteHisLog = new FBBReturnedRevalueAssetS4HANAWriteLog();
             FBSSInvSendTerminateReturn ResultLogOutFoa = new FBSSInvSendTerminateReturn();
            var historyLog = new FBB_HISTORY_LOG();
            var fixAssConfig = new FBSS_FIXED_ASSET_CONFIG();

            var P_ORDER_TYPE_Query = _cfgLov.Get(x => x.LOV_TYPE == "ReturnedRevalueAsset" && x.LOV_NAME == "P_ORDER_TYPE").FirstOrDefault();


            var P_DATE_START = new OracleParameter();
            P_DATE_START.ParameterName = "P_DATE_START";
            P_DATE_START.Size = 2000;
            P_DATE_START.OracleDbType = OracleDbType.Varchar2;
            P_DATE_START.Direction = ParameterDirection.Input;
            P_DATE_START.Value = query.p_date_start;

            var P_DATE_TO = new OracleParameter();
            P_DATE_TO.ParameterName = "P_DATE_TO";
            P_DATE_TO.Size = 2000;
            P_DATE_TO.OracleDbType = OracleDbType.Varchar2;
            P_DATE_TO.Direction = ParameterDirection.Input;
            P_DATE_TO.Value = query.p_date_to;

            var P_ORDER_TYPE = new OracleParameter();
            P_ORDER_TYPE.ParameterName = "P_ORDER_TYPE";
            P_ORDER_TYPE.Size = 2000;
            P_ORDER_TYPE.OracleDbType = OracleDbType.Varchar2;
            P_ORDER_TYPE.Direction = ParameterDirection.Input;
            P_ORDER_TYPE.Value = P_ORDER_TYPE_Query.LOV_VAL1.ToSafeString();

            var ret_cur_get_order_revalue = new OracleParameter();
            ret_cur_get_order_revalue.ParameterName = "ret_cur_get_order_revalue";
            ret_cur_get_order_revalue.OracleDbType = OracleDbType.RefCursor;
            ret_cur_get_order_revalue.Direction = ParameterDirection.Output;

            List<OrderRevalue> respListOrderRevalue = new List<OrderRevalue>();
            DataTable resp = new DataTable();
            var executeResult = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_order_revalue",
                new object[]
                {
                                    //INPUT
                                    P_DATE_START,
                                    P_DATE_TO,
                                    P_ORDER_TYPE,
                                    //OUTPUT
                                    ret_cur_get_order_revalue

                });

            if (executeResult[0] != null)
            {
                resp = (DataTable)executeResult[0];
                respListOrderRevalue = resp.DataTableToList<OrderRevalue>();

                foreach (var order in respListOrderRevalue)
                {
                    var p_ORDER_NO_Product_List = new OracleParameter();
                    p_ORDER_NO_Product_List.ParameterName = "p_ORDER_NO";
                    p_ORDER_NO_Product_List.Size = 2000;
                    p_ORDER_NO_Product_List.OracleDbType = OracleDbType.Varchar2;
                    p_ORDER_NO_Product_List.Direction = ParameterDirection.Input;
                    p_ORDER_NO_Product_List.Value = order.ORDER_NO;

                    var p_INTERNET_NO_Product_List = new OracleParameter();
                    p_INTERNET_NO_Product_List.ParameterName = "p_INTERNET_NO";
                    p_INTERNET_NO_Product_List.Size = 2000;
                    p_INTERNET_NO_Product_List.OracleDbType = OracleDbType.Varchar2;
                    p_INTERNET_NO_Product_List.Direction = ParameterDirection.Input;
                    p_INTERNET_NO_Product_List.Value = order.ACCESS_NUMBER;

                    var action_Product_List = new OracleParameter();
                    action_Product_List.ParameterName = "action";
                    action_Product_List.Size = 2000;
                    action_Product_List.OracleDbType = OracleDbType.Varchar2;
                    action_Product_List.Direction = ParameterDirection.Input;

                    var ret_code_Product_List = new OracleParameter();
                    ret_code_Product_List.ParameterName = "ret_code";
                    ret_code_Product_List.Size = 2000;
                    ret_code_Product_List.OracleDbType = OracleDbType.Varchar2;
                    ret_code_Product_List.Direction = ParameterDirection.Output;

                    var cur_order_Product_List = new OracleParameter();
                    cur_order_Product_List.ParameterName = "cur_order";
                    cur_order_Product_List.OracleDbType = OracleDbType.RefCursor;
                    cur_order_Product_List.Direction = ParameterDirection.Output;

                    var cur_product_Product_List = new OracleParameter();
                    cur_product_Product_List.ParameterName = "cur_product";
                    cur_product_Product_List.OracleDbType = OracleDbType.RefCursor;
                    cur_product_Product_List.Direction = ParameterDirection.Output;

                    var cur_service_Product_List = new OracleParameter();
                    cur_service_Product_List.ParameterName = "cur_service";
                    cur_service_Product_List.OracleDbType = OracleDbType.RefCursor;
                    cur_service_Product_List.Direction = ParameterDirection.Output;

                    var resultGetPDList = _objService.ExecuteStoredProcMultipleCursor("WBB.P_FBB_GET_PRODUCTLIST",
                        new object[]
                        {
                            p_ORDER_NO_Product_List,
                            p_INTERNET_NO_Product_List,
                            ret_code_Product_List,//[0]
                            cur_order_Product_List,//[1]
                            cur_product_Product_List,//[2]
                            cur_service_Product_List,//[3]
                            action_Product_List
                        });
                    FBBReturnedRevalueAssetProductList cur_order_res_Product_List = new FBBReturnedRevalueAssetProductList();//cur_order

                    DataTable dtTableMainRespones = (DataTable)resultGetPDList[1];
                    foreach (DataRow dr in dtTableMainRespones.Rows)
                    {
                        cur_order_res_Product_List.AccessNo = dr.ItemArray[0].ToSafeString();
                        cur_order_res_Product_List.OrderNumber = dr.ItemArray[1].ToSafeString();
                        cur_order_res_Product_List.SubcontractorCode = dr.ItemArray[2].ToSafeString();
                        cur_order_res_Product_List.SubcontractorName = dr.ItemArray[3].ToSafeString();
                        cur_order_res_Product_List.ProductName = dr.ItemArray[4].ToSafeString();
                        cur_order_res_Product_List.ServiceName = dr.ItemArray[5].ToSafeString();
                        cur_order_res_Product_List.OrderType = dr.ItemArray[6].ToSafeString();
                        cur_order_res_Product_List.SubmitFlag = dr.ItemArray[7].ToSafeString();
                        cur_order_res_Product_List.RejectReason = dr.ItemArray[8].ToSafeString();
                        if (dr.ItemArray[9].ToSafeString() != "")
                        {
                            cur_order_res_Product_List.FOA_Submit_date_value = (DateTime?)dr.ItemArray[9];
                        }
                        else
                        {
                            cur_order_res_Product_List.FOA_Submit_date_value = null;
                        }
                        cur_order_res_Product_List.OLT_NAME = dr.ItemArray[10].ToSafeString();
                        cur_order_res_Product_List.BUILDING_NAME = dr.ItemArray[11].ToSafeString();
                        cur_order_res_Product_List.Mobile_Contact = dr.ItemArray[12].ToSafeString();
                        cur_order_res_Product_List.ADDRESS_ID = dr.ItemArray[13].ToSafeString();
                        cur_order_res_Product_List.ORG_ID = dr.ItemArray[14].ToSafeString();
                        cur_order_res_Product_List.REUSE_FLAG = dr.ItemArray[15].ToSafeString();
                        cur_order_res_Product_List.EVENT_FLOW_FLAG = dr.ItemArray[16].ToSafeString();
                        cur_order_res_Product_List.SUBCONTRACT_TYPE = dr.ItemArray[17].ToSafeString();
                        cur_order_res_Product_List.SUBCONTRACT_SUB_TYPE = dr.ItemArray[18].ToSafeString();
                        cur_order_res_Product_List.REQUEST_SUB_FLAG = dr.ItemArray[19].ToSafeString();
                        cur_order_res_Product_List.SUB_ACCESS_MODE = dr.ItemArray[20].ToSafeString();
                    }

                    //Product List
                    DataTable cur_product_res_Product_List = (DataTable)resultGetPDList[2];
                    List<FBBReturnedRevalueAssetProduct> _productList = cur_product_res_Product_List.DataTableToList<FBBReturnedRevalueAssetProduct>();
                    cur_order_res_Product_List.ProductList = _productList;

                    //Service List
                    DataTable cur_service_res_Product_List = (DataTable)resultGetPDList[3];
                    List<FBBReturnedRevalueAssetInstall> _serviceList = cur_service_res_Product_List.DataTableToList<FBBReturnedRevalueAssetInstall>();
                    cur_order_res_Product_List.InstallList = _serviceList;

                    var packageMappingObjectModel = new PackageMappingObjectModel
                    {
                        FBB_PRODUCT_LIST =
                                 cur_order_res_Product_List.ProductList.Select(
                                     a => new Product_Mapping_ArrayMapping
                                     {
                                         SerialNumber = a.SerialNumber,
                                         MaterialCode = a.MaterialCode,
                                         CompanyCode = a.CompanyCode,
                                         Plant = a.Plant,
                                         StorageLocation = a.StorageLocation,
                                         SNPattern = a.SNPattern,
                                         MovementType = a.MovementType

                                     }).ToArray()
                    };

                    var p_ACCESS_NUMBER_upsert_foa_rev = new OracleParameter();
                    p_ACCESS_NUMBER_upsert_foa_rev.ParameterName = "p_ACCESS_NUMBER";
                    p_ACCESS_NUMBER_upsert_foa_rev.Size = 2000;
                    p_ACCESS_NUMBER_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_ACCESS_NUMBER_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_ACCESS_NUMBER_upsert_foa_rev.Value = cur_order_res_Product_List.AccessNo;

                    var p_ORDER_NO_upsert_foa_rev = new OracleParameter();
                    p_ORDER_NO_upsert_foa_rev.ParameterName = "p_ORDER_NO";
                    p_ORDER_NO_upsert_foa_rev.Size = 2000;
                    p_ORDER_NO_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_ORDER_NO_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_ORDER_NO_upsert_foa_rev.Value = cur_order_res_Product_List.OrderNumber;

                    var p_ORDER_TYPE_upsert_foa_rev = new OracleParameter();
                    p_ORDER_TYPE_upsert_foa_rev.ParameterName = "p_ORDER_TYPE";
                    p_ORDER_TYPE_upsert_foa_rev.Size = 2000;
                    p_ORDER_TYPE_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_ORDER_TYPE_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_ORDER_TYPE_upsert_foa_rev.Value = cur_order_res_Product_List.OrderType;

                    var p_SUBCONTRACT_CODE_upsert_foa_rev = new OracleParameter();
                    p_SUBCONTRACT_CODE_upsert_foa_rev.ParameterName = "p_SUBCONTRACT_CODE";
                    p_SUBCONTRACT_CODE_upsert_foa_rev.Size = 2000;
                    p_SUBCONTRACT_CODE_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_SUBCONTRACT_CODE_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_SUBCONTRACT_CODE_upsert_foa_rev.Value = cur_order_res_Product_List.SubcontractorCode;

                    var p_SUBCONTRACT_NAME_upsert_foa_rev = new OracleParameter();
                    p_SUBCONTRACT_NAME_upsert_foa_rev.ParameterName = "p_SUBCONTRACT_NAME";
                    p_SUBCONTRACT_NAME_upsert_foa_rev.Size = 2000;
                    p_SUBCONTRACT_NAME_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_SUBCONTRACT_NAME_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_SUBCONTRACT_NAME_upsert_foa_rev.Value = cur_order_res_Product_List.SubcontractorName;

                    var p_PRODUCT_NAME_upsert_foa_rev = new OracleParameter();
                    p_PRODUCT_NAME_upsert_foa_rev.ParameterName = "p_PRODUCT_NAME";
                    p_PRODUCT_NAME_upsert_foa_rev.Size = 2000;
                    p_PRODUCT_NAME_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_PRODUCT_NAME_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_PRODUCT_NAME_upsert_foa_rev.Value = cur_order_res_Product_List.ProductName;

                    var p_SERVICE_LIST_upsert_foa_rev = new OracleParameter();
                    p_SERVICE_LIST_upsert_foa_rev.ParameterName = "p_SERVICE_LIST";
                    p_SERVICE_LIST_upsert_foa_rev.Size = 2000;
                    p_SERVICE_LIST_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_SERVICE_LIST_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_SERVICE_LIST_upsert_foa_rev.Value = cur_order_res_Product_List.ServiceList;

                    var p_Product_List_upsert_foa_rev = new OracleParameter();
                    p_Product_List_upsert_foa_rev.ParameterName = "p_Product_List";
                    p_Product_List_upsert_foa_rev.OracleDbType = OracleDbType.RefCursor;
                    p_Product_List_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Product_List_upsert_foa_rev.Value = packageMappingObjectModel;

                    var p_SUBMIT_FLAG_upsert_foa_rev = new OracleParameter();
                    p_SUBMIT_FLAG_upsert_foa_rev.ParameterName = "p_SUBMIT_FLAG";
                    p_SUBMIT_FLAG_upsert_foa_rev.Size = 2000;
                    p_SUBMIT_FLAG_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_SUBMIT_FLAG_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_SUBMIT_FLAG_upsert_foa_rev.Value = cur_order_res_Product_List.SubmitFlag;

                    var p_Reject_reason_upsert_foa_rev = new OracleParameter();
                    p_Reject_reason_upsert_foa_rev.ParameterName = "p_Reject_reason";
                    p_Reject_reason_upsert_foa_rev.Size = 2000;
                    p_Reject_reason_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Reject_reason_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Reject_reason_upsert_foa_rev.Value = cur_order_res_Product_List.RejectReason;


                    String FOA_Submit_date = null;
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    DateTime dt;
                    if (DateTime.TryParseExact(cur_order_res_Product_List.FOA_Submit_date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture, DateTimeStyles.None, out dt))
                    {
                        DateTime dtFOA_Submit_date = DateTime.ParseExact(cur_order_res_Product_List.FOA_Submit_date, "yyyy-MM-ddTHH:mm:ss.fffzzz", culture);
                        FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss", culture);
                    }
                    else if (DateTime.TryParseExact(cur_order_res_Product_List.FOA_Submit_date, "dd/MM/yyyy HH:mm:ss", culture, DateTimeStyles.None, out dt))
                    {
                        DateTime dtFOA_Submit_date = DateTime.ParseExact(cur_order_res_Product_List.FOA_Submit_date, "dd/MM/yyyy HH:mm:ss", culture);
                        FOA_Submit_date = dtFOA_Submit_date.ToString("dd/MM/yyyy HHmmss", culture);
                    }

                    var p_foa_submit_date_upsert_foa_rev = new OracleParameter();
                    p_foa_submit_date_upsert_foa_rev.ParameterName = "p_foa_submit_date";
                    p_foa_submit_date_upsert_foa_rev.Size = 2000;
                    p_foa_submit_date_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_foa_submit_date_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_foa_submit_date_upsert_foa_rev.Value = FOA_Submit_date;


                    var p_post_date_upsert_foa_rev = new OracleParameter();
                    p_post_date_upsert_foa_rev.ParameterName = "p_post_date";
                    p_post_date_upsert_foa_rev.Size = 2000;
                    p_post_date_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_post_date_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_post_date_upsert_foa_rev.Value = DateTime.Now.ToString("dd/MM/yyyy");

                    var p_olt_name_upsert_foa_rev = new OracleParameter();
                    p_olt_name_upsert_foa_rev.ParameterName = "p_olt_name";
                    p_olt_name_upsert_foa_rev.Size = 2000;
                    p_olt_name_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_olt_name_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_olt_name_upsert_foa_rev.Value = cur_order_res_Product_List.OLT_NAME;

                    var p_building_name_upsert_foa_rev = new OracleParameter();
                    p_building_name_upsert_foa_rev.ParameterName = "p_building_name";
                    p_building_name_upsert_foa_rev.Size = 2000;
                    p_building_name_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_building_name_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_building_name_upsert_foa_rev.Value = cur_order_res_Product_List.BUILDING_NAME;

                    var p_mobile_contact_upsert_foa_rev = new OracleParameter();
                    p_mobile_contact_upsert_foa_rev.ParameterName = "p_mobile_contact";
                    p_mobile_contact_upsert_foa_rev.Size = 2000;
                    p_mobile_contact_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_mobile_contact_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_mobile_contact_upsert_foa_rev.Value = cur_order_res_Product_List.Mobile_Contact;
                    //add new 
                    var p_addess_id_upsert_foa_rev = new OracleParameter();
                    p_addess_id_upsert_foa_rev.ParameterName = "p_addess_id";
                    p_addess_id_upsert_foa_rev.Size = 2000;
                    p_addess_id_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_addess_id_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_addess_id_upsert_foa_rev.Value = cur_order_res_Product_List.ADDRESS_ID;

                    //add new 17.12.21
                    var p_org_id_upsert_foa_rev = new OracleParameter();
                    p_org_id_upsert_foa_rev.ParameterName = "p_org_id";
                    p_org_id_upsert_foa_rev.Size = 2000;
                    p_org_id_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_org_id_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_org_id_upsert_foa_rev.Value = cur_order_res_Product_List.ORG_ID;

                    var p_Reuse_Flag_upsert_foa_rev = new OracleParameter();
                    p_Reuse_Flag_upsert_foa_rev.ParameterName = "p_reuse_flag";
                    p_Reuse_Flag_upsert_foa_rev.Size = 2000;
                    p_Reuse_Flag_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Reuse_Flag_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Reuse_Flag_upsert_foa_rev.Value = cur_order_res_Product_List.REUSE_FLAG;

                    var p_Event_Flow_Flag_upsert_foa_rev = new OracleParameter();
                    p_Event_Flow_Flag_upsert_foa_rev.ParameterName = "p_event_flow_flag";
                    p_Event_Flow_Flag_upsert_foa_rev.Size = 2000;
                    p_Event_Flow_Flag_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Event_Flow_Flag_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Event_Flow_Flag_upsert_foa_rev.Value = cur_order_res_Product_List.EVENT_FLOW_FLAG;

                    //En add new 17.12.21

                    ////add new 18.06.28
                    var p_Subcontract_Type_upsert_foa_rev = new OracleParameter();
                    p_Subcontract_Type_upsert_foa_rev.ParameterName = "p_subcontract_type";
                    p_Subcontract_Type_upsert_foa_rev.Size = 2000;
                    p_Subcontract_Type_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Subcontract_Type_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Subcontract_Type_upsert_foa_rev.Value = cur_order_res_Product_List.SUBCONTRACT_TYPE;

                    var p_Subcontract_Sub_Type_upsert_foa_rev = new OracleParameter();
                    p_Subcontract_Sub_Type_upsert_foa_rev.ParameterName = "p_subcontract_sub_type";
                    p_Subcontract_Sub_Type_upsert_foa_rev.Size = 2000;
                    p_Subcontract_Sub_Type_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Subcontract_Sub_Type_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Subcontract_Sub_Type_upsert_foa_rev.Value = cur_order_res_Product_List.SUBCONTRACT_SUB_TYPE;

                    var p_Request_Sub_Flag_upsert_foa_rev = new OracleParameter();
                    p_Request_Sub_Flag_upsert_foa_rev.ParameterName = "p_request_sub_flag";
                    p_Request_Sub_Flag_upsert_foa_rev.Size = 2000;
                    p_Request_Sub_Flag_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Request_Sub_Flag_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Request_Sub_Flag_upsert_foa_rev.Value = cur_order_res_Product_List.REQUEST_SUB_FLAG;

                    var p_Sub_Access_Mode_upsert_foa_rev = new OracleParameter();
                    p_Sub_Access_Mode_upsert_foa_rev.ParameterName = "p_sub_access_mode";
                    p_Sub_Access_Mode_upsert_foa_rev.Size = 2000;
                    p_Sub_Access_Mode_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    p_Sub_Access_Mode_upsert_foa_rev.Direction = ParameterDirection.Input;
                    p_Sub_Access_Mode_upsert_foa_rev.Value = cur_order_res_Product_List.SUB_ACCESS_MODE;

                    //end add new 18.06.28


                    var p_ws_revalue_cur_upsert_foa_rev = new OracleParameter();
                    p_ws_revalue_cur_upsert_foa_rev.ParameterName = "p_ws_revalue_cur";
                    p_ws_revalue_cur_upsert_foa_rev.OracleDbType = OracleDbType.RefCursor;
                    p_ws_revalue_cur_upsert_foa_rev.Direction = ParameterDirection.Output;

                    var ret_code_upsert_foa_rev = new OracleParameter();
                    ret_code_upsert_foa_rev.ParameterName = "ret_code";
                    ret_code_upsert_foa_rev.Size = 2000;
                    ret_code_upsert_foa_rev.OracleDbType = OracleDbType.Decimal;
                    ret_code_upsert_foa_rev.Direction = ParameterDirection.Output;

                    var ret_msg_upsert_foa_rev = new OracleParameter();
                    ret_msg_upsert_foa_rev.ParameterName = "ret_msg";
                    ret_msg_upsert_foa_rev.Size = 2000;
                    ret_msg_upsert_foa_rev.OracleDbType = OracleDbType.Varchar2;
                    ret_msg_upsert_foa_rev.Direction = ParameterDirection.Output;
                    var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_Product_List", "WBB.FBB_PRODUCT_LIST", packageMappingObjectModel);


                    var executeResult_upsert_foa_rev = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_upsert_foa_revalue",
                            new object[]
                            {
                                                //List
                                                  p_ACCESS_NUMBER_upsert_foa_rev,
                                                  p_ORDER_NO_upsert_foa_rev,
                                                  p_ORDER_TYPE_upsert_foa_rev,
                                                  p_SUBCONTRACT_CODE_upsert_foa_rev,
                                                  p_SUBCONTRACT_NAME_upsert_foa_rev,
                                                  p_PRODUCT_NAME_upsert_foa_rev,
                                                  p_SERVICE_LIST_upsert_foa_rev,
                                                  packageMapping,
                                                  p_SUBMIT_FLAG_upsert_foa_rev,
                                                  p_Reject_reason_upsert_foa_rev,
                                                  p_foa_submit_date_upsert_foa_rev,
                                                  p_post_date_upsert_foa_rev,
                                                  p_olt_name_upsert_foa_rev,
                                                  p_building_name_upsert_foa_rev,
                                                  p_mobile_contact_upsert_foa_rev,
                                                  p_addess_id_upsert_foa_rev,
                                                  p_org_id_upsert_foa_rev,
                                                  p_Reuse_Flag_upsert_foa_rev,
                                                  p_Event_Flow_Flag_upsert_foa_rev,
                                                  p_Subcontract_Type_upsert_foa_rev,
                                                  p_Subcontract_Sub_Type_upsert_foa_rev,
                                                  p_Request_Sub_Flag_upsert_foa_rev,
                                                  p_Sub_Access_Mode_upsert_foa_rev,

                                                //Return
                                                //p_Product_List,
                                                  p_ws_revalue_cur_upsert_foa_rev,
                                                  ret_code_upsert_foa_rev,
                                                  ret_msg_upsert_foa_rev

                            });

                    DataTable resp2 = new DataTable();
                    List<FBBReturnedRevalueAssetS4HANAQueryReturn> respList = new List<FBBReturnedRevalueAssetS4HANAQueryReturn>();
                    resp = (DataTable)executeResult_upsert_foa_rev[0];
                    if (executeResult_upsert_foa_rev != null)
                    {
                        resp = (DataTable)executeResult_upsert_foa_rev[0];
                        respList = resp.DataTableToList<FBBReturnedRevalueAssetS4HANAQueryReturn>();
                        int ItemNumber = 0;
                        foreach (var main in respList)
                        {
                            string ret_code = null;
                            string ret_msg = null;
                            ItemNumber++;
                            try
                            {
                                var sapData = new FBSS_SubmitFOARevalueResponse
                                {
                                    TRANS_ID = main.TRANS_ID,
                                    REF_DOC_NO = main.REF_DOC_NO,
                                    ITEM_NUMBER = ItemNumber.ToSafeString(),
                                    RUN_GROUP = main.RUN_GROUP,
                                    COM_CODE = main.COM_CODE,
                                    MAIN_ASSET = main.MAIN_ASSET,
                                    SUBNUMBER = main.SUBNUMBER,
                                    DOC_DATE = main.DOC_DATE,
                                    POST_DATE = main.POST_DATE,
                                    ASSET_VALUE_DATE = main.ASSET_VALUE_DATE,
                                    ITEM_TEXT = main.ITEM_TEXT,
                                    INTERNET_NO = main.INTERNET_NO,
                                    ASSIGNMENT = main.ASSIGNMENT
                                };

                                // Call the method to post data to SAP
                                var responeSap = PostToSAP(sapData);

                                POSTSAPResponseRev sapResponse = JsonConvert.DeserializeObject<POSTSAPResponseRev>(responeSap);
                                string assetNumber = sapResponse.ITEMS?.FirstOrDefault()?.AssetNumber ?? string.Empty;

                                string p_ERR_CODE = string.Empty;

                                if (sapResponse.MessageType == "S")
                                {
                                    p_ERR_CODE = "000";
                                }
                                else if(sapResponse.MessageType == "E")
                                {
                                    p_ERR_CODE = "999";
                                }


                                //Update Table
                                var updateResponse2 = _objService.ExecuteReadStoredProc("WBB.pkg_fbb_foa_order_management.p_upsert_resp_revalue",
                                new
                                {
                                    p_TRANS_ID = main.TRANS_ID.ToSafeString(),
                                    p_ACTION = main.ACTION.ToSafeString(),
                                    p_RUN_GROUP = main.RUN_GROUP.ToSafeString(),
                                    p_INTERNET_NO = main.INTERNET_NO.ToSafeString(),
                                    p_ASSET_CODE = assetNumber.ToSafeString(),
                                    p_SUBNUMBER = main.SUBNUMBER.ToSafeString(),
                                    p_ERR_CODE = p_ERR_CODE,
                                    p_ERR_MSG = sapResponse.MessageDesc.ToSafeString(),
                                    p_ORDER_NO = main.REF_DOC_NO.ToSafeString(),
                                    ret_code = ret_code,
                                    ret_msg = ret_msg

                                }).ToList();
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                }
            }
            else
            {
                _logger.Info("wbb.pkg_fbb_foa_order_management.p_get_order_revalue : data not found");
            }
            return null;
        }
        public static bool SAPResponseValid(string code, string msg)
        {
            bool c = true;
            if (code.Equals("000") && msg.Equals("")) return false;
            return c;
        }

        public string PostToSAP(FBSS_SubmitFOARevalueResponse sapData)
        {
            var COM_CODE_query = from item in _fixAssConfig.Get()
                                     where item.PROGRAM_NAME == "FlagTypeItem_S4"
                                     select item.COM_CODE;
            var POST_SAP = _cfgLov.Get(x => x.LOV_TYPE == "ReturnedRevalueAsset" && x.LOV_NAME == "POST_SAP").FirstOrDefault();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                string url = POST_SAP.LOV_VAL1;

                client.DefaultRequestHeaders.Add(POST_SAP.LOV_VAL2, POST_SAP.LOV_VAL3);

                var requestBody = new
                {
                    MessageID = "",
                    PartnerName = "PAYG",
                    PartnerMessageID = sapData.TRANS_ID,
                    FlagTypeHeader = "V",
                    LegacyDocNo = sapData.REF_DOC_NO,
                    Item = new
                    {
                        ItemNumber = sapData.ITEM_NUMBER,
                        FlagTypeItem = COM_CODE_query.ToSafeString(),
                        ItemID = sapData.RUN_GROUP,
                        CompanyCode = sapData.COM_CODE,
                        AssetNumber = sapData.MAIN_ASSET,
                        AssetSubNumber = sapData.SUBNUMBER,
                        DocumentDate = sapData.DOC_DATE,
                        PostingDate = sapData.POST_DATE,
                        AssetValueDate = sapData.ASSET_VALUE_DATE,
                        Text = sapData.ITEM_TEXT,
                        Reference = sapData.INTERNET_NO,
                        Assignment = sapData.ASSIGNMENT
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();

                    var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine("Response from SAP API: " + responseString);
                    return responseString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error posting to SAP: " + ex.InnerException);
                    return $"Error: {ex.Message}";
                }
            }
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


    }
}
