using Oracle.ManagedDataAccess.Client;
using System;
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
    public class ChangeCountInstallDateQueryHandler : IQueryHandler<ChangeCountInstallDateQuery, ChangeCountInstallDateQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ChangeCountInstallDateQueryModel> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public ChangeCountInstallDateQueryHandler(ILogger logger,
            IAirNetEntityRepository<ChangeCountInstallDateQueryModel> objServiceSubj
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _intfLog = intfLog;
            _uow = uow;
        }
        public ChangeCountInstallDateQueryModel Handle(ChangeCountInstallDateQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var ret_result = new OracleParameter();
            ret_result.OracleDbType = OracleDbType.Decimal;
            ret_result.Direction = ParameterDirection.Output;

            ChangeCountInstallDateQueryModel ResultData = new ChangeCountInstallDateQueryModel();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_order_no, "PROC_COUNT_CHANGE_INSTALL_DATE", "COUNT_CHANGE_INSTALL_DATE", query.p_id_card, "", "WEB");
                // 
                var executeResult = _objServiceSubj.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_COUNT_CHANGE_INSTALL_DATE",
                     new
                     {
                         p_id_card = query.p_id_card,
                         p_order_no = query.p_order_no,
                         // out
                         ret_code = ret_code,
                         ret_msg = ret_msg,
                         ret_result = ret_result
                     }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                decimal retre = Decimal.Parse(ret_result.Value.ToSafeString());
                ResultData.RET_CODE = ret;
                ResultData.RET_MSG = ret_msg.Value.ToSafeString();
                ResultData.RET_RESULT = retre;
            }
            catch (Exception ex)
            {
                ResultData.RET_RESULT = -1;
                ResultData.RET_CODE = -1;
                ResultData.RET_MSG = ex.Message;
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
