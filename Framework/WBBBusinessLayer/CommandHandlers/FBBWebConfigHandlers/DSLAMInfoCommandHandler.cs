using System;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class DSLAMInfoCommandHandler : ICommandHandler<DSLAMInfoCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_DSLAM_INFO> _dslamInfo;
        private readonly IWBBUnitOfWork _uow;

        public DSLAMInfoCommandHandler(ILogger logger, IEntityRepository<FBB_DSLAM_INFO> dslamInfo, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _dslamInfo = dslamInfo;
            _uow = uow;
        }

        public void Handle(DSLAMInfoCommand command)
        {
            try
            {
                var dslamModel = new FBB_DSLAM_INFO();
                //dslamModel.DSLAMID = command.DSLAMID;
                dslamModel.CREATED_BY = command.CREATED_BY;
                dslamModel.CREATED_DATE = command.CREATED_DATE;
                dslamModel.UPDATED_BY = command.UPDATED_BY;
                dslamModel.UPDATED_DATE = command.UPDATED_DATE;
                dslamModel.DSLAMNUMBER = command.DSLAMNUMBER;
                dslamModel.DSLAMMODELID = command.DSLAMMODELID;
                dslamModel.ACTIVEFLAG = command.ACTIVEFLAG;
                dslamModel.NODEID = command.NODEID;
                dslamModel.REGION_CODE = command.REGION_CODE;
                dslamModel.LOT_NUMBER = command.LOT_NUMBER;
                _dslamInfo.Create(dslamModel);

                _uow.Persist();

                command.Return_Code = 1;
                command.Return_Desc = "Success";
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.Return_Code = -1;
                command.Return_Desc = "Error call save dslaminfo  : " + ex.GetErrorMessage();
            }
        }
    }
}
