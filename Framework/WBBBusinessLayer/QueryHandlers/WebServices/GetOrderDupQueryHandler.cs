using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetOrderDupQueryHandler : IQueryHandler<GetOrderDupQuery, List<OrderDupModel>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<OrderDupModel> _storedOrderDup;

        public GetOrderDupQueryHandler(ILogger logger, IAirNetEntityRepository<OrderDupModel> storedOrderDup)
        {
            _logger = logger;
            _storedOrderDup = storedOrderDup;
        }

        public List<OrderDupModel> Handle(GetOrderDupQuery query)
        {
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var v_error_msg = new OracleParameter();
            v_error_msg.OracleDbType = OracleDbType.Varchar2;
            v_error_msg.Size = 2000;
            v_error_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var cursor = new OracleParameter();
            cursor.OracleDbType = OracleDbType.RefCursor;
            cursor.Direction = ParameterDirection.Output;

            // 3209800123027
            List<OrderDupModel> executeResult = _storedOrderDup.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_LIST_ORD_DUP",
               new
               {
                   p_id_card = query.p_id_card,
                   p_eng_flag = query.p_eng_flag,
                   ret_code = ret_code,
                   ret_msg = v_error_msg,
                   ret_data = cursor
               }).ToList();

            var Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToSafeString()) : -1;
            var Return_Desc = v_error_msg.Value.ToSafeString();

            return executeResult;
        }

    }
}
