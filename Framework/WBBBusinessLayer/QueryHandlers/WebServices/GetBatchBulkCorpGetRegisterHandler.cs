using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;


namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetBatchBulkCorpRegisterHandler : IQueryHandler<GetBatchBulkCorpRegister, GetBlukCorpRegisterModel>
    {
        private readonly ILogger _logger;
        private readonly EntityRepository<DetialBlukCorpRegister> _objServiceSubj;
        public GetBatchBulkCorpRegisterHandler(ILogger logger, EntityRepository<DetialBlukCorpRegister> objServiceSubj)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
        }
        public GetBlukCorpRegisterModel Handle(GetBatchBulkCorpRegister query)
        {
            List<DetialBlukCorpRegister> executeResult = new List<DetialBlukCorpRegister>();
            GetBlukCorpRegisterModel executeResults = new GetBlukCorpRegisterModel();
            try
            {
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

                var P_RES_DATA = new OracleParameter();
                P_RES_DATA.ParameterName = "P_RES_DATA";
                P_RES_DATA.OracleDbType = OracleDbType.RefCursor;
                P_RES_DATA.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBBULK_CORP_BATCH.PROC_GET_BULKCORP_REGISTER");
                executeResult = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_GET_BULKCORP_REGISTER",
                        new
                        {
                            //return code
                            P_RETURN_CODE = P_RETURN_CODE,
                            P_RETURN_MESSAGE = P_RETURN_MESSAGE,
                            P_RES_DATA = P_RES_DATA
                        }).ToList();
                executeResults.P_RES_DATA = executeResult;
                executeResults.P_RETURN_CODE = P_RETURN_CODE.Value != null ? P_RETURN_CODE.Value.ToString() : "-1";
                executeResults.P_RETURN_MESSAGE = P_RETURN_MESSAGE.Value.ToSafeString();
                _logger.Info("PKG_FBBBULK_CORP_BATCH " + executeResults.P_RETURN_MESSAGE);

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_BATCH handles : " + ex.Message);

                executeResults.P_RETURN_CODE = "-1";
                executeResults.P_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
