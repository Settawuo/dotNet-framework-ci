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
    public class UpdateAWCconfigCommandHandler : ICommandHandler<UpdateAWCconfigCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_AP_INFO> _info;

        public UpdateAWCconfigCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_AP_INFO> info)
        {
            _logger = logger;
            _uow = uow;
            _info = info;
        }
        public void Handle(UpdateAWCconfigCommand command)
        {
            try
            {
                #region Update

                _logger.Info("Delete AP Info.");

                var info = (from r in _info.Get()
                            where r.AP_ID == command.AP_ID
                            select r);

                var check = (from r in _info.Get()
                             where r.AP_NAME == command.AP_NAME
                             select r);


                //if (info.Any())
                //{
                //    command.FlagDup = true;
                //}
                //else
                //{
                if (check.Count() > 1)
                {
                    command.FlagDup = true;
                }
                else
                {
                    foreach (var a in info)
                    {
                        if (a.AP_NAME == command.AP_NAME)
                        {
                            command.FlagDup = true;
                        }
                        else
                        {
                            a.UPDATED_BY = command.user;
                            a.UPDATED_DATE = DateTime.Now;
                            a.AP_NAME = command.AP_NAME.Trim();
                            a.SECTOR = command.SECTOR;
                            _info.Update(a);
                        }
                    }
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
