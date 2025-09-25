using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetLastMileByDistanceOrderListQueryHandler : IQueryHandler<GetLastMileByDistanceOrderListQuery, List<LastMileByDistanceOrderReturn>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LastMileByDistanceOrderReturn> _objService;

        public GetLastMileByDistanceOrderListQueryHandler(ILogger logger, IEntityRepository<LastMileByDistanceOrderReturn> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<LastMileByDistanceOrderReturn> Handle(GetLastMileByDistanceOrderListQuery query)
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

                _logger.Info("StartCommand_FIXED_ASSET_LASTMILE");

                List<LastMileByDistanceOrderReturn> executeResultCmm = _objService.SqlQuery(query.p_Command).ToList();

                var LMD = executeResultCmm.Select(lmd => new LastMileByDistanceOrderReturn()
                {
                    PERIOD = lmd.PERIOD.ToString(),
                    ORDER_STATUS_DT = lmd.ORDER_STATUS_DT,
                    ACCESS_NUMBER = lmd.ACCESS_NUMBER,
                    ACCOUNT_NAME = lmd.ACCOUNT_NAME.ToString(),
                    APPOINTMENNT_DT = lmd.APPOINTMENNT_DT,
                    PROMOTION_NAME = lmd.PROMOTION_NAME.ToString(),
                    SBC_CPY = lmd.SBC_CPY.ToString(),
                    DISTANCE_TOTAL = lmd.DISTANCE_TOTAL,
                    TOTAL_PAID = lmd.TOTAL_PAID,
                    ENTRY_FEE = lmd.ENTRY_FEE,
                    ORG_ID = lmd.ORG_ID,
                    ORDER_STATUS = lmd.ORDER_STATUS,
                    subcontract_email = lmd.subcontract_email
                }).ToList();


                return LMD;
            }
            catch (Exception ex)
            {
                _logger.Info("Error call Command_FIXED_ASSET_LASTMILE handles : " + ex.Message);

                //returnForm.ret_code = "-1";
                //returnForm.ret_code = "Command_FIXED_ASSET_LASTMILE.p_search_order_list Error : " + ex.Message;

                return null;
            }
        }
    }
}
