using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class ConfigurationLMREmailCommandHandler : ICommandHandler<ConfigurationLMREmailCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public ConfigurationLMREmailCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
             IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _historyLog = historyLog;
            _uow = uow;
            _lov = lov;
        }
        public void Handle(ConfigurationLMREmailCommand command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            try
            {

                var latestQuery = (from t in _lov.Get()
                                   where t.LOV_ID == command.Id
                                   select t).FirstOrDefault();

                latestQuery.LOV_VAL1 = command.Text;
                latestQuery.DISPLAY_VAL = command.Text;
                latestQuery.UPDATED_DATE = DateTime.Now;
                latestQuery.UPDATED_BY = command.updated_by;

                _lov.Update(latestQuery);
                _uow.Persist();

                command.ret_code = "0";
                command.ret_msg = "Success";

            }
            catch (Exception ex)
            {
                if (command.ret_code != "0")
                {
                    historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                    historyLog.ACTION = ActionHistory.ADD.ToString();
                    historyLog.APPLICATION = "ConfigLRMEmail GUI";
                    historyLog.CREATED_BY = "ConfigLRMEmail";
                    historyLog.CREATED_DATE = DateTime.Now;
                    historyLog.DESCRIPTION = ex.GetErrorMessage();
                    historyLog.REF_KEY = "ConfigLRMEmail";
                    historyLog.REF_NAME = "NODEID";
                    _historyLog.Create(historyLog);
                    _uow.Persist();
                }

                _logger.Info(ex.GetErrorMessage());
                command.ret_code = "-1";
                command.ret_msg = "Error";
            }


        }
    }
}
