using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetFixedOM010SendMailFileLogQueryHandler : IQueryHandler<GetFixedOM010SendMailFileLogQuery, ReturnFBSSSendMailFileLogBatchModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBSSFixedOM010SendMailFileLogModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        public GetFixedOM010SendMailFileLogQueryHandler(
            ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBSSFixedOM010SendMailFileLogModel> objService,
            IEntityRepository<FBB_HISTORY_LOG> historyLog
               )
        {
            _logger = logger;
            _uow = uow;
            _historyLog = historyLog;
            _objService = objService;


        }
        public ReturnFBSSSendMailFileLogBatchModel Handle(GetFixedOM010SendMailFileLogQuery query)
        {
            var historyLog = new FBB_HISTORY_LOG();
            ReturnFBSSSendMailFileLogBatchModel Result = new ReturnFBSSSendMailFileLogBatchModel();

            try
            {
                #region OracleParameter


                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var p_ws_return_cur = new OracleParameter();
                p_ws_return_cur.OracleDbType = OracleDbType.RefCursor;
                p_ws_return_cur.Direction = ParameterDirection.Output;


                #endregion
                List<FBSSFixedOM010SendMailFileLogModel> resultReturn = _objService.ExecuteReadStoredProc(
                      "WBB.PKG_FBBEMAILERROR_LOADFILE.p_sent_mail",
                      new
                      {

                          ret_code,
                          ret_msg,
                          p_ws_return_cur

                      }).ToList();
                Result.ret_code = ret_code.Value != null ? ret_code.Value.ToSafeString() : "1";
                Result.ret_msg = ret_msg.Value != null ? ret_msg.Value.ToString() : "";
                Result.cur = resultReturn;

                _logger.Info("Call WBB.PKG_FBBEMAILERROR_LOADFILE.p_sent_mail: ret_code : " + ret_code + " ret_msg:" + ret_msg);

                if (Result.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Batch FBSSFixedOM010SendMailFileLogBatch";
                    historyLog.CREATED_BY = "FBSSFixedOM010SendMailFileLogBatch";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = "Error ConfigFBSSFixedOM010SendMailFileLogBatch " + Result.ret_msg;
                    historyLog.REF_KEY = "FBSSFixedOM010SendMailFileLogBatch";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
                return Result;
            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Batch FBSSFixedOM010SendMailFileLogBatch";
                historyLog.CREATED_BY = "FBSSFixedOM010SendMailFileLogBatch";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Error ConfigFBSSFixedOM010SendMailFileLogBatch " + ex.GetErrorMessage();
                historyLog.REF_KEY = "FBSSFixedOM010SendMailFileLogBatch";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();
                Result.ret_code = "1";
                Result.ret_msg = ex.GetErrorMessage();

                return Result;
            }

        }
    }
}
