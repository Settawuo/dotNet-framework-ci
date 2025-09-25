using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryBuildingVillage
{
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.WebServices.FBSS;
    using WBBContract.Queries.WebServices.FBSS;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.WebServiceModels;

    public class QueryBuildingVillageJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<AlterChangedBuildingCommand> _alterChangeBuildingCmd;
        private readonly ICommandHandler<NotificationBatchCommand> _Sendmail;

        public QueryBuildingVillageJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<NotificationBatchCommand> Sendmail,
            ICommandHandler<AlterChangedBuildingCommand> alterChangeBuildingCmd)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _alterChangeBuildingCmd = alterChangeBuildingCmd;
            _Sendmail = Sendmail;
        }

        public void Execute()
        {
            _logger.Info("QueryBuildingVillageJob Accquiring the changed building and village data.");

            try
            {
                var data = QueryBuild();
                //var filteredData = data.Where(t => t.Language == "T" && t.AddressType != "D").ToList();
                if (data.Any())
                {
                    _logger.Info("QueryBuildingVillageJob Altering the changed building and village data.");
                    AlterChangeBuild(data);
                }
                else
                {
                    Sendmail("Success", "", "0", "0", "0");
                }
                _logger.Info("QueryBuildingVillageJob End With Success");
            }
            catch (Exception ex)
            {
                _logger.Info("QueryBuildingVillageJob" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                Sendmail("ERROR", ex.GetErrorMessage(), "", "", "");
                _logger.Info(ex.StackTrace);
            }
        }

        private List<FBSSChangedAddressInfo> QueryBuild()
        {
            var query = new GetFBSSChangedBuilding();
            return _queryProcessor.Execute(query);
        }

        private void AlterChangeBuild(List<FBSSChangedAddressInfo> data)
        {
            int numRecInsertAll = 0;
            int numRecUpdateAll = 0;
            int numRecDeleteAll = 0;
            string errormsgAll = "";
            int maxRange = 500;

            int countDatasList = (data.Count / maxRange);
            if ((data.Count % maxRange) > 0)
                countDatasList = countDatasList + 1;

            List<List<FBSSChangedAddressInfo>> dataFilter = new List<List<FBSSChangedAddressInfo>>();

            for (int i = 0; i < countDatasList; i++)
            {
                int minRange = 0;
                int maxRangeMultiplyListAll = maxRange;
                if (i > 0)
                {
                    minRange = i * maxRange;
                    maxRangeMultiplyListAll = (i + 1) * maxRange;
                }
                else
                {
                    minRange = 0;
                    maxRangeMultiplyListAll = maxRange;
                }

                if (data.Count < maxRangeMultiplyListAll)
                    maxRange = data.Count - minRange;

                dataFilter.Add(data.GetRange(minRange, maxRange));
            }

            foreach (var dataItem in dataFilter)
            {
                var command = new AlterChangedBuildingCommand
                {
                    FBSSChangedAddressInfos = dataItem,
                    ActionBy = "FBBCONFIG",
                    ActionDate = DateTime.Now,
                };

                _alterChangeBuildingCmd.Handle(command);
                numRecInsertAll = numRecInsertAll + command.numRecInsert.ToSafeInteger();
                numRecUpdateAll = numRecUpdateAll + command.numRecUpdate.ToSafeInteger();
                numRecDeleteAll = numRecDeleteAll + command.numRecDelete.ToSafeInteger();
                errormsgAll = errormsgAll + command.errormsg.ToSafeString();
            }

            Sendmail("Success", errormsgAll.ToSafeString(),
                numRecInsertAll.ToSafeString(), numRecUpdateAll.ToSafeString(), numRecDeleteAll.ToSafeString());
        }

        public void Sendmail(string result, string errormsg, string numRecInsert, string numRecUpdate, string numRecDelete)
        {
            _logger.Info("QueryBuildingVillageJob sending an Email.");

            try
            {
                var command = new NotificationBatchCommand
                {
                    Cause = "",
                    result = result,
                    errormsg = errormsg,
                    numRecInsert = numRecInsert,
                    numRecUpdate = numRecUpdate,
                    numRecDelete = numRecDelete
                };

                _Sendmail.Handle(command);

            }
            catch (Exception ex)
            {
                _logger.Info("QueryBuildingVillageJob sending an Email." + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                throw ex;
            }
        }
    }
}