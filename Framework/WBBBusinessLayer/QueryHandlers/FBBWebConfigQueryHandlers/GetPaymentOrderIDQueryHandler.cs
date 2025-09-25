using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetPaymentOrderIDQueryHandler : IQueryHandler<GetPaymentOrderIDQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;

        public GetPaymentOrderIDQueryHandler(ILogger logger, IEntityRepository<string> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public string Handle(GetPaymentOrderIDQuery query)
        {
            string data = "";
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "RET_CODE";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.ParameterName = "RET_MESSAGE";
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var ret_order_id = new OracleParameter();
                ret_order_id.ParameterName = "RET_ORDER_ID";
                ret_order_id.OracleDbType = OracleDbType.Varchar2;
                ret_order_id.Size = 2000;
                ret_order_id.Direction = ParameterDirection.Output;

                var executeResult = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_GEN_PAYMENT_ORDER_ID",
                    new
                    {
                        //  return code
                        ret_code = ret_code,
                        ret_message = ret_msgerr,
                        ret_order_id = ret_order_id
                    });

                data = ret_order_id.Value.ToString();

            }
            catch (Exception ex)
            {
                _logger.Info("GetStdAddressFullConHandler : Error.");
                _logger.Info(ex.Message);
            }
            return data;

        }
    }

    public class GetORDPendingPaymentQueryHandler : IQueryHandler<GetORDPendingPaymentQuery, GetORDPendingPaymentModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetORDPendingPayment> _objService;

        public GetORDPendingPaymentQueryHandler(ILogger logger, IEntityRepository<GetORDPendingPayment> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public GetORDPendingPaymentModel Handle(GetORDPendingPaymentQuery query)
        {
            GetORDPendingPaymentModel data = new GetORDPendingPaymentModel();
            data.GetORDPendingPaymentList = new List<GetORDPendingPayment>();
            try
            {
                var p_payment_method = new OracleParameter();
                p_payment_method.ParameterName = "p_payment_method";
                p_payment_method.OracleDbType = OracleDbType.Varchar2;
                p_payment_method.Size = 2000;
                p_payment_method.Direction = ParameterDirection.Input;
                p_payment_method.Value = query.p_payment_method;

                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.ParameterName = "ret_message";
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "list_order_paending_payment";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<GetORDPendingPayment> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_ORD_PENDING_PAYMENT",
                    new
                    {
                        p_payment_method = p_payment_method,

                        //  return code
                        ret_code = ret_code,
                        ret_message = ret_msgerr,

                        list_order_paending_payment = ioResults
                    }).ToList();

                data.ret_code = ret_code.Value.ToSafeString();
                data.ret_msgerr = ret_msgerr.Value.ToSafeString();
                if (executeResult != null && executeResult.Count > 0)
                {
                    data.GetORDPendingPaymentList = executeResult;
                }

            }
            catch (Exception ex)
            {
                data.ret_code = "-1";
                data.ret_msgerr = ex.Message;
                _logger.Info("GetStdAddressFullConHandler : Error.");
                _logger.Info(ex.Message);
            }

            return data;

        }
    }

    public class GetListORDDetailCreateQueryHandler : IQueryHandler<GetListORDDetailCreateQuery, GetListORDDetailCreateModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetListORDDetailCreateModel> _objServiceSubj;
        private readonly IEntityRepository<object> _fbblovRepository;
        public GetListORDDetailCreateQueryHandler(ILogger logger, IEntityRepository<GetListORDDetailCreateModel> objServiceSubj
            , IEntityRepository<object> fbblovRepository)
        {
            _logger = logger;
            _objServiceSubj = objServiceSubj;
            _fbblovRepository = fbblovRepository;
        }

        public GetListORDDetailCreateModel Handle(GetListORDDetailCreateQuery query)
        {
            GetListORDDetailCreateModel executeResult = new GetListORDDetailCreateModel();

            try
            {
                var p_order_id = new OracleParameter();
                p_order_id.ParameterName = "p_order_id";
                p_order_id.OracleDbType = OracleDbType.Varchar2;
                p_order_id.Size = 2000;
                p_order_id.Direction = ParameterDirection.Input;
                p_order_id.Value = query.p_order_id;

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "ret_code";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "ret_message";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var list_ord_detail_customer = new OracleParameter();
                list_ord_detail_customer.ParameterName = "list_ord_detail_customer";
                list_ord_detail_customer.OracleDbType = OracleDbType.RefCursor;
                list_ord_detail_customer.Direction = ParameterDirection.Output;

                var list_ord_detail_package = new OracleParameter();
                list_ord_detail_package.ParameterName = "list_ord_detail_package";
                list_ord_detail_package.OracleDbType = OracleDbType.RefCursor;
                list_ord_detail_package.Direction = ParameterDirection.Output;

                var list_ord_detail_file = new OracleParameter();
                list_ord_detail_file.ParameterName = "list_ord_detail_file";
                list_ord_detail_file.OracleDbType = OracleDbType.RefCursor;
                list_ord_detail_file.Direction = ParameterDirection.Output;

                var list_ord_detail_splitter = new OracleParameter();
                list_ord_detail_splitter.ParameterName = "list_ord_detail_splitter";
                list_ord_detail_splitter.OracleDbType = OracleDbType.RefCursor;
                list_ord_detail_splitter.Direction = ParameterDirection.Output;

                var list_ord_detail_cpe = new OracleParameter();
                list_ord_detail_cpe.ParameterName = "list_ord_detail_cpe";
                list_ord_detail_cpe.OracleDbType = OracleDbType.RefCursor;
                list_ord_detail_cpe.Direction = ParameterDirection.Output;

                _logger.Info("Start PROC_LIST_ORD_DETAIL_CREATE");

                var result = _fbblovRepository.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_ORD_DETAIL_CREATE",
                       new object[]
                    {
                        p_order_id,

                        p_return_code,
                        p_return_message,
                        list_ord_detail_customer,
                        list_ord_detail_package,
                        list_ord_detail_file,
                        list_ord_detail_splitter,
                        list_ord_detail_cpe
                    });
                executeResult.ret_code = result[0] != null ? result[0].ToString() : "-1";
                executeResult.ret_message = result[1].ToString();

                DataTable list1 = (DataTable)result[2];
                List<ODRDetailCustomer> ODRDetailCustomerList = list1.DataTableToList<ODRDetailCustomer>();
                executeResult.ODRDetailCustomerList = new List<ODRDetailCustomer>();
                executeResult.ODRDetailCustomerList = ODRDetailCustomerList;

                DataTable list2 = (DataTable)result[3];
                List<ODRDetailPackage> ODRDetailPackageList = list2.DataTableToList<ODRDetailPackage>();
                executeResult.ODRDetailPackageList = new List<ODRDetailPackage>();
                executeResult.ODRDetailPackageList = ODRDetailPackageList;

                DataTable list3 = (DataTable)result[4];
                List<ODRDetailFile> ODRDetailFileList = list3.DataTableToList<ODRDetailFile>();
                executeResult.ODRDetailFileList = new List<ODRDetailFile>();
                executeResult.ODRDetailFileList = ODRDetailFileList;

                DataTable list4 = (DataTable)result[5];
                List<ODRDetailSplitter> ODRDetailSplitterList = list4.DataTableToList<ODRDetailSplitter>();
                executeResult.ODRDetailSplitterList = new List<ODRDetailSplitter>();
                executeResult.ODRDetailSplitterList = ODRDetailSplitterList;

                DataTable list5 = (DataTable)result[6];
                List<ODRDetailCPE> ODRDetailCPEList = list5.DataTableToList<ODRDetailCPE>();
                executeResult.ODRDetailCPEList = new List<ODRDetailCPE>();
                executeResult.ODRDetailCPEList = ODRDetailCPEList;

                _logger.Info("End PROC_LIST_ORD_DETAIL_CREATE " + p_return_message.Value.ToString());

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PROC_LIST_ORD_DETAIL_CREATE handles : " + ex.Message);

                return null;
            }
            return executeResult;
        }
    }

    public class CheckOrderPaymentStatusQueryHandler : IQueryHandler<CheckOrderPaymentStatusQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovData;

        public CheckOrderPaymentStatusQueryHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_CFG_LOV> lovData)
        {
            _logger = logger;
            _objService = objService;
            _lovData = lovData;
        }

        public string Handle(CheckOrderPaymentStatusQuery query)
        {
            string CheckOrderPaymentStatus = "";

            var lovDataInfo = _lovData.Get(t => t.LOV_NAME == "QUERY_ORDER_PAYMENT_STATUS_FLAG").FirstOrDefault();

            if (lovDataInfo != null)
            {
                CheckOrderPaymentStatus = lovDataInfo.LOV_VAL1;
            }

            return CheckOrderPaymentStatus;
        }
    }

    public class GetORDPendingPaymentTimeOutQueryHandler : IQueryHandler<GetORDPendingPaymentTimeOutQuery, GetORDPendingPaymentTimeOutModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<GetORDPendingPaymentTimeOut> _objService;

        public GetORDPendingPaymentTimeOutQueryHandler(ILogger logger, IEntityRepository<GetORDPendingPaymentTimeOut> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public GetORDPendingPaymentTimeOutModel Handle(GetORDPendingPaymentTimeOutQuery query)
        {
            GetORDPendingPaymentTimeOutModel data = new GetORDPendingPaymentTimeOutModel();
            data.GetORDPendingPaymentTimeOutList = new List<GetORDPendingPaymentTimeOut>();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.ParameterName = "ret_message";
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var ioResults = new OracleParameter();
                ioResults.ParameterName = "list_order_paending_timeout";
                ioResults.OracleDbType = OracleDbType.RefCursor;
                ioResults.Direction = ParameterDirection.Output;

                List<GetORDPendingPaymentTimeOut> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_LIST_ORD_PENDING_TIMEOUT",
                    new
                    {
                        //  return code
                        ret_code = ret_code,
                        ret_message = ret_msgerr,

                        list_order_paending_timeout = ioResults
                    }).ToList();

                data.ret_code = ret_code.Value.ToSafeString();
                data.ret_msgerr = ret_msgerr.Value.ToSafeString();
                if (executeResult != null && executeResult.Count > 0)
                {
                    data.GetORDPendingPaymentTimeOutList = executeResult;
                }

            }
            catch (Exception ex)
            {
                data.ret_code = "-1";
                data.ret_msgerr = ex.Message;
                _logger.Info("GetStdAddressFullConHandler : Error.");
                _logger.Info(ex.Message);
            }

            return data;

        }
    }


    public class OrderPaymentQueryHandler : IQueryHandler<OrderPaymentQuery, OrderPaymentModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public OrderPaymentQueryHandler(ILogger logger,
            IEntityRepository<string> objService,
            IEntityRepository<FBB_CFG_LOV> lovData, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _objService = objService;
            _uow = uow;
            _intfLog = intfLog;
        }

        public OrderPaymentModel Handle(OrderPaymentQuery query)
        {
            var data = new OrderPaymentModel();
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.p_order_id, "OrderPaymentQuery", "OrderPaymentQueryHandler", "", "FBB", "");



                var retCode = new OracleParameter();
                retCode.ParameterName = "ret_code";
                retCode.OracleDbType = OracleDbType.Varchar2;
                retCode.Size = 2000;
                retCode.Direction = ParameterDirection.Output;

                var retMessage = new OracleParameter();
                retMessage.ParameterName = "ret_message";
                retMessage.OracleDbType = OracleDbType.Varchar2;
                retMessage.Size = 2000;
                retMessage.Direction = ParameterDirection.Output;

                var retPayment = new OracleParameter();
                retPayment.ParameterName = "ret_payment";
                retPayment.OracleDbType = OracleDbType.Varchar2;
                retPayment.Size = 2000;
                retPayment.Direction = ParameterDirection.Output;

                var pPaymentOrderId = new OracleParameter();
                pPaymentOrderId.ParameterName = "p_payment_order_id";
                pPaymentOrderId.OracleDbType = OracleDbType.Varchar2;
                pPaymentOrderId.Size = 2000;
                pPaymentOrderId.Direction = ParameterDirection.Input;
                pPaymentOrderId.Value = query.p_order_id;

                var result = _objService.ExecuteStoredProc("WBB.PKG_FBB_REGISTER_PAYMENT.PROC_CHECK_ORD_PAYMENT",
                    new
                    {
                        p_payment_order_id = pPaymentOrderId,
                        //  return code
                        ret_code = retCode,
                        ret_message = retMessage,
                        ret_payment = retPayment

                    });

                data.ret_code = retCode.Value.ToSafeString();
                data.ret_message = retMessage.Value.ToSafeString();
                data.payment_status = retPayment.Value.ToSafeString();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", string.Empty, "");

            }
            catch (Exception ex)
            {
                data.ret_code = "-1:handler";
                data.ret_message = ex.Message;
                _logger.Info("GetStdAddressFullConHandler : Error.");
                _logger.Info(ex.Message);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Error", ex.Message, "");

            }

            return data;
        }
    }

}
