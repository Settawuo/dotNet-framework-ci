using Oracle.ManagedDataAccess.Client;
using System;
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
    public class CheckNotiProductHandler : IQueryHandler<CheckNotiProductQuery, CheckNotiProductModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _obj;

        public CheckNotiProductHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> obj)
        {
            _uow = uow;
            _intfLog = intfLog;
            _obj = obj;
        }

        public CheckNotiProductModel Handle(CheckNotiProductQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new CheckNotiProductModel();
            var logStatus = "Failed";
            var logDesc = "Failed";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transaction_id, "CheckNotiProduct", "CheckNotiProductHandler", "", "FBB|" + query.FullUrl, "WEB");

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Size = 2000;
                ret_message.Direction = ParameterDirection.Output;

                var ret_product = new OracleParameter();
                ret_product.ParameterName = "ret_product";
                ret_product.OracleDbType = OracleDbType.Varchar2;
                ret_product.Size = 2000;
                ret_product.Direction = ParameterDirection.Output;

                var executeResult = _obj.ExecuteReadStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_CHECK_NOTI_PRODUCT",
                   new
                   {
                       p_transaction_id = query.transaction_id,
                       p_order_transaction_id = query.order_transaction_id,
                       // out
                       ret_code = ret_code,
                       ret_message = ret_message,
                       ret_product = ret_product
                   }).ToList();

                result.ret_code = ret_code != null ? ret_code.Value.ToSafeString() : "-1";
                result.ret_message = ret_message != null ? ret_message.Value.ToSafeString() : "";
                result.ret_product = ret_product != null ? ret_product.Value.ToSafeString() : "";

                if (result.ret_code == "0")
                {
                    logStatus = "Success";
                    logDesc = "";
                }
                else
                {
                    logStatus = "Failed";
                    logDesc = String.Format("ret_code={0}:ret_message={1}",
                        result.ret_code,
                        result.ret_message);
                }
            }
            catch (Exception ex)
            {
                logStatus = "Failed";
                logDesc = ex.GetErrorMessage();
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, logStatus, logDesc, "");
            }
            return result;
        }
    }
}