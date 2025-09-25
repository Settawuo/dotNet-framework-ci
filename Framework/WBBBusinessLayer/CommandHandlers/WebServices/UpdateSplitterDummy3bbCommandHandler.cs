using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.WebServices
{
    public class UpdateSplitterDummy3bbCommandHandler : ICommandHandler<UpdateSplitterDummy3bbCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public UpdateSplitterDummy3bbCommandHandler(
            ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> obj
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public void Handle(UpdateSplitterDummy3bbCommand command)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id,
                    "UpdateSplitterDummy3bbCommand", "UpdateSplitterDummy3bbCommandHandler", "", "FBB|" + "", "");
            try
            {
                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = command.p_address_id;

                var P_SPLITTER_NO = new OracleParameter();
                P_SPLITTER_NO.ParameterName = "P_SPLITTER_NO";
                P_SPLITTER_NO.Size = 2000;
                P_SPLITTER_NO.OracleDbType = OracleDbType.Varchar2;
                P_SPLITTER_NO.Direction = ParameterDirection.Input;
                P_SPLITTER_NO.Value = command.p_splitter_no;

                var P_RETURN_CODE = new OracleParameter();
                P_RETURN_CODE.ParameterName = "P_RETURN_CODE";
                P_RETURN_CODE.Size = 2000;
                P_RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_CODE.Direction = ParameterDirection.Output;

                var P_RETURN_MESSAGE = new OracleParameter();
                P_RETURN_MESSAGE.ParameterName = "P_RETURN_MESSAGE";
                P_RETURN_MESSAGE.Size = 2000;
                P_RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var resultExecute = _obj.ExecuteStoredProcMultipleCursor("WBB.PKG_INTERFACE_LOG_3BB.PROC_UPDATE_SPLITTER",
                      new object[]
                      {
                          //Parameter Input
                          P_ADDRESS_ID,
                          P_SPLITTER_NO,
                          //Parameter Output
                          P_RETURN_CODE,
                          P_RETURN_MESSAGE
                      });

                if (resultExecute != null)
                {
                    command.ReturnCode = resultExecute[0] != null ? resultExecute[0].ToSafeString() : "-1";
                    command.ReturnMsg = resultExecute[1] != null ? resultExecute[1].ToSafeString() : "error";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    command.ReturnMsg = "resultExecute is null.";
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log,
                            "Failed", "", "");
                }
            }
            catch (Exception ex)
            {
                command.ReturnMsg = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log,
                            "Failed", "", "");
            }
        }
    }
}
