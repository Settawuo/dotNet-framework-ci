using System;
using System.Linq;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class UpdatePreregisterStatusCommandHandler : ICommandHandler<UpdatePreregisterStatusCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_PRE_REGISTER> _PREREGISTERService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public UpdatePreregisterStatusCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_PRE_REGISTER> PREREGISTERService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _PREREGISTERService = PREREGISTERService;
            _intfLog = intfLog;
        }

        public void Handle(UpdatePreregisterStatusCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.p_refference_no, "PROC_LIST_ORD", "GetTracking", command.p_refference_no, "FBB", "WEB");

                var UpdateModelList = _PREREGISTERService.Get(c => c.REFFERENCE_NO == command.p_refference_no);

                if (UpdateModelList != null && UpdateModelList.Count() > 0)
                {
                    var UpdateModel = _PREREGISTERService.Get(c => c.REFFERENCE_NO == command.p_refference_no).FirstOrDefault();
                    UpdateModel.STATUS = command.p_status;
                    UpdateModel.UPDATE_BY = command.p_user_name;
                    UpdateModel.UPDATE_DTM = DateTime.Now;
                    UpdateModel.STATUS_DATE = DateTime.Now;
                    _PREREGISTERService.Update(UpdateModel);

                    _uow.Persist();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "Success", log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "No data", "");
                }
            }
            catch (Exception ex)
            {

                _logger.Info("Error occured when handle UpdatePreregisterStatusCommandHandler");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");

            }
        }
    }
}
