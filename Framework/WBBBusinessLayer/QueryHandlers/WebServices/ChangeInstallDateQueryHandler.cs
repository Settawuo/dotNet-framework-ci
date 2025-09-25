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
    public class ChangeInstallDateQueryHandler : IQueryHandler<ChangeInstallDateQuery, ChangeInstallDateModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ListTimeSlotModel> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public ChangeInstallDateQueryHandler(ILogger logger, IAirNetEntityRepository<ListTimeSlotModel> objServiceSubj
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _uow = uow;
        }
        public ChangeInstallDateModel Handle(ChangeInstallDateQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            ChangeInstallDateModel ResultData = new ChangeInstallDateModel();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_ORDER_NO, "PROC_CHANGE_INSTALL_DATE", "ChangeInstallDate", query.P_ID_CARD, query.P_MOBILE_NO, "WEB");
                // 
                var executeResult = _objServiceSubj.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_CHANGE_INSTALL_DATE",
                     new
                     {
                         p_order_no = query.P_ORDER_NO,
                         p_id_card = query.P_ID_CARD,
                         p_mobile_no = query.P_MOBILE_NO,
                         p_non_mobile_no = query.P_NON_MOBILE_NO,
                         p_install_date = query.P_INSTALL_DATE,
                         p_time_slot = query.P_TIME_SLOT,
                         // out
                         ret_code = ret_code,
                         ret_msg = ret_msg,
                     }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                ResultData.RET_CODE = ret;
                ResultData.RET_MSG = ret_msg.Value.ToSafeString();


            }
            catch (Exception ex)
            {
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
