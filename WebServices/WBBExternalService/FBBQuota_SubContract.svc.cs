using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBEntity.Extensions;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FBBQuota_SubContract" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FBBQuota_SubContract.svc or FBBQuota_SubContract.svc.cs at the Solution Explorer and start debugging.
    public class FBBQuota_SubContract : IFBBQuota_SubContract
    {
        private ILogger _logger;
        private ICommandHandler<Quota_SubcontractCommand> _Quota_SubcontractCommand;
        private ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public FBBQuota_SubContract(ILogger logger,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<Quota_SubcontractCommand> Quota_SubcontractCommand)
        {
            _logger = logger;
            _intfLogCommand = intfLogCommand;
            _Quota_SubcontractCommand = Quota_SubcontractCommand;
        }

        public void Quota_Subcontract(Quota_SubcontractCommand command, out int ret_code)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = StartInterface<Quota_SubcontractCommand>(command, "Quota_SubcontractCommandHandle",
                    "", "");

                _Quota_SubcontractCommand.Handle(command);
                ret_code = command.ReturnCode;

                EndInterface<Quota_SubcontractCommand>(command, log, "", "Success", "");
            }
            catch (System.Exception ex)
            {
                ret_code = -1;
                EndInterface<Quota_SubcontractCommand>(command, log, "",
                    "ERROR", ex.GetErrorMessage());
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
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbSbnInterfaceLog",
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