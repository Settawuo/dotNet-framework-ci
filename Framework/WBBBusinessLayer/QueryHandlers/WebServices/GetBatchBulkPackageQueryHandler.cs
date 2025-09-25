using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetBatchBulkPackageQueryHandler : IQueryHandler<GetBatchBulkCorpPackageQuery, GetBulkCorpPackage>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DetailAIR_REGIST_PACKAGE_ARRAY> _objPackage;

        public GetBatchBulkPackageQueryHandler(ILogger logger, IEntityRepository<DetailAIR_REGIST_PACKAGE_ARRAY> objPackage)
        {
            _logger = logger;
            _objPackage = objPackage;
        }
        public GetBulkCorpPackage Handle(GetBatchBulkCorpPackageQuery query)
        {
            List<DetailAIR_REGIST_PACKAGE_ARRAY> executeResult = new List<DetailAIR_REGIST_PACKAGE_ARRAY>();
            GetBulkCorpPackage executeResults = new GetBulkCorpPackage();

            try
            {
                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "P_RETURN_CODE";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "P_RETURN_MESSAGE";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "P_RES_DATA";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;


                _logger.Info("Start PKG_FBBBULK_CORP_BATCH.PROC_GET_PACKAGE ");

                executeResult = _objPackage.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_GET_PACKAGE",
                      new
                      {
                          P_CUST_ROW_ID = query.P_CUST_ROW_ID,
                          //return code
                          P_RETURN_CODE = p_return_code,
                          P_RETURN_MESSAGE = p_return_message,
                          P_RES_DATA = p_res_data
                      }).ToList();


                executeResults.P_RES_DATA = executeResult;
                executeResults.P_RETURN_CODE = p_return_code.Value != null ? p_return_code.Value.ToString() : "-1";
                executeResults.P_RETURN_MESSAGE = p_return_message.Value.ToString();

                _logger.Info("End PKG_FBBBULK_CORP_BATCH.PROC_GET_PACKAGE " + p_return_message.Value.ToString());

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_BATCH.PROC_GET_PACKAGE handles : " + ex.Message);

                executeResults.P_RETURN_CODE = "-1";
                executeResults.P_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
