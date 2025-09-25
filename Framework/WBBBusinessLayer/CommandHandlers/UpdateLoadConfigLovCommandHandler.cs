using Notify;
using System;
using System.Linq;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdateLoadConfigLovCommandHandler : ICommandHandler<UpdateLoadConfigLovCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_LOAD_CONFIG_LOV> _LOVService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public UpdateLoadConfigLovCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_LOAD_CONFIG_LOV> LOVService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _LOVService = LOVService;
            _intfLog = intfLog;
        }

        public void Handle(UpdateLoadConfigLovCommand command)
        {

            try
            {
                var UpdateModelList = _LOVService.Get(c => c.EVENT_NAME == command.EVENT_NAME);

                if (UpdateModelList != null && UpdateModelList.Count() > 0)
                {
                    var UpdateModel = _LOVService.Get(c => c.EVENT_NAME == command.EVENT_NAME).FirstOrDefault();
                    if (command.FLAG_NUMBER == "1")
                        UpdateModel.FLAG = "N " + command.IP;
                    else if (command.FLAG_NUMBER == "2")
                        UpdateModel.FLAG2 = "N " + command.IP;
                    UpdateModel.UPDATED_DATE = DateTime.Today;
                    _LOVService.Update(UpdateModel);
                    _uow.Persist();
                    LineNotify.SendMessage(ServicesConstants.NotifyKey.LineNotifyFBB, this.GetType().Name, "Update LoadLOV to N");

                }
            }
            catch (Exception ex)
            {

                _logger.Info("Error occured when handle UpdateLoadConfigLovCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);

            }
        }
    }
}
