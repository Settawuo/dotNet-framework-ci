using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WBBBusinessLayer.BindingQSServices;
using WBBBusinessLayer.CommandHandlers.WebServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetListPmMobileDetailsHandler : IQueryHandler<GetListPmMobileDetialQuery, PmModleDetailResponse>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;

        public GetListPmMobileDetailsHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLovRepository)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
        }

        public PmModleDetailResponse Handle(GetListPmMobileDetialQuery query)
        {
            InterfaceLogCommand log = null;
            var pmModleDetailResponse = new PmModleDetailResponse();

            try
            {
                var loveConfigList = from item in _cfgLovRepository.Get()
                                     where item.LOV_TYPE == "PAYMENT_OUTSTANDINGBAL_WEB"
                                     select item;
                var lovToggle = loveConfigList.FirstOrDefault(item => item.LOV_NAME == "Toggle_OUTSTANDINGBAL_WEB").LOV_VAL1.ToSafeString();

                if (loveConfigList != null && loveConfigList.Count() > 0 && lovToggle == "Y")
                {
                    PaymentOutstandingbalConfigResult result = new PaymentOutstandingbalConfigResult();
                    PaymentOutstandingbalResponse PaymentResponse = new PaymentOutstandingbalResponse();

                    PaymentOutstandingbalConfigModel config = new PaymentOutstandingbalConfigModel();
                    config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_OutStandingBal") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "URL_OutStandingBal").LOV_VAL1 : "";
                    config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "UseSecurityProtocol") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "UseSecurityProtocol").LOV_VAL1 : "";
                    config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "Content-Type") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "Content-Type").LOV_VAL1 : "";
                    config.Authorization = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "Authorization") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "Authorization").LOV_VAL1 : "";
                    config.ProjectCode = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ProjectCode") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "ProjectCode").LOV_VAL1 : "";

                    var p_debtType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_debtType") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_debtType").LOV_VAL1 : "";
                    var p_invRespFlag = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_invRespFlag") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_invRespFlag").LOV_VAL1 : "";
                    var p_orderRespFlag = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_orderRespFlag") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_orderRespFlag").LOV_VAL1 : "";
                    var p_creditLimitRespFlag = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_creditLimitRespFlag") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_creditLimitRespFlag").LOV_VAL1 : "";
                    var p_queryInactiveFlag = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_queryInactiveFlag") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_queryInactiveFlag").LOV_VAL1 : "";
                    var p_orderGroup = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_orderGroup") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_orderGroup").LOV_VAL1 : "";
                    var p_userId = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_userId") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "cfg_p_userId").LOV_VAL1 : "";

                    pmModleDetailResponse.InternetNo = query.InternetNo.ToSafeString();

                    string[] mobileArray = new string[1];
                    mobileArray[0] = query.InternetNo.ToSafeString();

                    PaymentOutstandingbalConfigBody paymentOutstandingbalConfigBody = new PaymentOutstandingbalConfigBody()
                    {
                        mobileList = mobileArray.ToArray(),
                        debtType = p_debtType.ToSafeString(),
                        invRespFlag = p_invRespFlag.ToSafeString(),
                        orderRespFlag = p_orderRespFlag.ToSafeString(),
                        creditLimitRespFlag = p_creditLimitRespFlag.ToSafeString(),
                        queryInactiveFlag = p_queryInactiveFlag.ToSafeString(),
                        orderGroup = p_orderGroup.ToSafeString(),
                        userId = p_userId.ToSafeString()
                    };

                    string BodyStr = JsonConvert.SerializeObject(paymentOutstandingbalConfigBody);
                    config.BodyStr = BodyStr;

                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, config, query.InternetNo, "OutStandingBal", "GetListPmMobileDetailsHandler", "", "FBB", "");

                    var client = new RestClient(config.Url);
                    var request = new RestRequest();
                    request.Method = Method.POST;
                    request.AddHeader("Content-Type", config.ContentType);
                    request.AddHeader("Authorization", config.Authorization);
                    request.AddHeader("ProjectCode", config.ProjectCode);
                    request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    if (config.UseSecurityProtocol == "Y")
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                    }

                    var responseData = client.Execute(request);
                    var content = responseData.Content; // raw content as string

                    if (HttpStatusCode.OK.Equals(responseData.StatusCode))
                    {
                        result = JsonConvert.DeserializeObject<PaymentOutstandingbalConfigResult>(responseData.Content) ?? new PaymentOutstandingbalConfigResult();
                        if (result != null)
                        {
                            if (result.Response != null && result.Response.Count > 0)
                            {
                                var baNo = result.Response.Select(item => item.baNo).FirstOrDefault();
                                if (baNo != null)
                                {
                                    PaymentResponse.baNo = baNo.ToSafeString();
                                }

                                var baStatus = result.Response.Select(item => item.baStatus).FirstOrDefault();
                                if (baStatus != null)
                                {
                                    PaymentResponse.baStatus = baStatus.ToSafeString();
                                }

                                var excessPaymentMNY = result.Response.Select(item => item.excessPaymentMNY).FirstOrDefault();
                                if (excessPaymentMNY != null)
                                {
                                    PaymentResponse.excessPaymentMNY = excessPaymentMNY.ToSafeString();
                                }

                                var invoiceBalMNY = result.Response.Select(item => item.invoiceBalMNY).FirstOrDefault();
                                if (invoiceBalMNY != null)
                                {
                                    PaymentResponse.invoiceBalMNY = invoiceBalMNY.ToSafeString();
                                }

                                var totalBalMNY = result.Response.Select(item => item.totalBalMNY).FirstOrDefault();
                                if (totalBalMNY != null)
                                {
                                    PaymentResponse.totalBalMNY = totalBalMNY.ToSafeString();
                                }

                                var baCompany = result.Response.Select(item => item.baCompany).FirstOrDefault();
                                if (baCompany != null)
                                {
                                    PaymentResponse.baCompany = baCompany.ToSafeString();
                                }

                                var caNo = result.Response.Select(item => item.caNo).FirstOrDefault();
                                if (caNo != null)
                                {
                                    PaymentResponse.caNo = caNo.ToSafeString();
                                }

                                var mobileNo = result.Response.Select(item => item.mobileNo).FirstOrDefault();
                                if (mobileNo != null)
                                {
                                    PaymentResponse.mobileNo = mobileNo.ToSafeString();
                                }

                                var mobileStatus = result.Response.Select(item => item.mobileStatus).FirstOrDefault();
                                if (mobileStatus != null)
                                {
                                    PaymentResponse.mobileStatus = mobileStatus.ToSafeString();
                                }

                                var suspendCreditFlag = result.Response.Select(item => item.suspendCreditFlag).FirstOrDefault();
                                if (suspendCreditFlag != null)
                                {
                                    PaymentResponse.suspendCreditFlag = suspendCreditFlag.ToSafeString();
                                }

                                var payAmt = result.Response.Select(item => item.payAmt).FirstOrDefault();
                                if (payAmt != null)
                                {
                                    PaymentResponse.payAmt = payAmt.ToSafeString();
                                }

                                var overUsage = result.Response.Select(item => item.overUsage).FirstOrDefault();
                                if (overUsage != null)
                                {
                                    PaymentResponse.overUsage = overUsage.ToSafeString();
                                }

                                var minAdvPayment = result.Response.Select(item => item.minAdvPayment).FirstOrDefault();
                                if (minAdvPayment != null)
                                {
                                    PaymentResponse.minAdvPayment = minAdvPayment.ToSafeString();
                                }

                                var invoiceList = result.Response.Select(item => item.invoiceList).FirstOrDefault();
                                if (invoiceList != null)
                                {
                                    PaymentResponse.invoiceList = new List<PaymentOutstandingbalInvoice>();
                                    PaymentResponse.invoiceList = invoiceList.ToList();
                                }
                            }

                            pmModleDetailResponse.TotalBalance = PaymentResponse.totalBalMNY.ToSafeDouble();
                            if (PaymentResponse.invoiceList != null && PaymentResponse.invoiceList.Count > 0)
                            {
                                pmModleDetailResponse.BillingNo = PaymentResponse.invoiceList.FirstOrDefault().billingAccount.ToSafeString();

                                if (PaymentResponse.invoiceList.FirstOrDefault().paymentDueDat != null &&
                                    PaymentResponse.invoiceList.FirstOrDefault().paymentDueDat != "")
                                {
                                    var valueDueDat = PaymentResponse.invoiceList.FirstOrDefault().paymentDueDat.ToSafeString();
                                    var strValue = valueDueDat.ToString().Replace("/", "-");
                                    var day = strValue.Substring(0, 2);
                                    var month = strValue.Substring(3, 2);
                                    var year = strValue.Substring(6, 4);
                                    var strReturn = string.Format("{0}-{1}-{2}", year, month, day);

                                    pmModleDetailResponse.DueDate = strReturn.ToSafeString();
                                }
                            }

                            pmModleDetailResponse.StatusDesc = result.ErrorDesc;
                            pmModleDetailResponse.StatusCode = result.ErrorCode == "000" ? "0" : result.ErrorCode;
                            pmModleDetailResponse.StatusMessage = result.ErrorMsg;

                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Success", "", "");
                        }
                        else
                        {
                            pmModleDetailResponse.StatusDesc = "result null";
                            pmModleDetailResponse.StatusCode = "1";
                            pmModleDetailResponse.StatusMessage = "result null";
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Failed", "", "");
                        }
                    }
                    else
                    {
                        pmModleDetailResponse.StatusDesc = responseData.StatusDescription.ToString();
                        pmModleDetailResponse.StatusCode = responseData.StatusCode.ToString();
                        pmModleDetailResponse.StatusMessage = responseData.ErrorMessage.ToString();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Failed", "", "");
                    }

                }
                else
                {//old
                    var aLstPmMobDtlResponse = new A_LstPMMobDtlResponse();
                    var resultLov = from item in _cfgLovRepository.Get() where item.LOV_TYPE == "CREDENTIAL_PAYMENT" select item;
                    var urlEnpoint = resultLov.FirstOrDefault(item => item.LOV_NAME == "URL_QUERY_INVOICE");
                    var userCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "CREDENTIAL_USER") ?? new FBB_CFG_LOV();
                    var passwordCredential = resultLov.FirstOrDefault(item => item.LOV_NAME == "CREDENTIAL_PASSWORD") ?? new FBB_CFG_LOV();

                    pmModleDetailResponse.InternetNo = query.InternetNo;
                    var servicePayment = new tuxsalt_BindingQSService
                    {
                        Url = urlEnpoint != null ? urlEnpoint.DISPLAY_VAL : string.Empty,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(userCredential.DISPLAY_VAL, passwordCredential.DISPLAY_VAL)
                    };

                    var aLstPmMobDtl = new A_LstPMMobDtl();
                    var pmMobileNum = new[] { query.InternetNo };

                    aLstPmMobDtl.inbuf = new fml32_A_LstPMMobDtl_In
                    {
                        PM_MOBLIE_NUM = pmMobileNum,
                    };

                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, aLstPmMobDtl, query.InternetNo, "A_LstPMMobDtl_PRT", "A_LstPMMobDtl", query.InternetNo, "FBB", "");

                    aLstPmMobDtlResponse = servicePayment.A_LstPMMobDtl(aLstPmMobDtl);

                    string logResult;
                    switch (aLstPmMobDtlResponse.outbuf.PM_TUX_CODE)
                    {
                        case "0":
                            logResult = "Success";
                            pmModleDetailResponse.StatusCode = aLstPmMobDtlResponse.outbuf.PM_TUX_CODE;
                            pmModleDetailResponse.StatusMessage = aLstPmMobDtlResponse.outbuf.PM_TUX_MSG;
                            pmModleDetailResponse.BillingNo = aLstPmMobDtlResponse.outbuf.PM_BILLING_ACC_NUM[0];
                            pmModleDetailResponse.TotalBalance = aLstPmMobDtlResponse.outbuf.PM_TOTAL_BALANCE_MNY.Sum(balance => balance);

                            if (aLstPmMobDtlResponse.outbuf != null &&
                                aLstPmMobDtlResponse.outbuf.PM_PAYMENT_DUE_DAT != null)
                            {
                                pmModleDetailResponse.DueDate = aLstPmMobDtlResponse.outbuf.PM_PAYMENT_DUE_DAT.OrderBy(item => item).FirstOrDefault();
                            }
                            break;
                        case "1":
                            logResult = pmModleDetailResponse.StatusMessage = aLstPmMobDtlResponse.outbuf.PM_TUX_MSG;
                            pmModleDetailResponse.StatusCode = aLstPmMobDtlResponse.outbuf.PM_TUX_CODE;
                            pmModleDetailResponse.StatusMessage = aLstPmMobDtlResponse.outbuf.PM_TUX_MSG;
                            break;
                        default:
                            logResult = "Failed";
                            pmModleDetailResponse.StatusCode = aLstPmMobDtlResponse.outbuf.PM_TUX_CODE;
                            pmModleDetailResponse.StatusMessage = aLstPmMobDtlResponse.outbuf.PM_TUX_MSG;
                            break;
                    }
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, aLstPmMobDtlResponse, log, logResult, pmModleDetailResponse.StatusMessage, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Failed", ex.Message, "");
            }

            return pmModleDetailResponse;
        }
    }

    public class ConfPMPayHandler : IQueryHandler<ConfPMPayQuery, ConfPMPayResponse>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLovRepository;
        private readonly IEntityRepository<string> _objService;

        public ConfPMPayHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> cfgLovRepository,
            IEntityRepository<string> objService)
        {
            _uow = uow;
            _intfLog = intfLog;
            _cfgLovRepository = cfgLovRepository;
            _objService = objService;
        }

        public ConfPMPayResponse Handle(ConfPMPayQuery query)
        {
            InterfaceLogCommand log = null;
            var resultStatus = "Failed";
            var resultDesc = "Failed";
            A_ConfPMPay aConfPMPay = new A_ConfPMPay();
            ConfPMPayResponse confPMPayResponse = new ConfPMPayResponse();
            A_ConfPMPayResponse aConfPMPayResponse = new A_ConfPMPayResponse();
            try
            {
                string username = query.username;
                string password = query.password;

                query.username = "";
                query.password = "";

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, !string.IsNullOrEmpty(query.PM_MOBLIE_NUM) ? query.PM_MOBLIE_NUM : query.PM_TRAN_ID, "ConfPMPay", "ConfPMPayHandler", "", "FBB|" + query.FullUrl, "");

                tuxsalt_BindingQSService servicePayment = new tuxsalt_BindingQSService
                {
                    Url = query.Url,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(username, password)
                };

                aConfPMPay.inbuf = new fml32_A_ConfPMPay_In();
                aConfPMPay.inbuf.PM_MOBLIE_NUM = query.PM_MOBLIE_NUM.ToSafeString();
                aConfPMPay.inbuf.PM_BILLING_ACC_NUM = query.PM_BILLING_ACC_NUM.ToSafeString();
                aConfPMPay.inbuf.PM_STATUS_CD = query.PM_STATUS_CD;
                aConfPMPay.inbuf.PM_PAID_AMT = query.PM_PAID_AMT;
                aConfPMPay.inbuf.PM_TAX_AMT = query.PM_TAX_AMT;
                aConfPMPay.inbuf.PM_RECEIPT_LOCATION = query.PM_RECEIPT_LOCATION;
                aConfPMPay.inbuf.PM_PAYMENT_CHANNEL_ID = query.PM_PAYMENT_CHANNEL_ID;
                aConfPMPay.inbuf.PM_SHIFT_NUM = query.PM_SHIFT_NUM;
                aConfPMPay.inbuf.PM_TERMINAL_ID = query.PM_TERMINAL_ID.ToSafeString();
                aConfPMPay.inbuf.PM_USER_ID = query.PM_USER_ID.ToSafeString();
                aConfPMPay.inbuf.PM_NEXT_BILL_FLAG = query.PM_NEXT_BILL_FLAG.ToSafeString();
                aConfPMPay.inbuf.PM_PAYMENT_METHOD_ID = query.PM_PAYMENT_METHOD_ID;
                aConfPMPay.inbuf.PM_PRINT_FLAG = query.PM_PRINT_FLAG;
                aConfPMPay.inbuf.PM_TRAN_ID = query.PM_TRAN_ID.ToSafeString();
                aConfPMPay.inbuf.PM_BANK_CODE = query.PM_BANK_CODE.ToSafeInteger();
                aConfPMPay.inbuf.PM_BANK_CODESpecified = true;
                aConfPMPay.inbuf.PM_SUB_CHANNEL = query.PM_SUB_CHANNEL.ToSafeString();

                SaveDeductionLogCommand command1 = new SaveDeductionLogCommand()
                {
                    p_action = "New",
                    p_user_name = query.User,
                    p_transaction_id = query.PM_TRAN_ID.ToSafeString(),
                    p_mobile_no = query.PM_MOBLIE_NUM.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = query.PM_ORDER_TRANSACTION_ID,
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command1, aConfPMPay);

                aConfPMPayResponse = servicePayment.A_ConfPMPay(aConfPMPay);
                string PM_TUX_CODE = "";
                string PM_RECEIPT_NUM = "";
                if (aConfPMPayResponse != null)
                {
                    confPMPayResponse.PM_RECEIPT_ID = aConfPMPayResponse.outbuf.PM_RECEIPT_ID;
                    confPMPayResponse.PM_RECEIPT_NUM = aConfPMPayResponse.outbuf.PM_RECEIPT_NUM;
                    confPMPayResponse.PM_BILLING_ACC_NUM = aConfPMPayResponse.outbuf.PM_BILLING_ACC_NUM;
                    confPMPayResponse.PM_RECEIPT_TOT_MNY = aConfPMPayResponse.outbuf.PM_RECEIPT_TOT_MNY;
                    confPMPayResponse.PM_TAX_MNY = aConfPMPayResponse.outbuf.PM_TAX_MNY;
                    confPMPayResponse.PM_TUX_CODE = aConfPMPayResponse.outbuf.PM_TUX_CODE;
                    confPMPayResponse.PM_TUX_MSG = aConfPMPayResponse.outbuf.PM_TUX_MSG;
                    confPMPayResponse.PM_USER_ERR_MSG = aConfPMPayResponse.outbuf.PM_USER_ERR_MSG;
                    confPMPayResponse.PM_SRV_ERROR = aConfPMPayResponse.outbuf.PM_SRV_ERROR;
                    PM_TUX_CODE = aConfPMPayResponse.outbuf.PM_TUX_CODE;
                    PM_RECEIPT_NUM = aConfPMPayResponse.outbuf.PM_RECEIPT_NUM[0];
                }
                SaveDeductionLogCommand command2 = new SaveDeductionLogCommand()
                {
                    p_action = "Update",
                    p_user_name = query.User,
                    p_transaction_id = query.PM_TRAN_ID.ToSafeString(),
                    p_mobile_no = query.PM_MOBLIE_NUM.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = PM_TUX_CODE,
                    p_pm_receipt_num = PM_RECEIPT_NUM,
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = query.PM_ORDER_TRANSACTION_ID,
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command2, aConfPMPayResponse);

                //TODO : Insert multi Receipt No
                InsertReceiptLog(query.PM_TRAN_ID.ToSafeString(), query.User, confPMPayResponse);

                resultStatus = "Success";
                resultDesc = "";
            }
            catch (Exception ex)
            {
                resultStatus = "Failed";
                resultDesc = ex.GetErrorMessage();
                SaveDeductionLogCommand command2 = new SaveDeductionLogCommand()
                {
                    p_action = "Update",
                    p_user_name = query.User,
                    p_transaction_id = query.PM_TRAN_ID.ToSafeString(),
                    p_mobile_no = query.PM_MOBLIE_NUM.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "Call A_ConfPMPay Error.",
                    p_order_transaction_id = query.PM_ORDER_TRANSACTION_ID,
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command2, "");
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, confPMPayResponse, log, resultStatus, resultDesc, "");
            }

            return confPMPayResponse;
        }


        private void InsertReceiptLog(string transactionId, string usrname, ConfPMPayResponse confPMPayResponse)
        {
            for (int i = 0; i < confPMPayResponse.PM_RECEIPT_ID?.Length; i++)
            {
                var receiptCommand = new SaveDeductionReceiptLogCommand()
                {
                    p_transaction_id = transactionId,
                    p_user_name = usrname,
                    p_pm_receipt_id = confPMPayResponse.PM_RECEIPT_ID[i],
                    p_pm_billing_acc_num = confPMPayResponse.PM_BILLING_ACC_NUM[i],
                    p_pm_receipt_num = confPMPayResponse.PM_RECEIPT_NUM[i],
                    p_pm_receipt_tot_mny = confPMPayResponse.PM_RECEIPT_TOT_MNY[i],
                    p_pm_tax_mny = confPMPayResponse.PM_TAX_MNY[i]
                };
                DeductionLogHelper.ReceiptLog(_objService, receiptCommand);
            }
        }
    }
}
