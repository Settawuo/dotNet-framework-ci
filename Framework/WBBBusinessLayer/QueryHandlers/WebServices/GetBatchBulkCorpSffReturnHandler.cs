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
    public class GetBatchBulkCorpSffReturnHandler : IQueryHandler<GetBatchBulkCorpSFFReturn, BatchBulkCorpSFFReturnModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<DetailSffReturn> _objServiceSubj;

        public GetBatchBulkCorpSffReturnHandler(ILogger logger, IAirNetEntityRepository<DetailSffReturn> objServiceSubj)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
        }
        public BatchBulkCorpSFFReturnModel Handle(GetBatchBulkCorpSFFReturn query)
        {
            List<DetailSffReturn> executeResult = new List<DetailSffReturn>();
            BatchBulkCorpSFFReturnModel executeResults = new BatchBulkCorpSFFReturnModel();
            try
            {
                var output_return_code = new OracleParameter();
                output_return_code.ParameterName = "output_return_code";
                output_return_code.Size = 2000;
                output_return_code.OracleDbType = OracleDbType.Varchar2;
                output_return_code.Direction = ParameterDirection.Output;

                var output_return_message = new OracleParameter();
                output_return_message.ParameterName = "output_return_message";
                output_return_message.Size = 2000;
                output_return_message.OracleDbType = OracleDbType.Varchar2;
                output_return_message.Direction = ParameterDirection.Output;

                var p_output_sff_detail = new OracleParameter();
                p_output_sff_detail.ParameterName = "p_output_sff_detail";
                p_output_sff_detail.OracleDbType = OracleDbType.RefCursor;
                p_output_sff_detail.Direction = ParameterDirection.Output;
                _logger.Info("Start PKG_QUERY_PACKAGE.PROC_SFF_RETURN");
                executeResult = _objServiceSubj.ExecuteReadStoredProc("AIR_ADMIN.PKG_QUERY_PACKAGE.PROC_SFF_RETURN",
                     new
                     {
                         p_order_bulk_number = query.P_BULK_NUMBER,
                         //return code
                         OUTPUT_RETURN_CODE = output_return_code,
                         OUTPUT_RETURN_MESSAGE = output_return_message,
                         P_OUTPUT_SFF_DETAIL = p_output_sff_detail
                     }).ToList();
                executeResults.P_OUTPUT_SFF_DETAIL = executeResult;
                executeResults.OUTPUT_return_code = output_return_code.Value != null ? output_return_code.Value.ToString() : "-1";
                executeResults.OUTPUT_return_message = output_return_message.Value.ToString();
                _logger.Info("PKG_QUERY_PACKAGE " + executeResults.OUTPUT_return_message);

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_QUERY_PACKAGE handles : " + ex.Message);

                executeResults.OUTPUT_return_code = "-1";
                executeResults.OUTPUT_return_message = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
