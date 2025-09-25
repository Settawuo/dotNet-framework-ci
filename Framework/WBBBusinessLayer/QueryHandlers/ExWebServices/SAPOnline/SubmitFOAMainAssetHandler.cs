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
    public class SubmitFOAMainAssetHandler : IQueryHandler<SubmitFOAMainAssetQuery, List<SubmitFOAMainAsset>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAMainAsset> _submitMainAssetLog;

        public SubmitFOAMainAssetHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAMainAsset> submitMainAssetLog)
        {
            _logger = logger;
            _submitMainAssetLog = submitMainAssetLog;
        }

        public List<SubmitFOAMainAsset> Handle(SubmitFOAMainAssetQuery query)
        {
            string[] subStr = query.dateFrom.Split('/');
            string dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            subStr = query.dateTo.Split('/');
            var dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_mainasset_cur = new OracleParameter();
            p_mainasset_cur.ParameterName = "mainasset_cur";
            p_mainasset_cur.OracleDbType = OracleDbType.RefCursor;
            p_mainasset_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAMainAsset> executeResult = _submitMainAssetLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.P_GET_MAINASSET",
                  new
                  {
                      p_ORDER_NO = query.orderNo.ToSafeString(),
                      p_INTERNET_NO = query.internetNo.ToSafeString(),
                      p_COM_CODE = query.companyCode.ToSafeString(),
                      p_ASSET_CLASS = query.assetClass.ToSafeString(),
                      p_STATUS = query.status.ToSafeString(),
                      p_ORDER_FROM = dateFrom,
                      p_ORDER_TO = dateTo,

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_mainasset_cur
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
