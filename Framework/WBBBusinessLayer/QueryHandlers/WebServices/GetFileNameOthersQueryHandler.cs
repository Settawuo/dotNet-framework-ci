using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetFileNameOthersQueryHandler : IQueryHandler<GetFileNameOthersQuery, AutoMoveFileBatchModel>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<AutoMoveFileBatchModel> _airnetRepository;

        public GetFileNameOthersQueryHandler(ILogger logger, IAirNetEntityRepository<AutoMoveFileBatchModel> airnetRepository)
        {
            _logger = logger;
            _airnetRepository = airnetRepository;
        }

        public AutoMoveFileBatchModel Handle(GetFileNameOthersQuery query)
        {
            AutoMoveFileBatchModel executeResults = new AutoMoveFileBatchModel();

            try
            {
                var res_code = new OracleParameter();
                res_code.ParameterName = "RES_CODE";
                res_code.Size = 2000;
                res_code.OracleDbType = OracleDbType.Varchar2;
                res_code.Direction = ParameterDirection.Output;

                var res_message = new OracleParameter();
                res_message.ParameterName = "RES_MESSAGE";
                res_message.Size = 2000;
                res_message.OracleDbType = OracleDbType.Varchar2;
                res_message.Direction = ParameterDirection.Output;

                var res_others_cur = new OracleParameter();
                res_others_cur.ParameterName = "RES_OTHERS_CUR";
                res_others_cur.OracleDbType = OracleDbType.RefCursor;
                res_others_cur.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR006.PROC_GET_FILE_OTHERS ");

                var result = _airnetRepository.ExecuteStoredProcMultipleCursor("AIR_ADMIN.PKG_FBBOR006.PROC_GET_FILE_OTHERS",
                    new object[]
                    {
                        //return code
                        res_code,
                        res_message,
                        res_others_cur
                    });

                if (result != null)
                {
                    executeResults.RES_CODE = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.RES_MESSAGE = result[1] != null ? result[1].ToString() : "";

                    var d_complete_cur = (DataTable)result[2];
                    var RES_OTHERS_CUR = d_complete_cur.DataTableToList<FilePathByOrderNo>();
                    executeResults.RES_OTHERS_CUR = RES_OTHERS_CUR;

                    _logger.Info("End PKG_FBBOR006.PROC_GET_FILE_OTHERS" + executeResults.RES_MESSAGE);
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR006.PROC_GET_FILE_OTHERS handles : " + ex.Message);

                executeResults.RES_CODE = "-1";
                executeResults.RES_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
