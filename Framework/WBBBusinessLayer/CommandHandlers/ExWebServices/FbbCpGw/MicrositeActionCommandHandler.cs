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
    public class MicrositeActionCommandHandler : ICommandHandler<MicrositeActionCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public MicrositeActionCommandHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(MicrositeActionCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.P_ORDER_NO, "MicrositeActionCommandHandler", "MicrositeActionCommandHandler", command.P_ORDER_NO, "FBB", "WEB");

                //Input
                var P_ORDER_NO = new OracleParameter();
                P_ORDER_NO.ParameterName = "P_ORDER_NO";
                P_ORDER_NO.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_NO.Size = 1000;
                P_ORDER_NO.Direction = ParameterDirection.Input;
                P_ORDER_NO.Value = command.P_ORDER_NO.ToSafeString();

                var P_ORDER_CHANNEL = new OracleParameter();
                P_ORDER_CHANNEL.ParameterName = "P_ORDER_CHANNEL";
                P_ORDER_CHANNEL.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_CHANNEL.Size = 1000;
                P_ORDER_CHANNEL.Direction = ParameterDirection.Input;
                P_ORDER_CHANNEL.Value = command.P_ORDER_CHANNEL.ToSafeString();

                var P_IS_CONTACT_CUSTOMER = new OracleParameter();
                P_IS_CONTACT_CUSTOMER.ParameterName = "P_IS_CONTACT_CUSTOMER";
                P_IS_CONTACT_CUSTOMER.OracleDbType = OracleDbType.Varchar2;
                P_IS_CONTACT_CUSTOMER.Size = 1000;
                P_IS_CONTACT_CUSTOMER.Direction = ParameterDirection.Input;
                P_IS_CONTACT_CUSTOMER.Value = command.P_IS_CONTACT_CUSTOMER.ToSafeString();

                var P_IS_IN_COVERAGE = new OracleParameter();
                P_IS_IN_COVERAGE.ParameterName = "P_IS_IN_COVERAGE";
                P_IS_IN_COVERAGE.OracleDbType = OracleDbType.Varchar2;
                P_IS_IN_COVERAGE.Size = 1000;
                P_IS_IN_COVERAGE.Direction = ParameterDirection.Input;
                P_IS_IN_COVERAGE.Value = command.P_IS_IN_COVERAGE.ToSafeString();

                var P_USER_ACTION = new OracleParameter();
                P_USER_ACTION.ParameterName = "P_USER_ACTION";
                P_USER_ACTION.OracleDbType = OracleDbType.Varchar2;
                P_USER_ACTION.Size = 1000;
                P_USER_ACTION.Direction = ParameterDirection.Input;
                P_USER_ACTION.Value = command.P_USER_ACTION.ToSafeString();

                var P_DATE_ACTION = new OracleParameter();
                P_DATE_ACTION.ParameterName = "P_DATE_ACTION";
                P_DATE_ACTION.OracleDbType = OracleDbType.Varchar2;
                P_DATE_ACTION.Size = 2000;
                P_DATE_ACTION.Direction = ParameterDirection.Input;
                P_DATE_ACTION.Value = command.P_DATE_ACTION.ToSafeString();

                var P_ORDER_PRE_REGISTER = new OracleParameter();
                P_ORDER_PRE_REGISTER.ParameterName = "P_ORDER_PRE_REGISTER";
                P_ORDER_PRE_REGISTER.OracleDbType = OracleDbType.Varchar2;
                P_ORDER_PRE_REGISTER.Size = 1000;
                P_ORDER_PRE_REGISTER.Direction = ParameterDirection.Input;
                P_ORDER_PRE_REGISTER.Value = command.P_ORDER_PRE_REGISTER.ToSafeString();

                var P_STATUS_ORDER = new OracleParameter();
                P_STATUS_ORDER.ParameterName = "P_STATUS_ORDER";
                P_STATUS_ORDER.OracleDbType = OracleDbType.Varchar2;
                P_STATUS_ORDER.Size = 1000;
                P_STATUS_ORDER.Direction = ParameterDirection.Input;
                P_STATUS_ORDER.Value = command.P_STATUS_ORDER.ToSafeString();

                var P_REMARK_NOTE = new OracleParameter();
                P_REMARK_NOTE.ParameterName = "P_REMARK_NOTE";
                P_REMARK_NOTE.OracleDbType = OracleDbType.Varchar2;
                P_REMARK_NOTE.Size = 4000;
                P_REMARK_NOTE.Direction = ParameterDirection.Input;
                P_REMARK_NOTE.Value = command.P_REMARK_NOTE.ToSafeString();

                //Return
                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "p_return_code";
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Size = 2000;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "p_return_message";
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Size = 2000;
                p_return_message.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_MICROSITE.PROC_INSERT_MICROSITE_HISTORY",
                    new
                    {
                        //Input
                        P_ORDER_NO,
                        P_ORDER_CHANNEL,
                        P_IS_CONTACT_CUSTOMER,
                        P_IS_IN_COVERAGE,
                        P_USER_ACTION,
                        P_DATE_ACTION,
                        P_ORDER_PRE_REGISTER,
                        P_STATUS_ORDER,
                        P_REMARK_NOTE,

                        //Return
                        p_return_code,
                        p_return_message

                    });

                command.p_return_code = p_return_code.Value.ToSafeString();
                command.p_return_message = p_return_message.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.p_return_code, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.p_return_code = "-1";
                command.p_return_message = "Error Service MicrositeActionCommandHandler" + ex.Message;
            }
        }
    }
}
