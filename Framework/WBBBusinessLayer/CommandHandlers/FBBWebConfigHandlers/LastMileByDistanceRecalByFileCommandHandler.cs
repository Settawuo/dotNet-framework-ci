using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class LastMileByDistanceRecalByFileCommandHandler : ICommandHandler<LastMileByDistanceRecalByFileCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public LastMileByDistanceRecalByFileCommandHandler(ILogger ILogger, IEntityRepository<string> objService
                  , IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = ILogger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }
        public void Handle(LastMileByDistanceRecalByFileCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                var packageMappingObjectModel = new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.LastMileByDistanceRecalByOrderCommandHandler.PackageMappingObjectModel
                {
                    FBB_RECAL_LMR_LIST =
                        command.p_recal_access_list.Select(
                           a => new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.LastMileByDistanceRecalByOrderCommandHandler.FBB_RECAL_LMR_LISTMapping
                           {
                               ACCESS_NUMBER = a.ACCESS_NUMBER,
                               ORDER_NO = a.ORDER_NO,
                               NEW_RULE_ID = a.NEW_RULE_ID,
                               REMARK = a.REMARK

                           }).ToArray()
                };

                var p_recal_access_list = OracleCustomTypeUtilities.CreateUDTArrayParameter("p_recal_access_list", "FBB_RECAL_LMR_LIST", packageMappingObjectModel);

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var msg_logfile = new OracleParameter();
                msg_logfile.OracleDbType = OracleDbType.Varchar2;
                msg_logfile.Size = 2000;
                msg_logfile.Direction = ParameterDirection.Output;


                var outp = new List<object>();
                var paramOut = outp.ToArray();
                _logger.Info("StartPKG_FIXED_ASSET_LASTMILE.p_re_cal_by_file");

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_re_cal_by_file",
                    out paramOut,
                    new
                    {
                        p_recal_access_list,
                        command.p_USER,
                        command.p_STATUS,
                        command.p_filename,
                        ret_code,
                        msg_logfile,
                        ret_msg
                    });
                command.ret_code = ret_code.Value.ToSafeString();
                command.ret_msg = ret_msg.Value.ToSafeString();
                command.messege_log_file = msg_logfile.Value.ToSafeString();

                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "Lastmile Re-Cal Distance";
                    historyLog.CREATED_BY = "Lastmile";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = command.ret_msg;
                    historyLog.REF_KEY = "LastmileReCal";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }
                _logger.Info("EndPKG_FIXED_ASSET_LASTMILE.p_re_cal_by_file" + ret_msg);

            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Lastmile Re-Cal Distance";
                historyLog.CREATED_BY = "Lastmile";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk Lastmile Re-cal By File " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "LastmileReCal";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error call LastMileBy Distance Re-cal By  File " + ex.GetErrorMessage();
            }

        }
    }
}
