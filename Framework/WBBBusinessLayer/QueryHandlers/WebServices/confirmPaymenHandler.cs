using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using RestSharp;
using System.Net;
using RestSharp.Serializers;
using WBBEntity.PanelModels.ExWebServiceModels;
using System.Web.Script.Serialization;
using System.CodeDom;
using WBBContract.Commands.WebServices;
using AIRNETEntity.Extensions;
using WBBBusinessLayer.CommandHandlers.WebServices;
using System.Collections.Generic;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class confirmPaymenHandler : IQueryHandler<confirmPaymenQuery, ConfPMPayResponse>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;

        public confirmPaymenHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }
        public ConfPMPayResponse Handle(confirmPaymenQuery query)
        {

             InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.bodyQuery.payInfo.FirstOrDefault().mobileNum, "confirmPaymen", "confirmPaymenHandler", "", "FBB|" + query.fullUrl, "");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            confirmPaymenModel confirmPaymen = new confirmPaymenModel();
            ConfPMPayResponse pMPayResponse = new ConfPMPayResponse();
            try
            {
                _logger.Info($"confirmPaymenHandler Start");
                SaveDeductionLogCommand command1 = new SaveDeductionLogCommand()
                {
                    p_action = "New",
                    p_user_name = query.User,
                    p_transaction_id = query.bodyQuery.tranId.ToSafeString(),
                    p_mobile_no = query.bodyQuery.payInfo.FirstOrDefault().mobileNum.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = query.transaction_id,
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command1, query.bodyQuery);

                string BodyStr = JsonConvert.SerializeObject(query.bodyQuery);
                InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, BodyStr, query.bodyQuery.payInfo.FirstOrDefault().mobileNum, "confirmPaymenbodyQueryApi", "confirmPaymenHandler", "", "FBB|" + query.fullUrl, "");

                var client = new RestClient(query.Url);
                var request = new RestRequest();
                string authInfo = $"{query.username}:{query.password}";
                authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
                _logger.Info($"confirmPaymenHandler Bodystr APi: {BodyStr}");

                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Basic {authInfo}");
                request.AddHeader("ProjectCode", "PAYMENT");
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                //RespontForTest
                //var content = RespontForTest();

                var responseData = client.Execute(request);
                var content = responseData.Content;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, responseData, log2, confirmPaymen.resp_desc, "", "");
                if (responseData != null && responseData.StatusCode == HttpStatusCode.OK)
                {
                    confirmPaymen = serializer.Deserialize<confirmPaymenModel>(content);
                if (confirmPaymen != null && confirmPaymen.resp_code.Equals("000") && confirmPaymen.resp_desc.Equals("Success"))
                {
                    var receiptId = confirmPaymen.resp_data.FirstOrDefault()?.receiptId ?? default(double);
                    var receiptNum = string.IsNullOrEmpty(confirmPaymen.resp_data.FirstOrDefault()?.receiptNum) ? default(string) : confirmPaymen.resp_data.FirstOrDefault()?.receiptNum;
                    var baNum = string.IsNullOrEmpty(confirmPaymen.resp_data.FirstOrDefault()?.baNum) ? default(string) : confirmPaymen.resp_data.FirstOrDefault()?.baNum;
                    var receiptTotMny = confirmPaymen.resp_data.FirstOrDefault()?.receiptTotMny ?? default(double);
                    var taxMny = confirmPaymen.resp_data.FirstOrDefault()?.taxMny ?? default(double);

                    pMPayResponse.PM_RECEIPT_ID = new double[] { receiptId };
                    pMPayResponse.PM_RECEIPT_NUM = new string[] { receiptNum };
                    pMPayResponse.PM_BILLING_ACC_NUM = new string[] { baNum };
                    pMPayResponse.PM_RECEIPT_TOT_MNY = new double[] { receiptTotMny };
                    pMPayResponse.PM_TAX_MNY = new double[] { taxMny };
                    pMPayResponse.PM_TUX_CODE = confirmPaymen.resp_code;
                    pMPayResponse.PM_TUX_MSG = confirmPaymen.resp_desc;
                    pMPayResponse.PM_USER_ERR_MSG = "";
                    pMPayResponse.PM_SRV_ERROR = "";
                    _logger.Info($"confirmPaymenHandler Repont: Success");
                }
                    else
                    {
                        throw new Exception(confirmPaymen.resp_desc);
                    }
                }
                else
                {
                    throw new Exception($"HTTP Status : {responseData.StatusCode} or responseData is null");
                }
                SaveDeductionLogCommand command2 = new SaveDeductionLogCommand()
                {
                    p_action = "Update",
                    p_user_name = query.User,
                    p_transaction_id = query.bodyQuery.tranId.ToSafeString(),
                    p_mobile_no = query.bodyQuery.payInfo.FirstOrDefault().mobileNum.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = pMPayResponse.PM_TUX_CODE,
                    p_pm_receipt_num = pMPayResponse.PM_RECEIPT_NUM.FirstOrDefault(),
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "",
                    p_order_transaction_id = query.transaction_id
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command2, pMPayResponse);

                //TODO : Insert multi Receipt No
                InsertReceiptLog(query.bodyQuery.tranId.ToSafeString().ToSafeString(), query.User, pMPayResponse);
            }
            catch (Exception ex)
            {
                pMPayResponse.PM_RECEIPT_ID = null;
                pMPayResponse.PM_RECEIPT_NUM = null;
                pMPayResponse.PM_BILLING_ACC_NUM = null;
                pMPayResponse.PM_RECEIPT_TOT_MNY = null;
                pMPayResponse.PM_TAX_MNY = null;
                pMPayResponse.PM_TUX_CODE = confirmPaymen.resp_code.ToString();
                pMPayResponse.PM_TUX_MSG = ex.Message;
                pMPayResponse.PM_USER_ERR_MSG = ex.Message;
                pMPayResponse.PM_SRV_ERROR = "";
                _logger.Info($"confirmPaymenHandler Exception: {confirmPaymen.resp_desc}");

                SaveDeductionLogCommand command2 = new SaveDeductionLogCommand()
                {
                    p_action = "Update",
                    p_user_name = query.User,
                    p_transaction_id = query.bodyQuery.tranId.ToSafeString(),
                    p_mobile_no = query.bodyQuery.payInfo.FirstOrDefault().mobileNum.ToSafeString(),
                    p_service_name = "A_ConfPMPay",
                    p_endpoint = query.Url,
                    p_pm_tux_code = "",
                    p_pm_receipt_num = "",
                    p_enq_status = "",
                    p_enq_status_code = "",
                    p_req_xml_param = "",
                    p_res_xml_param = "Call A_ConfPMPay Error.",
                    p_order_transaction_id = query.transaction_id
                };
                InterfaceLogServiceHelper.DeductionLog(_objService, command2, "");
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pMPayResponse, log, confirmPaymen.resp_desc, string.IsNullOrEmpty(pMPayResponse.PM_TUX_MSG)?string.Empty:pMPayResponse.PM_TUX_MSG, "");
            }
            return pMPayResponse;
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
        private void validateValue(confirmPaymenModel response)
        {
            //List<string> columnNameNullOrEmpty = new List<string>();
            //if (string.IsNullOrEmpty(response.baNo)) { columnNameNullOrEmpty.Add("baNo"); }
            //if (string.IsNullOrEmpty(response.baStatus)) { columnNameNullOrEmpty.Add("baStatus"); }
            //if (response.excessPaymentMNY == -1) { columnNameNullOrEmpty.Add("excessPaymentMNY"); }
            //if (response.invoiceBalMNY == -1) { columnNameNullOrEmpty.Add("invoiceBalMNY"); }
            //if (response.orderBalMNY == -1) { columnNameNullOrEmpty.Add("totalBalMNY"); }
            //if (response.totalBalMNY == -1) { columnNameNullOrEmpty.Add("totalBalMNY"); }
            //if (string.IsNullOrEmpty(response.baCompany)) { columnNameNullOrEmpty.Add("baCompany"); }
            //if (string.IsNullOrEmpty(response.caNo)) { columnNameNullOrEmpty.Add("caNo"); }
            //if (string.IsNullOrEmpty(response.mobileNo)) { columnNameNullOrEmpty.Add("mobileNo"); }
            //if (string.IsNullOrEmpty(response.mobileStatus)) { columnNameNullOrEmpty.Add("mobileStatus"); }
            //if (string.IsNullOrEmpty(response.suspendCreditFlag)) { columnNameNullOrEmpty.Add("suspendCreditFlag"); }
            //if (string.IsNullOrEmpty(response.baNameMasking)) { columnNameNullOrEmpty.Add("baNameMasking"); }
            //_logger.Info($"Outstandingbal:{JsonConvert.SerializeObject(columnNameNullOrEmpty)}");
            //if (columnNameNullOrEmpty.Count > 0)
            //{
            //    throw new Exception(JsonConvert.SerializeObject(columnNameNullOrEmpty));
            //}
        }
        private string RespontForTest()
        {
            var content = JsonConvert.SerializeObject(new
            {
                resp_desc = "Success",
                resp_code = "000",
                resp_data = new[] {
                    new {
                        baNum = "32200050401704",
                        mobileNum = "8850099315",
                        receiptId = 163173403,
                        receiptNum = "W-CS-1004-6710-10000066",
                        receiptTotMny = 1.0,
                        taxMny = 0.0,
                        payForCode = "1",
                        eReceiptType = "ET",
                        receiptDetail = new[] {
                            new {
                                docNum = "W-IN-16-6508-4000007",
                                totalMny = 1.0
                            }
                        },
                        caNum = "32200050401703",
                        vatRate = 7.0,
                        vatMny = 0.07,
                        nonVatMny = 0.0,
                        excVatMny = 0.93,
                        roundingAdjMny = 0.0
                    }
                }
            });
            return content;
        }
    }
}
