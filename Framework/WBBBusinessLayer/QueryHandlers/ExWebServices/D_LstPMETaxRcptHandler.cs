using System;
using System.Linq;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class D_LstPMETaxRcptHandler : IQueryHandler<D_LstPMETaxRcptQuery, D_LstPMETaxRcptModel>
    {
        private readonly IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> _queryConfigHandler;
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public D_LstPMETaxRcptHandler(IQueryHandler<GetConfigReqPaymentQuery, GetConfigReqPaymentModel> queryConfigHandler, ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _queryConfigHandler = queryConfigHandler;
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
        }

        public D_LstPMETaxRcptModel Handle(D_LstPMETaxRcptQuery query)
        {
            InterfaceLogCommand log = null;
            var resultStatus = "Failed";
            var resultDesc = "Failed";
            var LstPMETaxRcptModel = new D_LstPMETaxRcptModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.InternetNo) ? query.InternetNo : query.TransactionId, "D_LstPMETaxRcpt", "D_LstPMETaxRcptHandler", "", "FBB|" + query.FullUrl, "");

                var config = _queryConfigHandler.Handle(
                    new GetConfigReqPaymentQuery()
                    {
                        p_transaction_id = query.TransactionId,
                        p_product_name = "PAYMENT",
                        p_service_name = "eReceipt",
                        p_non_mobile_no = query.InternetNo
                    });

                var urlEndpoint = config.list_config_req_payment?.FirstOrDefault(x => x.attr_name == "endpoint")?.attr_value;
                var serviceOption = config.list_config_req_payment?.FirstOrDefault(x => x.attr_name == "PM_SERVICE_OPTION")?.attr_value;
                var usrname = config.list_config_req_payment?.FirstOrDefault(x => x.attr_name == "UserName")?.attr_value;
                var pwd = config.list_config_req_payment?.FirstOrDefault(x => x.attr_name == "Password")?.attr_value;

                var service = new D_LstPMETaxRcptServices.eBill_webservices_BindingQSService1()
                {
                    Url = urlEndpoint,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(usrname, pwd)
                };
                var inBuf = new D_LstPMETaxRcptServices.fml32_D_LstPMETaxRcpt_In()
                {
                    PM_SERVICE_OPTION = serviceOption,
                    PM_RECEIPT_ID = query.PM_RECEIPT_ID.ToArray()
                };
                var response = service.D_LstPMETaxRcpt(new D_LstPMETaxRcptServices.D_LstPMETaxRcpt() { inbuf = inBuf });

                if (response?.outbuf?.PM_TUX_CODE == "0")
                {
                    LstPMETaxRcptModel.PM_RECEIPT_ID = response.outbuf.PM_RECEIPT_ID;
                    LstPMETaxRcptModel.PM_ETAX_FLAG = response.outbuf.PM_ETAX_FLAG;
                    LstPMETaxRcptModel.PM_TUX_CODE = response.outbuf.PM_TUX_CODE;
                    LstPMETaxRcptModel.PM_TUX_MSG = response.outbuf.PM_TUX_MSG;
                    LstPMETaxRcptModel.PM_RECEIPT_NUM = response.outbuf.PM_RECEIPT_NUM;
                    LstPMETaxRcptModel.PM_RECEIPT_DAT = response.outbuf.PM_RECEIPT_DAT;
                    LstPMETaxRcptModel.PM_RECEIPT_TOT_MNY = response.outbuf.PM_RECEIPT_TOT_MNY;
                    LstPMETaxRcptModel.PM_BILL_START_DAT = response.outbuf.PM_BILL_START_DAT;
                    LstPMETaxRcptModel.PM_BILL_END_DAT = response.outbuf.PM_BILL_END_DAT;
                    LstPMETaxRcptModel.PM_DOC_TYPE = response.outbuf.PM_DOC_TYPE;
                }

                resultStatus = "Success";
                resultDesc = "";
            }
            catch (Exception ex)
            {
                _logger.Info("Error call D_LstPMETaxRcptHandler : " + ex.GetErrorMessage());
                resultStatus = "Failed";
                resultDesc = ex.GetErrorMessage();
                LstPMETaxRcptModel = null;
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, LstPMETaxRcptModel, log, resultStatus, resultDesc, "");
            }

            return LstPMETaxRcptModel;
        }
    }
}
