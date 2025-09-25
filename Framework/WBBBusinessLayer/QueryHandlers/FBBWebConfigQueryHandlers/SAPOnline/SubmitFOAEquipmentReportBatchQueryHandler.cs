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
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    class SubmitFOAEquipmentReportBatchQueryHandler : IQueryHandler<SubmitFOAEquipmentReportBatchQuery, List<SubmitFOAEquipment>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SubmitFOAEquipmentReport> _submitOrderLog;
        private readonly IEntityRepository<SubmitFOAEquipment> _submitEquipmentLog;

        public SubmitFOAEquipmentReportBatchQueryHandler(
            ILogger logger,
            IEntityRepository<SubmitFOAEquipmentReport> submitOrderLog,
            IEntityRepository<SubmitFOAEquipment> submitEquipmentLog)
        {
            _logger = logger;
            _submitOrderLog = submitOrderLog;
            _submitEquipmentLog = submitEquipmentLog;
        }

        public List<SubmitFOAEquipment> Handle(SubmitFOAEquipmentReportBatchQuery query)
        {
            //string[] subStr = query.dateFrom.Split('/');
            //string dateFrom = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            //subStr = query.dateTo.Split('/');
            //var dateTo = subStr[0].ToSafeString() + subStr[1].ToSafeString() + subStr[2].ToSafeString();

            var p_ret_code = new OracleParameter();
            p_ret_code.ParameterName = "ret_code";
            p_ret_code.OracleDbType = OracleDbType.Decimal;
            p_ret_code.Direction = ParameterDirection.Output;

            var p_equip_cur = new OracleParameter();
            p_equip_cur.ParameterName = "equip_cur";
            p_equip_cur.OracleDbType = OracleDbType.RefCursor;
            p_equip_cur.Direction = ParameterDirection.Output;

            try
            {
                List<SubmitFOAEquipment> executeResult = _submitEquipmentLog.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_equip_batch",
                  new
                  {
                      p_ORDER_NO = query.orderNo.ToSafeString(),
                      p_INTERNET_NO = query.internetNo.ToSafeString(),
                      p_PRODUCT_NAME = query.productName.ToSafeString(),
                      p_SERVICE_NAME = query.serviceName.ToSafeString(),
                      p_ORDER_TYPE = query.orderType.ToSafeString(),
                      p_SUBCONT_CODE = query.subcontractorCode.ToSafeString(),
                      p_COM_CODE = query.companyCode.ToSafeString(),
                      p_MATERIAL_NO = query.materialCode.ToSafeString(),
                      p_PLANT = query.plant.ToSafeString(),
                      p_STORAGE = query.storLocation.ToSafeString(),
                      p_STATUS = query.status.ToSafeString(),
                      p_ORDER_FROM = query.dateFrom.ToSafeString(),
                      p_ORDER_TO = query.dateTo.ToSafeString(),

                      // return code
                      //0 = Success/Send, 1 = Error/Not Send, 2 = Success/Not Send
                      ret_code = p_ret_code,
                      cur = p_equip_cur
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
