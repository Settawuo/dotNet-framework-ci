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
    public class DeletetempAPCommandHandler : ICommandHandler<DeletetempAPCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_AP_INFO> _info;

        public DeletetempAPCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_AP_INFO> info)
        {
            _logger = logger;
            _uow = uow;
            _info = info;
        }
        public void Handle(DeletetempAPCommand command)
        {
            try
            {
                #region Update

                _logger.Info("Delete AP Info.");

                var info = (from r in _info.Get()
                            where r.SITE_ID == 0
                            select r).ToList();

                foreach (var a in info)
                {
                    var xxx = _info.GetByKey(a.AP_ID);
                    if (null != xxx)
                    {
                        _info.Delete(xxx);
                    }

                    //_info.Delete(info);
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
