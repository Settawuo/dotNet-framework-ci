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
    public class GetListDeleteFileOrderCancelQueryHandler : IQueryHandler<GetListDeleteFileOrderCancelQuery, FBBDeleteFileOrderCancelModel>
    {

        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<FBBDeleteFileOrderCancelModel> _airnetRepository;

        public GetListDeleteFileOrderCancelQueryHandler(ILogger logger, IAirNetEntityRepository<FBBDeleteFileOrderCancelModel> airnetRepository)
        {
            _logger = logger;
            _airnetRepository = airnetRepository;
        }

        public FBBDeleteFileOrderCancelModel Handle(GetListDeleteFileOrderCancelQuery query)
        {
            FBBDeleteFileOrderCancelModel executeResults = new FBBDeleteFileOrderCancelModel();

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

                var res_cancel_wf_cur = new OracleParameter();
                res_cancel_wf_cur.ParameterName = "RES_CANCEL_WF_CUR";
                res_cancel_wf_cur.OracleDbType = OracleDbType.RefCursor;
                res_cancel_wf_cur.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR022_BATCH.PROC_GET_FILE_CANCEL ");

                var result = _airnetRepository.ExecuteStoredProcMultipleCursor("AIR_ADMIN.PKG_FBBOR022_BATCH.PROC_GET_FILE_CANCEL",
                    new object[]
                    {
                        //return code
                        res_code,
                        res_message,
                        res_cancel_wf_cur
                    });

                if (result != null)
                {
                    executeResults.RES_CODE = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.RES_MESSAGE = result[1] != null ? result[1].ToString() : "";

                    var d_cancel_wf_cur = (DataTable)result[2];
                    var RES_CANCEL_WF_CUR = d_cancel_wf_cur.DataTableToList<FilePathByOrderNo>();
                    executeResults.RES_CANCEL_WF_CUR = RES_CANCEL_WF_CUR;

                    _logger.Info("End PKG_FBBOR022_BATCH.PROC_GET_FILE_CANCEL" + executeResults.RES_MESSAGE);
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR022_BATCH.PROC_GET_FILE_CANCEL handles : " + ex.Message);

                executeResults.RES_CODE = "-1";
                executeResults.RES_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
