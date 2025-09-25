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
    public class GetSelfServiceChangeHandler : IQueryHandler<SelfServiceChangeQuery, SelfServiceChangeModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<ListTimeSlotModel> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        public GetSelfServiceChangeHandler(ILogger logger, IAirNetEntityRepository<ListTimeSlotModel> objServiceSubj
            , IEntityRepository<FBB_INTERFACE_LOG> intfLog
            , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _uow = uow;
        }
        public SelfServiceChangeModel Handle(SelfServiceChangeQuery query)
        {
            InterfaceLogCommand log = null;
            var ret_code = new OracleParameter();
            ret_code.OracleDbType = OracleDbType.Decimal;
            ret_code.Direction = ParameterDirection.Output;

            var ret_msg = new OracleParameter();
            ret_msg.OracleDbType = OracleDbType.Varchar2;
            ret_msg.Size = 2000;
            ret_msg.Direction = ParameterDirection.Output;

            var outp = new List<object>();
            var paramOut = outp.ToArray();

            var ret_data = new OracleParameter();
            ret_data.OracleDbType = OracleDbType.RefCursor;
            ret_data.Direction = ParameterDirection.Output;

            SelfServiceChangeModel ResultData = new SelfServiceChangeModel();
            List<ListTimeSlotModel> executeResult = new List<ListTimeSlotModel>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "PROC_GET_ORD_TIME_SLOT", "GetTimeSlot", query.P_MOBILE_NO, query.P_ID_CARD, "WEB");
                // 
                executeResult = _objServiceSubj.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR005.PROC_GET_ORD_TIME_SLOT",
                   new
                   {
                       p_id_card = query.P_ID_CARD,
                       p_mobile_no = query.P_MOBILE_NO,
                       p_id_card_type = query.P_ID_CARD_TYPE,
                       p_order_id = query.P_ORDER_ID,
                       // out
                       ret_code = ret_code,
                       ret_msg = ret_msg,
                       ret_data = ret_data
                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                decimal ret = Decimal.Parse(ret_code.Value.ToSafeString());
                ResultData.RET_CODE = ret;
                ResultData.RET_MSG = ret_msg.Value.ToSafeString();
                if (executeResult.Count > 0)
                {
                    ResultData.RET_DATA = executeResult;
                }

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
