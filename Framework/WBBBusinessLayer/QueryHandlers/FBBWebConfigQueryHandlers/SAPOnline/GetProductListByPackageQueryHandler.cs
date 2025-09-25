using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class GetProductListByPackageQueryHandler : IQueryHandler<GetProductListByPackageQuery, SubmitFOAResend>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SubmitFOAResend> _objService;

        public GetProductListByPackageQueryHandler(
            ILogger logger, IEntityRepository<SubmitFOAResend> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public SubmitFOAResend Handle(GetProductListByPackageQuery query)
        {

            var p_ORDER_NO = new OracleParameter();
            p_ORDER_NO.ParameterName = "p_ORDER_NO";
            p_ORDER_NO.Size = 2000;
            p_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
            p_ORDER_NO.Direction = ParameterDirection.Input;
            p_ORDER_NO.Value = query.OrderNo;

            var p_INTERNET_NO = new OracleParameter();
            p_INTERNET_NO.ParameterName = "p_INTERNET_NO";
            p_INTERNET_NO.Size = 2000;
            p_INTERNET_NO.OracleDbType = OracleDbType.Varchar2;
            p_INTERNET_NO.Direction = ParameterDirection.Input;
            p_INTERNET_NO.Value = query.AccessNo;

            var action = new OracleParameter();
            action.ParameterName = "action";
            action.Size = 2000;
            action.OracleDbType = OracleDbType.Varchar2;
            action.Direction = ParameterDirection.Input;
            action.Value = query.flag_auto_resend;

            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var cur_order = new OracleParameter();
            cur_order.ParameterName = "cur_order";
            cur_order.OracleDbType = OracleDbType.RefCursor;
            cur_order.Direction = ParameterDirection.Output;

            var cur_product = new OracleParameter();
            cur_product.ParameterName = "cur_product";
            cur_product.OracleDbType = OracleDbType.RefCursor;
            cur_product.Direction = ParameterDirection.Output;

            var cur_service = new OracleParameter();
            cur_service.ParameterName = "cur_service";
            cur_service.OracleDbType = OracleDbType.RefCursor;
            cur_service.Direction = ParameterDirection.Output;

            var result = _objService.ExecuteStoredProcMultipleCursor("WBB.P_FBB_GET_PRODUCTLIST",
                new object[]
                {
                    p_ORDER_NO,
                    p_INTERNET_NO,
                    ret_code,

                    cur_order,
                    cur_product,
                    cur_service,
                    action
                });

            SubmitFOAResend executeResults = new SubmitFOAResend();

            DataTable dtTableMainRespones = (DataTable)result[1];
            foreach (DataRow dr in dtTableMainRespones.Rows)
            {
                executeResults.AccessNo = dr.ItemArray[0].ToSafeString();
                executeResults.OrderNumber = dr.ItemArray[1].ToSafeString();
                executeResults.SubcontractorCode = dr.ItemArray[2].ToSafeString();
                executeResults.SubcontractorName = dr.ItemArray[3].ToSafeString();
                executeResults.ProductName = dr.ItemArray[4].ToSafeString();
                executeResults.ServiceName = dr.ItemArray[5].ToSafeString();
                executeResults.OrderType = dr.ItemArray[6].ToSafeString();
                executeResults.SubmitFlag = dr.ItemArray[7].ToSafeString();
                executeResults.RejectReason = dr.ItemArray[8].ToSafeString();
                if (dr.ItemArray[9].ToSafeString() != "")
                {
                    executeResults.FOA_Submit_date_value = (DateTime?)dr.ItemArray[9];
                }
                else
                {
                    executeResults.FOA_Submit_date_value = null;
                }
                executeResults.OLT_NAME = dr.ItemArray[10].ToSafeString();
                executeResults.BUILDING_NAME = dr.ItemArray[11].ToSafeString();
                executeResults.Mobile_Contact = dr.ItemArray[12].ToSafeString();
                executeResults.ADDRESS_ID = dr.ItemArray[13].ToSafeString();
                executeResults.ORG_ID = dr.ItemArray[14].ToSafeString();
                executeResults.REUSE_FLAG = dr.ItemArray[15].ToSafeString();
                executeResults.EVENT_FLOW_FLAG = dr.ItemArray[16].ToSafeString();
                executeResults.SUBCONTRACT_TYPE = dr.ItemArray[17].ToSafeString();
                executeResults.SUBCONTRACT_SUB_TYPE = dr.ItemArray[18].ToSafeString();
                executeResults.REQUEST_SUB_FLAG = dr.ItemArray[19].ToSafeString();
                executeResults.SUB_ACCESS_MODE = dr.ItemArray[20].ToSafeString();
                executeResults.PRODUCT_OWNER = dr.ItemArray[21].ToSafeString();
                executeResults.MAIN_PROMO_CODE = dr.ItemArray[22].ToSafeString();
                executeResults.TEAM_ID = dr.ItemArray[23].ToSafeString();
            }

            //Product List
            DataTable dtTableProductListRespones = (DataTable)result[2];
            List<SubmitFOAProduct> _productList = dtTableProductListRespones.DataTableToList<SubmitFOAProduct>();
            executeResults.ProductList = _productList;

            //Service List
            DataTable dtTableServiceListRespones = (DataTable)result[2];
            List<SubmitFOAInstall> _serviceList = dtTableServiceListRespones.DataTableToList<SubmitFOAInstall>();
            executeResults.InstallList = _serviceList;

            return executeResults;
        }

    }
}
