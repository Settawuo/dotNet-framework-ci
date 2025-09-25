using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class ChangePackageBundlingQueryHandler : IQueryHandler<ChangePackageBundlingQuery, ChangePackageBundlingData>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ChangePackageBundlingList> _changePackageBundlingList;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public ChangePackageBundlingQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<ChangePackageBundlingList> changePackageBundlingList)
        {
            _logger = logger;
            _changePackageBundlingList = changePackageBundlingList;
            _intfLog = intfLog;
            _uow = uow;
        }

        public ChangePackageBundlingData Handle(ChangePackageBundlingQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Size = 2000;
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

            ChangePackageBundlingData ResultData = new ChangePackageBundlingData();
            List<ChangePackageBundlingList> executeResult = new List<ChangePackageBundlingList>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "PROC_CHANGEPROMOTION", "ChangePackageBundling", query.P_MOBILE, "FBB|" + query.FullUrl, "WEB");

                // 
                executeResult = _changePackageBundlingList.ExecuteReadStoredProc("WBB.PKG_FBBOR018.PROC_CHANGEPROMOTION",
                   new
                   {
                       p_non_mobile = query.P_NON_MOBILE,
                       p_mobile = query.P_MOBILE,
                       p_product_main_cd = query.P_PRODUCT_MAIN_CD,
                       p_promotion_ontop_cd = query.P_PROMOTION_ONTOP_CD,
                       p_flag_discount = query.P_FLAG_DISCOUNT,

                       // out
                       p_return_code = ret_code,
                       p_return_message = v_error_msg,
                       p_res_data_head = cursor
                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");

                ResultData.return_code = ret_code.Value.ToSafeString();
                ResultData.return_message = v_error_msg.Value.ToSafeString();
                if (executeResult.Count > 0)
                {
                    ResultData.ChangePackageBundlingList = executeResult;
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }

    }
}
