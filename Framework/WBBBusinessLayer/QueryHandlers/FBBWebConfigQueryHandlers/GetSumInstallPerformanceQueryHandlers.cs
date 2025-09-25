using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetSumInstallPerformanceQueryHandlers : IQueryHandler<GetSumInstallQuery, List<SumInstallPerformanceList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SumInstallPerformanceList> _objService;

        public GetSumInstallPerformanceQueryHandlers(ILogger logger, IEntityRepository<SumInstallPerformanceList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<SumInstallPerformanceList> Handle(GetSumInstallQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "ret_data";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<SumInstallPerformanceList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_fbbdorm_payg_RPT008.PROC_PAYG_RPT008",
                            new
                            {
                                START_DT = (command.dateFrom != null) ? command.dateFrom.Value.Date.ToString("dd/MM/yyyy") : "",
                                END_DT = (command.dateTo != null) ? command.dateTo.Value.Date.ToString("dd/MM/yyyy") : "",

                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                ret_data = ret_data

                            }).ToList();

                command.Return_Code = 1;
                command.Return_Desc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.Return_Code = -1;
                command.Return_Desc = "Error call save event service " + ex.Message;

                return null;
            }

        }
    }
}
