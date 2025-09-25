using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace One2NetBusinessLayer.QueryHandlers.InWebServices
{
    public class GetBatchBulkCorpQueryHandler : IQueryHandler<GetBatchBulkCorpQuery, BatchBulkCorpModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _fbblovRepository;

        public GetBatchBulkCorpQueryHandler(ILogger logger, IEntityRepository<object> fbblovRepository)

        {
            _logger = logger;
            _fbblovRepository = fbblovRepository;
        }

        public BatchBulkCorpModel Handle(GetBatchBulkCorpQuery query)
        {
            BatchBulkCorpModel executeResults = new BatchBulkCorpModel();

            try
            {
                var p_bulk_number = new OracleParameter();
                p_bulk_number.ParameterName = "p_bulk_number";
                p_bulk_number.Size = 2000;
                p_bulk_number.OracleDbType = OracleDbType.Varchar2;
                p_bulk_number.Direction = ParameterDirection.Input;
                p_bulk_number.Value = query.P_BULK_NUMBER;

                var p_order_no = new OracleParameter();
                p_order_no.ParameterName = "p_order_no";
                p_order_no.Size = 2000;
                p_order_no.OracleDbType = OracleDbType.Varchar2;
                p_order_no.Direction = ParameterDirection.Input;
                p_order_no.Value = query.P_ORDER_NUMBER;

                var OUTPUT_bulk_no = new OracleParameter();
                OUTPUT_bulk_no.ParameterName = "OUTPUT_bulk_no";
                OUTPUT_bulk_no.Size = 2000;
                OUTPUT_bulk_no.OracleDbType = OracleDbType.Varchar2;
                OUTPUT_bulk_no.Direction = ParameterDirection.Output;

                var OUTPUT_return_code = new OracleParameter();
                OUTPUT_return_code.ParameterName = "OUTPUT_return_code";
                OUTPUT_return_code.OracleDbType = OracleDbType.Int64;
                OUTPUT_return_code.Direction = ParameterDirection.Output;

                var OUTPUT_return_message = new OracleParameter();
                OUTPUT_return_message.ParameterName = "OUTPUT_return_message";
                OUTPUT_return_message.Size = 2000;
                OUTPUT_return_message.OracleDbType = OracleDbType.Varchar2;
                OUTPUT_return_message.Direction = ParameterDirection.Output;

                var p_call_workflow = new OracleParameter();
                p_call_workflow.ParameterName = "p_call_workflow";
                p_call_workflow.OracleDbType = OracleDbType.RefCursor;
                p_call_workflow.Direction = ParameterDirection.Output;

                var air_regist_file_array = new OracleParameter();
                air_regist_file_array.ParameterName = "air_regist_file_array";
                air_regist_file_array.OracleDbType = OracleDbType.RefCursor;
                air_regist_file_array.Direction = ParameterDirection.Output;

                var air_regist_package_array = new OracleParameter();
                air_regist_package_array.ParameterName = "air_regist_package_array";
                air_regist_package_array.OracleDbType = OracleDbType.RefCursor;
                air_regist_package_array.Direction = ParameterDirection.Output;

                var air_regist_splitter_array = new OracleParameter();
                air_regist_splitter_array.ParameterName = "air_regist_splitter_array";
                air_regist_splitter_array.OracleDbType = OracleDbType.RefCursor;
                air_regist_splitter_array.Direction = ParameterDirection.Output;

                var air_regist_cpe_serial_array = new OracleParameter();
                air_regist_cpe_serial_array.ParameterName = "air_regist_cpe_serial_array";
                air_regist_cpe_serial_array.OracleDbType = OracleDbType.RefCursor;
                air_regist_cpe_serial_array.Direction = ParameterDirection.Output;


                _logger.Info("StartPKG_FBBBULK_CORP_BATCH ");

                var result = _fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.pkg_fbbbulk_corp_register.proc_detail_workflow1",
                      new object[]
                      {
                          p_bulk_number,
                          p_order_no,
                          //return code
                          OUTPUT_bulk_no    ,
                          OUTPUT_return_code      ,
                          OUTPUT_return_message   ,
                          p_call_workflow         ,
                          air_regist_file_array   ,
                          air_regist_package_array,
                          air_regist_splitter_array,
                          air_regist_cpe_serial_array
                      });

                if (result != null)
                {
                    executeResults.OUTPUT_BULK_NO = result[0].ToSafeString();//result[0] != null ? result[0].ToString() : "";
                    executeResults.OUTPUT_RETURN_CODE = result[1] != null ? result[1].ToString() : "-1";
                    executeResults.OUTPUT_RETURN_MESSAGE = result[2] != null ? result[2].ToString() : "";

                    DataTable call_workflow = (DataTable)result[3];
                    List<DetailWfCallWorkFlow> P_CALL_WORKFLOW = call_workflow.DataTableToList<DetailWfCallWorkFlow>();
                    executeResults.P_CALL_WORKFLOW = P_CALL_WORKFLOW;

                    DataTable d_air_regist_file_array = (DataTable)result[4];
                    List<DetailWfAirRegistFileArray> AIR_REGIST_FILE_ARRAY = d_air_regist_file_array.DataTableToList<DetailWfAirRegistFileArray>();
                    executeResults.AIR_REGIST_FILE_ARRAY = AIR_REGIST_FILE_ARRAY;

                    DataTable d_air_regist_package_array = (DataTable)result[5];
                    List<DetailWfAirRegistPackageArray> AIR_REGIST_PACKAGE_ARRAY = d_air_regist_package_array.DataTableToList<DetailWfAirRegistPackageArray>();
                    executeResults.AIR_REGIST_PACKAGE_ARRAY = AIR_REGIST_PACKAGE_ARRAY;

                    DataTable d_air_regist_splitter_array = (DataTable)result[6];
                    List<DetailWfAirRegistSplitterArray> AIR_REGIST_SPLITTER_ARRAY = d_air_regist_splitter_array.DataTableToList<DetailWfAirRegistSplitterArray>();
                    executeResults.AIR_REGIST_SPLITTER_ARRAY = AIR_REGIST_SPLITTER_ARRAY;

                    DataTable d_air_regist_cpe_serial_array = (DataTable)result[7];
                    List<DetailWfAirRegistCpeSerialArray> AIR_REGIST_CPE_SERIAL_ARRAY = d_air_regist_cpe_serial_array.DataTableToList<DetailWfAirRegistCpeSerialArray>();
                    executeResults.AIR_REGIST_CPE_SERIAL_ARRAY = AIR_REGIST_CPE_SERIAL_ARRAY;

                    _logger.Info("EndPKG_FBBBULK_CORP_BATCH " + executeResults.OUTPUT_RETURN_MESSAGE);
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_BATCH handles : " + ex.Message);

                executeResults.OUTPUT_RETURN_CODE = "-1";
                executeResults.OUTPUT_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
