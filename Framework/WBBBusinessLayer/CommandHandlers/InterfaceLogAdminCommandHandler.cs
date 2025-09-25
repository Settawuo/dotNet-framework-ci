using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InterfaceLogAdminCommandHandler : ICommandHandler<InterfaceLogAdminCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<string> _objService;

        public InterfaceLogAdminCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _objService = objService;
        }

        public void Handle(InterfaceLogAdminCommand command)
        {
            var log = InterfaceLogAdminHelper.Log(_interfaceLog, command);
            _uow.Persist();
            command.OutInterfaceLogId = log.INTERFACE_ID;
        }
    }

    public static class InterfaceLogAdminHelper
    {
        public static FBB_INTERFACE_LOG_ADMIN Log(IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog,
            InterfaceLogAdminCommand command)
        {
            if (command.ActionType == ActionType.Insert)
            {

                var data = new FBB_INTERFACE_LOG_ADMIN();
                data.IN_TRANSACTION_ID = command.IN_TRANSACTION_ID;
                data.METHOD_NAME = command.METHOD_NAME;
                data.SERVICE_NAME = command.SERVICE_NAME;
                data.IN_ID_CARD_NO = command.IN_ID_CARD_NO;
                data.IN_XML_PARAM = command.IN_XML_PARAM;
                data.OUT_RESULT = command.OUT_RESULT;
                data.OUT_ERROR_RESULT = command.OUT_ERROR_RESULT;
                data.OUT_XML_PARAM = command.OUT_XML_PARAM;
                data.REQUEST_STATUS = "PENDING";
                data.INTERFACE_NODE = command.INTERFACE_NODE;
                data.CREATED_BY = command.CREATED_BY;
                data.CREATED_DATE = DateTime.Now;

                interfaceLog.Create(data);
                return data;
            }
            else if (command.ActionType == ActionType.Update)
            {
                var log = (from t in interfaceLog.Get()
                           where t.INTERFACE_ID == command.OutInterfaceLogId
                           select t).FirstOrDefault();

                if (null == log)
                    throw new Exception("InterfaceLogCommand Error : Transaction ID Not Found " + command.IN_TRANSACTION_ID);

                log.REQUEST_STATUS = command.REQUEST_STATUS;
                log.OUT_RESULT = command.OUT_RESULT;
                log.OUT_ERROR_RESULT = command.OUT_ERROR_RESULT;
                log.OUT_XML_PARAM = command.OUT_XML_PARAM;

                log.UPDATED_BY = "FBB";
                log.UPDATED_DATE = DateTime.Now;

                interfaceLog.Update(log, interfaceLog.GetByKey(command.OutInterfaceLogId));
                return log;
            }
            else
            {
                throw new Exception("InterfaceLogCommand Error");
            }

        }
    }
}
