using AIRNETEntity.Extensions;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw
{

    public class UpdateCoverageRusultHandler : ICommandHandler<UpdateCoverageRusultCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public UpdateCoverageRusultHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(UpdateCoverageRusultCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_channel, "UpdateCoverageRusultHandler", "UpdateCoverageRusultHandler", command.p_channel, "FBB", "WEB");

                //Input
                var p_order_no = new OracleParameter();
                p_order_no.ParameterName = "p_order_no";
                p_order_no.OracleDbType = OracleDbType.Varchar2;
                p_order_no.Size = 1000;
                p_order_no.Direction = ParameterDirection.Input;
                p_order_no.Value = command.p_order_no.ToSafeString();

                var p_channel = new OracleParameter();
                p_channel.ParameterName = "p_channel";
                p_channel.OracleDbType = OracleDbType.Varchar2;
                p_channel.Size = 1000;
                p_channel.Direction = ParameterDirection.Input;
                p_channel.Value = command.p_channel.ToSafeString();

                var p_status_plan = new OracleParameter();
                p_status_plan.ParameterName = "p_status_plan";
                p_status_plan.OracleDbType = OracleDbType.Varchar2;
                p_status_plan.Size = 2000;
                p_status_plan.Direction = ParameterDirection.Input;
                p_status_plan.Value = command.p_status_plan.ToSafeString();

                var p_user_verify = new OracleParameter();
                p_user_verify.ParameterName = "p_user_verify";
                p_user_verify.OracleDbType = OracleDbType.Varchar2;
                p_user_verify.Size = 1000;
                p_user_verify.Direction = ParameterDirection.Input;
                p_user_verify.Value = command.p_user_verify.ToSafeString();

                var p_flag_verify = new OracleParameter();
                p_flag_verify.ParameterName = "p_flag_verify";
                p_flag_verify.OracleDbType = OracleDbType.Varchar2;
                p_flag_verify.Size = 1000;
                p_flag_verify.Direction = ParameterDirection.Input;
                p_flag_verify.Value = command.p_flag_verify.ToSafeString();

                var p_date_verify = new OracleParameter();
                p_date_verify.ParameterName = "p_date_verify";
                p_date_verify.OracleDbType = OracleDbType.Varchar2;
                p_date_verify.Size = 1000;
                p_date_verify.Direction = ParameterDirection.Input;
                p_date_verify.Value = command.p_date_verify.ToSafeString();

                var p_remark = new OracleParameter();
                p_remark.ParameterName = "p_remark";
                p_remark.OracleDbType = OracleDbType.Varchar2;
                p_remark.Size = 5000;
                p_remark.Direction = ParameterDirection.Input;
                p_remark.Value = command.p_remark.ToSafeString();

                //Return
                var o_return_code = new OracleParameter();
                o_return_code.ParameterName = "o_return_code";
                o_return_code.OracleDbType = OracleDbType.Varchar2;
                o_return_code.Size = 2000;
                o_return_code.Direction = ParameterDirection.Output;

                var o_return_message = new OracleParameter();
                o_return_message.ParameterName = "o_return_message";
                o_return_message.OracleDbType = OracleDbType.Varchar2;
                o_return_message.Size = 2000;
                o_return_message.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBBOR004.PROC_UPDATE_COVERAGE",
                    new
                    {
                        //Input
                        p_order_no,
                        p_channel,
                        p_status_plan,
                        p_user_verify,
                        p_flag_verify,
                        p_date_verify,
                        p_remark,

                        //Return
                        o_return_code,
                        o_return_message

                    });

                command.o_return_code = o_return_code.Value.ToSafeString();
                command.o_return_message = o_return_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.o_return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.o_return_code = "-1";
                command.o_return_message = "Error Service UpdateCoverageRusultHandler" + ex.Message;
            }
        }
    }

}
