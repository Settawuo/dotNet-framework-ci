using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.SAPOnline
{
    public class SubmitFOARevalueBatchQueryHandler : IQueryHandler<SubmitFOARevalueBatchQuery, List<SubmitFOARevalue>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOARevalue> _submit;
        public SubmitFOARevalueBatchQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOARevalue> submit)
        {
            _logger = logger;
            _submit = submit;
        }
        public List<SubmitFOARevalue> Handle(SubmitFOARevalueBatchQuery query)
        {
            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_revalue_cur = new OracleParameter();
            p_revalue_cur.ParameterName = "revalue_cur";
            p_revalue_cur.OracleDbType = OracleDbType.RefCursor;
            p_revalue_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOARevalue> executeResult = _submit.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_REVALUE",
                  new
                  {
                      p_ORDER_NO = query.orderNo.ToSafeString(),
                      p_INTERNET_NO = query.internetNo.ToSafeString(),
                      p_COM_CODE = query.companyCode.ToSafeString(),
                      p_MAIN_ASSET = query.mainasset.ToSafeString(),
                      p_ACTION = query.action.ToSafeString(),
                      p_STATUS = query.status.ToSafeString(),
                      p_ERR_MSG = query.errormessage.ToSafeString(),
                      p_ORDER_FROM = query.dateFrom.ToSafeString(),
                      p_ORDER_TO = query.dateTo.ToSafeString(),

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_revalue_cur
                  }).ToList();

                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return null;
            }
        }

    }
}
