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
    public class SendMailLastMileCommandHandler : ICommandHandler<SendMailLastMileCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        public SendMailLastMileCommandHandler(ILogger logger,
             IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IWBBUnitOfWork uow,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;

        }

        public void Handle(SendMailLastMileCommand command)
        {

            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                var PackageMappingObjectModelSendMail = new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.UpdateLastMileByDistanceByOrderCommandHandler.PackageMappingObjectModel

                {
                    FBB_LASTMILE_ACCESS_LIST =
                        command.fixed_order.Select(
                            a => new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.UpdateLastMileByDistanceByOrderCommandHandler.FBB_LASTMILE_ACCESS_LISTMapping
                            {
                                ACCESS_NUMBER = a.ACCESS_NUMBER
                            }).ToArray()
                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("FIXED_ORDER", "FBB_LASTMILE_ACCESS_LIST", PackageMappingObjectModelSendMail);
                //Parameter Out
                var ret_Code = new OracleParameter();
                ret_Code.OracleDbType = OracleDbType.Int32;
                ret_Code.Direction = ParameterDirection.Output;

                var ret_Msg = new OracleParameter();
                ret_Msg.OracleDbType = OracleDbType.Varchar2;
                ret_Msg.Size = 2000;
                ret_Msg.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                var execute = _objService.ExecuteStoredProc("WBB.PKG_FIXED_ASSET_LASTMILE.p_update_period",
                    out paramOut,
                    new
                    {
                        period_from = command.p_period_from,
                        period_to = command.p_period_to,
                        packageMapping,
                        user_name = command.p_USER,

                        // out
                        ret_code = ret_Code,
                        ret_msg = ret_Msg,


                    });
                command.ret_code = (ret_Code.Value.ToString() != null) ? int.Parse(ret_Code.Value.ToString()) : 1;
                command.ret_msg = ret_Msg.Value.ToSafeString();


            }
            catch (Exception ex)
            {
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.ADD.ToString();
                historyLog.APPLICATION = "Lastmile SendMail";
                historyLog.CREATED_BY = "LastmileSendmail";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk Lastmile p_update_period " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "LastmileSendmail";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = 1;
                command.ret_msg = "Error call SendMailLastMileCommand Handler: " + ex.GetErrorMessage();
            }

        }



    }
}
