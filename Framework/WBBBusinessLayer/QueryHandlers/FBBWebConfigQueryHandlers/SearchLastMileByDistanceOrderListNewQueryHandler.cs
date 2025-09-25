using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SearchLastMileByDistanceOrderListNewQueryHandler : IQueryHandler<SearchLastMileByDistanceOrderListNewQuery, LastMileByDistanceOrderListReturn>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LastMileByDistanceOrderListModel> _objService;

        public SearchLastMileByDistanceOrderListNewQueryHandler(ILogger logger, IEntityRepository<LastMileByDistanceOrderListModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public LastMileByDistanceOrderListReturn Handle(SearchLastMileByDistanceOrderListNewQuery query)
        {
            var returnForm = new LastMileByDistanceOrderListReturn();
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                var cur = new OracleParameter
                {
                    ParameterName = "cur",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE");


                var executeResult = _objService.ExecuteReadStoredProc(
                    //"WBB.PKG_FIXED_ASSET_LASTMILE.p_search_order_list",
                    "WBB.PKG_FIXED_ASSET_LASTMILE.p_search_order",
                    new
                    {
                        query.p_ORDER_NO,
                        query.p_ACCESS_NO,
                        query.p_PRODUCT_NAME,
                        query.p_SUBCONT_CODE,
                        query.p_ORG_ID,
                        query.p_SUBCONT_TYPE,
                        query.p_SUBCONT_SUB_TYPE,
                        //query.p_SUBCONT_NAME,
                        query.p_IR_DOC,
                        query.p_INVOICE_NO,
                        query.p_WORK_STATUS,
                        query.p_ORDER_STATUS,
                        //query.p_ORD_STATUS,
                        query.p_ORDER_TYPE,
                        query.p_REGION,
                        query.p_FOA_FM,
                        query.p_FOA_TO,
                        query.p_APPROVE_FM,
                        query.p_APPROVE_TO,
                        query.p_PERIOD_FM,
                        query.p_PERIOD_TO,
                        query.p_TRANS_FM,
                        query.p_TRANS_TO,
                        query.p_PRODUCT_OWNER,
                        //query.p_UPDATE_BY,
                        //query.P_PAGE_INDEX,
                        //query.P_PAGE_SIZE,
                        ret_code,
                        cur
                    }).ToList();

                returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                returnForm.ret_code = ret_code.Value != null ? ret_code.Value.ToString() : "-1";
                returnForm.cur = executeResult;

                return returnForm;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FIXED_ASSET_LASTMILE handles : " + ex.Message);

                returnForm.ret_code = "-1";
                returnForm.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_search_order_list Error : " + ex.Message;

                return null;
            }
        }
    }
}
