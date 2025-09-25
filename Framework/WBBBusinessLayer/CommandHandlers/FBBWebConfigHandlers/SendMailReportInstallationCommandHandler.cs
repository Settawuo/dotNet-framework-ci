using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SendMailReportInstallationCommandHandler : ICommandHandler<SendMailReportInstallationCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interLog;
        private string transactionID;
        public SendMailReportInstallationCommandHandler(ILogger logger,
             IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IWBBUnitOfWork uow,
             IEntityRepository<FBB_INTERFACE_LOG> interLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
            transactionID = DateTime.Now.ToString("yyyyMMddHHmmss") + "0001";
            _interLog = interLog;

        }

        public void Handle(SendMailReportInstallationCommand command)
        {

            var historyLog = new FBB_HISTORY_LOG();
            try
            {
                var PackageMappingObjectModelSendMail = new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.UpdateReportInstallationCostbyOrderCommandHandler.PackageMappingObjectModel

                {
                    PAYG_INS_RPT_ACCESS_LIST =
                        command.fixed_order.Select(
                            a => new WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers.UpdateReportInstallationCostbyOrderCommandHandler.PAYG_INS_RPT_ACCESS_LISTMapping
                            {
                                ACCESS_NUMBER = a.ACCESS_NUMBER
                            }).ToArray()
                };

                var packageMapping = OracleCustomTypeUtilities.CreateUDTArrayParameter("fixed_order", "WBB.PAYG_INS_RPT_ACCESS_LIST", PackageMappingObjectModelSendMail);
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

                var execute = _objService.ExecuteStoredProc("WBB.PKG_PAYG_INSTALL_COST_RPT.p_update_period",
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
                historyLog.APPLICATION = "ReportInstallation SendMail";
                historyLog.CREATED_BY = "ReportInstallationSendmail";
                historyLog.CREATED_DATE = DateTime.Now;
                historyLog.DESCRIPTION = "Exception Pk ReportInstallation p_update_period " + ex.GetErrorMessage().ToSafeString();
                historyLog.REF_KEY = "ReportInstallationSendmail";
                historyLog.REF_NAME = "NODEID";
                _historyLog.Create(historyLog);
                _uow.Persist();

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = 1;
                command.ret_msg = "Error call SendMailReportInstallationCommandHandler: " + ex.GetErrorMessage();
            }

        }
    }
}