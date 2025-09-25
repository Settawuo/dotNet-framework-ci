using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InsertLogDataPrivacyCommandHandler : ICommandHandler<InsertLogDataPrivacyCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public InsertLogDataPrivacyCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InsertLogDataPrivacyCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id, "InsertLogDataPrivacy", "InsertLogDataPrivacyCommandHandler", "", "FBB", "");

                var P_CHANNEL = new OracleParameter();
                P_CHANNEL.ParameterName = "P_CHANNEL";
                P_CHANNEL.Size = 2000;
                P_CHANNEL.OracleDbType = OracleDbType.Varchar2;
                P_CHANNEL.Direction = ParameterDirection.Input;
                P_CHANNEL.Value = command.P_CHANNEL.ToSafeString();

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "P_FIBRENET_ID";
                P_FIBRENET_ID.Size = 2000;
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = command.P_FIBRENET_ID.ToSafeString();

                var P_MOBILE_NO = new OracleParameter();
                P_MOBILE_NO.ParameterName = "P_MOBILE_NO";
                P_MOBILE_NO.Size = 2000;
                P_MOBILE_NO.OracleDbType = OracleDbType.Varchar2;
                P_MOBILE_NO.Direction = ParameterDirection.Input;
                P_MOBILE_NO.Value = command.P_MOBILE_NO.ToSafeString();

                var P_CONFIRM_MKT = new OracleParameter();
                P_CONFIRM_MKT.ParameterName = "P_CONFIRM_MKT";
                P_CONFIRM_MKT.Size = 2000;
                P_CONFIRM_MKT.OracleDbType = OracleDbType.Varchar2;
                P_CONFIRM_MKT.Direction = ParameterDirection.Input;
                P_CONFIRM_MKT.Value = command.P_CONFIRM_MKT.ToSafeString();

                var P_CONFIRM_PRIVILEGE = new OracleParameter();
                P_CONFIRM_PRIVILEGE.ParameterName = "P_CONFIRM_PRIVILEGE";
                P_CONFIRM_PRIVILEGE.Size = 2000;
                P_CONFIRM_PRIVILEGE.OracleDbType = OracleDbType.Varchar2;
                P_CONFIRM_PRIVILEGE.Direction = ParameterDirection.Input;
                P_CONFIRM_PRIVILEGE.Value = command.P_CONFIRM_PRIVILEGE.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 2000;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR051.INSERT_LOG_DATA_PRIVACY ",
                    new object[]
                    {
                        //in 
                        P_CHANNEL,
                        P_FIBRENET_ID,
                        P_MOBILE_NO,
                        P_CONFIRM_MKT,
                        P_CONFIRM_PRIVILEGE,

                        /// Out
                        RETURN_CODE,
                        RETURN_MESSAGE
                    });

                command.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                command.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";

                if (command.RETURN_CODE != "-1")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                command.RETURN_CODE = "-1";
                command.RETURN_MESSAGE = "error InsertLogDataPrivacyCommand " + ex.Message;
            }
        }
    }
}
