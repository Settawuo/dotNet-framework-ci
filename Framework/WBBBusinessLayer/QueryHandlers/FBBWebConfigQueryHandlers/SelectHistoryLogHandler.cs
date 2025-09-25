using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectHistoryLogHandler : IQueryHandler<SelectHistoryLogQuery, List<FBB_HISTORY_LOG>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;

        public SelectHistoryLogHandler(ILogger logger, IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IEntityRepository<FBB_CFG_LOV> cfgLov)
        {
            _logger = logger;
            _historyLog = historyLog;
            _cfgLov = cfgLov;
        }

        public List<FBB_HISTORY_LOG> Handle(SelectHistoryLogQuery query)
        {
            var listHistoryLog = from h in _historyLog.Get()
                                 where h.APPLICATION == query.Application
                                 select h;

            if (query.FirstLoad)
            {
                //select c.display_val from wbb.fbb_cfg_lov c
                //where c.lov_type = 'DEFAULT_HISTORY_LOG' and c.lov_name = 'DAY_BACKWARD'

                var listCfgLov = from c in _cfgLov.Get()
                                 where c.LOV_TYPE == "DEFAULT_HISTORY_LOG"
                                 && c.LOV_NAME == "DAY_BACKWARD"
                                 select c.DISPLAY_VAL;

                if (listCfgLov.Any())
                {
                    var dayBackWard = listCfgLov.FirstOrDefault();

                    var dateTo = DateTime.Now;
                    var dateFrom = DateTime.Now.AddDays(Convert.ToDouble(dayBackWard));

                    listHistoryLog = from h in listHistoryLog
                                     where (h.CREATED_DATE >= dateFrom && h.CREATED_DATE <= dateTo)
                                     select h;
                }
            }
            if (!string.IsNullOrEmpty(query.Ref_Key))
            {
                listHistoryLog = from h in listHistoryLog
                                 where h.REF_KEY.Contains(query.Ref_Key)
                                 select h;
            }
            if (!string.IsNullOrEmpty(query.Ref_Name))
            {
                listHistoryLog = from h in listHistoryLog
                                 where h.REF_NAME.Contains(query.Ref_Name)
                                 select h;
            }

            return listHistoryLog.ToList();
        }
    }
}
