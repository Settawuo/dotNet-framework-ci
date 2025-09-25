using System;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class CreateNewVoucherProjectCommandHandler : ICommandHandler<CreateNewVoucherProjectCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<FBB_VOUCHER_MASTER> _VoucherMasterTable;

        public CreateNewVoucherProjectCommandHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, IEntityRepository<FBB_VOUCHER_MASTER> VoucherMasterTable)
        {
            _logger = logger;
            _uow = uow;
            _VoucherMasterTable = VoucherMasterTable;
            _interfaceLog = interfaceLog;
        }

        public void Handle(CreateNewVoucherProjectCommand command)
        {
            InterfaceLogAdminCommand log = null;
            try
            {
                var data = _VoucherMasterTable.Get(t => t.VOUCHER_PROJECT_DES == command.voucher_project_des);
                log = InterfaceLogAdminServiceHelper.StartInterfaceAdminLog(_uow, _interfaceLog, command, DateTime.Now.ToString("yyyyMMddHHmmss"), "CreateNewVoucherProjectCommandHandler", "CreateNewVoucherProject", "");
                if (!data.Any())
                {
                    command.resultMessage = "SUCCESS";
                    FBB_VOUCHER_MASTER newdata = new FBB_VOUCHER_MASTER()
                    {
                        VOUCHER_PROJECT_CODE = command.voucher_project_code,
                        VOUCHER_PROJECT_GROUP = command.voucher_project_group,
                        VOUCHER_PROJECT_DES = command.voucher_project_des,

                        PROJECT_STATUS = "On Service",
                        CREATED_DATE = DateTime.Now,
                        CREATED_BY = "FBBOR014",
                        TRANSACTION_ID = long.Parse(log.OutInterfaceLogId.ToString())
                    };
                    _VoucherMasterTable.Create(newdata);
                    _uow.Persist();
                    InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, command, log, "Success", "");
                }
                else
                {
                    command.resultMessage = "DUPLICATE";
                    InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, command, log, "Failed", "Duplicate");
                }
            }
            catch (Exception ex)
            {
                command.resultMessage = ex.Message;
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, command, log, "Failed", ex.Message.ToString());
            }
        }
    }
}
