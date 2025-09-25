using System;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class EmailTransactionCommandHandler : ICommandHandler<EmailTransactionCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_EMAIL_TRANSACTION> _emailProcService;

        public EmailTransactionCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_EMAIL_TRANSACTION> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _emailProcService = emailProcService;
        }

        public void Handle(EmailTransactionCommand command)
        {
            try
            {
                var value = new FBB_EMAIL_TRANSACTION();
                value.PROCESS_NAME = command.PROCESS_NAME;
                value.EMAIL_TO = command.EMAIL_TO;
                value.EMAIL_CONTENT = command.EMAIL_CONTENT;
                value.EMAIL_ATTACH = command.EMAIL_ATTACH;
                value.STATUS = command.STATUS;
                value.STATUS_DESC = command.STATUS_DESC;
                value.CREATE_BY = command.CREATE_BY;
                value.CREATE_DATE = command.CREATE_DATE;

                _emailProcService.Create(value);
                _uow.Persist();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
