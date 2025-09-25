using System;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.ExWebServices
{
    public class ChangPackFieldworkCustomerRegisterCommandHandler : ICommandHandler<ChangPackFieldworkCustomerRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly ICommandHandler<CustRegisterCommand> _command;

        public ChangPackFieldworkCustomerRegisterCommandHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, ICommandHandler<CustRegisterCommand> command)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _command = command;
        }

        public void Handle(ChangPackFieldworkCustomerRegisterCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.InternetNo, "ChangPackFieldworkCustomerRegisterCommand", "ChangPackFieldworkCustomerRegisterCommandHandler", null, "Wobbegong", command.User);

                var xml = command.XmlData;
                var custRegisterCommand = (CustRegisterCommand)xml.DeserializedToObj<CustRegisterCommand>();
                custRegisterCommand.CreateBy = command.User;
                custRegisterCommand.QuickWinPanelModel.TransactionID = command.TrasactionId;
                _command.Handle(custRegisterCommand);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Success", "", "");

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.Message, "");
            }
        }
    }
}
