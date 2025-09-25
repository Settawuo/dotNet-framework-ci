using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DeleteAWCconfigCommandHandler : ICommandHandler<DeleteAWCconfigCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_AP_INFO> _info;

        public DeleteAWCconfigCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_AP_INFO> info)
        {
            _logger = logger;
            _uow = uow;
            _info = info;
        }
        public void Handle(DeleteAWCconfigCommand command)
        {
            try
            {
                #region Update

                _logger.Info("Delete AWC AP.");

                var info = from r in _info.Get()
                           where r.AP_ID == command.AP_ID && r.ACTIVE_FLAG == "Y"
                           select r;


                foreach (var a in info)
                {
                    a.ACTIVE_FLAG = "N";
                    a.UPDATED_BY = command.UPDATED_BY;
                    a.UPDATED_DATE = command.UPDATED_DATE;
                    _info.Update(a);
                }

                _uow.Persist();

                #endregion Update
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle DeleteAWCconfigCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }

}
