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
    public class GetBatchMailStatusQueryHandler : IQueryHandler<GetBatchMailStatus, GetEmailStatusModel>
    {
        private readonly ILogger _logger;
        private readonly EntityRepository<DetailGetEmailStatus> _objServiceSubj;
        public GetBatchMailStatusQueryHandler(ILogger logger, EntityRepository<DetailGetEmailStatus> objServiceSubj)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
        }

        public GetEmailStatusModel Handle(GetBatchMailStatus query)
        {
            List<DetailGetEmailStatus> executeResult = new List<DetailGetEmailStatus>();
            GetEmailStatusModel executeResults = new GetEmailStatusModel();
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

                _logger.Info("Start PKG_FBBBULK_CORP_BATCH.PROC_GET_STATUS_MAIL_BY_BULKNO");
                executeResult = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_BATCH.PROC_GET_STATUS_MAIL_BY_BULKNO",
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
                _logger.Info("WBB.PKG_FBBBULK_CORP_BATCH.PROC_GET_STATUS_MAIL_BY_BULKNO " + executeResults.P_RETURN_MESSAGE);

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_BATCH.PROC_GET_STATUS_MAIL_BY_BULKNO handles : " + ex.Message);

                executeResults.P_RETURN_CODE = "-1";
                executeResults.P_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }

    }
}
