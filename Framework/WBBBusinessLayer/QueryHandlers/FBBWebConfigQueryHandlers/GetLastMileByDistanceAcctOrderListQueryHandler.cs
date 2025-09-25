using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetLastMileByDistanceAcctOrderListQueryHandler : IQueryHandler<GetLastMileByDistanceAcctOrderListQuery, List<AcctOrderListModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<AcctOrderListModel> _objService;

        public GetLastMileByDistanceAcctOrderListQueryHandler(ILogger logger, IEntityRepository<AcctOrderListModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<AcctOrderListModel> Handle(GetLastMileByDistanceAcctOrderListQuery query)
        {
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

                var executeResult = _objService.ExecuteReadStoredProc(
                    "WBB.PKG_FIXED_ASSET_LASTMILE.p_acct_order_list",
                    new
                    {
                        query.p_ORDER_NO,
                        query.p_ACCESS_NO,
                        query.p_PRODUCT_NAME,
                        query.p_SUBCONT_CODE,
                        query.p_SUBCONT_NAME,
                        query.p_IR_DOC,
                        query.p_INVOICE_NO,
                        query.p_ORDER_STATUS,
                        query.p_REGION,
                        query.p_APPROVE_FM,
                        query.p_APPROVE_TO,
                        query.p_PERIOD_FM,
                        query.p_PERIOD_TO,
                        query.p_UPDATE_DT,
                        query.p_UPDATE_BY,
                        ret_code,
                        cur
                    }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.ret_code = "-1";
                query.ret_code = "PKG_FIXED_ASSET_LASTMILE.p_acct_order_list Error : " + ex.Message;

                return null;
            }
        }
    }
}
