using System;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class MailLogCommandHandler : ICommandHandler<MailLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;

        public MailLogCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService)
        {
            _logger = logger;
            _uow = uow;
            _lovService = lovService;
            _emailProcService = emailProcService;
            _emailLogService = emailLogService;
        }

        public void Handle(MailLogCommand command)
        {
            try
            {
                var newMailLog = new FBB_SENDMAIL_LOG();
                newMailLog.CUST_ROW_ID = command.CustomerId;
                newMailLog.PROCESS_NAME = "Sending";
                newMailLog.CREATE_USER = "CUSTOMER";
                newMailLog.CREATE_DATE = DateTime.Now;

                _emailLogService.Create(newMailLog);
                _uow.Persist();

                command.RunningNo = newMailLog.RUNNING_NO;
                command.ReturnCode = 0;
                command.ReturnDesc = "";
            }
            catch (Exception ex)
            {
                command.RunningNo = -1;
                command.ReturnCode = -1;
                command.ReturnDesc = ex.GetErrorMessage();
            }
        }
    }
}
