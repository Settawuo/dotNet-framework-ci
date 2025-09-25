using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class StatLogCommandHandler : ICommandHandler<StatLogCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_VISIT_LOG> _FBB_VISIT_LOG;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public StatLogCommandHandler(ILogger logger, IEntityRepository<FBB_VISIT_LOG> FBB_VISIT_LOG, IWBBUnitOfWork uow, IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _FBB_VISIT_LOG = FBB_VISIT_LOG;
            _uow = uow;
            _lov = lov;
        }

        public void Handle(StatLogCommand command)
        {
            try
            {
                if (command.LC != "SEQ_FILE")
                {
                    DateTime now = DateTime.Now;
                    //insert 
                    var Log = new FBB_VISIT_LOG
                    {
                        USERNAME = command.Username,
                        VISIT_TYPE = command.VisitType,
                        REQ_IPADDRESS = command.REQ_IPADDRESS,
                        SELECT_PAGE = command.SelectPage,
                        HOST = command.HOST,
                        LC = command.LC,
                        CREATED_DATE = now,
                        CREATED_BY = "SYSTEM"
                    };

                    _FBB_VISIT_LOG.Create(Log);
                    _uow.Persist();

                    command.ReturnCode = 1;
                    command.ReturnDesc = "Success";
                }
                else
                {
                    var latestQuery = (from t in _lov.Get()
                                       where t.LOV_TYPE == "SCREEN"
                                        && t.LOV_NAME == "FILE_SEQ"
                                       select t).FirstOrDefault();

                    latestQuery.LOV_VAL1 = command.HOST;
                    _lov.Update(latestQuery);
                    _uow.Persist();
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ReturnCode = -1;
                command.ReturnDesc = "Error call StatLog handles : " + ex.GetErrorMessage();
            }

        }
    }
}
