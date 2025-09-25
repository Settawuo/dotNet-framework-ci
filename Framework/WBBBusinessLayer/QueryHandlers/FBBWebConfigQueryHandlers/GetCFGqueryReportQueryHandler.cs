using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers
{
    public class GetCFGqueryReportQueryHandler : IQueryHandler<GetCFGqueryReportQuery, List<CFGqueryReportModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_QUERY_REPORT> _cfgService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetCFGqueryReportQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_CFG_QUERY_REPORT> cfgService, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _cfgService = cfgService;
            _uow = uow;
        }

        public List<CFGqueryReportModel> Handle(GetCFGqueryReportQuery query)
        {
            List<CFGqueryReportModel> result = new List<CFGqueryReportModel>();
            InterfaceLogCommand log = null;
            try
            {
                //log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.query_id.ToString(),
                //  "GetCFGqueryReportQueryHandler", "GetCFGqueryReportQueryHandler", query.query_id.ToString(),
                //  "FBBSendMailAutoLMD", "BATCH");
                var cfgList = _cfgService
         .Get(cfg => cfg.SHEET_NAME.Equals(query.Sheet_Name));

                var cfgModelList = cfgList.AsEnumerable().Select(l => new CFGqueryReportModel
                {
                    query_id = l.QUERY_ID,
                    report_id = l.REPORT_ID,
                    sheet_name = l.SHEET_NAME,
                    query_1 = l.QUERY_1,
                    query_2 = l.QUERY_2,
                    query_3 = l.QUERY_3,
                    query_4 = l.QUERY_4,
                    query_5 = l.QUERY_5,
                    owner_db = l.OWNER_DB
                }).ToList();

                result = cfgModelList;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                //if (null != log)
                //{
                //    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                //           "");
                //}
            }
            return result;
        }
    }
}
