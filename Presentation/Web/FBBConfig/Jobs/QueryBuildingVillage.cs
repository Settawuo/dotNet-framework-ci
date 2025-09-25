using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.WebServices.FBSS;
using WBBContract.Queries.WebServices.FBSS;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBConfig.Jobs
{
    using Quartz;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryBuildingVillage : IJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<AlterChangedBuildingCommand> _alterChangeBuildingCmd;

        public QueryBuildingVillage(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<AlterChangedBuildingCommand> alterChangeBuildingCmd)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _alterChangeBuildingCmd = alterChangeBuildingCmd;
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Accquiring the changed building and village data.");

            var data = QueryBuild();
            if (data.Any())
            {
                _logger.Info("Altering the changed building and village data.");
                AlterChangeBuild(data);
            }

            _logger.Info("SimpleJob says: " + context.JobDetail.Key + " executing at " + SystemTime.Now());
        }

        private List<FBSSChangedAddressInfo> QueryBuild()
        {
            var query = new GetFBSSChangedBuilding();
            return _queryProcessor.Execute(query);
        }

        private void AlterChangeBuild(List<FBSSChangedAddressInfo> data)
        {
            var command = new AlterChangedBuildingCommand
            {
                FBSSChangedAddressInfos = data,
                ActionBy = "FBBCONFIG",
                ActionDate = DateTime.Now,
            };

            _alterChangeBuildingCmd.Handle(command);
        }
    }
}