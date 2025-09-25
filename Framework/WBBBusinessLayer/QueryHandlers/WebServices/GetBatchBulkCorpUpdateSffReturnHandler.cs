using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;
namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetBatchBulkCorpUpdateSffReturnHandler : IQueryHandler<GetBatchBulkCorpUpdateSFFReturn, BatchBulkCorpUpdateSFFReturnModel>
    {
        private readonly ILogger _logger;
        private readonly EntityRepository<DetailSffReturn> _objServiceSubj;
        public GetBatchBulkCorpUpdateSffReturnHandler(ILogger logger, EntityRepository<DetailSffReturn> objServiceSubj)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
        }
        public BatchBulkCorpUpdateSFFReturnModel Handle(GetBatchBulkCorpUpdateSFFReturn query)
        {

            BatchBulkCorpUpdateSFFReturnModel executeResults = new BatchBulkCorpUpdateSFFReturnModel();
            try
            {
                var P_RETURN_CODE = new OracleParameter();
                P_RETURN_CODE.ParameterName = "OUTPUT_return_code";
                P_RETURN_CODE.Size = 2000;
                P_RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_CODE.Direction = ParameterDirection.Output;

                var P_RETURN_MESSAGE = new OracleParameter();
                P_RETURN_MESSAGE.ParameterName = "OUTPUT_return_message";
                P_RETURN_MESSAGE.Size = 2000;
                P_RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_MESSAGE.Direction = ParameterDirection.Output;


                _logger.Info("Start PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_CABASA");
                var result = _objServiceSubj.ExecuteReadStoredProc("WBB.PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_CABASA",
                        new
                        {
                            P_ORDER_NO = query.P_ORDER_NO,
                            p_user = query.P_USER,
                            P_CA_NO = query.P_CA_NO,
                            P_BA_NO = query.P_BA_NO,
                            P_SA_NO = query.P_SA_NO,
                            p_mobile_no = query.P_MOBILE_NO,
                            p_error_reason = query.p_error_reason,
                            p_interface_result = query.p_interface_result,
                            //return code
                            OUTPUT_return_code = P_RETURN_CODE,
                            OUTPUT_return_message = P_RETURN_MESSAGE,

                        }).ToList();
                executeResults.P_RETURN_CODE = P_RETURN_CODE.Value != null ? P_RETURN_CODE.Value.ToString() : "-1";
                executeResults.P_RETURN_MESSAGE = P_RETURN_MESSAGE.Value.ToSafeString();
                _logger.Info("PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_CABASA " + executeResults.P_RETURN_MESSAGE);

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBBULK_CORP_REGISTER.PROC_INSERT_CABASA handles : " + ex.Message);

                executeResults.P_RETURN_CODE = "-1";
                executeResults.P_RETURN_MESSAGE = "Error";

                return null;
            }
            return executeResults;
        }
    }
}
