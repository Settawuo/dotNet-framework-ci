using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetScreenBulkCorpQueryHandler : IQueryHandler<GetScreenBulkCorpQuery, BatchBulkCorpModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _fbblovRepository;

        public GetScreenBulkCorpQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IWBBUnitOfWork uow,
            IEntityRepository<GetDetailBulkCorpRegister> objServiceSubj
             , IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _fbblovRepository = fbblovRepository;
        }

        public BatchBulkCorpModel Handle(GetScreenBulkCorpQuery query)
        {
            BatchBulkCorpModel executeResults = new BatchBulkCorpModel();
            InterfaceLogCommand log = null;

            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.P_BULK_NUMBER +
                    "GetScreenBulkCorpQuery", "GetScreenBulkCorpQuery", "saveOrderNew", null, "FBB", "FBBBULK001");

            try
            {
                var p_bulk_number = new OracleParameter();
                p_bulk_number.ParameterName = "p_bulk_number";
                p_bulk_number.Size = 2000;
                p_bulk_number.OracleDbType = OracleDbType.Varchar2;
                p_bulk_number.Direction = ParameterDirection.Input;
                p_bulk_number.Value = query.P_BULK_NUMBER;

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


                _logger.Info("StartPROC_DETAIL_WORKFLOW ");

                var result = _fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_DETAIL_WORKFLOW",

                      new object[]
                      {
                          p_bulk_number, 

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

                executeResults.OUTPUT_BULK_NO = result[0] != null ? result[0].ToString() : "";
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

                _logger.Info("EndPROC_DETAIL_WORKFLOW " + executeResults.OUTPUT_RETURN_MESSAGE);

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PROC_DETAIL_WORKFLOW handles : " + ex.Message);

                executeResults.OUTPUT_RETURN_CODE = "-1";
                executeResults.OUTPUT_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
