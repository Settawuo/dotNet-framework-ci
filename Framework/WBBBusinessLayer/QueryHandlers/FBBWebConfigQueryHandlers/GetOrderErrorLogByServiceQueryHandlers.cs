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
    public class GetOrderErrorLogByServiceQueryHandlers : IQueryHandler<GetPackagebyServiceQuery, FB_Interfce_log_byServiceModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetXML_PARAM> _objService;

        public GetOrderErrorLogByServiceQueryHandlers(ILogger logger, IEntityRepository<GetXML_PARAM> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public FB_Interfce_log_byServiceModel Handle(GetPackagebyServiceQuery query)
        {
            FB_Interfce_log_byServiceModel orderErrorLog = new FB_Interfce_log_byServiceModel();

            try
            {
                var P_IN_TRANSACTION_ID = new OracleParameter();
                P_IN_TRANSACTION_ID.ParameterName = "P_IN_TRANSACTION_ID";
                P_IN_TRANSACTION_ID.OracleDbType = OracleDbType.Varchar2;
                P_IN_TRANSACTION_ID.Direction = ParameterDirection.Input;

                var P_RETURN_CODE = new OracleParameter();
                P_RETURN_CODE.ParameterName = "P_RETURN_CODE";
                P_RETURN_CODE.Size = 2000;
                P_RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_CODE.Direction = ParameterDirection.Output;

                var P_RETURN_MESSAGE = new OracleParameter();
                P_RETURN_MESSAGE.ParameterName = "P_RETURN_MESSAGE";
                P_RETURN_MESSAGE.Size = 2000;
                P_RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var P_IN_XML_PARAM = new OracleParameter();
                P_IN_XML_PARAM.ParameterName = "P_IN_XML_PARAM";
                P_IN_XML_PARAM.OracleDbType = OracleDbType.RefCursor;
                P_IN_XML_PARAM.Direction = ParameterDirection.Output;

                _logger.Info("Start PROC_INTERFACE_LOG_BYSERVICE");

                List<GetXML_PARAM> executeResult = _objService.ExecuteReadStoredProc("PKG_FBB_RESEND_ORDER.PROC_INTERFACE_LOG_BYSERVICE",
                            new
                            {
                                P_IN_TRANSACTION_ID = query.P_IN_TRANSACTION_ID,

                                P_RETURN_CODE = P_RETURN_CODE,
                                P_RETURN_MESSAGE = P_RETURN_MESSAGE,
                                P_IN_XML_PARAM = P_IN_XML_PARAM

                            }).ToList();

                //orderErrorLog.P_PAGE_COUNT = Int32.Parse(p_page_count.Value.ToString());
                orderErrorLog.P_RETURN_CODE = P_RETURN_CODE.Value != null ? P_RETURN_CODE.Value.ToString() : "-1";
                orderErrorLog.P_RETURN_MESSAGE = P_RETURN_MESSAGE.Value.ToString();
                orderErrorLog.P_IN_XML_PARAM = executeResult;

                _logger.Info("End PROC_INTERFACE_LOG_BYSERVICE" + P_RETURN_MESSAGE.Value.ToString());

                return orderErrorLog;

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBB_RESEND_ORDER handles : " + ex.Message);
                orderErrorLog.P_RETURN_CODE = "-1";
                orderErrorLog.P_RETURN_MESSAGE = ex.Message;
                return null;
            }

        }
    }


    //public class GetLogStatusQueryHandler : IQueryHandler<GetStatusLogQuery, List<StatusLogDropdown>>
    //{
    //    private readonly ILogger _logger;
    //    private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

    //    public GetLogStatusQueryHandler(ILogger logger,
    //        IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
    //    {
    //        _logger = logger;
    //        _interfaceLog = interfaceLog;
    //    }
    //}
}

