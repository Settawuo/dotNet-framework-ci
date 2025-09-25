using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetPaymentEnquiryHandler : IQueryHandler<GetPaymentEnquiryQuery, GetPaymentEnquiryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetPaymentEnquiryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetPaymentEnquiryModel Handle(GetPaymentEnquiryQuery query)
        {
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.tranTime, "GetPaymentEnquiry", "GetPaymentEnquiryHandler", null, "FBB", "");

            GetPaymentEnquiryModel results = new GetPaymentEnquiryModel();
            try
            {

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.Size = 2000;
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Direction = ParameterDirection.Output;

                var ret_message = new OracleParameter();
                ret_message.ParameterName = "ret_message";
                ret_message.Size = 2000;
                ret_message.OracleDbType = OracleDbType.Varchar2;
                ret_message.Direction = ParameterDirection.Output;

                var list_order_deduct_enquiry = new OracleParameter();
                list_order_deduct_enquiry.ParameterName = "list_order_deduct_enquiry";
                list_order_deduct_enquiry.OracleDbType = OracleDbType.RefCursor;
                list_order_deduct_enquiry.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_PAYMENT_ENQUIRY",
                    new object[]
                    {
                         //return
                         ret_code,
                         ret_message,
                         list_order_deduct_enquiry
                    });

                results.ret_code = result[0] != null ? result[0].ToSafeString() : "-1";
                results.ret_message = result[1] != null ? result[1].ToSafeString() : "error";
                if (results.ret_code != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    results.OrderDeductEnquiryDatas = data1.DataTableToList<OrderDeductEnquiry>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, results, log, "Failed", results.ret_message, "");
                }

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", ex.Message, "");
                results.ret_code = "-1";
                results.ret_message = "Error";

                return null;
            }
            return results;
        }
    }
}
