using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    class GetDeveloperQueryHandler : IQueryHandler<GetDeveloperQuery, GetDeveloperQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<DETAIL_DEV_CUR> _objServiceSubj;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public GetDeveloperQueryHandler(ILogger logger, IEntityRepository<DETAIL_DEV_CUR> objServiceSubj, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _uow = uow;
            _intfLog = intfLog;
        }

        public GetDeveloperQueryModel Handle(GetDeveloperQuery query)
        {
            InterfaceLogCommand log = null;
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

            GetDeveloperQueryModel ResultData = new GetDeveloperQueryModel();
            List<DETAIL_DEV_CUR> executeResult = new List<DETAIL_DEV_CUR>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "PKG_FBB_DEVELPER", "PROC_GET_DEVELOPER", "", "", "WEB");
                // 
                executeResult = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBB_DEVELPER.PROC_GET_DEVELOPER",
                   new
                   {
                       // out
                       p_return_code,
                       p_return_message,
                       p_res_data
                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");
                //  decimal ret = Decimal.Parse(p_return_code.Value.ToSafeString());
                ResultData.P_RETURN_CODE = p_return_code.Value.ToSafeString();
                ResultData.P_RETURN_MESSAGE = p_return_message.Value.ToSafeString();
                if (executeResult.Count > 0)
                {
                    ResultData.P_RES_DATA = executeResult;
                }

            }
            catch (Exception ex)
            {
                ResultData.P_RETURN_CODE = "-1";
                ResultData.P_RETURN_MESSAGE = ex.Message;
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }
    }
}
