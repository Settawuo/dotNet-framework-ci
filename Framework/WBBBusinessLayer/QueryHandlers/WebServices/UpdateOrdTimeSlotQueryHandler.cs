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
    public class UpdateOrdTimeSlotQueryHandler : IQueryHandler<UpdateOrdTimeSlotQuery, UpdateOrdTimeSlotModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<UpdateOrdTimeSlotModel> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public UpdateOrdTimeSlotQueryHandler(ILogger logger, IAirNetEntityRepository<UpdateOrdTimeSlotModel> objServiceSubj
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _uow = uow;
        }
        public UpdateOrdTimeSlotModel Handle(UpdateOrdTimeSlotQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            UpdateOrdTimeSlotModel ResultData = new UpdateOrdTimeSlotModel();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_ORDER_NO, "PROC_UPDATE_ORD_TIME_SLOT", "UpdateTimeslot", query.ID_CARD_NO, query.P_RESERVED_ID, "WEB");
                // 
                var executeResult = _objServiceSubj.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_UPDATE_ORD_TIME_SLOT",
                     new
                     {
                         p_order_no = query.P_ORDER_NO,
                         p_install_date = query.P_INSTALL_DATE,
                         p_time_slot = query.P_TIME_SLOT,
                         p_reserved_id = query.P_RESERVED_ID,
                         p_user = query.P_USER,
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
