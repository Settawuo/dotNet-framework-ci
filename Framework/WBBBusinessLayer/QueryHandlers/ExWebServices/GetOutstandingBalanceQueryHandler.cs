using AIRNETEntity.Extensions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices
{
    public class GetOutstandingBalanceQueryHandler : IQueryHandler<GetOutstandingBalanceQuery, PmModleDetailResponse>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly ILogger _logger;
        public GetOutstandingBalanceQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            ILogger logger)
        {
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
            _logger = logger;
        }
        public PmModleDetailResponse Handle(GetOutstandingBalanceQuery query)
        {
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.InternetNo, "GetOutstandingBalanceQuery", "GetOutstandingBalanceQueryHandler", "", "", "");// Out param handler
            InterfaceLogCommand log2 = null;                                                                                                                                                                               // 
            GetOutstandingBalanceModel Outstandingbal = new GetOutstandingBalanceModel();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var pmModleDetailResponse = new PmModleDetailResponse();
            try
            {   //create by chita571 18/09/2024
                _logger.Info("Start GetOutstandingBalanceQueryHandler");
                var lovConfig = (from z in _lov.Get()
                                 where z.LOV_TYPE == "CONFIG_OUTSANDINGBAL" && z.ACTIVEFLAG == "Y"
                                 select z).ToList();
                var config = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "OUTSANDINGBAL").LOV_VAL1) && !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "OUTSANDINGBAL").LOV_VAL2) && !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "OUTSANDINGBAL").LOV_VAL3) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "OUTSANDINGBAL") : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var DebtType = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "DEBTYPE").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "DEBTYPE").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var InvRespFlag = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "INVRESPFLAG").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "INVRESPFLAG").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var OrderRespFlag = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "ORDERRESPFLAG").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "ORDERRESPFLAG").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var CreditLimitRespFlag = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "CREDITLIMITRESPFLAG").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "CREDITLIMITRESPFLAG").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var QueryInactiveFlag = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "QUERYINACTIVEFLAG").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "QUERYINACTIVEFLAG").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var OrderGroup = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "ORDERGROUP").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "ORDERGROUP").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");
                var UserId = lovConfig != null && lovConfig.Count > 0 ? !string.IsNullOrEmpty(lovConfig.FirstOrDefault(i => i.LOV_NAME == "USERID").LOV_VAL1) ? lovConfig.FirstOrDefault(i => i.LOV_NAME == "USERID").LOV_VAL1 : throw new Exception("value in lov is null or empty") : throw new Exception("Lov config is null or Empty");

                List<string> MobileList = new List<string>();
                MobileList.Add(query.InternetNo);
                OutstandingbalConfigBody configBody = new OutstandingbalConfigBody()
                {
                    mobileList = MobileList,
                    debtType = DebtType,
                    invRespFlag = InvRespFlag,
                    orderRespFlag = OrderRespFlag,
                    creditLimitRespFlag = CreditLimitRespFlag,
                    queryInactiveFlag = QueryInactiveFlag,
                    orderGroup = OrderGroup,
                    userId = UserId
                };
                log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, configBody, query.InternetNo, "GetOutstandingBalResponseAPI", "GetOutstandingBalanceQueryHandler", "", "", "");// Response API
                string BodyStr = JsonConvert.SerializeObject(configBody);
                var client = new RestClient(config.LOV_VAL1);
                var request = new RestRequest();
                string authInfo = $"{config.LOV_VAL2}:{config.LOV_VAL3}";
                authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
                _logger.Info($"Bodystr APi: {BodyStr}");

                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Basic {authInfo}");
                request.AddHeader("ProjectCode", config.LOV_VAL4);
                request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;

                var responseData = client.Execute(request);
                var content = responseData.Content;
                if (responseData != null && responseData.StatusCode == HttpStatusCode.OK)
                {
                    Outstandingbal = serializer.Deserialize<GetOutstandingBalanceModel>(content);
                    if (Outstandingbal.Response != null && Outstandingbal.Response.Count > 0)
                    {
                        var response = Outstandingbal.Response.FirstOrDefault();
                        validateValue(response);
                        pmModleDetailResponse.TotalBalance = response.totalBalMNY;
                        if (response.invoiceList != null && response.invoiceList.Count > 0)
                        {
                            pmModleDetailResponse.BillingNo = response.invoiceList.FirstOrDefault().billingAccount.ToSafeString();
                            if (response.invoiceList.FirstOrDefault().paymentDueDat != null &&
                                response.invoiceList.FirstOrDefault().paymentDueDat != "")
                            {
                                var valueDueDat = response.invoiceList.FirstOrDefault().paymentDueDat.ToSafeString();
                                var strValue = valueDueDat.ToString().Replace("/", "-");
                                var day = strValue.Substring(0, 2);
                                var month = strValue.Substring(3, 2);
                                var year = strValue.Substring(6, 4);
                                var strReturn = string.Format("{0}-{1}-{2}", year, month, day);

                                pmModleDetailResponse.DueDate = strReturn.ToSafeString();
                            }
                        }         
                    }
                    else
                    {
                        throw new Exception("Response Data Not Found");
                    }
                    pmModleDetailResponse.StatusDesc = Outstandingbal.ErrorDesc;
                    pmModleDetailResponse.StatusCode = Outstandingbal.ErrorCode == "000" ? "0" : Outstandingbal.ErrorCode;
                    pmModleDetailResponse.StatusMessage = Outstandingbal.ErrorMsg;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Success", "", "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Outstandingbal, log2, "Success", "", "");
                    _logger.Info($"OutstandingbalResponse Success: {Outstandingbal.ErrorCode}");
                }
                else
                {
                    Outstandingbal = serializer.Deserialize<GetOutstandingBalanceModel>(content);
                    pmModleDetailResponse.StatusDesc = Outstandingbal.ErrorDesc;
                    pmModleDetailResponse.StatusCode = Outstandingbal.ErrorCode == "000" ? "0" : Outstandingbal.ErrorCode;
                    pmModleDetailResponse.StatusMessage = Outstandingbal.ErrorMsg;
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Failed", "", "");
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Outstandingbal, log2, "Failed", "", "");
                    _logger.Info($"OutstandingbalResponse Failed: {Outstandingbal.ErrorCode}");
                }
            }
            catch (Exception ex)
            {
                pmModleDetailResponse.StatusDesc = Outstandingbal.ErrorDesc;
                pmModleDetailResponse.StatusCode = Outstandingbal.ErrorCode == "000" ? "0" : Outstandingbal.ErrorCode;
                pmModleDetailResponse.StatusMessage = Outstandingbal.ErrorMsg;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, pmModleDetailResponse, log, "Failed", ex.Message, "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Outstandingbal, log2, "Failed", "", "");
                _logger.Info($"OutstandingbalResponse Exception: {ex.Message}");
            }
            return pmModleDetailResponse;
        }

        private void validateValue(OutstandingbalResponse response)
        {
            List<string> columnNameNullOrEmpty = new List<string>();
            if (string.IsNullOrEmpty(response.baNo)) { columnNameNullOrEmpty.Add("baNo"); }
            if (string.IsNullOrEmpty(response.baStatus)) { columnNameNullOrEmpty.Add("baStatus"); }
            if (response.excessPaymentMNY == -1) { columnNameNullOrEmpty.Add("excessPaymentMNY"); }
            if (response.invoiceBalMNY == -1) { columnNameNullOrEmpty.Add("invoiceBalMNY"); }
            if (response.orderBalMNY == -1) { columnNameNullOrEmpty.Add("totalBalMNY"); }
            if (response.totalBalMNY == -1) { columnNameNullOrEmpty.Add("totalBalMNY"); }
            if (string.IsNullOrEmpty(response.baCompany)) { columnNameNullOrEmpty.Add("baCompany"); }
            if (string.IsNullOrEmpty(response.caNo)) { columnNameNullOrEmpty.Add("caNo"); }
            if (string.IsNullOrEmpty(response.mobileNo)) { columnNameNullOrEmpty.Add("mobileNo"); }
            if (string.IsNullOrEmpty(response.mobileStatus)) { columnNameNullOrEmpty.Add("mobileStatus"); }
            if (string.IsNullOrEmpty(response.suspendCreditFlag)) { columnNameNullOrEmpty.Add("suspendCreditFlag"); }
            if (string.IsNullOrEmpty(response.baNameMasking)) { columnNameNullOrEmpty.Add("baNameMasking"); }
            _logger.Info($"Outstandingbal:{JsonConvert.SerializeObject(columnNameNullOrEmpty)}");
            //if (columnNameNullOrEmpty.Count > 0)
            //{
            //    throw new Exception(JsonConvert.SerializeObject(columnNameNullOrEmpty));
            //}
        }
    }
}
