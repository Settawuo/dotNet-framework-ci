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
    public class DelectInterfaceLogARCQueryHandler : IQueryHandler<DelectInterfaceLogARCQuery, DelectInterfaceLogARCModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;

        public DelectInterfaceLogARCQueryHandler(ILogger logger, IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public DelectInterfaceLogARCModel Handle(DelectInterfaceLogARCQuery query)
        {
            DelectInterfaceLogARCModel executeResults = new DelectInterfaceLogARCModel();

            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RET_CODE";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "RET_MSG";
                ret_msg.Size = 2000;
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Direction = ParameterDirection.Output;

                var p_email = new OracleParameter();
                p_email.ParameterName = "P_EMAIL";
                p_email.OracleDbType = OracleDbType.RefCursor;
                p_email.Direction = ParameterDirection.Output;

                _logger.Info("Start WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_DELETE_INTERFACE_LOG.P_ARCHIVE_LOG",
                    new object[]
                    {
                        //return code
                        ret_code,
                        ret_msg,
                        p_email
                    });

                if (result != null)
                {
                    executeResults.RET_CODE = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.RET_MSG = result[1] != null ? result[1].ToString() : "";

                    var d_list_config_redeem_code = (DataTable)result[2];
                    var P_EMAIL = d_list_config_redeem_code.DataTableToList<DataSendEmailDelectInterface>();
                    executeResults.P_EMAIL = P_EMAIL;

                    _logger.Info("End WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE output msg: " + executeResults.RET_MSG);

                }
                else
                {
                    _logger.Info("Error return -1 call service WBB.PKG_DELETE_INTERFACE_LOG.P_ARCHIVE_LOG output msg: " + "Error");
                    executeResults.RET_CODE = "-1";
                    executeResults.RET_MSG = "Error";
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_DELETE_INTERFACE_LOG.P_ARCHIVE_LOG" + ex.Message);
                executeResults.RET_CODE = "-1";
                executeResults.RET_MSG = "Error";
            }

            return executeResults;
        }
    }
}
