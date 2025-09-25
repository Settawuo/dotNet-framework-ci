using System;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBEntity.Extensions;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FBBProfileCustomer" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FBBProfileCustomer.svc or FBBProfileCustomer.svc.cs at the Solution Explorer and start debugging.
    public class FBBProfileCustomer : IFBBProfileCustomer
    {
        private ILogger _logger;
        private ICommandHandler<ProfileCustomerCommand> _ProfileCustomerCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public FBBProfileCustomer(ILogger logger,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<ProfileCustomerCommand> profileCustomerCommand)
        {
            _logger = logger;
            _intfLogCommand = intfLogCommand;
            _ProfileCustomerCommand = profileCustomerCommand;
        }

        public void SaveProfileCustomer(ProfileCustomerCommand command, out int ret_code, out string ret_message)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<ProfileCustomerCommand>(command, "SaveProfileCustomer", "", "");
                _ProfileCustomerCommand.Handle(command);
                ret_code = command.Return_Code;
                ret_message = command.Return_Desc;
                EndInterface<ProfileCustomerCommand>(command, log, "", "Success", command.Return_Code.ToSafeString());
            }
            catch (Exception ex)
            {
                ret_code = command.Return_Code;
                ret_message = command.Return_Desc;
                EndInterface<ProfileCustomerCommand>(command, log, "", "Failed", ex.Message.ToSafeString());
            }
        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "CallSaveProfileCustomer",
                CREATED_BY = "WBBEx",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd,
            string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }
    }
}
