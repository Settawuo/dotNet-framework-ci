using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBShareplex;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBBusinessLayer.QueryHandlers.FBBShareplex
{
    public class GetLowUtilizeSaleReportQueryHandler : IQueryHandler<GetLowUtilizeSaleReportQuery, List<LowUtilizeSaleReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IFBBShareplexEntityRepository<LowUtilizeSaleReportList> _fbbShareplexRepository;

        public GetLowUtilizeSaleReportQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IFBBShareplexEntityRepository<LowUtilizeSaleReportList> fbbShareplexRepository)
        {
            _logger = logger;
            _fbbShareplexRepository = fbbShareplexRepository;
            _intfLog = intfLog;
            _uow = uow;
        }

        public List<LowUtilizeSaleReportList> Handle(GetLowUtilizeSaleReportQuery query)
        {
            InterfaceLogCommand log = null;

            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.Size = 2000;
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Direction = ParameterDirection.Output;

            var ret_message = new OracleParameter();
            ret_message.ParameterName = "ret_message";
            ret_message.Size = 2000;
            ret_message.OracleDbType = OracleDbType.Varchar2;
            ret_message.Direction = ParameterDirection.Output;

            var list_low_utilize_rpt = new OracleParameter();
            list_low_utilize_rpt.ParameterName = "list_low_utilize_rpt";
            list_low_utilize_rpt.OracleDbType = OracleDbType.RefCursor;
            list_low_utilize_rpt.Direction = ParameterDirection.Output;

            List<LowUtilizeSaleReportList> executeResults = new List<LowUtilizeSaleReportList>();

            try
            {
                _logger.Info("Start FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT");

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "QUERY_LOW_UTILIZE_RPT", "GetLowUtilizeSaleReport", query.p_location_code, "FBB", "LowUtilizeSaleReport.exe");

                executeResults = _fbbShareplexRepository.ExecuteReadStoredProc("FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT",
                    new
                    {
                        p_location_code = query.p_location_code.ToSafeString(),
                        //return code
                        ret_code = ret_code,
                        ret_message = ret_message,
                        list_low_utilize_rpt = list_low_utilize_rpt

                    }).ToList();

                _logger.Info("End FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT output msg: " + query.ret_message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info("Error call FBBADM.PKG_FBB_LOW_UTILIZE_RPT.QUERY_LOW_UTILIZE_RPT handles : " + ex.Message);
                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }
            return executeResults;
        }
    }
}
