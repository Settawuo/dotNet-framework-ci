using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetConfigRedeemCodeQueryHandler : IQueryHandler<GetConfigRedeemCodeQuery, RedeemCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public GetConfigRedeemCodeQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public RedeemCodeModel Handle(GetConfigRedeemCodeQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_mobileno + query.ClientIP, "QUERY_CONFIG_REDEEM_CODE", "GetConfigRedeemCodeQuery", query.p_idcardno, "FBB|" + query.FullUrl, "WEB");

            RedeemCodeModel executeResults = new RedeemCodeModel();

            var p_language = new OracleParameter();
            p_language.ParameterName = "p_language";
            p_language.Size = 2000;
            p_language.OracleDbType = OracleDbType.Varchar2;
            p_language.Direction = ParameterDirection.Input;
            p_language.Value = query.p_language;


            var ret_code = new OracleParameter();
            ret_code.ParameterName = "ret_code";
            ret_code.OracleDbType = OracleDbType.Varchar2;
            ret_code.Size = 2000;
            ret_code.Direction = ParameterDirection.Output;

            var ret_message = new OracleParameter();
            ret_message.ParameterName = "ret_message";
            ret_message.OracleDbType = OracleDbType.Varchar2;
            ret_message.Size = 2000;
            ret_message.Direction = ParameterDirection.Output;

            var list_config_redeem_code = new OracleParameter();
            list_config_redeem_code.ParameterName = "list_config_redeem_code";
            list_config_redeem_code.OracleDbType = OracleDbType.RefCursor;
            list_config_redeem_code.Direction = ParameterDirection.Output;

            try
            {
                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE",
                    new object[]
                    {
                        p_language,

                        // return code
                        ret_code,
                        ret_message,
                        list_config_redeem_code

                    });

                if (result != null)
                {
                    executeResults.ret_code = result[0] != null ? result[0].ToString() : "-1";
                    executeResults.ret_message = result[1] != null ? result[1].ToString() : "";

                    var d_list_config_redeem_code = (DataTable)result[2];
                    var LIST_CONFIG_REDEEM_CODE = d_list_config_redeem_code.DataTableToList<RedeemCodeConfigDataModel>();
                    executeResults.list_config_redeem_code = LIST_CONFIG_REDEEM_CODE;

                    _logger.Info("End WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE output msg: " + executeResults.ret_message);

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Success", "", "");

                }
                else
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE output msg: " + "Error");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", "Failed", "");
                    executeResults.ret_code = "-1";
                    executeResults.ret_message = "Error";

                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR042.QUERY_CONFIG_REDEEM_CODE" + ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, query, log, "Failed", ex.Message, "");
                executeResults.ret_code = "-1";
                executeResults.ret_message = "Error";
            }

            return executeResults;
        }
    }
}
