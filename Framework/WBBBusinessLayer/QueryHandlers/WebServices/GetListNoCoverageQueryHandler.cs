using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetListNoCoverageQueryHandler : IQueryHandler<GetListNoCoverageQuery, AutoCheckCoverageModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<NoCoverageModel> _objSubJ;

        public GetListNoCoverageQueryHandler(ILogger logger, IEntityRepository<NoCoverageModel> objSubJ)
        {
            _logger = logger;
            _objSubJ = objSubJ;
        }

        public AutoCheckCoverageModel Handle(GetListNoCoverageQuery query)
        {
            AutoCheckCoverageModel resultData = new AutoCheckCoverageModel();
            List<NoCoverageModel> execute = new List<NoCoverageModel>();

            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RET_CODE";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "RET_MESSAGE";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var list_no_coverage_result = new OracleParameter();
                list_no_coverage_result.ParameterName = "LIST_NO_COVERAGE_RESULT";
                list_no_coverage_result.OracleDbType = OracleDbType.RefCursor;
                list_no_coverage_result.Direction = ParameterDirection.Output;

                _logger.Info("Start WBB.PKG_FBB_AUTO_CHECK_COVERAGE.LIST_NO_COVERAGE_RESULT");

                execute = _objSubJ.ExecuteReadStoredProc("WBB.PKG_FBB_AUTO_CHECK_COVERAGE.PROC_LIST_NO_COVERAGE_RESULT",
                    new
                    {
                        ret_code,
                        ret_message,
                        list_no_coverage_result
                    }).ToList();

                if (execute != null)
                {
                    resultData.RET_CODE = ret_code.Value.ToSafeString();
                    resultData.RET_MESSAGE = ret_message.Value.ToSafeString();
                    resultData.NO_COVERAGE_RESULT = execute;
                }

                _logger.Info("END CALL WBB.PKG_FBB_AUTO_CHECK_COVERAGE.PROC_LIST_NO_COVERAGE_RESULT");

            }
            catch (Exception ex)
            {

                _logger.Info("Error CALL WBB.PKG_FBB_AUTO_CHECK_COVERAGE.PROC_LIST_NO_COVERAGE_RESULT handles : " + ex.Message);

                resultData.RET_CODE = "-1";
                resultData.RET_MESSAGE = "Error";
            }

            return resultData;
        }
    }
}
