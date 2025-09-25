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
    public class GetBatchBulkCorpGetWfAndSffStatusHandler : IQueryHandler<GetBatchBulkCorpWfAndSffStatus, GetWFAndSFFStatus>
    {
        private readonly ILogger _logger;
        private readonly EntityRepository<DetailWFAndSFFStatus> _objServiceSubj;
        public GetBatchBulkCorpGetWfAndSffStatusHandler(ILogger logger, EntityRepository<DetailWFAndSFFStatus> objServiceSubj)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
        }
        public GetWFAndSFFStatus Handle(GetBatchBulkCorpWfAndSffStatus query)
        {
            List<DetailWFAndSFFStatus> executeResult = new List<DetailWFAndSFFStatus>();
            GetWFAndSFFStatus executeResults = new GetWFAndSFFStatus();
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

                _logger.Info("Start PKG_FBBBULK_CORP_BATCH.PROC_GET_WF_AND_SFF_STATUS");
                executeResult = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_GET_WF_AND_SFF_STATUS",
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
