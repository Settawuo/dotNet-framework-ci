using System;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class TrackingLogCommandHandler : ICommandHandler<TrackingLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_TRACKING_IMPLEMENT_LOG> _trackingLogService;

        public TrackingLogCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_TRACKING_IMPLEMENT_LOG> trackingLogService)
        {
            _logger = logger;
            _uow = uow;
            _trackingLogService = trackingLogService;
        }

        public void Handle(TrackingLogCommand command)
        {
            //try
            //{
            var data = new FBB_TRACKING_IMPLEMENT_LOG();
            data.IDCARD = command.IDCard;
            data.FIRSTNAME = command.FirstName;
            data.LASTNAME = command.LastName;
            data.RESULT_CODE = command.ResultCode;
            data.RETURN_ORDER = command.ReturnOrder;
            data.CREATED_BY = command.CreatedBy;
            data.CREATED_DATE = DateTime.Now;

            _trackingLogService.Create(data);
            _uow.Persist();
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }
}
